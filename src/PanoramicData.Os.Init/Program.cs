using PanoramicData.Os.Init.Linux;
using PanoramicData.Os.Init.Logging;
using PanoramicData.Os.Init.Shell;

namespace PanoramicData.Os.Init;

/// <summary>
/// PanoramicData.Os Init Process
/// This is the first userspace process (PID 1) that runs after the kernel boots.
/// </summary>
public static class Program
{
	private static volatile bool _running = true;
	private static readonly SerialLogger _logger = new("/dev/console");

	public static int Main(string[] args)
	{
		try
		{
			_logger.Info("====================================");
			_logger.Info(" PanoramicData.Os Init Starting");
			_logger.Info("====================================");
			_logger.Info($"Hello from .NET {Environment.Version}!");
			_logger.Info($"Process ID: {Environment.ProcessId}");
			_logger.Info($"Machine: {Environment.MachineName}");

			// Set up signal handlers
			SetupSignalHandlers();

			// Mount essential filesystems
			MountFilesystems();

			// Populate /dev
			PopulateDevices();

			// Set hostname
			SetHostname();

			// Configure network
			ConfigureNetwork();

			_logger.Info("Init complete. System ready.");
			_logger.Info("====================================");
			_logger.Info("Starting shell...");

			// Start the shell
			using var shell = new PanShell();
			var shellResult = shell.Run();

			_logger.Info($"Shell exited with code {shellResult}");

			// If shell exits, enter idle loop (init must never exit)
			_logger.Info("Shell exited. Entering idle loop...");
			while (_running)
			{
				// Reap zombie processes (as PID 1, we're the ultimate parent)
				Syscalls.waitpid(-1, out _, Syscalls.WNOHANG);

				// Sleep to prevent busy-waiting
				Thread.Sleep(1000);
			}

			_logger.Info("Init shutting down...");
			return 0;
		}
		catch (Exception ex)
		{
			_logger.Error($"Fatal error in init: {ex.Message}");
			_logger.Error(ex.StackTrace ?? "No stack trace available");

			// Init should never exit, but if we get here, halt the system
			Thread.Sleep(5000);
			return 1;
		}
	}

	private static void SetupSignalHandlers()
	{
		_logger.Info("Setting up signal handlers...");

		// Handle SIGTERM and SIGINT for graceful shutdown
		Console.CancelKeyPress += (sender, e) =>
		{
			_logger.Info("Received shutdown signal");
			e.Cancel = true;
			_running = false;
		};

		// Note: In a real implementation, we'd use P/Invoke to set up
		// proper signal handlers for SIGTERM, SIGCHLD, etc.
		_logger.Info("Signal handlers configured");
	}

	private static void MountFilesystems()
	{
		_logger.Info("Mounting filesystems...");

		// Mount /proc
		_logger.Info("  Mounting /proc...");
		var result = Mount.MountFs("proc", "/proc", "proc", 0, null);
		if (result != 0)
		{
			_logger.Warn($"  Failed to mount /proc: error {result}");
		}
		else
		{
			_logger.Info("  /proc mounted");
		}

		// Mount /sys
		_logger.Info("  Mounting /sys...");
		result = Mount.MountFs("sysfs", "/sys", "sysfs", 0, null);
		if (result != 0)
		{
			_logger.Warn($"  Failed to mount /sys: error {result}");
		}
		else
		{
			_logger.Info("  /sys mounted");
		}

		// Mount /dev (devtmpfs should already be mounted by kernel if configured)
		_logger.Info("  Mounting /dev...");
		result = Mount.MountFs("devtmpfs", "/dev", "devtmpfs", 0, null);
		if (result != 0)
		{
			_logger.Warn($"  /dev mount returned {result} (may already be mounted)");
		}
		else
		{
			_logger.Info("  /dev mounted");
		}

		// Mount /dev/pts for pseudo-terminals
		_logger.Info("  Mounting /dev/pts...");
		try
		{
			Directory.CreateDirectory("/dev/pts");
		}
		catch { }

		result = Mount.MountFs("devpts", "/dev/pts", "devpts", 0, "gid=5,mode=620");
		if (result != 0)
		{
			_logger.Warn($"  Failed to mount /dev/pts: error {result}");
		}
		else
		{
			_logger.Info("  /dev/pts mounted");
		}

		// Mount /tmp
		_logger.Info("  Mounting /tmp...");
		result = Mount.MountFs("tmpfs", "/tmp", "tmpfs", 0, "size=64M");
		if (result != 0)
		{
			_logger.Warn($"  Failed to mount /tmp: error {result}");
		}
		else
		{
			_logger.Info("  /tmp mounted");
		}

		// Mount /run
		_logger.Info("  Mounting /run...");
		result = Mount.MountFs("tmpfs", "/run", "tmpfs", 0, "size=32M");
		if (result != 0)
		{
			_logger.Warn($"  Failed to mount /run: error {result}");
		}
		else
		{
			_logger.Info("  /run mounted");
		}

		_logger.Info("Filesystem mounting complete");
	}

	private static void PopulateDevices()
	{
		_logger.Info("Populating device nodes...");

		// Create symlinks for standard I/O
		try
		{
			// stdin, stdout, stderr
			CreateSymlink("/proc/self/fd/0", "/dev/stdin");
			CreateSymlink("/proc/self/fd/1", "/dev/stdout");
			CreateSymlink("/proc/self/fd/2", "/dev/stderr");
			CreateSymlink("/proc/self/fd", "/dev/fd");

			_logger.Info("Device nodes populated");
		}
		catch (Exception ex)
		{
			_logger.Warn($"Error populating devices: {ex.Message}");
		}
	}

	private static void CreateSymlink(string target, string link)
	{
		try
		{
			if (!File.Exists(link) && !Directory.Exists(link))
			{
				File.CreateSymbolicLink(link, target);
			}
		}
		catch
		{
			// Ignore errors - symlink may already exist
		}
	}

	private static void SetHostname()
	{
		_logger.Info("Setting hostname...");

		try
		{
			var hostname = "panos";

			// Try to read hostname from /etc/hostname
			if (File.Exists("/etc/hostname"))
			{
				hostname = File.ReadAllText("/etc/hostname").Trim();
			}

			// Set the hostname via /proc/sys/kernel/hostname
			if (File.Exists("/proc/sys/kernel/hostname"))
			{
				File.WriteAllText("/proc/sys/kernel/hostname", hostname);
				_logger.Info($"Hostname set to: {hostname}");
			}
			else
			{
				_logger.Warn("Cannot set hostname: /proc/sys/kernel/hostname not available");
			}
		}
		catch (Exception ex)
		{
			_logger.Warn($"Failed to set hostname: {ex.Message}");
		}
	}

	private static void ConfigureNetwork()
	{
		_logger.Info("Configuring network...");

		try
		{
			// Configure loopback interface (IPv4: 127.0.0.1/8)
			// Note: IPv6 (::1/128) is auto-configured by the kernel when the interface is up
			_logger.Info("  Configuring lo (127.0.0.1/8, ::1/128)...");
			var (loResult, loMessage) = NetworkConfig.ConfigureInterface("lo", "127.0.0.1", "255.0.0.0");
			if (loResult == 0)
			{
				_logger.Info($"  {loMessage}");
			}
			else
			{
				_logger.Warn($"  Failed to configure lo: {loMessage}");
			}

			// Configure eth0 with QEMU user-mode networking defaults
			// QEMU user-mode networking uses 10.0.2.0/24, with gateway at 10.0.2.2
			// IPv6 link-local addresses are auto-configured by the kernel
			_logger.Info("  Configuring eth0 (10.0.2.15/24)...");
			var (ethResult, ethMessage) = NetworkConfig.ConfigureInterface("eth0", "10.0.2.15", "255.255.255.0");
			if (ethResult == 0)
			{
				_logger.Info($"  {ethMessage}");
			}
			else
			{
				_logger.Warn($"  Failed to configure eth0: {ethMessage}");
			}

			_logger.Info("Network configuration complete");
		}
		catch (Exception ex)
		{
			_logger.Warn($"Failed to configure network: {ex.Message}");
		}
	}
}

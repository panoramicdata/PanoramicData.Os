using System.Runtime.InteropServices;

namespace PanoramicData.Os.Init.Linux;

/// <summary>
/// P/Invoke declarations for Linux system calls.
/// </summary>
internal static partial class Syscalls
{
    // Mount flags
    public const ulong MS_RDONLY = 1;
    public const ulong MS_NOSUID = 2;
    public const ulong MS_NODEV = 4;
    public const ulong MS_NOEXEC = 8;
    public const ulong MS_SYNCHRONOUS = 16;
    public const ulong MS_REMOUNT = 32;
    public const ulong MS_MANDLOCK = 64;
    public const ulong MS_NOATIME = 1024;
    public const ulong MS_NODIRATIME = 2048;
    public const ulong MS_BIND = 4096;
    public const ulong MS_MOVE = 8192;
    public const ulong MS_REC = 16384;
    public const ulong MS_SILENT = 32768;

    // Waitpid flags
    public const int WNOHANG = 1;
    public const int WUNTRACED = 2;
    public const int WCONTINUED = 8;

    // Reboot commands
    public const int LINUX_REBOOT_MAGIC1 = unchecked((int)0xfee1dead);
    public const int LINUX_REBOOT_MAGIC2 = 672274793;
    public const int LINUX_REBOOT_CMD_RESTART = 0x01234567;
    public const int LINUX_REBOOT_CMD_HALT = unchecked((int)0xCDEF0123);
    public const int LINUX_REBOOT_CMD_POWER_OFF = 0x4321FEDC;

    /// <summary>
    /// Mount a filesystem.
    /// </summary>
    [LibraryImport("libc", SetLastError = true, StringMarshalling = StringMarshalling.Utf8)]
    internal static partial int mount(
        string? source,
        string target,
        string? filesystemtype,
        ulong mountflags,
        string? data);

    /// <summary>
    /// Unmount a filesystem.
    /// </summary>
    [LibraryImport("libc", SetLastError = true, StringMarshalling = StringMarshalling.Utf8)]
    internal static partial int umount(string target);

    /// <summary>
    /// Unmount a filesystem with flags.
    /// </summary>
    [LibraryImport("libc", SetLastError = true, StringMarshalling = StringMarshalling.Utf8)]
    internal static partial int umount2(string target, int flags);

    /// <summary>
    /// Wait for a child process.
    /// </summary>
    [LibraryImport("libc", SetLastError = true)]
    internal static partial int waitpid(int pid, out int status, int options);

    /// <summary>
    /// Reboot or halt the system.
    /// </summary>
    [LibraryImport("libc", SetLastError = true)]
    internal static partial int reboot(int cmd);

    /// <summary>
    /// Set the hostname.
    /// </summary>
    [LibraryImport("libc", SetLastError = true, StringMarshalling = StringMarshalling.Utf8)]
    internal static partial int sethostname(string name, nint len);

    /// <summary>
    /// Sync all filesystems.
    /// </summary>
    [LibraryImport("libc")]
    internal static partial void sync();

    /// <summary>
    /// Create a directory.
    /// </summary>
    [LibraryImport("libc", SetLastError = true, StringMarshalling = StringMarshalling.Utf8)]
    internal static partial int mkdir(string pathname, int mode);

    /// <summary>
    /// Create a device node.
    /// </summary>
    [LibraryImport("libc", SetLastError = true, StringMarshalling = StringMarshalling.Utf8)]
    internal static partial int mknod(string pathname, int mode, ulong dev);

    /// <summary>
    /// Open a file.
    /// </summary>
    [LibraryImport("libc", SetLastError = true, StringMarshalling = StringMarshalling.Utf8)]
    internal static partial int open(string pathname, int flags, int mode);

    /// <summary>
    /// Close a file descriptor.
    /// </summary>
    [LibraryImport("libc", SetLastError = true)]
    internal static partial int close(int fd);

    /// <summary>
    /// Write to a file descriptor.
    /// </summary>
    [LibraryImport("libc", SetLastError = true)]
    internal static partial nint write(int fd, nint buf, nint count);

    /// <summary>
    /// Read from a file descriptor.
    /// </summary>
    [LibraryImport("libc", SetLastError = true)]
    internal static partial nint read(int fd, nint buf, nint count);

    /// <summary>
    /// Duplicate a file descriptor to a specific number.
    /// </summary>
    [LibraryImport("libc", SetLastError = true)]
    internal static partial int dup2(int oldfd, int newfd);

    /// <summary>
    /// Execute a program.
    /// </summary>
    [LibraryImport("libc", SetLastError = true, StringMarshalling = StringMarshalling.Utf8)]
    internal static partial int execve(string pathname, nint argv, nint envp);

    /// <summary>
    /// Fork the current process.
    /// </summary>
    [LibraryImport("libc", SetLastError = true)]
    internal static partial int fork();

    /// <summary>
    /// Create a new session.
    /// </summary>
    [LibraryImport("libc", SetLastError = true)]
    internal static partial int setsid();

    /// <summary>
    /// Get the last error number.
    /// </summary>
    public static int GetLastError() => Marshal.GetLastPInvokeError();
}

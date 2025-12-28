using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using PanoramicData.Os.CommandLine;
using PanoramicData.Os.CommandLine.Specifications;
using PanoramicData.Os.Init.Linux;

namespace PanoramicData.Os.Init.Shell.Commands;

/// <summary>
/// Represents a single ping result for typed output streams.
/// </summary>
public class PingResult
{
	/// <summary>The target host.</summary>
	public required string Host { get; init; }

	/// <summary>The target IP address.</summary>
	public required string IpAddress { get; init; }

	/// <summary>Sequence number.</summary>
	public int Sequence { get; init; }

	/// <summary>Round-trip time in milliseconds.</summary>
	public double RoundTripMs { get; init; }

	/// <summary>Time-to-live value.</summary>
	public int Ttl { get; init; }

	/// <summary>Whether the ping succeeded.</summary>
	public bool Success { get; init; }

	/// <summary>Error message if failed.</summary>
	public string? Error { get; init; }
}

/// <summary>
/// Ping command - send ICMP echo requests to a host.
/// </summary>
public class PingCommand : ShellCommand
{
	private static readonly ShellCommandSpecification _specification = new()
	{
		Name = "ping",
		Description = "Send ICMP echo requests to a host",
		Usage = "ping [-c count] [-W timeout] <host>",
		Category = "Network",
		Examples = ["ping 8.8.8.8", "ping -c 5 192.168.1.1", "ping -c 10 -W 2 10.0.0.1"],
		Options =
		[
			new OptionSpec<int>
			{
				Name = "c",
				ShortName = "c",
				LongName = "count",
				Description = "Number of ping requests to send",
				IsPositional = false,
				IsRequired = false,
				DefaultValue = 4,
				MinValue = 1,
				MaxValue = 100000
			},
			new OptionSpec<int>
			{
				Name = "W",
				ShortName = "W",
				LongName = "timeout",
				Description = "Timeout in seconds for each reply",
				IsPositional = false,
				IsRequired = false,
				DefaultValue = 1,
				MinValue = 1,
				MaxValue = 60
			},
			new OptionSpec<string>
			{
				Name = "host",
				Description = "Host to ping (IP address or hostname)",
				IsPositional = true,
				Position = 0,
				IsRequired = true
			}
		],
		InputStreams = [],
		OutputStreams =
		[
			new StreamSpec<PingResult>
			{
				Name = "results",
				Description = "Ping results for each request",
				Requirement = StreamRequirement.Required
			}
		],
		ExitCodes =
		[
			StandardExitCodes.Success,
			StandardExitCodes.InvalidArguments,
			StandardExitCodes.NetworkError,
			StandardExitCodes.Timeout,
			StandardExitCodes.PermissionDenied
		],
		ExecutionMode = ExecutionMode.Blocking
	};

	public override ShellCommandSpecification Specification => _specification;

	// ICMP message types
	private const byte ICMP_ECHO_REQUEST = 8;
	private const byte ICMP_ECHO_REPLY = 0;
	private const byte ICMPV6_ECHO_REQUEST = 128;
	private const byte ICMPV6_ECHO_REPLY = 129;

	// Default values
	private const int DEFAULT_COUNT = 4;
	private const int DEFAULT_TIMEOUT_MS = 1000;
	private const int PACKET_SIZE = 64;

	protected override Task<CommandResult> ExecuteAsync(
		CommandExecutionContext context,
		CancellationToken cancellationToken)
	{
		var args = context.GetParameter<string[]>("args", []);

		if (args.Length == 0)
		{
			context.Console.WriteWarning($"Usage: {Usage}");
			return Task.FromResult(CommandResult.BadRequest());
		}

		// Parse arguments
		int count = DEFAULT_COUNT;
		int timeoutMs = DEFAULT_TIMEOUT_MS;
		string? host = null;

		for (int i = 0; i < args.Length; i++)
		{
			switch (args[i])
			{
				case "-c":
					if (i + 1 < args.Length && int.TryParse(args[++i], out var c))
						count = c;
					else
					{
						context.Console.WriteError("ping: invalid count");
						return Task.FromResult(CommandResult.BadRequest());
					}
					break;
				case "-W":
					if (i + 1 < args.Length && int.TryParse(args[++i], out var t))
						timeoutMs = t * 1000; // Convert to ms
					else
					{
						context.Console.WriteError("ping: invalid timeout");
						return Task.FromResult(CommandResult.BadRequest());
					}
					break;
				default:
					if (!args[i].StartsWith("-"))
						host = args[i];
					break;
			}
		}

		if (string.IsNullOrEmpty(host))
		{
			context.Console.WriteError("ping: missing host operand");
			return Task.FromResult(CommandResult.BadRequest());
		}

		// Resolve host to IP address
		IPAddress? targetAddress = null;

		// Try to parse as IP address first
		if (!IPAddress.TryParse(host, out targetAddress))
		{
			// For now, we don't have DNS resolution, so just fail
			context.Console.WriteError($"ping: {host}: Name or service not known");
			context.Console.WriteWarning("Note: DNS resolution not yet implemented. Please use IP addresses.");
			return Task.FromResult(CommandResult.NotFound());
		}

		bool isIPv6 = targetAddress.AddressFamily == AddressFamily.InterNetworkV6;

		context.Console.WriteLine($"PING {host} ({targetAddress}): {PACKET_SIZE - 8} data bytes");

		// Create raw socket for ICMP
		int domain = isIPv6 ? Syscalls.AF_INET6 : Syscalls.AF_INET;
		int protocol = isIPv6 ? Syscalls.IPPROTO_ICMPV6 : Syscalls.IPPROTO_ICMP;

		// Try SOCK_RAW first (requires root), fall back to SOCK_DGRAM (requires ping_group_range)
		int sockfd = Syscalls.socket(domain, Syscalls.SOCK_RAW, protocol);
		if (sockfd < 0)
		{
			// Try SOCK_DGRAM as fallback (ping socket)
			sockfd = Syscalls.socket(domain, Syscalls.SOCK_DGRAM, protocol);
		}

		if (sockfd < 0)
		{
			int error = Syscalls.GetLastError();
			context.Console.WriteError($"ping: socket creation failed (error {error})");
			context.Console.WriteWarning("Note: ICMP may require elevated privileges");
			return Task.FromResult(CommandResult.NetworkError());
		}

		try
		{
			// Set receive timeout
			SetSocketTimeout(sockfd, timeoutMs);

			// Statistics
			int sent = 0;
			int received = 0;
			double minTime = double.MaxValue;
			double maxTime = 0;
			double totalTime = 0;

			ushort identifier = (ushort)(Environment.ProcessId & 0xFFFF);

			for (int seq = 1; seq <= count && !cancellationToken.IsCancellationRequested; seq++)
			{
				try
				{
					// Build ICMP packet
					byte[] packet = BuildIcmpPacket(isIPv6, identifier, (ushort)seq);

					// Send the packet
					var stopwatch = Stopwatch.StartNew();
					bool sendResult = SendIcmpPacket(sockfd, targetAddress, packet, isIPv6);

					if (!sendResult)
					{
						context.Console.WriteError($"ping: sendto failed");
						sent++;
						continue;
					}

					sent++;

					// Receive reply
					byte[] recvBuffer = new byte[1024];
					var (success, replyAddress, ttl) = ReceiveIcmpReply(sockfd, recvBuffer, isIPv6, identifier, (ushort)seq);
					stopwatch.Stop();

					if (success)
					{
						double rtt = stopwatch.Elapsed.TotalMilliseconds;
						received++;
						totalTime += rtt;
						minTime = Math.Min(minTime, rtt);
						maxTime = Math.Max(maxTime, rtt);

						context.Console.WriteLine($"{PACKET_SIZE} bytes from {replyAddress}: icmp_seq={seq} ttl={ttl} time={rtt:F1} ms");
					}
					else
					{
						context.Console.WriteWarning($"Request timeout for icmp_seq {seq}");
					}

					// Wait between pings (except for the last one)
					if (seq < count && !cancellationToken.IsCancellationRequested)
					{
						Thread.Sleep(1000);
					}
				}
				catch (Exception ex)
				{
					context.Console.WriteError($"ping: {ex.Message}");
				}
			}

			// Print statistics
			context.Console.WriteLine();
			context.Console.WriteLine($"--- {host} ping statistics ---");
			int lossPercent = sent > 0 ? (int)(((sent - received) * 100.0) / sent) : 0;
			context.Console.WriteLine($"{sent} packets transmitted, {received} packets received, {lossPercent}% packet loss");

			if (received > 0)
			{
				double avgTime = totalTime / received;
				context.Console.WriteLine($"round-trip min/avg/max = {minTime:F3}/{avgTime:F3}/{maxTime:F3} ms");
			}

			return Task.FromResult(received > 0 ? CommandResult.Ok() : CommandResult.Unavailable());
		}
		finally
		{
			Syscalls.close(sockfd);
		}
	}

	private static void SetSocketTimeout(int sockfd, int timeoutMs)
	{
		// timeval structure: { tv_sec (long), tv_usec (long) }
		long[] timeval = new long[2];
		timeval[0] = timeoutMs / 1000;       // seconds
		timeval[1] = (timeoutMs % 1000) * 1000; // microseconds

		var handle = GCHandle.Alloc(timeval, GCHandleType.Pinned);
		try
		{
			Syscalls.setsockopt(sockfd, Syscalls.SOL_SOCKET, Syscalls.SO_RCVTIMEO,
				handle.AddrOfPinnedObject(), Marshal.SizeOf<long>() * 2);
		}
		finally
		{
			handle.Free();
		}
	}

	private static byte[] BuildIcmpPacket(bool isIPv6, ushort identifier, ushort sequence)
	{
		// ICMP packet structure:
		// Type (1) | Code (1) | Checksum (2) | Identifier (2) | Sequence (2) | Data (56)
		byte[] packet = new byte[PACKET_SIZE - 8]; // 56 bytes for ICMP (excluding IP header)

		packet[0] = isIPv6 ? ICMPV6_ECHO_REQUEST : ICMP_ECHO_REQUEST; // Type
		packet[1] = 0; // Code

		// Identifier (network byte order)
		packet[4] = (byte)(identifier >> 8);
		packet[5] = (byte)(identifier & 0xFF);

		// Sequence number (network byte order)
		packet[6] = (byte)(sequence >> 8);
		packet[7] = (byte)(sequence & 0xFF);

		// Fill data with pattern
		for (int i = 8; i < packet.Length; i++)
		{
			packet[i] = (byte)(i & 0xFF);
		}

		// Calculate checksum (only for ICMPv4, ICMPv6 uses pseudo-header)
		if (!isIPv6)
		{
			ushort checksum = CalculateChecksum(packet);
			packet[2] = (byte)(checksum >> 8);
			packet[3] = (byte)(checksum & 0xFF);
		}

		return packet;
	}

	private static ushort CalculateChecksum(byte[] data)
	{
		uint sum = 0;

		// Sum 16-bit words
		for (int i = 0; i < data.Length - 1; i += 2)
		{
			sum += (uint)((data[i] << 8) | data[i + 1]);
		}

		// Add odd byte if present
		if (data.Length % 2 != 0)
		{
			sum += (uint)(data[^1] << 8);
		}

		// Fold 32-bit sum to 16 bits
		while ((sum >> 16) != 0)
		{
			sum = (sum & 0xFFFF) + (sum >> 16);
		}

		return (ushort)~sum;
	}

	private static bool SendIcmpPacket(int sockfd, IPAddress target, byte[] packet, bool isIPv6)
	{
		byte[] sockaddr;
		int addrLen;

		if (isIPv6)
		{
			// sockaddr_in6 structure (28 bytes)
			sockaddr = new byte[28];
			sockaddr[0] = Syscalls.AF_INET6;
			sockaddr[1] = 0;
			// Port (2 bytes) - 0
			// Flow info (4 bytes) - 0
			// Address starts at offset 8
			var addrBytes = target.GetAddressBytes();
			Array.Copy(addrBytes, 0, sockaddr, 8, 16);
			addrLen = 28;
		}
		else
		{
			// sockaddr_in structure (16 bytes)
			sockaddr = new byte[16];
			sockaddr[0] = Syscalls.AF_INET;
			sockaddr[1] = 0;
			// Port (2 bytes at offset 2) - 0
			// Address starts at offset 4
			var addrBytes = target.GetAddressBytes();
			Array.Copy(addrBytes, 0, sockaddr, 4, 4);
			addrLen = 16;
		}

		var packetHandle = GCHandle.Alloc(packet, GCHandleType.Pinned);
		var addrHandle = GCHandle.Alloc(sockaddr, GCHandleType.Pinned);

		try
		{
			var result = Syscalls.sendto(sockfd, packetHandle.AddrOfPinnedObject(),
				(nuint)packet.Length, 0, addrHandle.AddrOfPinnedObject(), addrLen);

			return result >= 0;
		}
		finally
		{
			packetHandle.Free();
			addrHandle.Free();
		}
	}

	private static (bool success, string address, int ttl) ReceiveIcmpReply(
		int sockfd, byte[] buffer, bool isIPv6, ushort expectedId, ushort expectedSeq)
	{
		byte[] sockaddr = new byte[128]; // Large enough for both IPv4 and IPv6
		int addrLen = sockaddr.Length;

		var bufHandle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
		var addrHandle = GCHandle.Alloc(sockaddr, GCHandleType.Pinned);

		try
		{
			var bytesReceived = Syscalls.recvfrom(sockfd, bufHandle.AddrOfPinnedObject(),
				(nuint)buffer.Length, 0, addrHandle.AddrOfPinnedObject(), ref addrLen);

			if (bytesReceived <= 0)
			{
				return (false, "", 0);
			}

			// Parse the response
			int icmpOffset = 0;
			int ttl = 64; // Default TTL

			if (!isIPv6)
			{
				// For IPv4 DGRAM socket, kernel handles IP header
				// ICMP starts at offset 0
				icmpOffset = 0;

				// Try to get TTL from IP header if available
				if (bytesReceived >= 20)
				{
					// Check if this looks like an IP header
					byte versionIhl = buffer[0];
					if ((versionIhl >> 4) == 4) // IPv4
					{
						ttl = buffer[8];
						int headerLen = (versionIhl & 0x0F) * 4;
						icmpOffset = headerLen;
					}
				}
			}

			// Validate ICMP reply
			if (bytesReceived > icmpOffset + 7)
			{
				byte type = buffer[icmpOffset];
				byte expectedReplyType = isIPv6 ? ICMPV6_ECHO_REPLY : ICMP_ECHO_REPLY;

				if (type == expectedReplyType)
				{
					ushort recvId = (ushort)((buffer[icmpOffset + 4] << 8) | buffer[icmpOffset + 5]);
					ushort recvSeq = (ushort)((buffer[icmpOffset + 6] << 8) | buffer[icmpOffset + 7]);

					if (recvId == expectedId && recvSeq == expectedSeq)
					{
						// Parse source address
						string srcAddr;
						if (isIPv6)
						{
							byte[] addrBytes = new byte[16];
							Array.Copy(sockaddr, 8, addrBytes, 0, 16);
							srcAddr = new IPAddress(addrBytes).ToString();
						}
						else
						{
							byte[] addrBytes = new byte[4];
							Array.Copy(sockaddr, 4, addrBytes, 0, 4);
							srcAddr = new IPAddress(addrBytes).ToString();
						}

						return (true, srcAddr, ttl);
					}
				}
			}

			return (false, "", 0);
		}
		finally
		{
			bufHandle.Free();
			addrHandle.Free();
		}
	}
}

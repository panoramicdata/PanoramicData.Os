using System.Net;
using System.Runtime.InteropServices;

namespace PanoramicData.Os.Init.Linux;

/// <summary>
/// Network configuration utilities using ioctl.
/// </summary>
public static class NetworkConfig
{
	// ifreq structure for ioctl (simplified, 40 bytes on x86_64)
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	private struct ifreq
	{
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
		public byte[] ifr_name;
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 24)]
		public byte[] ifr_data;
	}

	// sockaddr_in structure for IPv4 addresses
	[StructLayout(LayoutKind.Sequential)]
	private struct sockaddr_in
	{
		public ushort sin_family;
		public ushort sin_port;
		public uint sin_addr;
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
		public byte[] sin_zero;
	}

	/// <summary>
	/// Bring up a network interface.
	/// </summary>
	public static int InterfaceUp(string interfaceName)
	{
		int sockfd = Syscalls.socket(Syscalls.AF_INET, Syscalls.SOCK_DGRAM, 0);
		if (sockfd < 0)
			return -1;

		try
		{
			var ifr = new ifreq
			{
				ifr_name = new byte[16],
				ifr_data = new byte[24]
			};

			// Copy interface name
			var nameBytes = System.Text.Encoding.ASCII.GetBytes(interfaceName);
			Array.Copy(nameBytes, ifr.ifr_name, Math.Min(nameBytes.Length, 15));

			// Get current flags
			var ifrPtr = Marshal.AllocHGlobal(Marshal.SizeOf<ifreq>());
			try
			{
				Marshal.StructureToPtr(ifr, ifrPtr, false);

				int result = Syscalls.ioctl(sockfd, Syscalls.SIOCGIFFLAGS, ifrPtr);
				if (result < 0)
					return Syscalls.GetLastError();

				// Read back the flags (stored in ifr_data as a short)
				ifr = Marshal.PtrToStructure<ifreq>(ifrPtr);
				short flags = BitConverter.ToInt16(ifr.ifr_data, 0);

				// Set UP flag
				flags |= Syscalls.IFF_UP;
				flags |= Syscalls.IFF_RUNNING;

				// Write flags back
				BitConverter.GetBytes(flags).CopyTo(ifr.ifr_data, 0);
				Marshal.StructureToPtr(ifr, ifrPtr, false);

				result = Syscalls.ioctl(sockfd, Syscalls.SIOCSIFFLAGS, ifrPtr);
				return result < 0 ? Syscalls.GetLastError() : 0;
			}
			finally
			{
				Marshal.FreeHGlobal(ifrPtr);
			}
		}
		finally
		{
			Syscalls.close(sockfd);
		}
	}

	/// <summary>
	/// Set IP address on an interface.
	/// </summary>
	public static int SetAddress(string interfaceName, IPAddress address)
	{
		if (address.AddressFamily != System.Net.Sockets.AddressFamily.InterNetwork)
			return -1; // Only IPv4 for now

		int sockfd = Syscalls.socket(Syscalls.AF_INET, Syscalls.SOCK_DGRAM, 0);
		if (sockfd < 0)
			return -1;

		try
		{
			var ifr = new ifreq
			{
				ifr_name = new byte[16],
				ifr_data = new byte[24]
			};

			// Copy interface name
			var nameBytes = System.Text.Encoding.ASCII.GetBytes(interfaceName);
			Array.Copy(nameBytes, ifr.ifr_name, Math.Min(nameBytes.Length, 15));

			// Build sockaddr_in structure
			var addrBytes = address.GetAddressBytes();
			var sockAddr = new sockaddr_in
			{
				sin_family = Syscalls.AF_INET,
				sin_port = 0,
				sin_addr = BitConverter.ToUInt32(addrBytes, 0),
				sin_zero = new byte[8]
			};

			// Copy sockaddr to ifr_data
			var sockAddrPtr = Marshal.AllocHGlobal(Marshal.SizeOf<sockaddr_in>());
			try
			{
				Marshal.StructureToPtr(sockAddr, sockAddrPtr, false);
				Marshal.Copy(sockAddrPtr, ifr.ifr_data, 0, 16);
			}
			finally
			{
				Marshal.FreeHGlobal(sockAddrPtr);
			}

			var ifrPtr = Marshal.AllocHGlobal(Marshal.SizeOf<ifreq>());
			try
			{
				Marshal.StructureToPtr(ifr, ifrPtr, false);
				int result = Syscalls.ioctl(sockfd, Syscalls.SIOCSIFADDR, ifrPtr);
				return result < 0 ? Syscalls.GetLastError() : 0;
			}
			finally
			{
				Marshal.FreeHGlobal(ifrPtr);
			}
		}
		finally
		{
			Syscalls.close(sockfd);
		}
	}

	/// <summary>
	/// Set netmask on an interface.
	/// </summary>
	public static int SetNetmask(string interfaceName, IPAddress netmask)
	{
		if (netmask.AddressFamily != System.Net.Sockets.AddressFamily.InterNetwork)
			return -1; // Only IPv4 for now

		int sockfd = Syscalls.socket(Syscalls.AF_INET, Syscalls.SOCK_DGRAM, 0);
		if (sockfd < 0)
			return -1;

		try
		{
			var ifr = new ifreq
			{
				ifr_name = new byte[16],
				ifr_data = new byte[24]
			};

			// Copy interface name
			var nameBytes = System.Text.Encoding.ASCII.GetBytes(interfaceName);
			Array.Copy(nameBytes, ifr.ifr_name, Math.Min(nameBytes.Length, 15));

			// Build sockaddr_in structure
			var addrBytes = netmask.GetAddressBytes();
			var sockAddr = new sockaddr_in
			{
				sin_family = Syscalls.AF_INET,
				sin_port = 0,
				sin_addr = BitConverter.ToUInt32(addrBytes, 0),
				sin_zero = new byte[8]
			};

			// Copy sockaddr to ifr_data
			var sockAddrPtr = Marshal.AllocHGlobal(Marshal.SizeOf<sockaddr_in>());
			try
			{
				Marshal.StructureToPtr(sockAddr, sockAddrPtr, false);
				Marshal.Copy(sockAddrPtr, ifr.ifr_data, 0, 16);
			}
			finally
			{
				Marshal.FreeHGlobal(sockAddrPtr);
			}

			var ifrPtr = Marshal.AllocHGlobal(Marshal.SizeOf<ifreq>());
			try
			{
				Marshal.StructureToPtr(ifr, ifrPtr, false);
				int result = Syscalls.ioctl(sockfd, Syscalls.SIOCSIFNETMASK, ifrPtr);
				return result < 0 ? Syscalls.GetLastError() : 0;
			}
			finally
			{
				Marshal.FreeHGlobal(ifrPtr);
			}
		}
		finally
		{
			Syscalls.close(sockfd);
		}
	}

	/// <summary>
	/// Configure a network interface with IP and netmask.
	/// </summary>
	public static (int result, string message) ConfigureInterface(string interfaceName, string ipAddress, string netmask)
	{
		if (!IPAddress.TryParse(ipAddress, out var ip))
			return (-1, $"Invalid IP address: {ipAddress}");

		if (!IPAddress.TryParse(netmask, out var mask))
			return (-1, $"Invalid netmask: {netmask}");

		// Set IP address
		int result = SetAddress(interfaceName, ip);
		if (result != 0)
			return (result, $"Failed to set IP address: error {result}");

		// Set netmask
		result = SetNetmask(interfaceName, mask);
		if (result != 0)
			return (result, $"Failed to set netmask: error {result}");

		// Bring interface up
		result = InterfaceUp(interfaceName);
		if (result != 0)
			return (result, $"Failed to bring interface up: error {result}");

		return (0, $"Interface {interfaceName} configured with {ipAddress}/{netmask}");
	}
}

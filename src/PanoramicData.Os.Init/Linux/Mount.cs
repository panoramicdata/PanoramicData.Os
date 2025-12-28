namespace PanoramicData.Os.Init.Linux;

/// <summary>
/// Provides filesystem mounting operations.
/// </summary>
public static class Mount
{
    /// <summary>
    /// Mount a filesystem.
    /// </summary>
    /// <param name="source">The device or source to mount (e.g., "proc", "/dev/sda1").</param>
    /// <param name="target">The mount point (e.g., "/proc").</param>
    /// <param name="fsType">The filesystem type (e.g., "proc", "ext4").</param>
    /// <param name="flags">Mount flags.</param>
    /// <param name="data">Filesystem-specific options.</param>
    /// <returns>0 on success, or the errno value on failure.</returns>
    public static int MountFs(string? source, string target, string? fsType, ulong flags, string? data)
    {
        var result = Syscalls.mount(source, target, fsType, flags, data);
        
        if (result != 0)
        {
            return Syscalls.GetLastError();
        }
        
        return 0;
    }

    /// <summary>
    /// Unmount a filesystem.
    /// </summary>
    /// <param name="target">The mount point to unmount.</param>
    /// <returns>0 on success, or the errno value on failure.</returns>
    public static int UnmountFs(string target)
    {
        var result = Syscalls.umount(target);
        
        if (result != 0)
        {
            return Syscalls.GetLastError();
        }
        
        return 0;
    }

    /// <summary>
    /// Sync all filesystems and unmount in reverse order for shutdown.
    /// </summary>
    public static void PrepareForShutdown()
    {
        // Sync all filesystems
        Syscalls.sync();
        
        // Unmount in reverse order
        // Note: In practice, we'd need to handle busy filesystems
        Syscalls.umount2("/run", 0);
        Syscalls.umount2("/tmp", 0);
        Syscalls.umount2("/dev/pts", 0);
        Syscalls.umount2("/dev", 0);
        Syscalls.umount2("/sys", 0);
        Syscalls.umount2("/proc", 0);
        
        // Final sync
        Syscalls.sync();
    }

    /// <summary>
    /// Mount all essential filesystems for boot.
    /// </summary>
    /// <returns>True if all mounts succeeded, false otherwise.</returns>
    public static bool MountEssentialFilesystems()
    {
        var success = true;

        // Mount /proc
        if (MountFs("proc", "/proc", "proc", 0, null) != 0)
        {
            success = false;
        }

        // Mount /sys
        if (MountFs("sysfs", "/sys", "sysfs", 0, null) != 0)
        {
            success = false;
        }

        // Mount /dev (devtmpfs)
        if (MountFs("devtmpfs", "/dev", "devtmpfs", 0, null) != 0)
        {
            // May already be mounted by kernel
        }

        return success;
    }
}

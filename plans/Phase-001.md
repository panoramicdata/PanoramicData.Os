# Phase 1: Foundation & Boot

**Duration:** 3 weeks  
**Dependencies:** None  
**Owner:** TBD  
**Status:** 🔲 Not Started

---

## Objectives

1. Establish a reproducible Docker-based build environment
2. Configure Buildroot for minimal Linux kernel
3. Create .NET 10 NativeAOT init process that replaces traditional init
4. Boot successfully on QEMU with serial console output
5. Set up GitHub Actions CI pipeline

---

## Prerequisites

- Docker Desktop or Docker Engine installed
- .NET 10 SDK available
- GitHub repository created
- Basic understanding of Linux boot process

---

## Tasks

### 1.1 Docker Build Environment

| ID | Task | Effort | Complexity | Acceptance Criteria |
| -- | ---- | ------ | ---------- | ------------------- |
| 1.1.1 | Create Dockerfile for build environment | 4h | Low | Dockerfile builds successfully |
| 1.1.2 | Install Buildroot in container | 2h | Low | `make menuconfig` runs |
| 1.1.3 | Install .NET 10 SDK in container | 2h | Low | `dotnet --version` shows 10.x |
| 1.1.4 | Install cross-compilation toolchain | 4h | Medium | Can compile for x86_64-linux-musl |
| 1.1.5 | Create build.sh entry script | 2h | Low | Single command builds full image |
| 1.1.6 | Add volume mounts for caching | 2h | Low | Incremental builds are fast |
| 1.1.7 | Document build process | 2h | Low | README has clear instructions |

**Subtotal:** 18 hours

### 1.2 Linux Kernel Configuration

| ID | Task | Effort | Complexity | Acceptance Criteria |
| -- | ---- | ------ | ---------- | ------------------- |
| 1.2.1 | Select kernel version (6.x LTS) | 1h | Low | Version documented in config |
| 1.2.2 | Create minimal kernel config | 8h | High | Kernel < 10 MB compressed |
| 1.2.3 | Enable required drivers: virtio, E1000 | 2h | Medium | Drivers built into kernel |
| 1.2.4 | Enable DRM/KMS for graphics | 2h | Medium | Framebuffer available |
| 1.2.5 | Enable evdev for input | 1h | Low | Input events readable |
| 1.2.6 | Disable unnecessary subsystems | 4h | Medium | No sound, USB mass storage, etc. |
| 1.2.7 | Enable serial console (ttyS0) | 1h | Low | Console output visible |
| 1.2.8 | Test kernel boots to panic (no init) | 2h | Low | Kernel panic: no init found |

**Subtotal:** 21 hours

### 1.3 .NET NativeAOT Init Process

| ID | Task | Effort | Complexity | Acceptance Criteria |
| -- | ---- | ------ | ---------- | ------------------- |
| 1.3.1 | Create PanoramicData.Os.Init project | 2h | Low | Project compiles |
| 1.3.2 | Configure NativeAOT publishing | 4h | Medium | Produces static binary |
| 1.3.3 | Target linux-musl-x64 runtime | 2h | Medium | Binary runs on musl libc |
| 1.3.4 | Implement minimal init (print message) | 2h | Low | "Hello from .NET!" on serial |
| 1.3.5 | Add infinite loop (prevent exit) | 1h | Low | System doesn't panic |
| 1.3.6 | Set up signal handlers (SIGTERM, etc.) | 4h | Medium | Clean shutdown possible |
| 1.3.7 | Mount essential filesystems (proc, sys, dev) | 4h | Medium | /proc, /sys, /dev mounted |
| 1.3.8 | Implement basic logging to serial | 4h | Medium | Structured log output |
| 1.3.9 | Binary size optimization | 4h | Medium | Init binary < 5 MB |

**Subtotal:** 27 hours

### 1.4 Root Filesystem Assembly

| ID | Task | Effort | Complexity | Acceptance Criteria |
| -- | ---- | ------ | ---------- | ------------------- |
| 1.4.1 | Configure Buildroot for minimal rootfs | 4h | Medium | No busybox, no shell |
| 1.4.2 | Add musl libc (static or minimal dynamic) | 2h | Low | libc available for CEF later |
| 1.4.3 | Create directory structure | 2h | Low | /bin, /etc, /dev, /proc, /sys |
| 1.4.4 | Place init binary at /init | 1h | Low | Kernel finds init |
| 1.4.5 | Create /etc/hostname | 1h | Low | Hostname set |
| 1.4.6 | Generate initramfs (cpio) | 2h | Low | initramfs.cpio created |
| 1.4.7 | Alternative: ext2/FAT32 disk image | 4h | Medium | Bootable disk image |

**Subtotal:** 16 hours

### 1.5 QEMU Boot Testing

| ID | Task | Effort | Complexity | Acceptance Criteria |
| -- | ---- | ------ | ---------- | ------------------- |
| 1.5.1 | Create run-qemu.sh script | 2h | Low | Single command boots VM |
| 1.5.2 | Configure QEMU with serial console | 1h | Low | Console redirects to terminal |
| 1.5.3 | Add memory and CPU options | 1h | Low | Reasonable defaults (2GB, 2 CPU) |
| 1.5.4 | Test boot to init message | 2h | Low | "Hello from .NET!" visible |
| 1.5.5 | Measure boot time | 1h | Low | Boot time < 3 seconds |
| 1.5.6 | Add graphics output option | 2h | Low | -display gtk shows window |
| 1.5.7 | Create expect script for CI | 4h | Medium | Automated boot verification |

**Subtotal:** 13 hours

### 1.6 GitHub Actions CI

| ID | Task | Effort | Complexity | Acceptance Criteria |
| -- | ---- | ------ | ---------- | ------------------- |
| 1.6.1 | Create .github/workflows/build.yml | 2h | Low | Workflow triggers on push |
| 1.6.2 | Build Docker image in CI | 2h | Low | Docker builds successfully |
| 1.6.3 | Build OS image in CI | 2h | Low | Image artifact produced |
| 1.6.4 | Run QEMU boot test in CI | 4h | Medium | Boot test passes |
| 1.6.5 | Cache Docker layers | 2h | Low | Builds are faster |
| 1.6.6 | Cache Buildroot downloads | 2h | Low | No repeated downloads |
| 1.6.7 | Upload image as artifact | 1h | Low | Image downloadable |
| 1.6.8 | Add status badge to README | 1h | Low | Badge shows build status |

**Subtotal:** 16 hours

---

## Technical Details

### Kernel Configuration Key Options

```text
# Essential
CONFIG_64BIT=y
CONFIG_SMP=y
CONFIG_PRINTK=y
CONFIG_SERIAL_8250=y
CONFIG_SERIAL_8250_CONSOLE=y

# Networking (for later phases)
CONFIG_NET=y
CONFIG_INET=y
CONFIG_IPV6=y

# Graphics
CONFIG_DRM=y
CONFIG_DRM_FBDEV_EMULATION=y
CONFIG_FB=y

# Input
CONFIG_INPUT=y
CONFIG_INPUT_EVDEV=y
CONFIG_INPUT_KEYBOARD=y
CONFIG_INPUT_MOUSE=y

# Virtio (QEMU)
CONFIG_VIRTIO=y
CONFIG_VIRTIO_PCI=y
CONFIG_VIRTIO_NET=y
CONFIG_VIRTIO_BLK=y

# Disable
CONFIG_MODULES=n        # Monolithic kernel
CONFIG_SOUND=n
CONFIG_USB_STORAGE=n
CONFIG_WIRELESS=n
```

### .NET Project Structure

```text
src/
└── PanoramicData.Os.Init/
    ├── PanoramicData.Os.Init.csproj
    ├── Program.cs
    ├── Linux/
    │   ├── Syscalls.cs      # P/Invoke declarations
    │   └── Mount.cs         # Mount operations
    └── Logging/
        └── SerialLogger.cs  # Serial console output
```

### NativeAOT Configuration

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <RuntimeIdentifier>linux-musl-x64</RuntimeIdentifier>
    <PublishAot>true</PublishAot>
    <SelfContained>true</SelfContained>
    <InvariantGlobalization>true</InvariantGlobalization>
    <IlcOptimizationPreference>Size</IlcOptimizationPreference>
    <IlcGenerateStackTraceData>false</IlcGenerateStackTraceData>
    <DebuggerSupport>false</DebuggerSupport>
  </PropertyGroup>
</Project>
```

### Key P/Invoke Syscalls

```csharp
// Mount filesystem
[LibraryImport("libc", SetLastError = true)]
internal static partial int mount(
    [MarshalAs(UnmanagedType.LPStr)] string source,
    [MarshalAs(UnmanagedType.LPStr)] string target,
    [MarshalAs(UnmanagedType.LPStr)] string filesystemtype,
    ulong mountflags,
    nint data);

// Reboot system
[LibraryImport("libc", SetLastError = true)]
internal static partial int reboot(int magic, int magic2, int cmd, nint arg);
```

---

## Testing Requirements

### Unit Tests

| Test | Description |
| ---- | ----------- |
| SerialLogger_WritesOutput | Logger writes to file descriptor |
| Mount_ProcFs_Succeeds | /proc mounts correctly |
| Mount_SysFs_Succeeds | /sys mounts correctly |
| SignalHandler_CatchesSigterm | SIGTERM is handled |

### Integration Tests

| Test | Description |
| ---- | ----------- |
| Init_StartsSuccessfully | Init process starts without crash |
| Init_MountsFilesystems | All essential mounts present |
| Init_LogsToSerial | Messages appear on serial console |

### Boot Tests (CI)

| Test | Description |
| ---- | ----------- |
| QEMU_Boots | System boots without kernel panic |
| QEMU_ShowsInitMessage | "Hello from .NET!" appears |
| QEMU_BootTime | Boot completes in < 5 seconds |

---

## Deliverables

1. **Docker build environment** - Dockerfile and build scripts
2. **Kernel configuration** - Minimal Linux kernel config
3. **Init binary** - PanoramicData.Os.Init compiled for linux-musl-x64
4. **Boot image** - Bootable kernel + initramfs
5. **QEMU scripts** - run-qemu.sh for local testing
6. **CI pipeline** - GitHub Actions workflow
7. **Documentation** - README with build and run instructions

---

## Risks

| Risk | Probability | Impact | Mitigation |
| ---- | ----------- | ------ | ---------- |
| NativeAOT linux-musl-x64 issues | Medium | High | Test early; have alpine fallback |
| Kernel config too minimal | Low | Medium | Add drivers incrementally |
| Init binary too large | Medium | Low | Optimize; strip symbols |
| Docker build slow | Low | Low | Layer caching; volume mounts |

---

## Exit Criteria

- [ ] Docker build completes in < 10 minutes (cached)
- [ ] Kernel boots to init without panic
- [ ] "Hello from .NET!" appears on serial console
- [ ] /proc, /sys, /dev are mounted
- [ ] Boot time < 5 seconds
- [ ] GitHub Actions CI passes
- [ ] All unit tests pass
- [ ] Documentation complete

---

## Demo Milestone

**Demo:** Boot to .NET init with serial console output showing "Hello from .NET!"

```sh
$ ./run-qemu.sh
[    0.000000] Linux version 6.x.x ...
[    0.100000] Kernel command line: console=ttyS0
...
[    1.500000] Run /init as init process
[    1.600000] [PanoramicData.Os] Hello from .NET!
[    1.601000] [PanoramicData.Os] Mounting /proc...
[    1.602000] [PanoramicData.Os] Mounting /sys...
[    1.603000] [PanoramicData.Os] Mounting /dev...
[    1.604000] [PanoramicData.Os] Init complete. System ready.
```

---

## Estimated Total Effort

| Section | Hours |
| ------- | ----- |
| 1.1 Docker Build Environment | 18 |
| 1.2 Linux Kernel Configuration | 21 |
| 1.3 .NET NativeAOT Init Process | 27 |
| 1.4 Root Filesystem Assembly | 16 |
| 1.5 QEMU Boot Testing | 13 |
| 1.6 GitHub Actions CI | 16 |
| **Total** | **111 hours** |

At 40 hours/week = **~3 weeks**

---

## References

- [Buildroot Documentation](https://buildroot.org/docs.html)
- [.NET NativeAOT Deployment](https://learn.microsoft.com/en-us/dotnet/core/deploying/native-aot/)
- [Linux Kernel Configuration](https://www.kernel.org/doc/html/latest/admin-guide/README.html)
- [QEMU Documentation](https://www.qemu.org/docs/master/)

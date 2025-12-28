# PanoramicData.Os

[![Build](https://github.com/panoramicdata/PanoramicData.Os/actions/workflows/build.yml/badge.svg)](https://github.com/panoramicdata/PanoramicData.Os/actions/workflows/build.yml)

A custom operating system with a minimal Linux kernel and pure .NET 10 NativeAOT userspace, featuring a browser-based windowing system built on Chromium/CEF.

## Features

- **Minimal Linux Kernel**: Stripped-down kernel optimized for virtualization
- **Pure .NET Userspace**: All userspace code written in C# using NativeAOT
- **Browser-Based GUI**: Windows and applications rendered via Chromium Embedded Framework
- **SSH Server**: Built-in SSH server for remote access
- **Custom Shell**: Basic shell with command-line utilities

## Quick Start

### Prerequisites

- Docker Desktop or Docker Engine
- .NET 10 SDK (for development)
- QEMU (for testing)

### Building

```bash
# Linux/macOS
./scripts/build.sh

# Windows PowerShell
.\scripts\build.ps1
```

This will:
1. Build the Docker build environment (first run only)
2. Compile the .NET init process
3. Build the Linux kernel
4. Create the initramfs
5. Output `vmlinuz` and `initramfs.cpio.gz` to the `output/` directory

### Running

```bash
# Linux/macOS
./scripts/run-qemu.sh

# Windows PowerShell
.\scripts\run-qemu.ps1

# With graphics
./scripts/run-qemu.sh --graphics
.\scripts\run-qemu.ps1 -Graphics
```

Press `Ctrl+A, X` to exit QEMU.

### SSH Access

Once the full system is running (Phase 3+), you can SSH in:

```bash
ssh -p 2222 root@localhost
```

## Project Structure

```
PanoramicData.Os/
├── configs/
│   └── kernel.config       # Linux kernel configuration
├── output/                  # Build output (gitignored)
│   ├── vmlinuz             # Linux kernel
│   └── initramfs.cpio.gz   # Initial RAM filesystem
├── plans/                   # Implementation plans
│   ├── MASTER_PLAN.md
│   ├── Phase-001.md        # Foundation & Boot
│   ├── Phase-002.md        # Network Stack
│   └── ...
├── scripts/
│   ├── build.sh            # Build script (Linux/macOS)
│   ├── build.ps1           # Build script (Windows)
│   ├── run-qemu.sh         # QEMU runner (Linux/macOS)
│   ├── run-qemu.ps1        # QEMU runner (Windows)
│   ├── boot-test.exp       # Automated boot test
│   └── ...
├── src/
│   └── PanoramicData.Os.Init/
│       ├── Program.cs      # Init process entry point
│       ├── Linux/          # Linux syscall bindings
│       └── Logging/        # Serial console logging
├── Dockerfile              # Build environment
└── README.md
```

## Development

### Kernel Configuration

To modify kernel configuration:

```bash
docker run -it --rm \
    -v $(pwd):/workspace \
    panos-builder \
    kernel-menuconfig
```

### Adding New .NET Projects

New projects should be added under `src/` and follow the NativeAOT configuration pattern in `PanoramicData.Os.Init.csproj`.

### Testing

Run the automated boot test:

```bash
./scripts/boot-test.exp
```

## Roadmap

- [x] Phase 1: Foundation & Boot
- [ ] Phase 2: Network Stack
- [ ] Phase 3: SSH Server
- [ ] Phase 4: Graphics & Input
- [ ] Phase 5: Chromium Integration
- [ ] Phase 6: Window Manager
- [ ] Phase 7: Built-in Apps
- [ ] Phase 8: Multi-VM Support & Polish

See [plans/MASTER_PLAN.md](plans/MASTER_PLAN.md) for detailed implementation plans.

## Target Platforms

- QEMU (primary development target)
- VirtualBox
- Hyper-V
- VMware

## Requirements

| Resource | Minimum | Recommended |
|----------|---------|-------------|
| RAM | 256 MB | 512 MB |
| Disk | 128 MB | 256 MB |
| CPUs | 1 | 2+ |

## License

[License TBD]

## Contributing

[Contribution guidelines TBD]

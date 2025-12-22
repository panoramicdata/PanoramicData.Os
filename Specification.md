# PanoramicData.Os - Specification

## Overview

PanoramicData.Os is a custom operating system built on a minimal Linux kernel with a pure .NET 10 userspace. The system provides a browser-based graphical interface where all windows are opened via URLs.

---

## Core Architecture

| Layer | Technology | Notes |
| ----- | ---------- | ----- |
| **Kernel** | Linux (minimal) | Hardware abstraction only |
| **Userspace** | .NET 10 NativeAOT | Everything above kernel |
| **Graphics** | Chromium/CEF | Browser-based windowing |
| **Init** | .NET binary | Replaces traditional init system |

### Key Constraint

**No traditional Linux userspace.** No systemd, no glibc applications, no bash/shell - only:

- Linux kernel
- .NET 10 NativeAOT compiled binaries
- Chromium/CEF binary (for browser rendering)

---

## Target Platforms

The OS must boot and function correctly on all of the following virtualization platforms:

| Platform | Priority | Notes |
| -------- | -------- | ----- |
| QEMU | Primary | Development and CI testing |
| VirtualBox | Primary | Common developer VM |
| Hyper-V | Primary | Windows-native, enterprise |
| VMware | Primary | Enterprise workloads |

---

## Networking

### Network Interface Drivers

Support NICs commonly available across all target VMs:

- Intel E1000 (emulated on most VMs)
- AMD PCnet-III (VirtualBox default)
- Realtek RTL8139 (legacy, widely supported)

### Protocol Support

| Protocol | Version | Status |
| ---------- | --------- | -------- |
| IPv4 | Full | Required |
| IPv6 | Full | Required |
| TCP | Full | Required |
| UDP | Full | Required |
| ICMP | v4 + v6 | Required |
| ARP | - | Required |
| NDP | - | Required (IPv6) |

### IP Configuration

| Method | Support |
| ------ | ------- |
| DHCPv4 | Required |
| DHCPv6 | Required |
| SLAAC | Required |
| Static IPv4 | Required |
| Static IPv6 | Required |

---

## SSH Access

### Authentication

- **Public key only** - no password authentication
- Supported key types: Ed25519, RSA (4096-bit minimum)
- Authorized keys stored on boot disk

### Shell

- Custom ANSI color terminal
- Built-in commands (no external binaries)

### Shell Commands

| Command | Description |
| --------- | ------------- |
| `help` | Display available commands |
| `ls` | List directory contents |
| `cd` | Change directory |
| `cat` | Display file contents |
| `pwd` | Print working directory |
| `clear` | Clear terminal |
| `ping` | ICMP ping |
| `ifconfig` | Network interface configuration |
| `ip` | IP configuration (modern) |
| `reboot` | Reboot system |
| `shutdown` | Shutdown system |
| `poweroff` | Power off immediately |
| `mount` | Mount filesystems |
| `df` | Disk usage |
| `free` | Memory usage |
| `ps` | Process list |
| `top` | Process monitor |
| `date` | Display date/time |
| `hostname` | Display/set hostname |
| `dmesg` | Kernel messages |
| `netstat` | Network statistics |
| `ss` | Socket statistics |
| `route` | Routing table |

---

## Disk / Filesystem

### Initial Scope

- Boot disk only (single disk support)
- Read-write access

### Filesystem

- FAT32 (maximum compatibility)
- Alternative: ext2 (if FAT32 proves limiting)

### Storage Driver

- IDE/ATA (PIO mode) - works across all VMs
- Future: AHCI, virtio-blk

---

## Graphics / Windowing

### Rendering Engine

- Chromium Embedded Framework (CEF)
- Full ES2024+ JavaScript support
- Modern CSS support
- WebAssembly support

### Window Management

| Feature | Behavior |
| ------- | -------- |
| **Default mode** | Floating windows |
| **Tiling** | Snap to edges/corners |
| **Fullscreen** | F11 or double-click title |
| **Taskbar** | None |
| **Launcher** | Super key opens switcher/launcher |
| **Window switching** | Super key or Alt+Tab |

### URL-Based Windows

All windows are opened via URL:

| URL Scheme | Purpose |
| ---------- | ------- |
| `panos://terminal` | Terminal/shell |
| `panos://settings` | System settings |
| `panos://files` | File manager |
| `panos://launcher` | App launcher |
| `https://...` | External websites |
| `file:///...` | Local HTML files |

### Input Support

| Input Method | Support |
| ------------ | ------- |
| Keyboard | PS/2, USB HID |
| Mouse | PS/2, USB HID |
| Touch | Multi-touch via libinput |

---

## Development Requirements

### Build Environment

- **Docker-based** build system
- Must build on Windows, macOS, and Linux hosts
- Reproducible builds

### Continuous Integration

- GitHub Actions workflows
- Automated image builds on every commit
- VM-based boot tests (QEMU)

### Testing

| Test Type | Tooling |
| --------- | ------- |
| Unit tests | xUnit / NUnit |
| Integration tests | Custom test harness |
| E2E tests | Playwright (browser testing) |
| Boot tests | QEMU + expect scripts |

### Debugging

- Serial console output (COM1)
- Kernel messages via dmesg
- .NET diagnostics where possible

---

## Licensing

- **Open source**
- Preferred: MIT or Apache 2.0
- All dependencies must be compatible

---

## Non-Goals (Initial Release)

The following are explicitly out of scope for v1.0:

- Multiple user accounts
- USB device hotplug
- Audio support
- Printing
- Multiple monitors
- GPU acceleration (software rendering only)
- Bare metal installation
- Package manager
- Third-party app installation

---

## Success Criteria

### Minimum Viable Product (MVP)

1. ✅ Boots on QEMU, VirtualBox, Hyper-V, VMware
2. ✅ Obtains IP address via DHCP (v4 and v6)
3. ✅ Accepts SSH connections with public key auth
4. ✅ Provides functional shell with listed commands
5. ✅ Displays graphical browser-based desktop
6. ✅ Opens windows via URL
7. ✅ Window management (float, tile, fullscreen)
8. ✅ Super key launcher works
9. ✅ Browses public websites
10. ✅ Boot time < 10 seconds (to graphical desktop)

---

## Version History

| Version | Date | Author | Changes |
| ------- | ---- | ------ | ------- |
| 0.1 | 2024-12-22 | Initial | Initial specification |

# PanoramicData.Os - Master Implementation Plan

## Project Overview

PanoramicData.Os is a custom operating system built on a minimal Linux kernel with a pure .NET 10 NativeAOT userspace. The system provides a browser-based graphical interface where all windows are opened via URLs, eliminating traditional Linux userspace components (no systemd, no glibc, no bash).

### Goals

- Boot on QEMU, VirtualBox, Hyper-V, and VMware
- Provide a fully functional .NET 10-based userspace
- Deliver a browser-based desktop using Chromium/CEF
- Support SSH access with public key authentication
- Achieve boot-to-desktop in under 10 seconds

---

## Architecture Diagram

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                              User Interface                                  │
├─────────────────────────────────────────────────────────────────────────────┤
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐  ┌─────────────────────┐ │
│  │  Terminal   │  │  Settings   │  │   Files     │  │  External URLs      │ │
│  │  panos://   │  │  panos://   │  │  panos://   │  │  https://...        │ │
│  └──────┬──────┘  └──────┬──────┘  └──────┬──────┘  └──────────┬──────────┘ │
│         └────────────────┴────────────────┴────────────────────┘            │
│                                    │                                         │
│  ┌─────────────────────────────────▼───────────────────────────────────────┐│
│  │                     Window Manager (.NET 10)                            ││
│  │           Floating • Tiling • Fullscreen • Super Key Launcher           ││
│  └─────────────────────────────────┬───────────────────────────────────────┘│
│                                    │                                         │
│  ┌─────────────────────────────────▼───────────────────────────────────────┐│
│  │                  Chromium Embedded Framework (CEF)                       ││
│  │              ES2024+ JavaScript • WebAssembly • Modern CSS               ││
│  └─────────────────────────────────┬───────────────────────────────────────┘│
├────────────────────────────────────┼────────────────────────────────────────┤
│                              System Services                                 │
├────────────────────────────────────┼────────────────────────────────────────┤
│  ┌──────────────┐  ┌──────────────┴──────────────┐  ┌──────────────────────┐│
│  │  SSH Server  │  │      Graphics & Input       │  │   Network Stack      ││
│  │  Ed25519     │  │  DRM/KMS • evdev • libinput │  │   IPv4/IPv6          ││
│  │  RSA 4096    │  │  KB • Mouse • Touch         │  │   TCP/UDP/ICMP       ││
│  └──────┬───────┘  └──────────────┬──────────────┘  │   DHCP • DNS         ││
│         │                         │                  └───────────┬──────────┘│
│  ┌──────┴─────────────────────────┴──────────────────────────────┴─────────┐│
│  │                        .NET 10 NativeAOT Runtime                        ││
│  │                    Init Process • System Services                        ││
│  └─────────────────────────────────┬───────────────────────────────────────┘│
├────────────────────────────────────┼────────────────────────────────────────┤
│                                    │                                         │
│  ┌─────────────────────────────────▼───────────────────────────────────────┐│
│  │                     Linux Kernel (Minimal)                               ││
│  │         Hardware Abstraction • Drivers • Memory Management               ││
│  └─────────────────────────────────┬───────────────────────────────────────┘│
├────────────────────────────────────┼────────────────────────────────────────┤
│                                    │                                         │
│  ┌─────────────────────────────────▼───────────────────────────────────────┐│
│  │                           Hardware / VM                                  ││
│  │              QEMU • VirtualBox • Hyper-V • VMware                        ││
│  └─────────────────────────────────────────────────────────────────────────┘│
└─────────────────────────────────────────────────────────────────────────────┘
```

---

## Technology Stack

| Component | Technology | Version | Purpose |
| --------- | ---------- | ------- | ------- |
| **Kernel** | Linux | 6.x LTS | Hardware abstraction, drivers |
| **Build System** | Buildroot | Latest | Cross-compilation, rootfs generation |
| **Runtime** | .NET | 10 | Application runtime |
| **Compilation** | NativeAOT | .NET 10 | Ahead-of-time compilation |
| **Graphics** | CEF | Latest | Browser-based rendering |
| **Display** | DRM/KMS | Kernel | Framebuffer management |
| **Input** | evdev/libinput | - | Keyboard, mouse, touch input |
| **Networking** | Custom .NET | - | Full TCP/IP stack |
| **SSH** | Custom .NET | SSH-2 | Remote access |
| **Build Container** | Docker | - | Reproducible builds |
| **CI/CD** | GitHub Actions | - | Automated testing |
| **Browser Testing** | Playwright | - | E2E testing |

---

## Phase Summary

| Phase | Name | Duration | Dependencies | Status | Document |
| ----- | ---- | -------- | ------------ | ------ | -------- |
| 1 | Foundation & Boot | 3 weeks | None | 🔲 Not Started | [Phase-001.md](Phase-001.md) |
| 2 | Network Stack | 3 weeks | Phase 1 | 🔲 Not Started | [Phase-002.md](Phase-002.md) |
| 3 | SSH Server | 3 weeks | Phase 2 | 🔲 Not Started | [Phase-003.md](Phase-003.md) |
| 4 | Graphics & Input | 3 weeks | Phase 1 | 🔲 Not Started | [Phase-004.md](Phase-004.md) |
| 5 | Chromium Integration | 4 weeks | Phase 4 | 🔲 Not Started | [Phase-005.md](Phase-005.md) |
| 6 | Window Manager | 3 weeks | Phase 5 | 🔲 Not Started | [Phase-006.md](Phase-006.md) |
| 7 | Built-in Apps | 3 weeks | Phase 6 | 🔲 Not Started | [Phase-007.md](Phase-007.md) |
| 8 | Multi-VM Support & Polish | 4 weeks | All Phases | 🔲 Not Started | [Phase-008.md](Phase-008.md) |
| 9 | Object Streaming Infrastructure | 4 weeks | Phase 1 | 🟡 In Progress | [Phase-009.md](Phase-009.md) |

**Total Duration:** 30 weeks (approximately 7 months)

---

## Timeline

```
Week:  1   2   3   4   5   6   7   8   9  10  11  12  13  14  15  16  17  18  19  20  21  22  23  24  25  26
       │   │   │   │   │   │   │   │   │   │   │   │   │   │   │   │   │   │   │   │   │   │   │   │   │   │
P1 ════╪═══╪═══╡   │   │   │   │   │   │   │   │   │   │   │   │   │   │   │   │   │   │   │   │   │   │   │
       │   │   │   │   │   │   │   │   │   │   │   │   │   │   │   │   │   │   │   │   │   │   │   │   │   │
P2     │   │   ├═══╪═══╪═══╡   │   │   │   │   │   │   │   │   │   │   │   │   │   │   │   │   │   │   │   │
       │   │   │   │   │   │   │   │   │   │   │   │   │   │   │   │   │   │   │   │   │   │   │   │   │   │
P3     │   │   │   │   │   ├═══╪═══╪═══╡   │   │   │   │   │   │   │   │   │   │   │   │   │   │   │   │   │
       │   │   │   │   │   │   │   │   │   │   │   │   │   │   │   │   │   │   │   │   │   │   │   │   │   │
P4     │   │   ├═══╪═══╪═══╪═══╪═══╪═══╡   │   │   │   │   │   │   │   │   │   │   │   │   │   │   │   │   │
       │   │   │   │   │   │   │   │   │   │   │   │   │   │   │   │   │   │   │   │   │   │   │   │   │   │
P5     │   │   │   │   │   │   │   │   ├═══╪═══╪═══╪═══╡   │   │   │   │   │   │   │   │   │   │   │   │   │
       │   │   │   │   │   │   │   │   │   │   │   │   │   │   │   │   │   │   │   │   │   │   │   │   │   │
P6     │   │   │   │   │   │   │   │   │   │   │   │   ├═══╪═══╪═══╡   │   │   │   │   │   │   │   │   │   │
       │   │   │   │   │   │   │   │   │   │   │   │   │   │   │   │   │   │   │   │   │   │   │   │   │   │
P7     │   │   │   │   │   │   │   │   │   │   │   │   │   │   │   ├═══╪═══╪═══╡   │   │   │   │   │   │   │
       │   │   │   │   │   │   │   │   │   │   │   │   │   │   │   │   │   │   │   │   │   │   │   │   │   │
P8     │   │   │   │   │   │   │   │   │   │   │   │   │   │   │   │   │   │   ├═══╪═══╪═══╪═══╡   │   │   │
       │   │   │   │   │   │   │   │   │   │   │   │   │   │   │   │   │   │   │   │   │   │   │   │   │   │

Legend: ═══ Active Development  ╡ Milestone/Demo
```

### Parallel Tracks

Phases 2 and 4 can run in parallel after Phase 1 completes:

- **Track A (CLI):** Phase 1 → Phase 2 → Phase 3
- **Track B (GUI):** Phase 1 → Phase 4 → Phase 5 → Phase 6 → Phase 7

Phase 8 begins after both tracks converge.

---

## Risk Register

| ID | Risk | Probability | Impact | Mitigation | Owner |
| -- | ---- | ----------- | ------ | ---------- | ----- |
| R1 | NativeAOT reflection limitations break CEF integration | Medium | High | Early proof-of-concept in Phase 5; fallback to source generators | TBD |
| R2 | Network stack performance insufficient for real use | Low | Medium | Profile early; optimize hot paths; consider kernel bypass | TBD |
| R3 | CEF binary size bloats boot image | High | Medium | Compress aggressively; lazy load; strip unused features | TBD |
| R4 | VM driver compatibility issues across platforms | Medium | High | Test early and often on all 4 VMs; maintain compat layer | TBD |
| R5 | Boot time exceeds 10-second target | Medium | Medium | Profile boot sequence; parallelize init; optimize NativeAOT | TBD |
| R6 | SSH implementation security vulnerabilities | Low | Critical | Use well-tested crypto primitives; security audit | TBD |
| R7 | DRM/KMS complexity with different VM graphics adapters | Medium | Medium | Focus on simple framebuffer; avoid advanced features | TBD |
| R8 | .NET 10 NativeAOT bugs or missing features | Low | High | Track .NET releases; have workarounds ready | TBD |
| R9 | Build reproducibility issues across host OSes | Medium | Low | Docker-only builds; pin all versions; CI verification | TBD |
| R10 | IPv6 stack complexity delays Phase 2 | Medium | Low | Implement IPv4 first; IPv6 as secondary milestone | TBD |

---

## External Dependencies

| Dependency | Purpose | License | NativeAOT Compatible |
| ---------- | ------- | ------- | -------------------- |
| **Linux Kernel** | Hardware abstraction | GPL-2.0 | N/A (kernel space) |
| **Buildroot** | Build system | GPL-2.0 | N/A (build-time) |
| **.NET 10 SDK** | Build runtime | MIT | Yes |
| **CEF** | Browser rendering | BSD-3-Clause | Requires P/Invoke |
| **musl libc** | Minimal C library for CEF | MIT | N/A (CEF dependency) |
| **libdrm** | DRM/KMS access | MIT | Via P/Invoke |
| **libinput** | Input device handling | MIT | Via P/Invoke |
| **xterm.js** | Terminal frontend | MIT | Yes (JavaScript) |
| **Playwright** | E2E testing | Apache-2.0 | N/A (test-time) |

### Cryptographic Libraries

| Library | Purpose | License |
| ------- | ------- | ------- |
| **BouncyCastle** or **.NET Crypto** | SSH key exchange, signatures | MIT / .NET Library License |
| **libsodium** (optional) | High-performance crypto | ISC |

---

## Success Metrics

### Minimum Viable Product (MVP) Criteria

| ID | Criterion | Measurement | Target |
| -- | --------- | ----------- | ------ |
| M1 | VM Boot Support | Boots successfully on | QEMU, VirtualBox, Hyper-V, VMware |
| M2 | DHCP Connectivity | Obtains IP via DHCP | IPv4 and IPv6 |
| M3 | SSH Access | Connect and authenticate | Public key auth works |
| M4 | Shell Functionality | All listed commands work | 100% of spec commands |
| M5 | Graphical Desktop | Browser-based desktop loads | Renders correctly |
| M6 | URL-based Windows | Open windows via URL | panos:// and https:// work |
| M7 | Window Management | Float, tile, fullscreen | All modes functional |
| M8 | Super Key Launcher | Opens app launcher | < 200ms response |
| M9 | Web Browsing | Load public websites | Passes Acid3 test |
| M10 | Boot Performance | Time to graphical desktop | < 10 seconds |

### Performance Targets

| Metric | Target | Measurement Method |
| ------ | ------ | ------------------ |
| Boot to init | < 2 seconds | Serial console timestamp |
| Init to network | < 3 seconds | DHCP lease acquisition |
| Network to SSH ready | < 1 second | SSH port accepting connections |
| Init to graphical desktop | < 10 seconds | First frame rendered |
| Window open latency | < 500ms | URL to visible content |
| Memory footprint (idle) | < 512 MB | /proc/meminfo |
| Disk image size | < 1 GB | Compressed image |

### Quality Gates

Each phase must pass:

1. ✅ All unit tests pass
2. ✅ Integration tests pass
3. ✅ Demo milestone achieved
4. ✅ Documentation updated
5. ✅ Code review completed
6. ✅ No critical bugs outstanding

---

## Development Environment

### Build Requirements

- Docker Desktop (Windows/macOS) or Docker Engine (Linux)
- 16 GB RAM minimum (32 GB recommended)
- 50 GB free disk space
- Git

### Quick Start

```bash
# Clone repository
git clone https://github.com/panoramicdata/PanoramicData.Os.git
cd PanoramicData.Os

# Build the OS image
./build.sh

# Run in QEMU
./run-qemu.sh
```

### CI/CD Pipeline

```
┌─────────────────────────────────────────────────────────────────────┐
│                        GitHub Actions                                │
├─────────────────────────────────────────────────────────────────────┤
│  Push/PR                                                             │
│     │                                                                │
│     ▼                                                                │
│  ┌──────────┐   ┌──────────┐   ┌──────────┐   ┌──────────────────┐  │
│  │  Build   │──▶│  Test    │──▶│  Boot    │──▶│  Publish Image   │  │
│  │  Docker  │   │  Unit    │   │  QEMU    │   │  (on main only)  │  │
│  └──────────┘   └──────────┘   └──────────┘   └──────────────────┘  │
└─────────────────────────────────────────────────────────────────────┘
```

---

## Appendix: Phase Milestones

| Phase | Milestone Demo |
|------ | ---------------|
| 1 | Boot to .NET init, serial console output "Hello from .NET!" |
| 2 | Ping 8.8.8.8 and 2001:4860:4860::8888 successfully |
| 3 | SSH in and run `ls` command |
| 4 | Display test pattern on screen, move mouse cursor |
| 5 | Render google.com in CEF window |
| 6 | Open 3 windows, tile them, Super key shows launcher |
| 7 | Open terminal, run shell command, see output |
| 8 | Boot and pass all tests on all 4 VM platforms |

---

## Version History

| Version | Date | Author | Changes |
|-------- | ---- | ------ | --------|
| 0.1 | 2024-12-22 | AI Generated | Initial master plan |

---

## Next Steps

1. ✅ Create MASTER_PLAN.md (this document)
2. 🔲 Create Phase-001.md through Phase-008.md
3. 🔲 Review and refine all phase documents
4. 🔲 Set up repository structure
5. 🔲 Begin Phase 1 implementation

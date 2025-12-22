# Phase 8: Multi-VM Support & Polish

**Duration:** 4 weeks  
**Dependencies:** Phase 7 (Built-in Apps)  
**Owner:** TBD  
**Status:** 🔲 Not Started

---

## Objectives

1. Optimize for VirtualBox, Hyper-V, and VMware (beyond QEMU)
2. Implement guest additions/tools for each hypervisor
3. Performance tuning across all components
4. Comprehensive documentation
5. Release preparation and CI/CD finalization

---

## Prerequisites

- All previous phases complete
- System functional on QEMU
- All core features implemented

---

## Tasks

### 8.1 VirtualBox Support

| ID | Task | Effort | Complexity | Acceptance Criteria |
| -- | ---- | ------ | ---------- | ------------------- |
| 8.1.1 | VirtualBox VDI image generation | 4h | Medium | VDI created |
| 8.1.2 | VirtualBox boot testing | 4h | Medium | Boots successfully |
| 8.1.3 | VirtualBox graphics driver (VMSVGA) | 8h | High | Display works |
| 8.1.4 | VirtualBox network driver (virtio-net) | 4h | Medium | Network works |
| 8.1.5 | VirtualBox shared folders | 8h | High | Host folders accessible |
| 8.1.6 | VirtualBox guest additions kernel module | 16h | High | Guest additions work |
| 8.1.7 | VirtualBox clipboard integration | 8h | High | Clipboard shared |
| 8.1.8 | VirtualBox auto-resize display | 4h | Medium | Resize with window |
| 8.1.9 | VirtualBox seamless mode investigation | 4h | Medium | Feasibility assessed |
| 8.1.10 | VirtualBox time sync | 2h | Low | Time synced with host |

**Subtotal:** 62 hours

### 8.2 Hyper-V Support

| ID | Task | Effort | Complexity | Acceptance Criteria |
| -- | ---- | ------ | ---------- | ------------------- |
| 8.2.1 | Hyper-V VHDX image generation | 4h | Medium | VHDX created |
| 8.2.2 | Hyper-V Gen1/Gen2 boot testing | 8h | High | Both gen boot |
| 8.2.3 | Hyper-V synthetic network (netvsc) | 8h | High | Network works |
| 8.2.4 | Hyper-V synthetic storage (storvsc) | 8h | High | Storage works |
| 8.2.5 | Hyper-V video driver (hyperv_fb) | 8h | High | Display works |
| 8.2.6 | Hyper-V Integration Services | 16h | High | Services running |
| 8.2.7 | Hyper-V enhanced session mode (RDP) | 16h | High | Enhanced session works |
| 8.2.8 | Hyper-V time sync (vmictimesync) | 2h | Low | Time synced |
| 8.2.9 | Hyper-V heartbeat (vmicheartbeat) | 2h | Low | Heartbeat working |
| 8.2.10 | Hyper-V shutdown (vmicshutdown) | 2h | Low | Graceful shutdown |

**Subtotal:** 74 hours

### 8.3 VMware Support

| ID | Task | Effort | Complexity | Acceptance Criteria |
| -- | ---- | ------ | ---------- | ------------------- |
| 8.3.1 | VMware VMDK image generation | 4h | Medium | VMDK created |
| 8.3.2 | VMware boot testing (Workstation/ESXi) | 8h | High | Boots on both |
| 8.3.3 | VMware SVGA driver (vmwgfx) | 8h | High | Display works |
| 8.3.4 | VMware vmxnet3 network driver | 4h | Medium | Network works |
| 8.3.5 | VMware PVSCSI storage driver | 4h | Medium | Storage works |
| 8.3.6 | VMware tools (open-vm-tools) | 16h | High | VMware tools work |
| 8.3.7 | VMware shared folders (HGFS) | 8h | High | Shared folders mount |
| 8.3.8 | VMware clipboard (drag-and-drop) | 8h | High | Clipboard works |
| 8.3.9 | VMware auto-resize display | 4h | Medium | Resolution follows window |
| 8.3.10 | VMware balloon driver (memory) | 4h | Medium | Balloon works |

**Subtotal:** 68 hours

### 8.4 Performance Optimization

| ID | Task | Effort | Complexity | Acceptance Criteria |
| -- | ---- | ------ | ---------- | ------------------- |
| 8.4.1 | Boot time profiling | 4h | Medium | Metrics collected |
| 8.4.2 | Boot time optimization | 8h | High | < 5s boot target |
| 8.4.3 | Memory usage profiling | 4h | Medium | Memory usage known |
| 8.4.4 | Memory optimization | 8h | High | < 64MB idle target |
| 8.4.5 | Disk I/O profiling | 4h | Medium | I/O patterns known |
| 8.4.6 | Network throughput testing | 4h | Medium | Throughput measured |
| 8.4.7 | Graphics/rendering profiling | 4h | Medium | FPS measured |
| 8.4.8 | CEF memory optimization | 8h | High | CEF memory reduced |
| 8.4.9 | .NET startup optimization | 4h | Medium | Startup faster |
| 8.4.10 | Lazy loading implementation | 4h | Medium | On-demand loading |

**Subtotal:** 52 hours

### 8.5 Stability & Reliability

| ID | Task | Effort | Complexity | Acceptance Criteria |
| -- | ---- | ------ | ---------- | ------------------- |
| 8.5.1 | Crash dump collection | 4h | Medium | Crashes captured |
| 8.5.2 | Watchdog implementation | 4h | Medium | Hung processes detected |
| 8.5.3 | Service restart on failure | 4h | Medium | Services auto-restart |
| 8.5.4 | Graceful shutdown sequence | 4h | Medium | Clean shutdown |
| 8.5.5 | Memory leak detection | 8h | High | Leaks identified |
| 8.5.6 | Stress testing (long-running) | 8h | High | Stable over 24h |
| 8.5.7 | Resource exhaustion handling | 4h | Medium | Handles OOM gracefully |
| 8.5.8 | Logging consolidation | 4h | Medium | Logs centralized |
| 8.5.9 | Error reporting | 4h | Medium | Errors reported |
| 8.5.10 | Recovery mode (safe boot) | 8h | High | Safe mode available |

**Subtotal:** 52 hours

### 8.6 Documentation

| ID | Task | Effort | Complexity | Acceptance Criteria |
| -- | ---- | ------ | ---------- | ------------------- |
| 8.6.1 | README.md (project overview) | 4h | Medium | Clear overview |
| 8.6.2 | INSTALL.md (build instructions) | 4h | Medium | Build works from docs |
| 8.6.3 | ARCHITECTURE.md (design docs) | 8h | High | Architecture clear |
| 8.6.4 | API documentation | 8h | High | APIs documented |
| 8.6.5 | User guide | 8h | High | User can use system |
| 8.6.6 | Developer guide | 8h | High | Dev can contribute |
| 8.6.7 | Troubleshooting guide | 4h | Medium | Common issues covered |
| 8.6.8 | FAQ | 2h | Low | Questions answered |
| 8.6.9 | Release notes template | 2h | Low | Template ready |
| 8.6.10 | License documentation | 2h | Low | Licenses clear |
| 8.6.11 | Video/GIF demos | 8h | High | Visual demos |
| 8.6.12 | Architecture diagrams | 4h | Medium | Diagrams created |

**Subtotal:** 62 hours

### 8.7 CI/CD & Release

| ID | Task | Effort | Complexity | Acceptance Criteria |
| -- | ---- | ------ | ---------- | ------------------- |
| 8.7.1 | Multi-format build (QCOW2, VDI, VHDX, VMDK) | 8h | High | All formats built |
| 8.7.2 | Automated testing matrix | 8h | High | Tests run on all VMs |
| 8.7.3 | Release versioning strategy | 2h | Low | SemVer implemented |
| 8.7.4 | GitHub release automation | 4h | Medium | Releases automated |
| 8.7.5 | Artifact hosting/distribution | 4h | Medium | Downloads available |
| 8.7.6 | Checksum generation | 2h | Low | SHA256 sums |
| 8.7.7 | Digital signing (optional) | 8h | High | Images signed |
| 8.7.8 | Container registry publishing | 4h | Medium | Docker images pushed |
| 8.7.9 | Documentation site (GitHub Pages) | 8h | High | Docs site live |
| 8.7.10 | Changelog automation | 4h | Medium | Changelog generated |

**Subtotal:** 52 hours

### 8.8 Security Hardening

| ID | Task | Effort | Complexity | Acceptance Criteria |
| -- | ---- | ------ | ---------- | ------------------- |
| 8.8.1 | SSH hardening (key-only, no root password) | 2h | Low | Password login disabled |
| 8.8.2 | Filesystem permissions audit | 4h | Medium | Permissions correct |
| 8.8.3 | Kernel security options | 4h | Medium | Security options enabled |
| 8.8.4 | Service privilege dropping | 4h | Medium | Services run minimal privs |
| 8.8.5 | Network firewall (iptables/nftables) | 8h | High | Firewall configured |
| 8.8.6 | Secure defaults audit | 4h | Medium | Defaults are secure |
| 8.8.7 | Dependency vulnerability scan | 4h | Medium | No known vulns |
| 8.8.8 | ASLR/PIE verification | 2h | Low | ASLR/PIE enabled |

**Subtotal:** 32 hours

### 8.9 Quality Assurance

| ID | Task | Effort | Complexity | Acceptance Criteria |
| -- | ---- | ------ | ---------- | ------------------- |
| 8.9.1 | Full test suite execution | 8h | High | All tests pass |
| 8.9.2 | Cross-VM compatibility testing | 16h | High | Works on all VMs |
| 8.9.3 | User acceptance testing | 8h | High | Users approve |
| 8.9.4 | Performance benchmarks | 4h | Medium | Benchmarks documented |
| 8.9.5 | Regression testing | 4h | Medium | No regressions |
| 8.9.6 | Edge case testing | 4h | Medium | Edge cases handled |
| 8.9.7 | Install/upgrade testing | 4h | Medium | Install works |
| 8.9.8 | Bug triage and fixes | 16h | High | Critical bugs fixed |

**Subtotal:** 64 hours

---

## Technical Details

### Multi-Format Image Build

```bash
#!/bin/bash
# build-images.sh

set -e

VERSION=${1:-"0.1.0"}
OUTPUT_DIR="output/v${VERSION}"

mkdir -p "${OUTPUT_DIR}"

# Build base QCOW2 image
echo "Building QCOW2 image..."
make qcow2

# Convert to other formats
echo "Converting to VDI (VirtualBox)..."
qemu-img convert -f qcow2 -O vdi \
    output/panos.qcow2 \
    "${OUTPUT_DIR}/panos-${VERSION}.vdi"

echo "Converting to VHDX (Hyper-V)..."
qemu-img convert -f qcow2 -O vhdx \
    output/panos.qcow2 \
    "${OUTPUT_DIR}/panos-${VERSION}.vhdx"

echo "Converting to VMDK (VMware)..."
qemu-img convert -f qcow2 -O vmdk -o subformat=streamOptimized \
    output/panos.qcow2 \
    "${OUTPUT_DIR}/panos-${VERSION}.vmdk"

# Copy QCOW2
cp output/panos.qcow2 "${OUTPUT_DIR}/panos-${VERSION}.qcow2"

# Generate checksums
echo "Generating checksums..."
cd "${OUTPUT_DIR}"
sha256sum *.qcow2 *.vdi *.vhdx *.vmdk > SHA256SUMS
```

### GitHub Actions Release Workflow

```yaml
# .github/workflows/release.yml
name: Release

on:
  push:
    tags:
      - 'v*'

jobs:
  build:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      
      - name: Set up Docker Buildx
        uses: docker/setup-buildx-action@v3
        
      - name: Build images
        run: |
          VERSION=${GITHUB_REF#refs/tags/v}
          ./scripts/build-images.sh ${VERSION}
          
      - name: Generate release notes
        run: |
          ./scripts/generate-changelog.sh > RELEASE_NOTES.md
          
      - name: Create Release
        uses: softprops/action-gh-release@v1
        with:
          files: |
            output/v*/*.qcow2
            output/v*/*.vdi
            output/v*/*.vhdx
            output/v*/*.vmdk
            output/v*/SHA256SUMS
          body_path: RELEASE_NOTES.md
          
  docker:
    needs: build
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      
      - name: Login to GHCR
        uses: docker/login-action@v3
        with:
          registry: ghcr.io
          username: ${{ github.actor }}
          password: ${{ secrets.GITHUB_TOKEN }}
          
      - name: Build and push builder image
        run: |
          VERSION=${GITHUB_REF#refs/tags/v}
          docker build -t ghcr.io/${{ github.repository }}/builder:${VERSION} .
          docker push ghcr.io/${{ github.repository }}/builder:${VERSION}
```

### Hypervisor Detection

```csharp
// src/PanoramicData.Os.Platform/HypervisorDetector.cs

public enum Hypervisor
{
    None,
    QEMU,
    VirtualBox,
    HyperV,
    VMware,
    Unknown
}

public static class HypervisorDetector
{
    public static Hypervisor Detect()
    {
        // Check cpuid (leaf 0x40000000)
        var hypervisorId = ReadCpuidHypervisorId();
        
        return hypervisorId switch
        {
            "KVMKVMKVM\0\0\0" => Hypervisor.QEMU,
            "VBoxVBoxVBox" => Hypervisor.VirtualBox,
            "Microsoft Hv" => Hypervisor.HyperV,
            "VMwareVMware" => Hypervisor.VMware,
            _ => File.Exists("/sys/devices/virtual/dmi/id/product_name")
                ? DetectFromDmi()
                : Hypervisor.Unknown
        };
    }
    
    private static Hypervisor DetectFromDmi()
    {
        var product = File.ReadAllText("/sys/devices/virtual/dmi/id/product_name").Trim();
        
        if (product.Contains("VirtualBox")) return Hypervisor.VirtualBox;
        if (product.Contains("Virtual Machine") && 
            File.ReadAllText("/sys/devices/virtual/dmi/id/sys_vendor").Contains("Microsoft"))
            return Hypervisor.HyperV;
        if (product.Contains("VMware")) return Hypervisor.VMware;
        
        return Hypervisor.Unknown;
    }
}
```

### Performance Metrics Collection

```csharp
// src/PanoramicData.Os.Telemetry/PerformanceMonitor.cs

public class PerformanceMetrics
{
    public TimeSpan BootTime { get; set; }
    public long MemoryUsedBytes { get; set; }
    public double CpuUsagePercent { get; set; }
    public long DiskReadBytes { get; set; }
    public long DiskWriteBytes { get; set; }
    public long NetworkRxBytes { get; set; }
    public long NetworkTxBytes { get; set; }
    public int FramesPerSecond { get; set; }
}

public class PerformanceMonitor
{
    private readonly Stopwatch _bootStopwatch = new();
    
    public void StartBootTimer() => _bootStopwatch.Start();
    
    public void StopBootTimer() => _bootStopwatch.Stop();
    
    public PerformanceMetrics Collect()
    {
        return new PerformanceMetrics
        {
            BootTime = _bootStopwatch.Elapsed,
            MemoryUsedBytes = GetMemoryUsage(),
            CpuUsagePercent = GetCpuUsage(),
            // ... other metrics
        };
    }
    
    private long GetMemoryUsage()
    {
        // Read /proc/meminfo
        var meminfo = File.ReadAllText("/proc/meminfo");
        // Parse MemTotal - MemAvailable
        // ...
    }
}
```

### Kernel Configuration for Hypervisors

```text
# configs/hypervisor.config (merged with base config)

# VirtualBox
CONFIG_VBOXGUEST=m
CONFIG_VBOXSF_FS=m

# Hyper-V
CONFIG_HYPERV=y
CONFIG_HYPERV_UTILS=m
CONFIG_HYPERV_BALLOON=m
CONFIG_HYPERV_STORAGE=m
CONFIG_HYPERV_NET=m
CONFIG_HYPERV_KEYBOARD=m
CONFIG_FB_HYPERV=m

# VMware
CONFIG_VMWARE_VMCI=m
CONFIG_VMWARE_VMCI_VSOCKETS=m
CONFIG_VMWARE_BALLOON=m
CONFIG_VMXNET3=m
CONFIG_VMWARE_PVSCSI=m
CONFIG_DRM_VMWGFX=m

# Common virtualization
CONFIG_VIRTIO=y
CONFIG_VIRTIO_PCI=y
CONFIG_VIRTIO_BLK=y
CONFIG_VIRTIO_NET=y
CONFIG_VIRTIO_CONSOLE=y
CONFIG_VIRTIO_INPUT=y
CONFIG_DRM_VIRTIO_GPU=m
```

### Documentation Structure

```text
docs/
├── README.md                 # Project overview
├── INSTALL.md               # Build & install instructions
├── QUICKSTART.md            # Get started in 5 minutes
├── CHANGELOG.md             # Version history
├── CONTRIBUTING.md          # How to contribute
├── LICENSE                  # License text
│
├── architecture/
│   ├── overview.md          # System architecture
│   ├── boot-process.md      # Boot sequence
│   ├── networking.md        # Network stack design
│   ├── graphics.md          # Graphics subsystem
│   └── diagrams/            # Architecture diagrams
│
├── user-guide/
│   ├── getting-started.md   # First steps
│   ├── terminal.md          # Terminal app guide
│   ├── settings.md          # Settings app guide
│   ├── files.md             # File manager guide
│   └── keyboard-shortcuts.md # Shortcuts reference
│
├── developer-guide/
│   ├── setup.md             # Dev environment setup
│   ├── building.md          # How to build
│   ├── testing.md           # Running tests
│   ├── debugging.md         # Debugging tips
│   ├── adding-apps.md       # Creating new apps
│   └── api-reference.md     # API documentation
│
├── hypervisors/
│   ├── qemu.md              # QEMU guide
│   ├── virtualbox.md        # VirtualBox guide
│   ├── hyper-v.md           # Hyper-V guide
│   └── vmware.md            # VMware guide
│
└── troubleshooting/
    ├── boot-issues.md       # Boot problems
    ├── network-issues.md    # Network problems
    ├── graphics-issues.md   # Display problems
    └── faq.md               # Frequently asked questions
```

---

## Testing Requirements

### Hypervisor Matrix

| Feature | QEMU | VirtualBox | Hyper-V | VMware |
| ------- | ---- | ---------- | ------- | ------ |
| Boot | ✓ | ✓ | ✓ | ✓ |
| Network | ✓ | ✓ | ✓ | ✓ |
| Display | ✓ | ✓ | ✓ | ✓ |
| Resize | ✓ | ✓ | ✓ | ✓ |
| Clipboard | N/A | ✓ | ✓ | ✓ |
| Shared Folders | 9p | ✓ | N/A | ✓ |
| Guest Additions | N/A | ✓ | ✓ | ✓ |

### Performance Benchmarks

| Metric | Target | QEMU | VirtualBox | Hyper-V | VMware |
| ------ | ------ | ---- | ---------- | ------- | ------ |
| Boot time | < 5s | | | | |
| Idle memory | < 64MB | | | | |
| Idle CPU | < 1% | | | | |
| Network (iperf) | > 500 Mbps | | | | |
| GUI FPS | > 30 | | | | |

### Stability Tests

| Test | Duration | Pass Criteria |
| ---- | -------- | ------------- |
| Long-running idle | 24h | No crashes, no memory growth |
| Continuous network | 8h | No connection drops |
| Window stress | 4h | No GUI freezes |
| Terminal stress | 4h | No PTY leaks |

---

## Deliverables

1. **Multi-format images** - QCOW2, VDI, VHDX, VMDK
2. **Hypervisor support** - VirtualBox, Hyper-V, VMware
3. **Guest tools** - Integration for each hypervisor
4. **Documentation** - Complete user and developer docs
5. **Release pipeline** - Automated builds and releases
6. **Performance baseline** - Benchmarks for all hypervisors

---

## Risks

| Risk | Probability | Impact | Mitigation |
| ---- | ----------- | ------ | ---------- |
| Hypervisor driver issues | High | High | Start with well-supported drivers |
| Guest additions complexity | High | Medium | Use open source implementations |
| Performance varies by hypervisor | Medium | Medium | Document requirements |
| Documentation takes too long | Medium | Low | Document as you go |
| Last-minute bugs | High | Medium | Budget time for fixes |

---

## Exit Criteria

- [ ] Boots on QEMU, VirtualBox, Hyper-V, VMware
- [ ] Network works on all hypervisors
- [ ] Display works on all hypervisors
- [ ] Clipboard sharing works (where applicable)
- [ ] Guest additions functional
- [ ] Boot time < 5 seconds
- [ ] Idle memory < 64 MB
- [ ] All documentation complete
- [ ] GitHub releases working
- [ ] All formats downloadable
- [ ] README and getting started guide complete
- [ ] 24-hour stability test passed

---

## Demo Milestone

**Demo:** Download image, boot on VirtualBox, use system.

```text
1. User visits GitHub releases page
2. Downloads panos-0.1.0.vdi
3. Creates new VirtualBox VM:
   - Name: PanoramicData.Os
   - Type: Linux
   - Version: Other Linux (64-bit)
   - RAM: 512 MB
   - Attach VDI as disk
   
4. Starts VM
5. Sees boot messages (< 5 seconds)
6. GUI appears with:
   - Desktop with launcher
   - Terminal, Settings, Files apps
   
7. Opens Terminal:
   - Types: uname -a
   - Sees: PanoramicData.Os v0.1.0
   
8. Opens Settings:
   - Views network config (DHCP IP visible)
   - Views About page (system info)
   
9. Opens Files:
   - Browses /etc
   - Views configuration files
   
10. Everything is responsive and stable
```

---

## Estimated Total Effort

| Section | Hours |
| ------- | ----- |
| 8.1 VirtualBox Support | 62 |
| 8.2 Hyper-V Support | 74 |
| 8.3 VMware Support | 68 |
| 8.4 Performance Optimization | 52 |
| 8.5 Stability & Reliability | 52 |
| 8.6 Documentation | 62 |
| 8.7 CI/CD & Release | 52 |
| 8.8 Security Hardening | 32 |
| 8.9 Quality Assurance | 64 |
| **Total** | **518 hours** |

At 40 hours/week = **~13 weeks** (Compressed to 4 weeks requires focus on critical hypervisors and essential polish)

---

## References

- [VirtualBox Guest Additions](https://www.virtualbox.org/manual/ch04.html)
- [Hyper-V Integration Services](https://docs.microsoft.com/en-us/virtualization/hyper-v-on-windows/reference/integration-services)
- [open-vm-tools](https://github.com/vmware/open-vm-tools)
- [qemu-img](https://qemu.readthedocs.io/en/latest/tools/qemu-img.html)
- [Linux Kernel Hypervisor Drivers](https://www.kernel.org/doc/html/latest/admin-guide/virt.html)

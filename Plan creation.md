# Plan Creation Instructions

## Overview

This document provides instructions for creating the detailed implementation plan for PanoramicData.Os. The plan should be structured as a master document with links to individual phase documents.

---

## Output Structure

Create the following files in the `plans/` directory:

```
plans/
├── MASTER_PLAN.md          # Overview and phase summary
├── Phase-001.md            # Foundation & Boot
├── Phase-002.md            # Network Stack
├── Phase-003.md            # SSH Server
├── Phase-004.md            # Graphics & Input
├── Phase-005.md            # Chromium Integration
├── Phase-006.md            # Window Manager
├── Phase-007.md            # Built-in Apps
└── Phase-008.md            # Multi-VM Support & Polish
```

---

## MASTER_PLAN.md Template

The master plan should include:

1. **Project Overview** - Brief description and goals
2. **Architecture Diagram** - ASCII art system architecture
3. **Technology Stack** - Table of all technologies used
4. **Phase Summary** - Table with links to each phase document
5. **Timeline** - Gantt-style overview (ASCII or table)
6. **Risk Register** - Known risks and mitigations
7. **Dependencies** - External libraries and their purposes
8. **Success Metrics** - How we measure completion

---

## Phase Document Template

Each Phase-XXX.md should include:

### Header

```markdown
# Phase XXX: [Phase Name]

**Duration:** X weeks
**Dependencies:** Phase XXX, Phase YYY
**Owner:** TBD
```

### Sections

1. **Objectives** - What this phase achieves
2. **Prerequisites** - What must be complete before starting
3. **Tasks** - Detailed task breakdown with:
   - Task ID (e.g., 1.1, 1.2)
   - Description
   - Estimated effort (hours/days)
   - Complexity (Low/Medium/High)
   - Acceptance criteria
4. **Technical Details** - Architecture decisions, code structure
5. **Testing Requirements** - How to verify completion
6. **Deliverables** - Concrete outputs
7. **Risks** - Phase-specific risks
8. **Exit Criteria** - Definition of done

---

## Phase Breakdown

### Phase 1: Foundation & Boot (Weeks 1-3)

- Buildroot configuration
- Minimal Linux kernel
- .NET NativeAOT init process
- Serial console output
- Docker build environment
- GitHub Actions CI

### Phase 2: Network Stack (Weeks 4-6)

- E1000 NIC driver integration
- Ethernet frame handling
- IPv4 + IPv6 dual stack
- ARP and NDP
- ICMP (ping)
- UDP and TCP
- DHCPv4 and DHCPv6 clients
- Static IP configuration
- DNS resolver

### Phase 3: SSH Server (Weeks 7-9)

- SSH-2 protocol implementation
- Key exchange (curve25519)
- Public key authentication (Ed25519, RSA)
- Session channel
- PTY allocation
- Shell integration

### Phase 4: Graphics & Input (Weeks 10-12)

- DRM/KMS framebuffer via P/Invoke
- evdev input handling
- Keyboard driver
- Mouse driver (with cursor)
- Touch input support
- Basic compositor

### Phase 5: Chromium Integration (Weeks 13-16)

- CEF integration with .NET
- Single browser window
- panos:// scheme handler
- .NET ↔ JavaScript bridge
- Window decorations
- Multi-window support

### Phase 6: Window Manager (Weeks 17-19)

- Floating window management
- Tiling (snap to edges)
- Fullscreen mode
- Super key launcher
- Window switcher (Alt+Tab)
- Keyboard shortcuts

### Phase 7: Built-in Apps (Weeks 20-22)

- Terminal app (xterm.js + WebSocket to .NET)
- Settings app
- File manager app
- Launcher/switcher app

### Phase 8: Multi-VM Support & Polish (Weeks 23-26)

- QEMU optimization
- VirtualBox testing & fixes
- Hyper-V testing & fixes
- VMware testing & fixes
- Performance optimization
- Documentation
- Release preparation

---

## Key Technical Decisions to Document

For each phase, ensure these decisions are captured:

1. **Library choices** - Why this library over alternatives?
2. **Architecture patterns** - Services, IPC mechanisms
3. **P/Invoke usage** - What Linux syscalls are needed?
4. **NativeAOT compatibility** - Any reflection/dynamic code issues?
5. **Cross-VM compatibility** - What differs between VMs?

---

## Prompts for AI Assistant

Use the following prompts to generate each phase document:

### For MASTER_PLAN.md:

```
Create plans/MASTER_PLAN.md for PanoramicData.Os based on Specification.md. 
Include:
- Project overview
- ASCII architecture diagram
- Technology stack table
- Phase summary with links to Phase-001.md through Phase-008.md
- Timeline table
- Risk register
- External dependencies
- Success metrics
```

### For each Phase document:

```
Create plans/Phase-00X.md for [Phase Name] based on Specification.md and MASTER_PLAN.md.
Include detailed tasks with effort estimates, technical details, testing requirements,
and exit criteria. Reference the phase breakdown in this document.
```

---

## Next Steps

1. Review and approve Specification.md
2. Create `plans/` directory
3. Generate MASTER_PLAN.md
4. Generate Phase-001.md through Phase-008.md
5. Review and refine each phase document
6. Begin Phase 1 implementation

---

## Notes

- Phases may run in parallel where dependencies allow
- Estimates are preliminary and should be refined during planning
- Each phase should have a working demo/milestone
- Regular checkpoints to assess progress and adjust timeline

# Phase 4: Graphics & Input

**Duration:** 3 weeks  
**Dependencies:** Phase 1 (Foundation & Boot)  
**Owner:** TBD  
**Status:** 🔲 Not Started

---

## Objectives

1. Initialize DRM/KMS for framebuffer access
2. Implement evdev-based input handling
3. Support keyboard, mouse, and touch input
4. Create basic compositor infrastructure
5. Render mouse cursor on screen
6. Prepare graphics layer for Chromium integration

---

## Prerequisites

- Phase 1 complete (bootable system)
- Kernel compiled with DRM/KMS and evdev support
- Graphics driver (virtio-gpu, bochs, or simple framebuffer)
- Input devices accessible via /dev/input/

---

## Tasks

### 4.1 DRM/KMS Initialization

| ID | Task | Effort | Complexity | Acceptance Criteria |
| -- | ---- | ------ | ---------- | ------------------- |
| 4.1.1 | P/Invoke for DRM ioctls | 8h | High | DRM ioctls callable |
| 4.1.2 | Enumerate DRM devices (/dev/dri/*) | 2h | Low | Devices discovered |
| 4.1.3 | Open DRM device and get resources | 4h | Medium | Resources retrieved |
| 4.1.4 | Enumerate connectors and CRTCs | 4h | Medium | Display outputs found |
| 4.1.5 | Mode setting (select resolution) | 8h | High | Mode applied successfully |
| 4.1.6 | Create dumb buffer (framebuffer) | 8h | High | Framebuffer allocated |
| 4.1.7 | Map framebuffer to userspace | 4h | Medium | Memory-mapped access works |
| 4.1.8 | Page flipping (double buffering) | 8h | High | Smooth updates |
| 4.1.9 | Handle hotplug events | 4h | Medium | Monitor connect/disconnect |

**Subtotal:** 50 hours

### 4.2 Framebuffer Operations

| ID | Task | Effort | Complexity | Acceptance Criteria |
| -- | ---- | ------ | ---------- | ------------------- |
| 4.2.1 | Pixel format handling (ARGB, XRGB) | 4h | Medium | Correct color output |
| 4.2.2 | Clear screen (solid color) | 2h | Low | Screen fills with color |
| 4.2.3 | Draw rectangle | 2h | Low | Rectangles rendered |
| 4.2.4 | Draw image (bitmap) | 4h | Medium | Images rendered correctly |
| 4.2.5 | Alpha blending | 4h | Medium | Transparency works |
| 4.2.6 | Dirty region tracking | 4h | Medium | Only changed areas updated |
| 4.2.7 | VSync synchronization | 4h | Medium | No tearing |

**Subtotal:** 24 hours

### 4.3 Input Device Discovery

| ID | Task | Effort | Complexity | Acceptance Criteria |
| -- | ---- | ------ | ---------- | ------------------- |
| 4.3.1 | Scan /dev/input/event* devices | 2h | Low | Devices enumerated |
| 4.3.2 | Query device capabilities (EVIOCGBIT) | 4h | Medium | Device types identified |
| 4.3.3 | Identify keyboard devices | 2h | Low | Keyboards detected |
| 4.3.4 | Identify mouse/touchpad devices | 2h | Low | Pointers detected |
| 4.3.5 | Identify touchscreen devices | 2h | Low | Touchscreens detected |
| 4.3.6 | Device hotplug via udev/inotify | 4h | Medium | New devices detected |
| 4.3.7 | Input device abstraction layer | 4h | Medium | Unified input API |

**Subtotal:** 20 hours

### 4.4 Keyboard Input

| ID | Task | Effort | Complexity | Acceptance Criteria |
| -- | ---- | ------ | ---------- | ------------------- |
| 4.4.1 | Read evdev keyboard events | 4h | Medium | Key events received |
| 4.4.2 | Scancode to keycode mapping | 4h | Medium | Correct keycodes |
| 4.4.3 | Keycode to character mapping | 8h | High | Characters produced |
| 4.4.4 | Modifier key tracking (Shift, Ctrl, Alt) | 4h | Medium | Modifiers tracked |
| 4.4.5 | Key repeat handling | 4h | Medium | Repeat rate applied |
| 4.4.6 | Keyboard layout support (US, UK) | 8h | High | Multiple layouts work |
| 4.4.7 | Special keys (F1-F12, arrows, etc.) | 2h | Low | Special keys mapped |
| 4.4.8 | Compose sequences (optional) | 4h | Medium | Accented chars work |

**Subtotal:** 38 hours

### 4.5 Mouse/Pointer Input

| ID | Task | Effort | Complexity | Acceptance Criteria |
| -- | ---- | ------ | ---------- | ------------------- |
| 4.5.1 | Read evdev mouse events | 4h | Medium | Mouse events received |
| 4.5.2 | Relative motion handling | 4h | Medium | Cursor moves correctly |
| 4.5.3 | Absolute position handling | 4h | Medium | Touchpad works |
| 4.5.4 | Button state tracking | 2h | Low | Clicks detected |
| 4.5.5 | Scroll wheel handling | 2h | Low | Scroll events work |
| 4.5.6 | Pointer acceleration | 4h | Medium | Natural cursor feel |
| 4.5.7 | Cursor confinement (screen bounds) | 2h | Low | Cursor stays on screen |

**Subtotal:** 22 hours

### 4.6 Touch Input

| ID | Task | Effort | Complexity | Acceptance Criteria |
| -- | ---- | ------ | ---------- | ------------------- |
| 4.6.1 | Read evdev touch events | 4h | Medium | Touch events received |
| 4.6.2 | Single touch handling | 4h | Medium | Single touch works |
| 4.6.3 | Multi-touch tracking (slots) | 8h | High | Multiple fingers tracked |
| 4.6.4 | Touch to mouse emulation | 4h | Medium | Touch acts like mouse |
| 4.6.5 | Gesture recognition (basic) | 8h | High | Pinch/swipe detected |
| 4.6.6 | Touch calibration | 4h | Medium | Accurate touch mapping |

**Subtotal:** 32 hours

### 4.7 Cursor Rendering

| ID | Task | Effort | Complexity | Acceptance Criteria |
| -- | ---- | ------ | ---------- | ------------------- |
| 4.7.1 | Load cursor image (PNG/bitmap) | 2h | Low | Cursor image loaded |
| 4.7.2 | Hardware cursor (if available) | 4h | Medium | DRM cursor plane used |
| 4.7.3 | Software cursor fallback | 4h | Medium | Cursor composited |
| 4.7.4 | Cursor hotspot handling | 2h | Low | Click point correct |
| 4.7.5 | Cursor visibility toggle | 1h | Low | Show/hide works |
| 4.7.6 | Different cursor shapes | 4h | Medium | Arrow, pointer, text, etc. |
| 4.7.7 | Animated cursors (optional) | 4h | Medium | Wait cursor animates |

**Subtotal:** 21 hours

### 4.8 Compositor Infrastructure

| ID | Task | Effort | Complexity | Acceptance Criteria |
| -- | ---- | ------ | ---------- | ------------------- |
| 4.8.1 | Surface abstraction | 4h | Medium | Surfaces created |
| 4.8.2 | Surface damage tracking | 4h | Medium | Damage regions tracked |
| 4.8.3 | Surface composition | 8h | High | Surfaces composited |
| 4.8.4 | Z-order management | 4h | Medium | Surfaces stacked |
| 4.8.5 | Render loop (vsync-driven) | 4h | Medium | Smooth 60fps possible |
| 4.8.6 | Frame scheduling | 4h | Medium | Consistent frame timing |
| 4.8.7 | Prepare for CEF integration | 4h | Medium | CEF surface slot ready |

**Subtotal:** 32 hours

### 4.9 Display Output

| ID | Task | Effort | Complexity | Acceptance Criteria |
| -- | ---- | ------ | ---------- | ------------------- |
| 4.9.1 | Resolution detection | 2h | Low | Native resolution found |
| 4.9.2 | Resolution switching | 4h | Medium | Resolution changes work |
| 4.9.3 | DPI/scaling detection | 4h | Medium | DPI calculated |
| 4.9.4 | Multi-monitor enumeration | 4h | Medium | Multiple displays found |
| 4.9.5 | Primary monitor selection | 2h | Low | Primary set correctly |
| 4.9.6 | Boot splash screen | 4h | Medium | Logo shown during boot |

**Subtotal:** 20 hours

---

## Technical Details

### Project Structure

```text
src/
└── PanoramicData.Os.Graphics/
    ├── PanoramicData.Os.Graphics.csproj
    ├── Drm/
    │   ├── DrmDevice.cs
    │   ├── DrmConnector.cs
    │   ├── DrmCrtc.cs
    │   ├── DrmFramebuffer.cs
    │   ├── DrmMode.cs
    │   └── DrmIoctls.cs
    ├── Framebuffer/
    │   ├── Framebuffer.cs
    │   ├── PixelFormat.cs
    │   └── RenderTarget.cs
    ├── Input/
    │   ├── InputManager.cs
    │   ├── InputDevice.cs
    │   ├── KeyboardDevice.cs
    │   ├── MouseDevice.cs
    │   ├── TouchDevice.cs
    │   └── EventTypes.cs
    ├── Keyboard/
    │   ├── KeyboardLayout.cs
    │   ├── UsLayout.cs
    │   └── UkLayout.cs
    ├── Cursor/
    │   ├── CursorManager.cs
    │   ├── CursorTheme.cs
    │   └── CursorShape.cs
    └── Compositor/
        ├── Compositor.cs
        ├── Surface.cs
        ├── DamageRegion.cs
        └── RenderLoop.cs
```

### Key DRM/KMS Ioctls

```csharp
// DRM ioctl numbers (Linux-specific)
internal const uint DRM_IOCTL_MODE_GETRESOURCES = 0xC04064A0;
internal const uint DRM_IOCTL_MODE_GETCRTC = 0xC06864A1;
internal const uint DRM_IOCTL_MODE_SETCRTC = 0xC06864A2;
internal const uint DRM_IOCTL_MODE_GETCONNECTOR = 0xC05064A7;
internal const uint DRM_IOCTL_MODE_CREATE_DUMB = 0xC02064B2;
internal const uint DRM_IOCTL_MODE_MAP_DUMB = 0xC01064B3;
internal const uint DRM_IOCTL_MODE_ADDFB = 0xC01C64AE;
internal const uint DRM_IOCTL_MODE_PAGE_FLIP = 0xC01864B0;

// P/Invoke
[LibraryImport("libc", SetLastError = true)]
internal static partial int ioctl(int fd, uint request, ref DrmModeGetResources resources);
```

### DRM Mode Setting Flow

```text
┌─────────────────────────────────────────────────────────────────────┐
│                         DRM Initialization                          │
├─────────────────────────────────────────────────────────────────────┤
│                                                                      │
│  1. Open /dev/dri/card0                                             │
│           │                                                          │
│           ▼                                                          │
│  2. DRM_IOCTL_MODE_GETRESOURCES                                     │
│     → Get CRTCs, Connectors, Encoders                               │
│           │                                                          │
│           ▼                                                          │
│  3. DRM_IOCTL_MODE_GETCONNECTOR                                     │
│     → Get connector status, modes                                   │
│           │                                                          │
│           ▼                                                          │
│  4. Select best mode (prefer native)                                │
│           │                                                          │
│           ▼                                                          │
│  5. DRM_IOCTL_MODE_CREATE_DUMB                                      │
│     → Create framebuffer memory                                     │
│           │                                                          │
│           ▼                                                          │
│  6. DRM_IOCTL_MODE_MAP_DUMB + mmap()                               │
│     → Map to userspace                                              │
│           │                                                          │
│           ▼                                                          │
│  7. DRM_IOCTL_MODE_ADDFB                                            │
│     → Register framebuffer with DRM                                 │
│           │                                                          │
│           ▼                                                          │
│  8. DRM_IOCTL_MODE_SETCRTC                                          │
│     → Apply mode and display framebuffer                            │
│                                                                      │
└─────────────────────────────────────────────────────────────────────┘
```

### evdev Input Event Structure

```csharp
[StructLayout(LayoutKind.Sequential)]
internal struct InputEvent
{
    public long Seconds;      // timeval.tv_sec
    public long Microseconds; // timeval.tv_usec
    public ushort Type;       // EV_KEY, EV_REL, EV_ABS, etc.
    public ushort Code;       // KEY_A, REL_X, ABS_X, etc.
    public int Value;         // Key state or axis value
}

// Event types
internal const ushort EV_SYN = 0x00;  // Synchronization
internal const ushort EV_KEY = 0x01;  // Key/button events
internal const ushort EV_REL = 0x02;  // Relative axis (mouse)
internal const ushort EV_ABS = 0x03;  // Absolute axis (touch)
```

### Multi-touch Slot Protocol

```text
# Touch down finger 0 at (100, 200)
EV_ABS ABS_MT_SLOT 0
EV_ABS ABS_MT_TRACKING_ID 45
EV_ABS ABS_MT_POSITION_X 100
EV_ABS ABS_MT_POSITION_Y 200
EV_SYN SYN_REPORT 0

# Touch down finger 1 at (300, 400)
EV_ABS ABS_MT_SLOT 1
EV_ABS ABS_MT_TRACKING_ID 46
EV_ABS ABS_MT_POSITION_X 300
EV_ABS ABS_MT_POSITION_Y 400
EV_SYN SYN_REPORT 0

# Finger 0 moves to (110, 210)
EV_ABS ABS_MT_SLOT 0
EV_ABS ABS_MT_POSITION_X 110
EV_ABS ABS_MT_POSITION_Y 210
EV_SYN SYN_REPORT 0

# Finger 0 lifts
EV_ABS ABS_MT_SLOT 0
EV_ABS ABS_MT_TRACKING_ID -1
EV_SYN SYN_REPORT 0
```

### Render Loop

```csharp
public async Task RunAsync(CancellationToken ct)
{
    var frameDuration = TimeSpan.FromMilliseconds(1000.0 / 60); // 60 FPS
    
    while (!ct.IsCancellationRequested)
    {
        var frameStart = Stopwatch.GetTimestamp();
        
        // Collect damage from all surfaces
        var damage = CollectDamage();
        
        if (!damage.IsEmpty)
        {
            // Composite surfaces to back buffer
            Composite(damage);
            
            // Render cursor
            RenderCursor();
            
            // Flip buffers
            await PageFlipAsync();
        }
        
        // Wait for next frame
        var elapsed = Stopwatch.GetElapsedTime(frameStart);
        var remaining = frameDuration - elapsed;
        if (remaining > TimeSpan.Zero)
        {
            await Task.Delay(remaining, ct);
        }
    }
}
```

---

## Testing Requirements

### Unit Tests

| Test | Description |
| ---- | ----------- |
| DrmDevice_Open | Device opens successfully |
| DrmDevice_GetResources | Resources retrieved |
| Framebuffer_Create | Framebuffer created |
| Framebuffer_Clear | Clear works |
| InputEvent_Parse | Events parsed correctly |
| Keyboard_Keycode | Keycodes correct |
| Keyboard_Character | Characters produced |
| Mouse_Motion | Motion calculated |
| Touch_MultiSlot | Multi-touch tracked |
| Compositor_Composite | Surfaces composed |

### Integration Tests

| Test | Description |
| ---- | ----------- |
| Graphics_DisplayTestPattern | Test pattern visible |
| Graphics_Cursor | Cursor visible and moves |
| Input_KeyboardEcho | Key presses echoed |
| Input_MouseClick | Clicks detected at position |
| Input_TouchTap | Touch events work |

### Visual Tests (Manual/Playwright)

| Test | Description |
| ---- | ----------- |
| TestPattern_Colors | Correct colors displayed |
| Cursor_AllShapes | All cursor shapes correct |
| Resolution_Switch | Resolution change works |

---

## Deliverables

1. **PanoramicData.Os.Graphics library** - DRM/KMS and compositor
2. **Input handling** - Keyboard, mouse, touch support
3. **Cursor rendering** - Hardware or software cursor
4. **Test pattern** - Visual verification
5. **Boot splash** - Logo displayed during boot

---

## Risks

| Risk | Probability | Impact | Mitigation |
| ---- | ----------- | ------ | ---------- |
| VM graphics driver variations | High | Medium | Focus on simple framebuffer |
| DRM ioctl complexity | Medium | Medium | Reference existing implementations |
| Touch input complexity | Medium | Low | Touch as secondary priority |
| Performance issues | Medium | Medium | Optimize compositing; dirty regions |
| Keyboard layout complexity | Low | Low | Start with US layout only |

---

## Exit Criteria

- [ ] Screen displays test pattern correctly
- [ ] Mouse cursor visible and moves smoothly
- [ ] Mouse clicks detected at correct position
- [ ] Keyboard events produce correct characters
- [ ] Resolution detection works
- [ ] Compositor can render multiple surfaces
- [ ] Page flipping works without tearing
- [ ] Boot splash screen displays
- [ ] All unit tests pass

---

## Demo Milestone

**Demo:** Display test pattern on screen, move mouse cursor.

```sh
$ ./run-qemu.sh -display gtk

[PanoramicData.Os] Initializing graphics...
[PanoramicData.Os] DRM: Found card0
[PanoramicData.Os] DRM: Connector HDMI-A-1 connected
[PanoramicData.Os] DRM: Mode 1920x1080@60Hz selected
[PanoramicData.Os] Framebuffer: 1920x1080 XRGB8888
[PanoramicData.Os] Displaying test pattern...
[PanoramicData.Os] Input: Found keyboard at /dev/input/event0
[PanoramicData.Os] Input: Found mouse at /dev/input/event1
[PanoramicData.Os] Cursor: Loaded default theme
[PanoramicData.Os] Graphics ready.
```

Screen shows:

- Colored test pattern (red, green, blue, white quadrants)
- Mouse cursor that moves with mouse input
- Boot logo in center

---

## Estimated Total Effort

| Section | Hours |
| ------- | ----- |
| 4.1 DRM/KMS Initialization | 50 |
| 4.2 Framebuffer Operations | 24 |
| 4.3 Input Device Discovery | 20 |
| 4.4 Keyboard Input | 38 |
| 4.5 Mouse/Pointer Input | 22 |
| 4.6 Touch Input | 32 |
| 4.7 Cursor Rendering | 21 |
| 4.8 Compositor Infrastructure | 32 |
| 4.9 Display Output | 20 |
| **Total** | **259 hours** |

At 40 hours/week = **~6.5 weeks** (Compressed to 3 weeks requires parallel work on Phases 2 and 4)

---

## References

- [DRM/KMS Documentation](https://www.kernel.org/doc/html/latest/gpu/drm-kms.html)
- [evdev Protocol](https://www.kernel.org/doc/html/latest/input/input.html)
- [Multi-touch Protocol](https://www.kernel.org/doc/html/latest/input/multi-touch-protocol.html)
- [libinput Documentation](https://wayland.freedesktop.org/libinput/doc/latest/)
- [XKB Keyboard Layouts](https://www.x.org/wiki/XKB/)

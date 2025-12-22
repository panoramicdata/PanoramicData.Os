# Phase 6: Window Manager

**Duration:** 3 weeks  
**Dependencies:** Phase 5 (Chromium Integration)  
**Owner:** TBD  
**Status:** 🔲 Not Started

---

## Objectives

1. Implement floating window management
2. Support window tiling (snap to edges/corners)
3. Implement fullscreen mode
4. Create Super key launcher/switcher
5. Implement Alt+Tab window switching
6. Handle keyboard shortcuts

---

## Prerequisites

- Phase 5 complete (browser windows rendering)
- Compositor infrastructure from Phase 4
- Input handling from Phase 4
- Multiple browser windows functional

---

## Tasks

### 6.1 Window Abstraction

| ID | Task | Effort | Complexity | Acceptance Criteria |
| -- | ---- | ------ | ---------- | ------------------- |
| 6.1.1 | Window class with position/size | 4h | Medium | Window properties stored |
| 6.1.2 | Window state (normal, maximized, minimized) | 4h | Medium | States tracked |
| 6.1.3 | Window z-order management | 4h | Medium | Z-order correct |
| 6.1.4 | Window focus tracking | 4h | Medium | Focus changes tracked |
| 6.1.5 | Window registry (all windows) | 4h | Medium | Windows enumerable |
| 6.1.6 | Window events (create, close, focus) | 4h | Medium | Events fire correctly |
| 6.1.7 | Window minimum/maximum size | 2h | Low | Size constraints work |

**Subtotal:** 26 hours

### 6.2 Floating Window Management

| ID | Task | Effort | Complexity | Acceptance Criteria |
| -- | ---- | ------ | ---------- | ------------------- |
| 6.2.1 | Window positioning (initial placement) | 4h | Medium | Windows placed sensibly |
| 6.2.2 | Window dragging (title bar) | 8h | High | Drag moves window |
| 6.2.3 | Window resizing (edges/corners) | 8h | High | Resize works |
| 6.2.4 | Resize cursors (edge detection) | 4h | Medium | Correct cursors shown |
| 6.2.5 | Click to focus (raise) | 4h | Medium | Click raises window |
| 6.2.6 | Double-click title bar (maximize) | 2h | Low | Maximize on double-click |
| 6.2.7 | Cascade new windows | 2h | Low | Windows offset |
| 6.2.8 | Center new windows option | 2h | Low | Centering works |

**Subtotal:** 34 hours

### 6.3 Window Tiling

| ID | Task | Effort | Complexity | Acceptance Criteria |
| -- | ---- | ------ | ---------- | ------------------- |
| 6.3.1 | Snap zones detection | 8h | High | Zones identified |
| 6.3.2 | Snap preview indicator | 4h | Medium | Preview shown while dragging |
| 6.3.3 | Snap left half (Win+Left) | 4h | Medium | Window snaps left |
| 6.3.4 | Snap right half (Win+Right) | 2h | Low | Window snaps right |
| 6.3.5 | Snap top half | 2h | Low | Window snaps top |
| 6.3.6 | Snap bottom half | 2h | Low | Window snaps bottom |
| 6.3.7 | Snap quarters (corners) | 4h | Medium | Corner snap works |
| 6.3.8 | Unsnap on drag away | 4h | Medium | Window unsnaps |
| 6.3.9 | Keyboard shortcuts for tiling | 4h | Medium | Shortcuts work |
| 6.3.10 | Adjacent snap fill | 4h | Medium | Windows fill gaps |

**Subtotal:** 38 hours

### 6.4 Fullscreen Mode

| ID | Task | Effort | Complexity | Acceptance Criteria |
| -- | ---- | ------ | ---------- | ------------------- |
| 6.4.1 | Enter fullscreen (F11) | 4h | Medium | Window goes fullscreen |
| 6.4.2 | Exit fullscreen (F11/Esc) | 2h | Low | Returns to normal |
| 6.4.3 | Hide window decorations | 2h | Low | Chrome hidden |
| 6.4.4 | Fullscreen from maximize | 4h | Medium | Double maximize = fullscreen |
| 6.4.5 | Restore previous size/position | 4h | Medium | Position remembered |
| 6.4.6 | Web API fullscreen support | 4h | Medium | JS fullscreen works |
| 6.4.7 | Fullscreen exit hint | 2h | Low | "Press Esc to exit" shown |

**Subtotal:** 22 hours

### 6.5 Super Key Launcher

| ID | Task | Effort | Complexity | Acceptance Criteria |
| -- | ---- | ------ | ---------- | ------------------- |
| 6.5.1 | Detect Super key press | 4h | Medium | Key detected |
| 6.5.2 | Launch overlay (panos://launcher) | 4h | Medium | Launcher opens |
| 6.5.3 | Close on Super key release (quick press) | 4h | Medium | Toggle behavior |
| 6.5.4 | Keep open if held and released | 2h | Low | Stays open if interacted |
| 6.5.5 | Close on click outside | 2h | Low | Click outside closes |
| 6.5.6 | Close on Escape | 2h | Low | Escape closes |
| 6.5.7 | App grid display | 8h | High | Apps shown in grid |
| 6.5.8 | Search functionality | 8h | High | Type to search |
| 6.5.9 | Keyboard navigation | 4h | Medium | Arrow keys work |
| 6.5.10 | Launch app on Enter/click | 4h | Medium | Apps launch |

**Subtotal:** 42 hours

### 6.6 Window Switcher (Alt+Tab)

| ID | Task | Effort | Complexity | Acceptance Criteria |
| -- | ---- | ------ | ---------- | ------------------- |
| 6.6.1 | Alt+Tab key detection | 4h | Medium | Combo detected |
| 6.6.2 | Switcher overlay display | 8h | High | Overlay appears |
| 6.6.3 | Window thumbnail generation | 8h | High | Thumbnails shown |
| 6.6.4 | Tab cycles through windows | 4h | Medium | Cycling works |
| 6.6.5 | Shift+Tab reverse cycle | 2h | Low | Reverse works |
| 6.6.6 | Switch on Alt release | 4h | Medium | Release switches |
| 6.6.7 | Click to select | 2h | Low | Click selects |
| 6.6.8 | Escape to cancel | 2h | Low | Cancel works |
| 6.6.9 | Window title display | 2h | Low | Titles shown |
| 6.6.10 | Close window from switcher | 4h | Medium | Middle-click closes |

**Subtotal:** 40 hours

### 6.7 Keyboard Shortcuts

| ID | Task | Effort | Complexity | Acceptance Criteria |
| -- | ---- | ------ | ---------- | ------------------- |
| 6.7.1 | Global shortcut manager | 4h | Medium | Shortcuts registered |
| 6.7.2 | Alt+F4 close window | 2h | Low | Window closes |
| 6.7.3 | Super+D show desktop | 4h | Medium | All windows minimize |
| 6.7.4 | Super+E file manager | 2h | Low | Files app opens |
| 6.7.5 | Super+L lock screen | 4h | Medium | Lock screen shows |
| 6.7.6 | Ctrl+Alt+Delete system menu | 4h | Medium | System menu shows |
| 6.7.7 | Print Screen screenshot | 4h | Medium | Screenshot taken |
| 6.7.8 | Super+Arrow tiling shortcuts | 4h | Medium | Tiling via keyboard |
| 6.7.9 | Super+M maximize | 2h | Low | Maximize shortcut |
| 6.7.10 | Configurable shortcuts | 8h | High | Settings UI for shortcuts |

**Subtotal:** 38 hours

### 6.8 Window Animations

| ID | Task | Effort | Complexity | Acceptance Criteria |
| -- | ---- | ------ | ---------- | ------------------- |
| 6.8.1 | Animation framework | 8h | High | Animations possible |
| 6.8.2 | Window open animation | 4h | Medium | Fade/scale in |
| 6.8.3 | Window close animation | 4h | Medium | Fade/scale out |
| 6.8.4 | Minimize animation | 4h | Medium | Shrink to taskbar |
| 6.8.5 | Maximize animation | 4h | Medium | Expand animation |
| 6.8.6 | Tiling snap animation | 4h | Medium | Smooth snap |
| 6.8.7 | Disable animations option | 2h | Low | Instant changes |
| 6.8.8 | Easing functions | 4h | Medium | Smooth curves |

**Subtotal:** 34 hours

### 6.9 Multi-Window Coordination

| ID | Task | Effort | Complexity | Acceptance Criteria |
| -- | ---- | ------ | ---------- | ------------------- |
| 6.9.1 | Window stacking order | 4h | Medium | Z-order correct |
| 6.9.2 | Always on top windows | 4h | Medium | Stays above others |
| 6.9.3 | Modal dialogs | 4h | Medium | Modal behavior |
| 6.9.4 | Parent-child windows | 4h | Medium | Child follows parent |
| 6.9.5 | Window groups | 4h | Medium | Group minimize |
| 6.9.6 | Focus stealing prevention | 4h | Medium | Focus not stolen |

**Subtotal:** 24 hours

---

## Technical Details

### Project Structure

```text
src/
└── PanoramicData.Os.WindowManager/
    ├── PanoramicData.Os.WindowManager.csproj
    ├── Core/
    │   ├── Window.cs
    │   ├── WindowState.cs
    │   ├── WindowManager.cs
    │   └── WindowRegistry.cs
    ├── Floating/
    │   ├── FloatingLayout.cs
    │   ├── WindowDrag.cs
    │   └── WindowResize.cs
    ├── Tiling/
    │   ├── TilingManager.cs
    │   ├── SnapZone.cs
    │   ├── SnapPreview.cs
    │   └── TilePosition.cs
    ├── Fullscreen/
    │   └── FullscreenManager.cs
    ├── Launcher/
    │   ├── LauncherOverlay.cs
    │   ├── AppGrid.cs
    │   └── SearchBar.cs
    ├── Switcher/
    │   ├── WindowSwitcher.cs
    │   ├── ThumbnailGenerator.cs
    │   └── SwitcherOverlay.cs
    ├── Shortcuts/
    │   ├── ShortcutManager.cs
    │   ├── Shortcut.cs
    │   └── GlobalHotkey.cs
    └── Animations/
        ├── AnimationManager.cs
        ├── WindowAnimation.cs
        └── Easing.cs
```

### Window State Machine

```text
                              ┌─────────────────────────┐
                              │                         │
                              ▼                         │
                         ┌────────┐                     │
     ┌──────────────────►│ NORMAL │◄────────────────┐   │
     │                   └────┬───┘                 │   │
     │                        │                     │   │
     │         ┌──────────────┼──────────────┐      │   │
     │         │              │              │      │   │
     │         ▼              ▼              ▼      │   │
     │   ┌──────────┐  ┌───────────┐  ┌──────────┐  │   │
     │   │MAXIMIZED │  │ MINIMIZED │  │  TILED   │  │   │
     │   └─────┬────┘  └─────┬─────┘  └────┬─────┘  │   │
     │         │             │             │        │   │
     │         │             │             │        │   │
     │         ▼             │             │        │   │
     │   ┌──────────┐        │             │        │   │
     └───│FULLSCREEN│◄───────┴─────────────┴────────┘   │
         └────┬─────┘                                   │
              │                                         │
              └─────────────────────────────────────────┘
```

### Snap Zones

```text
┌─────────────────────────────────────────────────────────────────────────┐
│                              Screen                                      │
│                                                                          │
│  ┌───────────────┬───────────────────────────────┬───────────────────┐  │
│  │               │                               │                   │  │
│  │   Top-Left    │          Top Half             │    Top-Right      │  │
│  │   Quarter     │                               │    Quarter        │  │
│  │               │                               │                   │  │
│  ├───────────────┤                               ├───────────────────┤  │
│  │               │                               │                   │  │
│  │   Left Half   │        (No Snap Zone)         │   Right Half      │  │
│  │               │                               │                   │  │
│  ├───────────────┤                               ├───────────────────┤  │
│  │               │                               │                   │  │
│  │  Bottom-Left  │        Bottom Half            │  Bottom-Right     │  │
│  │   Quarter     │                               │    Quarter        │  │
│  │               │                               │                   │  │
│  └───────────────┴───────────────────────────────┴───────────────────┘  │
│                                                                          │
│  Drag to edge/corner to trigger snap                                     │
│                                                                          │
└─────────────────────────────────────────────────────────────────────────┘
```

### Keyboard Shortcuts

| Shortcut | Action | Category |
| -------- | ------ | -------- |
| `Super` | Open/close launcher | Launcher |
| `Alt+Tab` | Window switcher | Switcher |
| `Alt+Shift+Tab` | Reverse window switcher | Switcher |
| `Alt+F4` | Close window | Window |
| `F11` | Toggle fullscreen | Window |
| `Super+Left` | Snap left half | Tiling |
| `Super+Right` | Snap right half | Tiling |
| `Super+Up` | Maximize | Window |
| `Super+Down` | Restore/minimize | Window |
| `Super+D` | Show desktop | Desktop |
| `Super+E` | Open file manager | Apps |
| `Super+L` | Lock screen | System |
| `Ctrl+Alt+Delete` | System menu | System |
| `Print Screen` | Screenshot | Utility |
| `Alt+Print Screen` | Window screenshot | Utility |

### Launcher Overlay Design

```text
┌─────────────────────────────────────────────────────────────────────────┐
│                          Screen (dimmed)                                 │
│                                                                          │
│     ┌─────────────────────────────────────────────────────────────┐     │
│     │                      Launcher Overlay                        │     │
│     │                                                              │     │
│     │  ┌────────────────────────────────────────────────────────┐  │     │
│     │  │  🔍 Search...                                          │  │     │
│     │  └────────────────────────────────────────────────────────┘  │     │
│     │                                                              │     │
│     │  ┌────────┐  ┌────────┐  ┌────────┐  ┌────────┐             │     │
│     │  │        │  │        │  │        │  │        │             │     │
│     │  │Terminal│  │Settings│  │ Files  │  │Browser │             │     │
│     │  │        │  │        │  │        │  │        │             │     │
│     │  └────────┘  └────────┘  └────────┘  └────────┘             │     │
│     │                                                              │     │
│     └─────────────────────────────────────────────────────────────┘     │
│                                                                          │
└─────────────────────────────────────────────────────────────────────────┘
```

### Window Switcher Design

```text
┌─────────────────────────────────────────────────────────────────────────┐
│                          Screen (dimmed)                                 │
│                                                                          │
│       ┌─────────────────────────────────────────────────────────┐       │
│       │                   Window Switcher                        │       │
│       │                                                          │       │
│       │  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐   │       │
│       │  │              │  │  ██████████  │  │              │   │       │
│       │  │   Terminal   │  │  ██ SELECTED│  │   Settings   │   │       │
│       │  │              │  │  ██████████  │  │              │   │       │
│       │  │              │  │              │  │              │   │       │
│       │  └──────────────┘  └──────────────┘  └──────────────┘   │       │
│       │      Terminal         Browser          Settings         │       │
│       │                                                          │       │
│       └─────────────────────────────────────────────────────────┘       │
│                                                                          │
└─────────────────────────────────────────────────────────────────────────┘
```

### Animation Framework

```csharp
public class WindowAnimation
{
    public Window Target { get; }
    public double Duration { get; } // milliseconds
    public EasingFunction Easing { get; }
    
    public Rectangle StartBounds { get; set; }
    public Rectangle EndBounds { get; set; }
    public float StartOpacity { get; set; }
    public float EndOpacity { get; set; }
    
    public void Update(double elapsed)
    {
        var t = Math.Min(elapsed / Duration, 1.0);
        var easedT = Easing.Ease(t);
        
        Target.Bounds = Rectangle.Lerp(StartBounds, EndBounds, easedT);
        Target.Opacity = (float)Lerp(StartOpacity, EndOpacity, easedT);
    }
}

public enum EasingFunction
{
    Linear,
    EaseInQuad,
    EaseOutQuad,
    EaseInOutQuad,
    EaseOutCubic,
    EaseOutBack, // Slight overshoot
}
```

---

## Testing Requirements

### Unit Tests

| Test | Description |
| ---- | ----------- |
| Window_Create | Window created with properties |
| Window_StateChange | State transitions work |
| WindowManager_ZOrder | Z-order correct |
| FloatingLayout_Drag | Drag updates position |
| FloatingLayout_Resize | Resize updates size |
| TilingManager_SnapLeft | Snap left works |
| TilingManager_SnapCorner | Corner snap works |
| Fullscreen_Enter | Fullscreen activates |
| Fullscreen_Exit | Fullscreen exits |
| Shortcut_Register | Shortcuts registered |
| Animation_Interpolate | Animation interpolates |

### Integration Tests

| Test | Description |
| ---- | ----------- |
| WindowManager_CreateWindow | Window appears on screen |
| WindowManager_DragWindow | Dragging works visually |
| WindowManager_SnapWindow | Snapping works visually |
| Launcher_Open | Super key opens launcher |
| Launcher_Search | Search filters apps |
| Switcher_Cycle | Alt+Tab cycles windows |
| Shortcuts_AltF4 | Alt+F4 closes window |

### E2E Tests (Playwright)

| Test | Description |
| ---- | ----------- |
| Launcher_LaunchApp | Launch app from launcher |
| Switcher_SwitchWindow | Switch via Alt+Tab |
| Tiling_DragSnap | Drag to edge snaps |

---

## Deliverables

1. **Window manager** - Floating and tiling support
2. **Super key launcher** - App launcher overlay
3. **Window switcher** - Alt+Tab functionality
4. **Keyboard shortcuts** - Global hotkeys
5. **Window animations** - Smooth transitions
6. **Fullscreen mode** - Browser fullscreen support

---

## Risks

| Risk | Probability | Impact | Mitigation |
| ---- | ----------- | ------ | ---------- |
| Input event conflicts | Medium | Medium | Clear event hierarchy |
| Animation performance | Medium | Low | Optional animations |
| Thumbnail generation slow | Medium | Low | Async generation; caching |
| Focus management complexity | Medium | Medium | Clear focus rules |
| Keyboard shortcut conflicts | Low | Low | Configurable shortcuts |

---

## Exit Criteria

- [ ] Floating windows work (drag, resize)
- [ ] Window tiling works (snap to edges/corners)
- [ ] Fullscreen mode works (F11)
- [ ] Super key opens launcher
- [ ] Alt+Tab switches windows
- [ ] All keyboard shortcuts functional
- [ ] Window animations smooth
- [ ] All unit tests pass
- [ ] E2E tests pass

---

## Demo Milestone

**Demo:** Open 3 windows, tile them, Super key shows launcher.

```sh
$ ./run-qemu.sh -display gtk

[PanoramicData.Os] Window manager ready.

# User actions:
1. Press Super key → Launcher opens
2. Click Terminal → Terminal window opens
3. Press Super key → Launcher opens  
4. Click Files → File manager opens
5. Drag terminal to left edge → Snaps to left half
6. Drag files to right edge → Snaps to right half
7. Press Super key → Click Settings → Settings opens
8. Press F11 → Settings goes fullscreen
9. Press Esc → Returns to normal
10. Press Alt+Tab → Cycle through windows
```

Screen shows:

- Three windows tiled or floating
- Launcher overlay with app grid
- Window switcher with thumbnails
- Smooth animations on state changes

---

## Estimated Total Effort

| Section | Hours |
| ------- | ----- |
| 6.1 Window Abstraction | 26 |
| 6.2 Floating Window Management | 34 |
| 6.3 Window Tiling | 38 |
| 6.4 Fullscreen Mode | 22 |
| 6.5 Super Key Launcher | 42 |
| 6.6 Window Switcher | 40 |
| 6.7 Keyboard Shortcuts | 38 |
| 6.8 Window Animations | 34 |
| 6.9 Multi-Window Coordination | 24 |
| **Total** | **298 hours** |

At 40 hours/week = **~7.5 weeks** (Compressed to 3 weeks requires scope reduction or parallel work)

---

## References

- [EWMH Specification](https://specifications.freedesktop.org/wm-spec/wm-spec-latest.html)
- [Windows 11 Snap Layouts](https://docs.microsoft.com/en-us/windows/apps/get-started/windows-developer-faq)
- [GNOME Shell Window Management](https://wiki.gnome.org/Projects/GnomeShell)
- [CSS Easing Functions](https://easings.net/)

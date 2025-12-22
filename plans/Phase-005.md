# Phase 5: Chromium Integration

**Duration:** 4 weeks  
**Dependencies:** Phase 4 (Graphics & Input)  
**Owner:** TBD  
**Status:** 🔲 Not Started

---

## Objectives

1. Integrate Chromium Embedded Framework (CEF) with .NET
2. Render web content to the compositor
3. Implement panos:// URL scheme handler
4. Create .NET ↔ JavaScript bridge
5. Support multiple browser windows
6. Handle window decorations

---

## Prerequisites

- Phase 4 complete (graphics and input working)
- CEF binaries compiled for Linux x64
- Understanding of CEF architecture
- NativeAOT compatibility considerations

---

## Tasks

### 5.1 CEF Binary Integration

| ID | Task | Effort | Complexity | Acceptance Criteria |
| -- | ---- | ------ | ---------- | ------------------- |
| 5.1.1 | Download/build CEF for Linux x64 | 8h | High | CEF binaries available |
| 5.1.2 | Strip unnecessary CEF components | 8h | High | Reduced binary size |
| 5.1.3 | Package CEF with rootfs | 4h | Medium | CEF files in image |
| 5.1.4 | Verify CEF dependencies (musl compat) | 8h | High | CEF loads without errors |
| 5.1.5 | Create CEF subprocess binary | 4h | Medium | Renderer process works |
| 5.1.6 | Configure CEF paths and resources | 4h | Medium | Resources found at runtime |

**Subtotal:** 36 hours

### 5.2 P/Invoke Bindings

| ID | Task | Effort | Complexity | Acceptance Criteria |
| -- | ---- | ------ | ---------- | ------------------- |
| 5.2.1 | Generate CEF C API bindings | 16h | High | All needed APIs wrapped |
| 5.2.2 | CefApp implementation | 8h | High | App callbacks work |
| 5.2.3 | CefClient implementation | 8h | High | Client callbacks work |
| 5.2.4 | CefBrowser wrapper | 8h | High | Browser controllable |
| 5.2.5 | CefFrame wrapper | 4h | Medium | Frame access works |
| 5.2.6 | Memory management (prevent leaks) | 8h | High | No memory leaks |
| 5.2.7 | String conversion (UTF-8/16) | 4h | Medium | Strings convert correctly |

**Subtotal:** 56 hours

### 5.3 Off-Screen Rendering (OSR)

| ID | Task | Effort | Complexity | Acceptance Criteria |
| -- | ---- | ------ | ---------- | ------------------- |
| 5.3.1 | Implement CefRenderHandler | 8h | High | OnPaint called |
| 5.3.2 | Configure OSR settings | 4h | Medium | Correct buffer format |
| 5.3.3 | Receive paint notifications | 4h | Medium | Frames received |
| 5.3.4 | Copy frame to compositor surface | 8h | High | Content displayed |
| 5.3.5 | Handle dirty rectangles | 4h | Medium | Partial updates work |
| 5.3.6 | Optimize frame transfer | 8h | High | 60 FPS possible |
| 5.3.7 | Handle resize events | 4h | Medium | Resize works smoothly |
| 5.3.8 | Handle popup windows | 8h | High | Popups render correctly |

**Subtotal:** 48 hours

### 5.4 Input Event Forwarding

| ID | Task | Effort | Complexity | Acceptance Criteria |
| -- | ---- | ------ | ---------- | ------------------- |
| 5.4.1 | Forward keyboard events to CEF | 8h | High | Key events reach browser |
| 5.4.2 | Forward mouse events to CEF | 4h | Medium | Mouse events work |
| 5.4.3 | Forward touch events to CEF | 8h | High | Touch events work |
| 5.4.4 | Handle focus changes | 4h | Medium | Focus tracked correctly |
| 5.4.5 | IME (Input Method) support | 8h | High | IME works |
| 5.4.6 | Cursor shape changes | 4h | Medium | Cursor changes on hover |
| 5.4.7 | Context menus | 4h | Medium | Right-click menus work |

**Subtotal:** 40 hours

### 5.5 URL Scheme Handler (panos://)

| ID | Task | Effort | Complexity | Acceptance Criteria |
| -- | ---- | ------ | ---------- | ------------------- |
| 5.5.1 | Implement CefSchemeHandlerFactory | 4h | Medium | Factory registered |
| 5.5.2 | Implement CefResourceHandler | 8h | High | Custom responses work |
| 5.5.3 | Route panos://terminal | 4h | Medium | Terminal app loads |
| 5.5.4 | Route panos://settings | 4h | Medium | Settings app loads |
| 5.5.5 | Route panos://files | 4h | Medium | File manager loads |
| 5.5.6 | Route panos://launcher | 4h | Medium | Launcher loads |
| 5.5.7 | Serve static assets from disk | 4h | Medium | CSS/JS/images served |
| 5.5.8 | Handle 404 for unknown routes | 2h | Low | Error page shown |

**Subtotal:** 34 hours

### 5.6 .NET ↔ JavaScript Bridge

| ID | Task | Effort | Complexity | Acceptance Criteria |
| -- | ---- | ------ | ---------- | ------------------- |
| 5.6.1 | Design bridge API | 4h | Medium | API documented |
| 5.6.2 | Implement CefV8Handler | 8h | High | V8 callbacks work |
| 5.6.3 | Expose .NET methods to JS | 8h | High | JS can call .NET |
| 5.6.4 | Call JS from .NET | 8h | High | .NET can call JS |
| 5.6.5 | Async method handling | 8h | High | Promises work |
| 5.6.6 | Object serialization (JSON) | 4h | Medium | Complex objects transfer |
| 5.6.7 | Event subscription system | 8h | High | Events fire to JS |
| 5.6.8 | Error handling across bridge | 4h | Medium | Errors propagated |

**Subtotal:** 52 hours

### 5.7 Browser Window Management

| ID | Task | Effort | Complexity | Acceptance Criteria |
| -- | ---- | ------ | ---------- | ------------------- |
| 5.7.1 | Single browser window creation | 4h | Medium | Window opens |
| 5.7.2 | Navigate to URL | 2h | Low | Navigation works |
| 5.7.3 | Browser lifecycle management | 4h | Medium | Clean create/destroy |
| 5.7.4 | Multiple browser instances | 8h | High | Multiple windows work |
| 5.7.5 | Window to browser mapping | 4h | Medium | Events routed correctly |
| 5.7.6 | Browser process management | 8h | High | Subprocesses managed |
| 5.7.7 | Crash recovery | 8h | High | Crashed tabs recover |

**Subtotal:** 38 hours

### 5.8 Window Decorations

| ID | Task | Effort | Complexity | Acceptance Criteria |
| -- | ---- | ------ | ---------- | ------------------- |
| 5.8.1 | Design window chrome | 4h | Medium | Design documented |
| 5.8.2 | Render title bar | 4h | Medium | Title bar visible |
| 5.8.3 | Close button | 2h | Low | Close button works |
| 5.8.4 | Maximize/restore button | 4h | Medium | Maximize works |
| 5.8.5 | Minimize button | 2h | Low | Minimize works |
| 5.8.6 | Window title display | 2h | Low | Title shown |
| 5.8.7 | Title from page title | 4h | Medium | Dynamic title update |
| 5.8.8 | Window icon from favicon | 4h | Medium | Favicon displayed |

**Subtotal:** 26 hours

### 5.9 Developer Tools

| ID | Task | Effort | Complexity | Acceptance Criteria |
| -- | ---- | ------ | ---------- | ------------------- |
| 5.9.1 | Enable DevTools | 4h | Medium | DevTools accessible |
| 5.9.2 | Open DevTools in new window | 4h | Medium | DevTools opens |
| 5.9.3 | F12 keyboard shortcut | 2h | Low | F12 opens DevTools |
| 5.9.4 | JavaScript console access | 2h | Low | Console works |
| 5.9.5 | Network tab functionality | 2h | Low | Network visible |
| 5.9.6 | Element inspector | 2h | Low | Inspector works |

**Subtotal:** 16 hours

---

## Technical Details

### Project Structure

```text
src/
└── PanoramicData.Os.Browser/
    ├── PanoramicData.Os.Browser.csproj
    ├── Cef/
    │   ├── CefInitializer.cs
    │   ├── CefApp.cs
    │   ├── CefClient.cs
    │   └── Interop/
    │       ├── CefNative.cs
    │       ├── CefTypes.cs
    │       └── CefCallbacks.cs
    ├── Browser/
    │   ├── BrowserWindow.cs
    │   ├── BrowserManager.cs
    │   └── BrowserEventArgs.cs
    ├── Rendering/
    │   ├── OffScreenRenderer.cs
    │   ├── RenderHandler.cs
    │   └── FrameBuffer.cs
    ├── Input/
    │   ├── InputConverter.cs
    │   ├── KeyboardHandler.cs
    │   └── MouseHandler.cs
    ├── Schemes/
    │   ├── PanosSchemeHandlerFactory.cs
    │   ├── PanosResourceHandler.cs
    │   └── Routes/
    │       ├── TerminalRoute.cs
    │       ├── SettingsRoute.cs
    │       ├── FilesRoute.cs
    │       └── LauncherRoute.cs
    ├── Bridge/
    │   ├── JsBridge.cs
    │   ├── V8Handler.cs
    │   └── BridgeApi.cs
    └── Chrome/
        ├── WindowChrome.cs
        ├── TitleBar.cs
        └── WindowButtons.cs
```

### CEF Architecture in PanoramicData.Os

```text
┌─────────────────────────────────────────────────────────────────────────────┐
│                              PanoramicData.Os                                │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                              │
│  ┌───────────────────────────────────────────────────────────────────────┐  │
│  │                         .NET Main Process                              │  │
│  │  ┌─────────────────────────────────────────────────────────────────┐  │  │
│  │  │                      BrowserManager                              │  │  │
│  │  │  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐           │  │  │
│  │  │  │ BrowserWindow│  │ BrowserWindow│  │ BrowserWindow│   ...     │  │  │
│  │  │  └───────┬──────┘  └───────┬──────┘  └───────┬──────┘           │  │  │
│  │  └──────────┼─────────────────┼─────────────────┼──────────────────┘  │  │
│  │             │                 │                 │                      │  │
│  │  ┌──────────▼─────────────────▼─────────────────▼──────────────────┐  │  │
│  │  │                    CEF C API (P/Invoke)                          │  │  │
│  │  └──────────────────────────────┬──────────────────────────────────┘  │  │
│  └─────────────────────────────────┼─────────────────────────────────────┘  │
│                                    │                                         │
│  ┌─────────────────────────────────▼─────────────────────────────────────┐  │
│  │                         libcef.so (Chromium)                           │  │
│  │                                                                        │  │
│  │  ┌───────────────┐  ┌───────────────┐  ┌───────────────┐              │  │
│  │  │ Browser Host  │  │  URL Handler  │  │  V8 Context   │              │  │
│  │  │  (per window) │  │ (panos://)    │  │  (JS Bridge)  │              │  │
│  │  └───────────────┘  └───────────────┘  └───────────────┘              │  │
│  │                                                                        │  │
│  └────────────────────────────────────────────────────────────────────────┘  │
│                                                                              │
│  ┌────────────────────────────────────────────────────────────────────────┐  │
│  │              CEF Subprocess (panos_helper)                             │  │
│  │                                                                        │  │
│  │  ┌───────────────┐  ┌───────────────┐  ┌───────────────┐              │  │
│  │  │  Renderer 1   │  │  Renderer 2   │  │  Renderer 3   │    ...       │  │
│  │  │  (Tab/Window) │  │  (Tab/Window) │  │  (Tab/Window) │              │  │
│  │  └───────────────┘  └───────────────┘  └───────────────┘              │  │
│  │                                                                        │  │
│  └────────────────────────────────────────────────────────────────────────┘  │
│                                                                              │
└─────────────────────────────────────────────────────────────────────────────┘
```

### Off-Screen Rendering Flow

```text
┌─────────────────────────────────────────────────────────────────────────┐
│                         OSR Frame Pipeline                               │
├─────────────────────────────────────────────────────────────────────────┤
│                                                                          │
│  CEF Renderer Process                                                    │
│       │                                                                  │
│       │ Render to shared memory                                          │
│       ▼                                                                  │
│  ┌─────────────────────────────────────────────────────────────────┐    │
│  │                    OnPaint Callback                              │    │
│  │  Parameters:                                                     │    │
│  │    - browser: CefBrowser                                        │    │
│  │    - type: PET_VIEW or PET_POPUP                                │    │
│  │    - dirtyRects: CefRect[]                                      │    │
│  │    - buffer: nint (BGRA pixels)                                 │    │
│  │    - width, height: int                                         │    │
│  └─────────────────────────────────────────────────────────────────┘    │
│       │                                                                  │
│       │ Copy buffer to surface                                           │
│       ▼                                                                  │
│  ┌─────────────────────────────────────────────────────────────────┐    │
│  │                  Compositor Surface                              │    │
│  └─────────────────────────────────────────────────────────────────┘    │
│       │                                                                  │
│       │ Composite all surfaces                                           │
│       ▼                                                                  │
│  ┌─────────────────────────────────────────────────────────────────┐    │
│  │                    DRM Framebuffer                               │    │
│  └─────────────────────────────────────────────────────────────────┘    │
│       │                                                                  │
│       │ Page flip                                                        │
│       ▼                                                                  │
│  ┌─────────────────────────────────────────────────────────────────┐    │
│  │                        Display                                   │    │
│  └─────────────────────────────────────────────────────────────────┘    │
│                                                                          │
└─────────────────────────────────────────────────────────────────────────┘
```

### panos:// URL Scheme

| URL | Description | App |
| --- | ----------- | --- |
| `panos://terminal` | Terminal emulator | Terminal app |
| `panos://terminal?cwd=/home` | Terminal with working directory | Terminal app |
| `panos://settings` | System settings | Settings app |
| `panos://settings/network` | Network settings | Settings app |
| `panos://files` | File manager | Files app |
| `panos://files/path/to/dir` | File manager at path | Files app |
| `panos://launcher` | Application launcher | Launcher app |

### JavaScript Bridge API

```javascript
// In browser JavaScript

// Get system information
const info = await panos.system.getInfo();
// { hostname: "panos", uptime: 3600, memory: { total: 2048, used: 512 } }

// Execute shell command
const result = await panos.shell.execute("ls -la /");
// { exitCode: 0, stdout: "...", stderr: "" }

// Read file
const content = await panos.fs.readFile("/etc/hostname");
// "panos\n"

// Write file
await panos.fs.writeFile("/tmp/test.txt", "Hello World");

// Subscribe to system events
panos.events.on("network:connected", (event) => {
    console.log("Network connected:", event.interface);
});

// Open new window
panos.window.open("panos://settings");

// Close current window
panos.window.close();
```

### CEF Initialization Settings

```csharp
var settings = new CefSettings
{
    LogSeverity = CefLogSeverity.Warning,
    LogFile = "/var/log/cef.log",
    ResourcesDirPath = "/usr/share/cef/Resources",
    LocalesDirPath = "/usr/share/cef/locales",
    CachePath = "/var/cache/cef",
    
    // Security settings
    RemoteDebuggingPort = 0, // Disabled unless DevTools needed
    
    // Performance
    WindowlessRenderingEnabled = true,
    ExternalMessagePump = false,
    MultiThreadedMessageLoop = true,
    
    // Disable unused features
    PersistSessionCookies = false,
    PersistUserPreferences = false,
};
```

---

## Testing Requirements

### Unit Tests

| Test | Description |
| ---- | ----------- |
| CefInitializer_Starts | CEF initializes without error |
| Browser_Create | Browser window creates |
| Browser_Navigate | Navigation works |
| SchemeHandler_PanosTerminal | panos://terminal resolves |
| SchemeHandler_UnknownRoute | 404 for unknown route |
| JsBridge_CallDotNet | JS can call .NET method |
| JsBridge_CallJs | .NET can call JS function |
| OSR_ReceivesFrames | OnPaint receives frames |
| Input_KeyboardForward | Keys forwarded to CEF |

### Integration Tests

| Test | Description |
| ---- | ----------- |
| Browser_RenderGoogle | google.com renders correctly |
| Browser_ExecuteJs | JavaScript executes |
| Bridge_AsyncMethod | Async methods work |
| MultiWindow_Create | Multiple windows work |
| DevTools_Open | DevTools opens |

### Visual Tests (Playwright)

| Test | Description |
| ---- | ----------- |
| PageLoad_Complete | Page fully renders |
| Interaction_Click | Clicks work |
| Interaction_Type | Typing works |
| TouchScroll_Works | Touch scrolling works |

---

## Deliverables

1. **CEF integration** - Browser rendering in .NET
2. **panos:// scheme** - Custom URL handler
3. **JS Bridge** - .NET ↔ JavaScript communication
4. **Window decorations** - Title bar and buttons
5. **Multi-window support** - Multiple browser windows
6. **DevTools** - Developer tools access

---

## Risks

| Risk | Probability | Impact | Mitigation |
| ---- | ----------- | ------ | ---------- |
| CEF + musl compatibility | High | Critical | Build CEF with musl; test early |
| CEF binary size (500MB+) | High | Medium | Strip unused components |
| NativeAOT reflection issues | Medium | High | Source generators; careful P/Invoke |
| Memory consumption | Medium | Medium | Limit concurrent browsers |
| OSR performance | Medium | Medium | Optimize copy; use shared memory |
| Subprocess management | Medium | Medium | Proper process lifetime handling |

---

## Exit Criteria

- [ ] CEF initializes successfully
- [ ] Browser window renders web content
- [ ] google.com loads and displays correctly
- [ ] panos://terminal loads terminal app
- [ ] JavaScript bridge works bidirectionally
- [ ] Window decorations render correctly
- [ ] Multiple windows work simultaneously
- [ ] DevTools accessible
- [ ] All unit tests pass
- [ ] No memory leaks

---

## Demo Milestone

**Demo:** Render google.com in a CEF window.

```sh
$ ./run-qemu.sh -display gtk

[PanoramicData.Os] Starting browser subsystem...
[PanoramicData.Os] CEF: Initializing...
[PanoramicData.Os] CEF: Version 120.0.0
[PanoramicData.Os] CEF: OSR mode enabled
[PanoramicData.Os] CEF: Subprocess /usr/bin/panos_helper
[PanoramicData.Os] CEF: Ready
[PanoramicData.Os] Opening https://www.google.com...
[PanoramicData.Os] Browser: Created window 1
[PanoramicData.Os] Browser: Navigating to https://www.google.com
[PanoramicData.Os] Browser: Load complete
```

Screen shows:

- Browser window with title bar
- Google homepage rendered correctly
- Mouse cursor changes over links
- Typing in search box works

---

## Estimated Total Effort

| Section | Hours |
| ------- | ----- |
| 5.1 CEF Binary Integration | 36 |
| 5.2 P/Invoke Bindings | 56 |
| 5.3 Off-Screen Rendering | 48 |
| 5.4 Input Event Forwarding | 40 |
| 5.5 URL Scheme Handler | 34 |
| 5.6 .NET ↔ JavaScript Bridge | 52 |
| 5.7 Browser Window Management | 38 |
| 5.8 Window Decorations | 26 |
| 5.9 Developer Tools | 16 |
| **Total** | **346 hours** |

At 40 hours/week = **~8.5 weeks** (Scheduled as 4 weeks - requires parallel work or scope reduction)

---

## References

- [CEF Documentation](https://bitbucket.org/chromiumembedded/cef/wiki/Home)
- [CEF C API](https://magpcss.org/ceforum/apidocs3/index-all.html)
- [CefSharp (reference)](https://github.com/cefsharp/CefSharp)
- [CEF Binary Distribution](https://cef-builds.spotifycdn.com/index.html)
- [Chromium OSR](https://www.chromium.org/developers/design-documents/oop-iframes/oop-iframes-rendering)

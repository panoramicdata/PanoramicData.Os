# Phase 7: Built-in Apps

**Duration:** 3 weeks  
**Dependencies:** Phase 6 (Window Manager)  
**Owner:** TBD  
**Status:** 🔲 Not Started

---

## Objectives

1. Create Terminal app using xterm.js with WebSocket to .NET backend
2. Create Settings app for system configuration
3. Create File Manager app for filesystem browsing
4. Create Launcher/Switcher app (Super key overlay)
5. Implement panos:// URL handling for all apps

---

## Prerequisites

- Phase 6 complete (window manager functional)
- JavaScript bridge working (Phase 5)
- panos:// scheme handler working (Phase 5)
- Shell commands from Phase 1

---

## Tasks

### 7.1 Terminal App - Backend

| ID | Task | Effort | Complexity | Acceptance Criteria |
| -- | ---- | ------ | ---------- | ------------------- |
| 7.1.1 | WebSocket server implementation | 8h | High | WS server running |
| 7.1.2 | PTY allocation per session | 4h | Medium | PTY created per WS |
| 7.1.3 | Shell process spawning | 4h | Medium | Shell starts |
| 7.1.4 | PTY ↔ WebSocket bridge | 8h | High | Data flows both ways |
| 7.1.5 | Session management | 4h | Medium | Sessions tracked |
| 7.1.6 | Resize handling (SIGWINCH) | 4h | Medium | Resize works |
| 7.1.7 | Session cleanup on disconnect | 2h | Low | Resources freed |
| 7.1.8 | Multiple terminal sessions | 4h | Medium | Multiple terminals work |

**Subtotal:** 38 hours

### 7.2 Terminal App - Frontend

| ID | Task | Effort | Complexity | Acceptance Criteria |
| -- | ---- | ------ | ---------- | ------------------- |
| 7.2.1 | xterm.js integration | 4h | Medium | xterm.js loads |
| 7.2.2 | WebSocket connection to backend | 4h | Medium | WS connects |
| 7.2.3 | Terminal rendering | 2h | Low | Characters display |
| 7.2.4 | Keyboard input handling | 4h | Medium | Input sent to shell |
| 7.2.5 | Terminal resize on window resize | 4h | Medium | Resize communicated |
| 7.2.6 | Copy/paste support | 4h | Medium | Clipboard works |
| 7.2.7 | Color scheme support | 4h | Medium | ANSI colors work |
| 7.2.8 | Font configuration | 2h | Low | Font changeable |
| 7.2.9 | Scrollback buffer | 2h | Low | Scroll history works |
| 7.2.10 | URL detection/click | 4h | Medium | URLs clickable |

**Subtotal:** 34 hours

### 7.3 Shell Enhancements

| ID | Task | Effort | Complexity | Acceptance Criteria |
| -- | ---- | ------ | ---------- | ------------------- |
| 7.3.1 | Command line editing (readline-like) | 8h | High | Arrow keys, backspace |
| 7.3.2 | Command history | 4h | Medium | Up/down recalls history |
| 7.3.3 | Tab completion (files/commands) | 8h | High | Tab completes |
| 7.3.4 | Colored output (ls, errors) | 4h | Medium | Colors displayed |
| 7.3.5 | Prompt customization | 2h | Low | Custom prompt |
| 7.3.6 | Environment variables | 4h | Medium | $PATH, $HOME work |
| 7.3.7 | Working directory tracking | 2h | Low | cd updates PWD |
| 7.3.8 | Signal handling (Ctrl+C, Ctrl+Z) | 4h | Medium | Signals work |
| 7.3.9 | Pipe support (basic) | 8h | High | cmd1 | cmd2 works |
| 7.3.10 | Redirection (>, <) | 4h | Medium | File redirection works |

**Subtotal:** 48 hours

### 7.4 Settings App - Framework

| ID | Task | Effort | Complexity | Acceptance Criteria |
| -- | ---- | ------ | ---------- | ------------------- |
| 7.4.1 | Settings page layout | 4h | Medium | Clean layout |
| 7.4.2 | Navigation sidebar | 4h | Medium | Category navigation |
| 7.4.3 | Settings storage (filesystem) | 4h | Medium | Settings persisted |
| 7.4.4 | Settings API (JS bridge) | 4h | Medium | Get/set settings |
| 7.4.5 | Apply button / auto-save | 2h | Low | Changes saved |
| 7.4.6 | Settings reset to default | 2h | Low | Reset works |
| 7.4.7 | Responsive design | 4h | Medium | Works at various sizes |

**Subtotal:** 24 hours

### 7.5 Settings App - Pages

| ID | Task | Effort | Complexity | Acceptance Criteria |
| -- | ---- | ------ | ---------- | ------------------- |
| 7.5.1 | Network settings page | 8h | High | IP config visible/editable |
| 7.5.2 | Display settings (resolution) | 4h | Medium | Resolution changeable |
| 7.5.3 | Appearance (theme selection) | 4h | Medium | Theme switchable |
| 7.5.4 | Keyboard settings (layout) | 4h | Medium | Layout changeable |
| 7.5.5 | Date/Time settings | 4h | Medium | Time configurable |
| 7.5.6 | About page (system info) | 4h | Medium | System info shown |
| 7.5.7 | SSH settings (authorized keys) | 4h | Medium | Keys manageable |
| 7.5.8 | Hostname settings | 2h | Low | Hostname editable |
| 7.5.9 | Reboot/Shutdown buttons | 2h | Low | Power actions work |

**Subtotal:** 36 hours

### 7.6 File Manager App - Backend

| ID | Task | Effort | Complexity | Acceptance Criteria |
| -- | ---- | ------ | ---------- | ------------------- |
| 7.6.1 | Directory listing API | 4h | Medium | Lists files |
| 7.6.2 | File info API (size, date, type) | 4h | Medium | Metadata returned |
| 7.6.3 | File read API | 4h | Medium | File content readable |
| 7.6.4 | File write API | 4h | Medium | Files writable |
| 7.6.5 | Create folder API | 2h | Low | Folders creatable |
| 7.6.6 | Delete file/folder API | 4h | Medium | Deletion works |
| 7.6.7 | Rename API | 2h | Low | Rename works |
| 7.6.8 | Copy/move API | 4h | Medium | Copy/move works |
| 7.6.9 | File type detection (MIME) | 4h | Medium | Types detected |
| 7.6.10 | Search API | 8h | High | File search works |

**Subtotal:** 40 hours

### 7.7 File Manager App - Frontend

| ID | Task | Effort | Complexity | Acceptance Criteria |
| -- | ---- | ------ | ---------- | ------------------- |
| 7.7.1 | Directory view (list) | 4h | Medium | Files listed |
| 7.7.2 | Directory view (grid/icons) | 4h | Medium | Icon view works |
| 7.7.3 | File/folder icons | 4h | Medium | Correct icons shown |
| 7.7.4 | Navigation (breadcrumb/path bar) | 4h | Medium | Path navigation |
| 7.7.5 | Sidebar (favorites, places) | 4h | Medium | Sidebar navigation |
| 7.7.6 | Double-click to open | 4h | Medium | Opens folders/files |
| 7.7.7 | Context menu (right-click) | 4h | Medium | Actions menu |
| 7.7.8 | Keyboard navigation | 4h | Medium | Arrow keys work |
| 7.7.9 | Multi-select | 4h | Medium | Ctrl/Shift select |
| 7.7.10 | Drag and drop | 8h | High | Drag to move/copy |
| 7.7.11 | New folder dialog | 2h | Low | Create folder UI |
| 7.7.12 | Rename inline | 2h | Low | Inline rename |
| 7.7.13 | Delete confirmation | 2h | Low | Confirm delete |
| 7.7.14 | Copy/paste (keyboard) | 4h | Medium | Ctrl+C/V works |
| 7.7.15 | Address bar (type path) | 4h | Medium | Direct path entry |

**Subtotal:** 58 hours

### 7.8 Launcher App

| ID | Task | Effort | Complexity | Acceptance Criteria |
| -- | ---- | ------ | ---------- | ------------------- |
| 7.8.1 | Launcher page layout | 4h | Medium | Clean full-screen layout |
| 7.8.2 | App grid display | 4h | Medium | Apps in grid |
| 7.8.3 | App icons and labels | 4h | Medium | Icons displayed |
| 7.8.4 | App registry (available apps) | 4h | Medium | Apps discoverable |
| 7.8.5 | Search input | 4h | Medium | Type to filter |
| 7.8.6 | Filter apps by search | 4h | Medium | Filtering works |
| 7.8.7 | Keyboard navigation | 4h | Medium | Arrow/Enter works |
| 7.8.8 | Launch app on click/enter | 4h | Medium | App launches |
| 7.8.9 | Close on launch/escape | 2h | Low | Overlay closes |
| 7.8.10 | Animation (fade in/out) | 4h | Medium | Smooth transitions |
| 7.8.11 | Running apps indicator | 4h | Medium | Show running apps |

**Subtotal:** 42 hours

### 7.9 Common Components

| ID | Task | Effort | Complexity | Acceptance Criteria |
| -- | ---- | ------ | ---------- | ------------------- |
| 7.9.1 | UI component library | 8h | High | Reusable components |
| 7.9.2 | Button component | 2h | Low | Styled buttons |
| 7.9.3 | Input/text field component | 4h | Medium | Text inputs |
| 7.9.4 | Dropdown/select component | 4h | Medium | Dropdowns work |
| 7.9.5 | Toggle/checkbox component | 2h | Low | Toggles work |
| 7.9.6 | Modal dialog component | 4h | Medium | Modal dialogs |
| 7.9.7 | Toast/notification component | 4h | Medium | Notifications |
| 7.9.8 | Loading spinner | 2h | Low | Loading indication |
| 7.9.9 | Icon system | 4h | Medium | Icon library |
| 7.9.10 | Theme/styling system | 8h | High | Consistent theming |

**Subtotal:** 42 hours

---

## Technical Details

### Project Structure

```
src/
├── PanoramicData.Os.Terminal/
│   ├── PanoramicData.Os.Terminal.csproj
│   ├── TerminalServer.cs
│   ├── TerminalSession.cs
│   └── WebSocketHandler.cs
├── PanoramicData.Os.Shell/
│   ├── PanoramicData.Os.Shell.csproj
│   ├── Shell.cs
│   ├── CommandParser.cs
│   ├── CommandHistory.cs
│   ├── TabCompletion.cs
│   └── Commands/
│       ├── LsCommand.cs
│       ├── CdCommand.cs
│       ├── CatCommand.cs
│       └── ...
├── PanoramicData.Os.FileSystem/
│   ├── PanoramicData.Os.FileSystem.csproj
│   ├── FileSystemApi.cs
│   └── MimeTypeDetector.cs
└── PanoramicData.Os.Settings/
    ├── PanoramicData.Os.Settings.csproj
    ├── SettingsManager.cs
    └── SettingsPages/
        ├── NetworkSettings.cs
        ├── DisplaySettings.cs
        └── ...

www/
├── apps/
│   ├── terminal/
│   │   ├── index.html
│   │   ├── terminal.js
│   │   └── terminal.css
│   ├── settings/
│   │   ├── index.html
│   │   ├── settings.js
│   │   └── settings.css
│   ├── files/
│   │   ├── index.html
│   │   ├── files.js
│   │   └── files.css
│   └── launcher/
│       ├── index.html
│       ├── launcher.js
│       └── launcher.css
├── components/
│   ├── button.js
│   ├── input.js
│   ├── modal.js
│   └── ...
├── styles/
│   ├── theme.css
│   └── common.css
└── icons/
    ├── terminal.svg
    ├── settings.svg
    ├── folder.svg
    └── ...
```

### Terminal Architecture

```
┌─────────────────────────────────────────────────────────────────────────┐
│                           Browser Window                                 │
│  ┌───────────────────────────────────────────────────────────────────┐  │
│  │                         xterm.js                                   │  │
│  │  ┌─────────────────────────────────────────────────────────────┐  │  │
│  │  │                                                              │  │  │
│  │  │  root@panos:~# ls                                           │  │  │
│  │  │  bin  dev  etc  proc  sys  tmp  var                         │  │  │
│  │  │  root@panos:~# _                                            │  │  │
│  │  │                                                              │  │  │
│  │  └─────────────────────────────────────────────────────────────┘  │  │
│  └───────────────────────────────────────────────────────────────────┘  │
│                                    │                                     │
│                                    │ WebSocket                           │
│                                    ▼                                     │
├─────────────────────────────────────────────────────────────────────────┤
│                           .NET Backend                                   │
│  ┌───────────────────────────────────────────────────────────────────┐  │
│  │                     WebSocket Server                               │  │
│  │                            │                                       │  │
│  │                            ▼                                       │  │
│  │  ┌─────────────────────────────────────────────────────────────┐  │  │
│  │  │                  Terminal Session                            │  │  │
│  │  │  ┌─────────────┐           ┌─────────────────────────────┐  │  │  │
│  │  │  │     PTY     │◄─────────▶│          Shell              │  │  │  │
│  │  │  │  (master)   │           │  (panos shell process)      │  │  │  │
│  │  │  └─────────────┘           └─────────────────────────────┘  │  │  │
│  │  └─────────────────────────────────────────────────────────────┘  │  │
│  └───────────────────────────────────────────────────────────────────┘  │
│                                                                          │
└─────────────────────────────────────────────────────────────────────────┘
```

### File Manager API

```typescript
// JavaScript API (via panos bridge)

interface FileInfo {
    name: string;
    path: string;
    isDirectory: boolean;
    size: number;
    modified: Date;
    permissions: string;
    mimeType: string;
}

// List directory
const files: FileInfo[] = await panos.fs.list('/home');

// Read file
const content: string = await panos.fs.readFile('/etc/hostname');

// Write file
await panos.fs.writeFile('/tmp/test.txt', 'Hello');

// Create directory
await panos.fs.mkdir('/home/documents');

// Delete
await panos.fs.delete('/tmp/test.txt');

// Move/rename
await panos.fs.move('/tmp/old.txt', '/tmp/new.txt');

// Copy
await panos.fs.copy('/tmp/source.txt', '/tmp/dest.txt');

// Search
const results: FileInfo[] = await panos.fs.search('/home', '*.txt');
```

### Settings Storage Format

```json
// /etc/panos/settings.json
{
    "system": {
        "hostname": "panos",
        "timezone": "UTC"
    },
    "network": {
        "interfaces": {
            "eth0": {
                "mode": "dhcp"
            }
        },
        "dns": ["8.8.8.8", "8.8.4.4"]
    },
    "display": {
        "resolution": "1920x1080",
        "dpi": 96
    },
    "appearance": {
        "theme": "dark",
        "accentColor": "#0078d4"
    },
    "keyboard": {
        "layout": "us",
        "repeatDelay": 500,
        "repeatRate": 30
    },
    "ssh": {
        "authorizedKeys": [
            "ssh-ed25519 AAAA... user@host"
        ]
    }
}
```

### Launcher App Registry

```json
// /etc/panos/apps.json
{
    "apps": [
        {
            "id": "terminal",
            "name": "Terminal",
            "icon": "/icons/terminal.svg",
            "url": "panos://terminal",
            "category": "system"
        },
        {
            "id": "settings",
            "name": "Settings",
            "icon": "/icons/settings.svg",
            "url": "panos://settings",
            "category": "system"
        },
        {
            "id": "files",
            "name": "Files",
            "icon": "/icons/folder.svg",
            "url": "panos://files",
            "category": "system"
        },
        {
            "id": "browser",
            "name": "Browser",
            "icon": "/icons/browser.svg",
            "url": "https://www.google.com",
            "category": "internet"
        }
    ]
}
```

### UI Component Example

```javascript
// components/button.js
export class PanosButton extends HTMLElement {
    constructor() {
        super();
        this.attachShadow({ mode: 'open' });
    }
    
    connectedCallback() {
        this.shadowRoot.innerHTML = `
            <style>
                button {
                    background: var(--accent-color, #0078d4);
                    color: white;
                    border: none;
                    border-radius: 4px;
                    padding: 8px 16px;
                    font-size: 14px;
                    cursor: pointer;
                    transition: background 0.2s;
                }
                button:hover {
                    background: var(--accent-hover, #006cbd);
                }
                button:active {
                    background: var(--accent-active, #005a9e);
                }
                button:disabled {
                    background: #ccc;
                    cursor: not-allowed;
                }
            </style>
            <button>
                <slot></slot>
            </button>
        `;
    }
}

customElements.define('panos-button', PanosButton);
```

---

## Testing Requirements

### Unit Tests

| Test | Description |
| ---- | ----------- |
| TerminalServer_AcceptConnection | WS connection accepted |
| TerminalSession_PtyCreated | PTY allocated |
| TerminalSession_DataFlow | Data flows both ways |
| Shell_CommandParsing | Commands parsed correctly |
| Shell_History | History navigable |
| Shell_TabComplete | Completion works |
| FileSystemApi_List | Directory listing works |
| FileSystemApi_ReadWrite | Read/write works |
| SettingsManager_Load | Settings loaded |
| SettingsManager_Save | Settings saved |

### Integration Tests

| Test | Description |
| ---- | ----------- |
| Terminal_FullSession | Type command, see output |
| Terminal_Resize | Resize updates terminal |
| Settings_NetworkChange | Network settings apply |
| FileManager_Navigate | Navigate directories |
| FileManager_CreateDelete | CRUD operations work |
| Launcher_Search | Search filters apps |
| Launcher_Launch | App launches from launcher |

### E2E Tests (Playwright)

| Test | Description |
| ---- | ----------- |
| Terminal_RunLs | Run ls command in terminal |
| Files_CreateFolder | Create folder via UI |
| Settings_ChangeHostname | Change hostname in settings |
| Launcher_OpenTerminal | Open terminal from launcher |

---

## Deliverables

1. **Terminal app** - xterm.js + WebSocket + PTY
2. **Settings app** - System configuration UI
3. **File Manager app** - Filesystem browser
4. **Launcher app** - Application launcher
5. **UI component library** - Reusable components
6. **Shell enhancements** - History, completion, editing

---

## Risks

| Risk | Probability | Impact | Mitigation |
| ---- | ----------- | ------ | ---------- |
| xterm.js integration issues | Low | Medium | Well-documented library |
| WebSocket performance | Low | Medium | Binary messages; compression |
| File system security | Medium | High | Proper permissions; sandboxing |
| Complex UI requirements | Medium | Medium | Start simple; iterate |
| Settings format changes | Low | Low | Version settings schema |

---

## Exit Criteria

- [ ] Terminal app runs shell commands
- [ ] Terminal supports colors and resize
- [ ] Shell has history and tab completion
- [ ] Settings app displays and saves settings
- [ ] Network settings functional
- [ ] File manager browses filesystem
- [ ] File operations (create, delete, rename) work
- [ ] Launcher shows all apps
- [ ] Launcher search works
- [ ] All unit tests pass
- [ ] E2E tests pass

---

## Demo Milestone

**Demo:** Open terminal, run shell command, see output.

```
# User opens launcher (Super key)
# Clicks Terminal icon

Terminal window opens:

┌─────────────────────────────────────────────────────────────────┐
│ 📁 Terminal                                        ─  □  ✕     │
├─────────────────────────────────────────────────────────────────┤
│                                                                  │
│  PanoramicData.Os v0.1                                          │
│  Welcome to panos shell                                          │
│                                                                  │
│  root@panos:~# ls                                               │
│  bin  dev  etc  proc  sys  tmp  var                             │
│                                                                  │
│  root@panos:~# cat /etc/hostname                                │
│  panos                                                           │
│                                                                  │
│  root@panos:~# ping 8.8.8.8                                     │
│  PING 8.8.8.8: 64 bytes, seq=1, ttl=64, time=12.3ms            │
│  ^C                                                              │
│                                                                  │
│  root@panos:~# _                                                │
│                                                                  │
└─────────────────────────────────────────────────────────────────┘

# User opens Settings from launcher
# Navigates to Network page
# Sees current IP configuration

# User opens Files from launcher  
# Browses to /etc/
# Sees list of config files
```

---

## Estimated Total Effort

| Section | Hours |
| ------- | ----- |
| 7.1 Terminal App - Backend | 38 |
| 7.2 Terminal App - Frontend | 34 |
| 7.3 Shell Enhancements | 48 |
| 7.4 Settings App - Framework | 24 |
| 7.5 Settings App - Pages | 36 |
| 7.6 File Manager App - Backend | 40 |
| 7.7 File Manager App - Frontend | 58 |
| 7.8 Launcher App | 42 |
| 7.9 Common Components | 42 |
| **Total** | **362 hours** |

At 40 hours/week = **~9 weeks** (Compressed to 3 weeks requires significant scope reduction or parallel work)

---

## References

- [xterm.js](https://xtermjs.org/)
- [Web Components](https://developer.mozilla.org/en-US/docs/Web/Web_Components)
- [WebSocket API](https://developer.mozilla.org/en-US/docs/Web/API/WebSockets_API)
- [GNOME Files (Nautilus)](https://wiki.gnome.org/Apps/Files)
- [GNOME Settings](https://wiki.gnome.org/Design/Apps/Settings)

# Phase 3: SSH Server

**Duration:** 3 weeks  
**Dependencies:** Phase 2 (Network Stack)  
**Owner:** TBD  
**Status:** 🔲 Not Started

---

## Objectives

1. Implement SSH-2 protocol server in .NET
2. Support public key authentication (Ed25519, RSA 4096-bit)
3. Provide PTY allocation for terminal sessions
4. Integrate with the shell from Phase 1
5. No password authentication (security requirement)

---

## Prerequisites

- Phase 2 complete (working TCP stack)
- TCP listener functional
- Cryptographic libraries available
- Understanding of SSH-2 protocol

---

## Tasks

### 3.1 SSH Protocol Foundation

| ID | Task | Effort | Complexity | Acceptance Criteria |
| -- | ---- | ------ | ---------- | ------------------- |
| 3.1.1 | SSH packet framing (binary packet protocol) | 4h | Medium | Packets framed correctly |
| 3.1.2 | SSH message parsing | 8h | High | All message types parsed |
| 3.1.3 | SSH message building | 8h | High | Valid messages constructed |
| 3.1.4 | Version exchange (SSH-2.0-PanoramicData) | 2h | Low | Version string sent/received |
| 3.1.5 | Packet MAC calculation | 4h | Medium | MACs calculated correctly |
| 3.1.6 | Packet encryption/decryption | 8h | High | Encryption works |
| 3.1.7 | Compression (optional, zlib) | 4h | Medium | Compression works if enabled |

**Subtotal:** 38 hours

### 3.2 Key Exchange

| ID | Task | Effort | Complexity | Acceptance Criteria |
| -- | ---- | ------ | ---------- | ------------------- |
| 3.2.1 | Algorithm negotiation (KEXINIT) | 8h | High | Algorithms negotiated |
| 3.2.2 | Curve25519 key exchange | 8h | High | Key exchange completes |
| 3.2.3 | Diffie-Hellman Group 14 (fallback) | 8h | High | DH works for older clients |
| 3.2.4 | Session key derivation | 4h | Medium | Keys derived correctly |
| 3.2.5 | Host key signing | 4h | Medium | Host key signs exchange |
| 3.2.6 | NEWKEYS message handling | 2h | Low | Encryption activated |
| 3.2.7 | Re-keying support | 4h | Medium | Re-key after 1GB or 1 hour |

**Subtotal:** 38 hours

### 3.3 Host Key Management

| ID | Task | Effort | Complexity | Acceptance Criteria |
| -- | ---- | ------ | ---------- | ------------------- |
| 3.3.1 | Ed25519 host key generation | 4h | Medium | Valid key pair generated |
| 3.3.2 | RSA host key generation | 4h | Medium | 4096-bit RSA key generated |
| 3.3.3 | Host key storage (filesystem) | 2h | Low | Keys persisted to disk |
| 3.3.4 | Host key loading on startup | 2h | Low | Keys loaded from disk |
| 3.3.5 | Host key fingerprint display | 2h | Low | Fingerprint shown on console |
| 3.3.6 | Auto-generate keys on first boot | 2h | Low | Keys created if missing |

**Subtotal:** 16 hours

### 3.4 Public Key Authentication

| ID | Task | Effort | Complexity | Acceptance Criteria |
| -- | ---- | ------ | ---------- | ------------------- |
| 3.4.1 | Authorized keys file parsing | 4h | Medium | OpenSSH format parsed |
| 3.4.2 | Ed25519 signature verification | 4h | Medium | Ed25519 auth works |
| 3.4.3 | RSA signature verification | 4h | Medium | RSA auth works |
| 3.4.4 | SSH_MSG_USERAUTH_REQUEST handling | 4h | Medium | Auth requests processed |
| 3.4.5 | Authentication state machine | 4h | Medium | Retry limits enforced |
| 3.4.6 | Reject password authentication | 1h | Low | Password auth refused |
| 3.4.7 | Authentication banner | 2h | Low | Banner displayed pre-auth |

**Subtotal:** 23 hours

### 3.5 Channel Management

| ID | Task | Effort | Complexity | Acceptance Criteria |
| -- | ---- | ------ | ---------- | ------------------- |
| 3.5.1 | Channel open/close handling | 4h | Medium | Channels created/destroyed |
| 3.5.2 | Channel data transfer | 4h | Medium | Data flows through channel |
| 3.5.3 | Window size management | 4h | Medium | Flow control works |
| 3.5.4 | Channel EOF handling | 2h | Low | EOF propagated correctly |
| 3.5.5 | Multiple channels per session | 4h | Medium | Multiple channels work |
| 3.5.6 | Channel request handling | 4h | Medium | Requests processed |

**Subtotal:** 22 hours

### 3.6 PTY Allocation

| ID | Task | Effort | Complexity | Acceptance Criteria |
| -- | ---- | ------ | ---------- | ------------------- |
| 3.6.1 | P/Invoke for pty operations | 4h | Medium | openpty/forkpty available |
| 3.6.2 | PTY allocation request handling | 4h | Medium | PTY created on request |
| 3.6.3 | Terminal size (SIGWINCH) | 2h | Low | Resize events handled |
| 3.6.4 | Terminal modes configuration | 4h | Medium | Modes set correctly |
| 3.6.5 | PTY read/write loop | 4h | Medium | Data flows through PTY |
| 3.6.6 | PTY cleanup on disconnect | 2h | Low | PTY freed on close |

**Subtotal:** 20 hours

### 3.7 Shell Integration

| ID | Task | Effort | Complexity | Acceptance Criteria |
| -- | ---- | ------ | ---------- | ------------------- |
| 3.7.1 | Shell process spawning | 4h | Medium | Shell starts on connection |
| 3.7.2 | Environment variable setup | 2h | Low | PATH, TERM, USER set |
| 3.7.3 | Working directory (home) | 2h | Low | Starts in home directory |
| 3.7.4 | Signal forwarding (SIGINT, etc.) | 4h | Medium | Ctrl+C works |
| 3.7.5 | Exit code propagation | 2h | Low | Exit code sent to client |
| 3.7.6 | Shell command from auth | 2h | Low | exec request handled |

**Subtotal:** 16 hours

### 3.8 SSH Server Service

| ID | Task | Effort | Complexity | Acceptance Criteria |
| -- | ---- | ------ | ---------- | ------------------- |
| 3.8.1 | TCP listener on port 22 | 2h | Low | Listens on port 22 |
| 3.8.2 | Connection acceptance | 2h | Low | Connections accepted |
| 3.8.3 | Connection limit enforcement | 2h | Low | Max connections enforced |
| 3.8.4 | Session timeout handling | 2h | Low | Idle sessions closed |
| 3.8.5 | Graceful shutdown | 2h | Low | Clean shutdown on SIGTERM |
| 3.8.6 | Logging (connections, auth attempts) | 4h | Medium | Security logging works |
| 3.8.7 | Failed auth rate limiting | 4h | Medium | Brute force protection |

**Subtotal:** 18 hours

### 3.9 Cipher Suites

| ID | Task | Effort | Complexity | Acceptance Criteria |
| -- | ---- | ------ | ---------- | ------------------- |
| 3.9.1 | ChaCha20-Poly1305 cipher | 8h | High | Modern cipher works |
| 3.9.2 | AES-256-GCM cipher | 8h | High | AES-GCM works |
| 3.9.3 | AES-256-CTR cipher (legacy) | 4h | Medium | CTR mode works |
| 3.9.4 | HMAC-SHA256 MAC | 4h | Medium | MAC calculation works |
| 3.9.5 | HMAC-SHA512 MAC | 2h | Low | SHA512 MAC works |

**Subtotal:** 26 hours

---

## Technical Details

### Project Structure

```text
src/
└── PanoramicData.Os.Ssh/
    ├── PanoramicData.Os.Ssh.csproj
    ├── Server/
    │   ├── SshServer.cs
    │   ├── SshConnection.cs
    │   └── SshSession.cs
    ├── Protocol/
    │   ├── SshPacket.cs
    │   ├── SshMessage.cs
    │   └── MessageTypes/
    │       ├── KexInit.cs
    │       ├── KexDhInit.cs
    │       ├── UserauthRequest.cs
    │       ├── ChannelOpen.cs
    │       └── ...
    ├── KeyExchange/
    │   ├── IKeyExchange.cs
    │   ├── Curve25519Kex.cs
    │   └── DiffieHellmanGroup14.cs
    ├── Authentication/
    │   ├── IAuthMethod.cs
    │   ├── PublicKeyAuth.cs
    │   └── AuthorizedKeys.cs
    ├── Ciphers/
    │   ├── ICipher.cs
    │   ├── ChaCha20Poly1305.cs
    │   ├── Aes256Gcm.cs
    │   └── Aes256Ctr.cs
    ├── Channels/
    │   ├── SshChannel.cs
    │   ├── SessionChannel.cs
    │   └── ChannelManager.cs
    ├── Pty/
    │   ├── PseudoTerminal.cs
    │   └── TerminalModes.cs
    └── HostKeys/
        ├── HostKeyManager.cs
        ├── Ed25519HostKey.cs
        └── RsaHostKey.cs
```

### Supported Algorithms

| Category | Algorithms (in order of preference) |
| -------- | ----------------------------------- |
| Key Exchange | curve25519-sha256, diffie-hellman-group14-sha256 |
| Host Key | ssh-ed25519, rsa-sha2-512, rsa-sha2-256 |
| Cipher | chacha20-poly1305@openssh.com, aes256-gcm@openssh.com, aes256-ctr |
| MAC | (implicit with AEAD), hmac-sha2-256, hmac-sha2-512 |
| Compression | none, zlib@openssh.com |

### SSH Packet Format

```text
uint32    packet_length  (excluding MAC and length field itself)
byte      padding_length
byte[n1]  payload        (n1 = packet_length - padding_length - 1)
byte[n2]  padding        (n2 = padding_length)
byte[m]   mac            (Message Authentication Code)
```

### Key Exchange Sequence

```text
Client                                          Server
   │                                               │
   │ ─────── SSH-2.0-OpenSSH_9.0 ───────────────▶ │
   │ ◀─────── SSH-2.0-PanoramicData_1.0 ──────── │
   │                                               │
   │ ─────── SSH_MSG_KEXINIT ───────────────────▶ │
   │ ◀─────── SSH_MSG_KEXINIT ────────────────── │
   │                                               │
   │ ─────── SSH_MSG_KEX_ECDH_INIT ─────────────▶ │
   │ ◀─────── SSH_MSG_KEX_ECDH_REPLY ─────────── │
   │          (contains host key + signature)      │
   │                                               │
   │ ─────── SSH_MSG_NEWKEYS ───────────────────▶ │
   │ ◀─────── SSH_MSG_NEWKEYS ────────────────── │
   │                                               │
   │        [Encrypted from here on]               │
```

### Authentication Sequence

```text
Client                                          Server
   │                                               │
   │ ─────── SSH_MSG_SERVICE_REQUEST ───────────▶ │
   │          (ssh-userauth)                       │
   │ ◀─────── SSH_MSG_SERVICE_ACCEPT ─────────── │
   │                                               │
   │ ─────── SSH_MSG_USERAUTH_REQUEST ──────────▶ │
   │          (publickey, with signature)          │
   │                                               │
   │ ◀─────── SSH_MSG_USERAUTH_SUCCESS ────────── │
   │   or     SSH_MSG_USERAUTH_FAILURE             │
```

### Critical P/Invoke Declarations

```csharp
// Open pseudo-terminal
[LibraryImport("libc", SetLastError = true)]
internal static partial int openpty(
    ref int amaster, 
    ref int aslave,
    nint name,
    ref Termios termp,
    ref Winsize winp);

// Set terminal window size
[LibraryImport("libc", SetLastError = true)]
internal static partial int ioctl(int fd, ulong request, ref Winsize ws);

// Structures
[StructLayout(LayoutKind.Sequential)]
internal struct Winsize
{
    public ushort ws_row;
    public ushort ws_col;
    public ushort ws_xpixel;
    public ushort ws_ypixel;
}
```

### Authorized Keys Format

```sh
# /etc/ssh/authorized_keys
ssh-ed25519 AAAAC3NzaC1lZDI1NTE5AAAAI... user@host
ssh-rsa AAAAB3NzaC1yc2EAAAADAQABAAACAQ... user@host
```

---

## Testing Requirements

### Unit Tests

| Test | Description |
| ---- | ----------- |
| SshPacket_Frame | Packet framing correct |
| SshPacket_Encrypt | Encryption works |
| SshPacket_Decrypt | Decryption works |
| SshPacket_Mac | MAC calculation correct |
| Curve25519Kex_Complete | Key exchange completes |
| Ed25519Verify_Valid | Valid signature accepted |
| Ed25519Verify_Invalid | Invalid signature rejected |
| RsaVerify_Valid | RSA signature accepted |
| AuthorizedKeys_Parse | OpenSSH format parsed |
| Channel_DataTransfer | Data flows correctly |

### Integration Tests

| Test | Description |
| ---- | ----------- |
| Ssh_VersionExchange | Version strings exchanged |
| Ssh_KeyExchange | Key exchange completes |
| Ssh_PublicKeyAuth | Ed25519 auth succeeds |
| Ssh_RejectPassword | Password auth rejected |
| Ssh_OpenSession | Session channel opens |
| Ssh_RunCommand | Shell command executes |
| Ssh_Interoperability | OpenSSH client connects |

### Security Tests

| Test | Description |
| ---- | ----------- |
| Ssh_InvalidSignature | Auth fails with bad sig |
| Ssh_UnknownKey | Auth fails with unknown key |
| Ssh_RateLimiting | Brute force blocked |
| Ssh_ReplayAttack | Replayed packets rejected |

---

## Deliverables

1. **PanoramicData.Os.Ssh library** - SSH-2 server implementation
2. **Host key management** - Generation and storage
3. **Authorized keys support** - Public key authentication
4. **PTY support** - Terminal allocation
5. **Shell integration** - Connected to built-in shell
6. **Security hardening** - Rate limiting, logging

---

## Risks

| Risk | Probability | Impact | Mitigation |
| ---- | ----------- | ------ | ---------- |
| Crypto implementation bugs | Medium | Critical | Use well-tested libraries |
| Protocol vulnerabilities | Low | Critical | Follow RFCs exactly; security review |
| Interoperability issues | Medium | Medium | Test with OpenSSH, PuTTY |
| PTY allocation complexity | Medium | Medium | Fallback to simple pipes |
| Performance under load | Low | Low | Connection limits; async I/O |

---

## Exit Criteria

- [ ] SSH server starts on port 22
- [ ] OpenSSH client connects successfully
- [ ] Ed25519 public key auth works
- [ ] RSA 4096-bit public key auth works
- [ ] Password auth is rejected
- [ ] Shell commands execute via SSH
- [ ] Terminal resizing works
- [ ] Multiple concurrent sessions work
- [ ] All unit tests pass
- [ ] Security tests pass

---

## Demo Milestone

**Demo:** SSH in and run `ls` command.

```bash
# From host machine
$ ssh -i ~/.ssh/id_ed25519 root@10.0.2.15

   ____                                        _      ____        _
  |  _ \ __ _ _ __   ___  _ __ __ _ _ __ ___ (_) ___|  _ \  __ _| |_ __ _
  | |_) / _` | '_ \ / _ \| '__/ _` | '_ ` _ \| |/ __| | | |/ _` | __/ _` |
  |  __/ (_| | | | | (_) | | | (_| | | | | | | | (__| |_| | (_| | || (_| |
  |_|   \__,_|_| |_|\___/|_|  \__,_|_| |_| |_|_|\___|____/ \__,_|\__\__,_|

  PanoramicData.Os v0.1
  Logged in as root

panos> ls
bin     dev     etc     init    proc    sys     tmp     var

panos> pwd
/

panos> exit
Connection to 10.0.2.15 closed.
```

---

## Estimated Total Effort

| Section | Hours |
| ------- | ----- |
| 3.1 SSH Protocol Foundation | 38 |
| 3.2 Key Exchange | 38 |
| 3.3 Host Key Management | 16 |
| 3.4 Public Key Authentication | 23 |
| 3.5 Channel Management | 22 |
| 3.6 PTY Allocation | 20 |
| 3.7 Shell Integration | 16 |
| 3.8 SSH Server Service | 18 |
| 3.9 Cipher Suites | 26 |
| **Total** | **217 hours** |

At 40 hours/week = **~5.5 weeks** (Compressed to 3 weeks assumes reduced cipher suite or parallel work)

---

## References

- [RFC 4251 - SSH Protocol Architecture](https://tools.ietf.org/html/rfc4251)
- [RFC 4252 - SSH Authentication Protocol](https://tools.ietf.org/html/rfc4252)
- [RFC 4253 - SSH Transport Layer Protocol](https://tools.ietf.org/html/rfc4253)
- [RFC 4254 - SSH Connection Protocol](https://tools.ietf.org/html/rfc4254)
- [RFC 8332 - RSA Keys in SSH](https://tools.ietf.org/html/rfc8332)
- [RFC 8709 - Ed25519 in SSH](https://tools.ietf.org/html/rfc8709)
- [OpenSSH Source](https://github.com/openssh/openssh-portable)

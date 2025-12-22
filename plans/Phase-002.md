# Phase 2: Network Stack

**Duration:** 3 weeks  
**Dependencies:** Phase 1 (Foundation & Boot)  
**Owner:** TBD  
**Status:** 🔲 Not Started

---

## Objectives

1. Implement a complete TCP/IP network stack in .NET
2. Support IPv4 and IPv6 dual-stack networking
3. Implement DHCP clients for automatic IP configuration
4. Support static IP configuration
5. Provide DNS resolution
6. Enable ping (ICMP) functionality

---

## Prerequisites

- Phase 1 complete (bootable system with .NET init)
- Network interface drivers enabled in kernel (E1000, virtio-net)
- Understanding of TCP/IP protocols
- Raw socket access via P/Invoke

---

## Tasks

### 2.1 Network Interface Layer

| ID | Task | Effort | Complexity | Acceptance Criteria |
| -- | ---- | ------ | ---------- | ------------------- |
| 2.1.1 | Create NetworkInterface abstraction | 4h | Medium | Interface detected and enumerable |
| 2.1.2 | P/Invoke for raw socket creation | 4h | Medium | AF_PACKET socket opens |
| 2.1.3 | Read MAC address from interface | 2h | Low | Correct MAC returned |
| 2.1.4 | Implement packet send (raw) | 4h | Medium | Frames transmitted |
| 2.1.5 | Implement packet receive (raw) | 4h | Medium | Frames received |
| 2.1.6 | Interface up/down control | 2h | Low | Link state controllable |
| 2.1.7 | MTU detection and handling | 2h | Low | MTU respected |

**Subtotal:** 22 hours

### 2.2 Ethernet Frame Handling

| ID | Task | Effort | Complexity | Acceptance Criteria |
| -- | ---- | ------ | ---------- | ------------------- |
| 2.2.1 | Ethernet frame parser | 4h | Medium | Frames parsed correctly |
| 2.2.2 | Ethernet frame builder | 4h | Medium | Valid frames constructed |
| 2.2.3 | EtherType dispatching | 2h | Low | IPv4, IPv6, ARP routed |
| 2.2.4 | Promiscuous mode support | 2h | Low | All frames received |
| 2.2.5 | Frame checksum validation | 2h | Low | Bad frames rejected |

**Subtotal:** 14 hours

### 2.3 ARP (Address Resolution Protocol)

| ID | Task | Effort | Complexity | Acceptance Criteria |
| -- | ---- | ------ | ---------- | ------------------- |
| 2.3.1 | ARP packet parser | 2h | Low | ARP requests/replies parsed |
| 2.3.2 | ARP packet builder | 2h | Low | Valid ARP packets created |
| 2.3.3 | ARP cache implementation | 4h | Medium | Cache stores entries with TTL |
| 2.3.4 | ARP request handling (respond) | 2h | Low | Replies sent for our IPs |
| 2.3.5 | ARP resolution (send request) | 4h | Medium | Resolves IP to MAC |
| 2.3.6 | Gratuitous ARP on IP config | 2h | Low | GARP sent on IP assignment |

**Subtotal:** 16 hours

### 2.4 IPv4 Implementation

| ID | Task | Effort | Complexity | Acceptance Criteria |
| -- | ---- | ------ | ---------- | ------------------- |
| 2.4.1 | IPv4 header parser | 4h | Medium | Headers parsed correctly |
| 2.4.2 | IPv4 header builder | 4h | Medium | Valid headers constructed |
| 2.4.3 | IPv4 checksum calculation | 2h | Low | Checksums valid |
| 2.4.4 | IPv4 address configuration | 4h | Medium | IP/subnet/gateway stored |
| 2.4.5 | IPv4 routing table | 8h | High | Route lookup works |
| 2.4.6 | IPv4 fragmentation (basic) | 4h | Medium | Large packets fragmented |
| 2.4.7 | IPv4 reassembly (basic) | 4h | Medium | Fragments reassembled |
| 2.4.8 | Protocol dispatching (ICMP, TCP, UDP) | 2h | Low | Packets routed to handlers |

**Subtotal:** 32 hours

### 2.5 IPv6 Implementation

| ID | Task | Effort | Complexity | Acceptance Criteria |
| -- | ---- | ------ | ---------- | ------------------- |
| 2.5.1 | IPv6 header parser | 4h | Medium | Headers parsed correctly |
| 2.5.2 | IPv6 header builder | 4h | Medium | Valid headers constructed |
| 2.5.3 | IPv6 address configuration | 4h | Medium | IP/prefix stored |
| 2.5.4 | Link-local address generation | 2h | Low | fe80:: address created |
| 2.5.5 | IPv6 routing table | 4h | Medium | Route lookup works |
| 2.5.6 | Extension header handling | 4h | Medium | Common headers parsed |
| 2.5.7 | Protocol dispatching (ICMPv6, TCP, UDP) | 2h | Low | Packets routed to handlers |

**Subtotal:** 24 hours

### 2.6 NDP (Neighbor Discovery Protocol)

| ID | Task | Effort | Complexity | Acceptance Criteria |
| -- | ---- | ------ | ---------- | ------------------- |
| 2.6.1 | ICMPv6 base implementation | 4h | Medium | ICMPv6 packets handled |
| 2.6.2 | Neighbor Solicitation handling | 4h | Medium | NS requests answered |
| 2.6.3 | Neighbor Advertisement handling | 4h | Medium | NA responses processed |
| 2.6.4 | Router Solicitation | 4h | Medium | RS sent on boot |
| 2.6.5 | Router Advertisement handling | 4h | Medium | RA processed for SLAAC |
| 2.6.6 | Neighbor cache | 4h | Medium | IPv6 neighbor cache works |
| 2.6.7 | Duplicate Address Detection | 4h | Medium | DAD performed before use |

**Subtotal:** 28 hours

### 2.7 ICMP (Ping)

| ID | Task | Effort | Complexity | Acceptance Criteria |
| -- | ---- | ------ | ---------- | ------------------- |
| 2.7.1 | ICMPv4 Echo Request builder | 2h | Low | Valid ping packets |
| 2.7.2 | ICMPv4 Echo Reply handler | 2h | Low | Replies received |
| 2.7.3 | ICMPv4 Echo Request handler | 2h | Low | Respond to pings |
| 2.7.4 | ICMPv6 Echo Request builder | 2h | Low | Valid ping6 packets |
| 2.7.5 | ICMPv6 Echo Reply handler | 2h | Low | Replies received |
| 2.7.6 | ICMPv6 Echo Request handler | 2h | Low | Respond to ping6 |
| 2.7.7 | Ping command implementation | 4h | Medium | `ping` shell command works |

**Subtotal:** 16 hours

### 2.8 UDP Implementation

| ID | Task | Effort | Complexity | Acceptance Criteria |
| -- | ---- | ------ | ---------- | ------------------- |
| 2.8.1 | UDP header parser | 2h | Low | Headers parsed |
| 2.8.2 | UDP header builder | 2h | Low | Valid headers built |
| 2.8.3 | UDP checksum (optional for IPv4) | 2h | Low | Checksum calculated |
| 2.8.4 | UDP socket abstraction | 4h | Medium | Bind/send/receive works |
| 2.8.5 | UDP port management | 2h | Low | Ports allocated/freed |
| 2.8.6 | UDP receive buffer | 4h | Medium | Datagrams queued |

**Subtotal:** 16 hours

### 2.9 TCP Implementation

| ID | Task | Effort | Complexity | Acceptance Criteria |
| -- | ---- | ------ | ---------- | ------------------- |
| 2.9.1 | TCP header parser | 4h | Medium | Headers parsed correctly |
| 2.9.2 | TCP header builder | 4h | Medium | Valid headers built |
| 2.9.3 | TCP checksum calculation | 2h | Low | Checksums valid |
| 2.9.4 | TCP state machine | 16h | High | All states implemented |
| 2.9.5 | TCP connection establishment (3-way) | 8h | High | SYN/SYN-ACK/ACK works |
| 2.9.6 | TCP connection termination | 4h | Medium | FIN handshake works |
| 2.9.7 | TCP send buffer and flow control | 8h | High | Window management |
| 2.9.8 | TCP receive buffer | 8h | High | Out-of-order handling |
| 2.9.9 | TCP retransmission | 8h | High | Lost packets retransmitted |
| 2.9.10 | TCP congestion control (basic) | 4h | Medium | Slow start implemented |
| 2.9.11 | TCP socket abstraction | 4h | Medium | Connect/listen/accept |
| 2.9.12 | TCP keep-alive | 2h | Low | Keep-alive optional |

**Subtotal:** 72 hours

### 2.10 DHCP Clients

| ID | Task | Effort | Complexity | Acceptance Criteria |
| -- | ---- | ------ | ---------- | ------------------- |
| 2.10.1 | DHCPv4 DISCOVER message | 4h | Medium | Valid DISCOVER sent |
| 2.10.2 | DHCPv4 OFFER parsing | 4h | Medium | OFFER parsed correctly |
| 2.10.3 | DHCPv4 REQUEST message | 2h | Low | Valid REQUEST sent |
| 2.10.4 | DHCPv4 ACK handling | 4h | Medium | IP configured from ACK |
| 2.10.5 | DHCPv4 lease renewal | 4h | Medium | Lease renewed before expiry |
| 2.10.6 | DHCPv4 option parsing (DNS, gateway) | 4h | Medium | Options extracted |
| 2.10.7 | DHCPv6 SOLICIT message | 4h | Medium | Valid SOLICIT sent |
| 2.10.8 | DHCPv6 ADVERTISE handling | 4h | Medium | ADVERTISE parsed |
| 2.10.9 | DHCPv6 REQUEST/REPLY | 4h | Medium | Address assigned |
| 2.10.10 | SLAAC implementation | 4h | Medium | Stateless config works |

**Subtotal:** 38 hours

### 2.11 DNS Resolver

| ID | Task | Effort | Complexity | Acceptance Criteria |
| -- | ---- | ------ | ---------- | ------------------- |
| 2.11.1 | DNS query builder | 4h | Medium | Valid DNS queries |
| 2.11.2 | DNS response parser | 4h | Medium | A/AAAA records parsed |
| 2.11.3 | DNS cache | 4h | Medium | Results cached with TTL |
| 2.11.4 | DNS resolver service | 4h | Medium | Async resolution works |
| 2.11.5 | /etc/resolv.conf parsing | 2h | Low | DNS servers configured |
| 2.11.6 | DNS over UDP | 2h | Low | UDP queries work |
| 2.11.7 | DNS over TCP (fallback) | 2h | Low | TCP for large responses |

**Subtotal:** 22 hours

### 2.12 Network Configuration Commands

| ID | Task | Effort | Complexity | Acceptance Criteria |
| -- | ---- | ------ | ---------- | ------------------- |
| 2.12.1 | `ifconfig` command | 4h | Medium | Shows/sets interface config |
| 2.12.2 | `ip addr` command | 4h | Medium | Modern IP config display |
| 2.12.3 | `ip route` command | 4h | Medium | Routing table display |
| 2.12.4 | `netstat` command | 4h | Medium | Connection list |
| 2.12.5 | `ss` command | 4h | Medium | Socket statistics |
| 2.12.6 | `route` command | 2h | Low | Legacy routing display |

**Subtotal:** 22 hours

---

## Technical Details

### Project Structure

```text
src/
└── PanoramicData.Os.Network/
    ├── PanoramicData.Os.Network.csproj
    ├── Interfaces/
    │   ├── INetworkInterface.cs
    │   └── NetworkInterface.cs
    ├── Ethernet/
    │   ├── EthernetFrame.cs
    │   └── EtherType.cs
    ├── Arp/
    │   ├── ArpPacket.cs
    │   └── ArpCache.cs
    ├── Ip/
    │   ├── IPv4/
    │   │   ├── IPv4Packet.cs
    │   │   ├── IPv4Address.cs
    │   │   └── IPv4RoutingTable.cs
    │   └── IPv6/
    │       ├── IPv6Packet.cs
    │       ├── IPv6Address.cs
    │       └── IPv6RoutingTable.cs
    ├── Icmp/
    │   ├── IcmpPacket.cs
    │   └── Icmpv6Packet.cs
    ├── Ndp/
    │   ├── NeighborSolicitation.cs
    │   ├── NeighborAdvertisement.cs
    │   └── NeighborCache.cs
    ├── Udp/
    │   ├── UdpPacket.cs
    │   └── UdpSocket.cs
    ├── Tcp/
    │   ├── TcpPacket.cs
    │   ├── TcpSocket.cs
    │   ├── TcpConnection.cs
    │   └── TcpStateMachine.cs
    ├── Dhcp/
    │   ├── DhcpClient.cs
    │   └── Dhcpv6Client.cs
    └── Dns/
        ├── DnsQuery.cs
        ├── DnsResponse.cs
        └── DnsResolver.cs
```

### Key Design Decisions

1. **Raw Sockets via P/Invoke**
   - Use AF_PACKET sockets for layer 2 access
   - Bypass kernel TCP/IP stack entirely
   - Full control over packet construction

2. **Async I/O Model**
   - All network operations are async
   - Use .NET async/await patterns
   - Non-blocking socket operations

3. **Memory Efficiency**
   - Use Span<byte> for packet parsing
   - Pool buffers for receive operations
   - Avoid allocations in hot paths

### Critical P/Invoke Declarations

```csharp
// Raw socket creation
[LibraryImport("libc", SetLastError = true)]
internal static partial int socket(int domain, int type, int protocol);

// Bind to interface
[LibraryImport("libc", SetLastError = true)]
internal static partial int bind(int sockfd, ref SockAddrLl addr, int addrlen);

// Send packet
[LibraryImport("libc", SetLastError = true)]
internal static partial nint sendto(int sockfd, byte[] buf, nuint len, 
    int flags, ref SockAddrLl dest_addr, int addrlen);

// Receive packet
[LibraryImport("libc", SetLastError = true)]
internal static partial nint recvfrom(int sockfd, byte[] buf, nuint len,
    int flags, ref SockAddrLl src_addr, ref int addrlen);

// ioctl for interface control
[LibraryImport("libc", SetLastError = true)]
internal static partial int ioctl(int fd, ulong request, ref IfReq ifr);
```

### TCP State Machine

```text
                              ┌───────────────────────────────────────┐
                              │                                       │
                              ▼                                       │
                         ┌────────┐                                   │
              ┌─────────►│ CLOSED │◄─────────────────────────────┐    │
              │          └────┬───┘                              │    │
              │               │ passive open                     │    │
              │               │ create TCB                       │    │
              │               ▼                                  │    │
              │          ┌────────┐                              │    │
              │          │ LISTEN │                              │    │
              │          └────┬───┘                              │    │
              │               │ rcv SYN                          │    │
              │               │ snd SYN,ACK                      │    │
              │               ▼                                  │    │
              │          ┌────────────┐     ┌───────────────┐    │    │
              │          │  SYN_RCVD  │     │   SYN_SENT    │◄───┼────┘
              │          └────┬───────┘     └───────┬───────┘    │
              │               │ rcv ACK           │ rcv SYN,ACK │
              │               │                   │ snd ACK     │
              │               ▼                   ▼             │
              │          ┌────────────────────────────┐         │
              │          │       ESTABLISHED          │         │
              │          └─────────────┬──────────────┘         │
              │                        │                        │
              │           ┌────────────┴────────────┐           │
              │           │                         │           │
              │           ▼                         ▼           │
              │    ┌─────────────┐          ┌─────────────┐     │
              │    │  FIN_WAIT_1 │          │ CLOSE_WAIT  │     │
              │    └──────┬──────┘          └──────┬──────┘     │
              │           │                        │            │
              │           ▼                        ▼            │
              │    ┌─────────────┐          ┌─────────────┐     │
              │    │  FIN_WAIT_2 │          │  LAST_ACK   │     │
              │    └──────┬──────┘          └──────┬──────┘     │
              │           │                        │            │
              │           ▼                        │            │
              │    ┌─────────────┐                 │            │
              │    │  TIME_WAIT  │─────────────────┼────────────┘
              │    └──────┬──────┘                 │
              │           │ 2MSL timeout          │
              └───────────┴────────────────────────┘
```

---

## Testing Requirements

### Unit Tests

| Test | Description |
| ---- | ----------- |
| EthernetFrame_ParseValid | Parses valid Ethernet frame |
| EthernetFrame_BuildValid | Builds valid Ethernet frame |
| ArpPacket_ParseRequest | Parses ARP request |
| ArpCache_Expiry | Cache entries expire |
| IPv4Packet_Checksum | Checksum calculated correctly |
| IPv6Address_LinkLocal | Link-local generated correctly |
| TcpStateMachine_Transitions | All state transitions work |
| DhcpClient_ParseOffer | DHCP offer parsed correctly |
| DnsResolver_ParseResponse | DNS A record parsed |

### Integration Tests

| Test | Description |
| ---- | ----------- |
| Network_DhcpAcquire | Obtain IP via DHCP |
| Network_PingIPv4 | Ping 8.8.8.8 succeeds |
| Network_PingIPv6 | Ping 2001:4860:4860::8888 succeeds |
| Network_DnsResolve | Resolve google.com |
| Network_TcpConnect | TCP connection to external server |
| Network_TcpListen | Accept incoming TCP connection |

### Boot Tests (CI)

| Test | Description |
| ---- | ----------- |
| QEMU_GetsDhcpAddress | VM obtains IP from QEMU user-mode network |
| QEMU_PingsGateway | VM can ping gateway |
| QEMU_ResolvesHostname | DNS resolution works |

---

## Deliverables

1. **PanoramicData.Os.Network library** - Complete network stack
2. **DHCP clients** - IPv4 and IPv6 auto-configuration
3. **DNS resolver** - Hostname resolution
4. **Shell commands** - ifconfig, ip, netstat, ss, route, ping
5. **Unit tests** - Comprehensive test coverage
6. **Integration tests** - Network functionality verification

---

## Risks

| Risk | Probability | Impact | Mitigation |
| ---- | ----------- | ------ | ---------- |
| TCP implementation bugs | High | High | Extensive testing; reference RFCs |
| Raw socket permissions | Medium | Medium | Run as root; CAP_NET_RAW |
| IPv6 complexity delays | Medium | Low | Complete IPv4 first |
| Performance issues | Medium | Medium | Profile; optimize hot paths |
| DHCP server variations | Low | Low | Test with multiple servers |

---

## Exit Criteria

- [ ] VM obtains IPv4 address via DHCP
- [ ] VM obtains IPv6 address via DHCP/SLAAC
- [ ] `ping 8.8.8.8` succeeds
- [ ] `ping 2001:4860:4860::8888` succeeds
- [ ] DNS resolution works
- [ ] TCP connections can be established
- [ ] All shell commands functional
- [ ] All unit tests pass
- [ ] Integration tests pass in CI

---

## Demo Milestone

**Demo:** Ping 8.8.8.8 and 2001:4860:4860::8888 successfully from the shell.

```sh
$ ./run-qemu.sh
[PanoramicData.Os] Starting network stack...
[PanoramicData.Os] eth0: link up
[PanoramicData.Os] DHCPv4: DISCOVER sent
[PanoramicData.Os] DHCPv4: OFFER received 10.0.2.15
[PanoramicData.Os] DHCPv4: REQUEST sent
[PanoramicData.Os] DHCPv4: ACK received
[PanoramicData.Os] eth0: 10.0.2.15/24 gateway 10.0.2.2
[PanoramicData.Os] DHCPv6: SOLICIT sent
[PanoramicData.Os] SLAAC: Configured fd00::15/64

panos> ping 8.8.8.8
PING 8.8.8.8: 64 bytes, seq=1, ttl=64, time=12.3ms
PING 8.8.8.8: 64 bytes, seq=2, ttl=64, time=11.8ms
^C

panos> ping 2001:4860:4860::8888
PING 2001:4860:4860::8888: 64 bytes, seq=1, ttl=64, time=15.1ms
^C
```

---

## Estimated Total Effort

| Section | Hours |
| ------- | ----- |
| 2.1 Network Interface Layer | 22 |
| 2.2 Ethernet Frame Handling | 14 |
| 2.3 ARP | 16 |
| 2.4 IPv4 Implementation | 32 |
| 2.5 IPv6 Implementation | 24 |
| 2.6 NDP | 28 |
| 2.7 ICMP | 16 |
| 2.8 UDP | 16 |
| 2.9 TCP | 72 |
| 2.10 DHCP Clients | 38 |
| 2.11 DNS Resolver | 22 |
| 2.12 Network Commands | 22 |
| **Total** | **322 hours** |

At 40 hours/week = **~8 weeks** (Note: Compressed to 3 weeks assumes parallel work or reduced scope for MVP)

---

## References

- [RFC 791 - IPv4](https://tools.ietf.org/html/rfc791)
- [RFC 8200 - IPv6](https://tools.ietf.org/html/rfc8200)
- [RFC 793 - TCP](https://tools.ietf.org/html/rfc793)
- [RFC 768 - UDP](https://tools.ietf.org/html/rfc768)
- [RFC 826 - ARP](https://tools.ietf.org/html/rfc826)
- [RFC 4861 - NDP](https://tools.ietf.org/html/rfc4861)
- [RFC 2131 - DHCPv4](https://tools.ietf.org/html/rfc2131)
- [RFC 8415 - DHCPv6](https://tools.ietf.org/html/rfc8415)
- [RFC 1035 - DNS](https://tools.ietf.org/html/rfc1035)

#!/bin/bash
# Run QEMU for PanoramicData.Os
# This script boots the OS image in QEMU with serial console output

set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(dirname "${SCRIPT_DIR}")"
OUTPUT_DIR="${REPO_ROOT}/output"

# Default options
MEMORY="512M"
CPUS="2"
GRAPHICS="none"  # Use "gtk" for graphical output
KERNEL="${OUTPUT_DIR}/vmlinuz"
INITRD="${OUTPUT_DIR}/initramfs.cpio.gz"

# Parse arguments
while [[ $# -gt 0 ]]; do
    case $1 in
        --graphics)
            GRAPHICS="gtk"
            shift
            ;;
        --memory)
            MEMORY="$2"
            shift 2
            ;;
        --cpus)
            CPUS="$2"
            shift 2
            ;;
        --kernel)
            KERNEL="$2"
            shift 2
            ;;
        --initrd)
            INITRD="$2"
            shift 2
            ;;
        *)
            echo "Unknown option: $1"
            exit 1
            ;;
    esac
done

# Check if files exist
if [[ ! -f "${KERNEL}" ]]; then
    echo "Error: Kernel not found at ${KERNEL}"
    echo "Run ./scripts/build.sh first to build the image."
    exit 1
fi

if [[ ! -f "${INITRD}" ]]; then
    echo "Error: Initrd not found at ${INITRD}"
    echo "Run ./scripts/build.sh first to build the image."
    exit 1
fi

echo "=== PanoramicData.Os QEMU Runner ==="
echo "Kernel: ${KERNEL}"
echo "Initrd: ${INITRD}"
echo "Memory: ${MEMORY}"
echo "CPUs: ${CPUS}"
echo "Graphics: ${GRAPHICS}"
echo ""
echo "Press Ctrl+A, X to exit QEMU"
echo "==================================="

# Build QEMU command
QEMU_CMD=(
    qemu-system-x86_64
    -kernel "${KERNEL}"
    -initrd "${INITRD}"
    -append "console=ttyS0,115200n8 console=tty0 panic=1"
    -m "${MEMORY}"
    -smp "${CPUS}"
    -enable-kvm
    -cpu host
    -device virtio-net-pci,netdev=net0
    -netdev user,id=net0,hostfwd=tcp::2222-:22
    -device virtio-rng-pci
    -nographic
)

# Add graphics if requested
if [[ "${GRAPHICS}" == "gtk" ]]; then
    QEMU_CMD[-1]="-display gtk"
    QEMU_CMD+=(-serial stdio)
else
    QEMU_CMD+=(-serial mon:stdio)
fi

# Run QEMU
exec "${QEMU_CMD[@]}"

#!/bin/bash
set -e

WORKSPACE="/workspace"
OUTPUT="/output"

# Set up cross-compiler environment for musl
# Use wrapper script that filters out clang-style flags
export CC="/usr/local/bin/musl-gcc-wrapper.sh"
export CXX="x86_64-linux-musl-g++"
export AR="x86_64-linux-musl-ar"
export LD="x86_64-linux-musl-ld"
export STRIP="x86_64-linux-musl-strip"

case "$1" in
    build)
        echo "=== PanoramicData.Os Build ==="
        echo "Building .NET init binary..."
        cd "${WORKSPACE}/src/PanoramicData.Os.Init"
        
        # Set cross-compiler for NativeAOT - use wrapper that strips --target flag
        export CppCompilerAndLinker="/usr/local/bin/musl-gcc-wrapper.sh"
        
        dotnet publish -c Release -r linux-musl-x64 -o "${WORKSPACE}/build/init" \
            /p:CppCompilerAndLinker=/usr/local/bin/musl-gcc-wrapper.sh \
            /p:SysRoot=/opt/x86_64-linux-musl-cross/x86_64-linux-musl \
            /p:ObjCopyName=x86_64-linux-musl-objcopy
        
        echo "Building kernel..."
        cd /opt/linux
        cp "${WORKSPACE}/configs/kernel.config" .config
        make -j$(nproc) bzImage
        cp arch/x86/boot/bzImage "${OUTPUT}/vmlinuz"
        
        echo "Creating initramfs..."
        cd "${WORKSPACE}"
        ./scripts/create-initramfs.sh
        cp "${WORKSPACE}/build/initramfs.cpio.gz" "${OUTPUT}/"
        
        echo "=== Build Complete ==="
        echo "Output files in ${OUTPUT}:"
        ls -la "${OUTPUT}"
        ;;
    
    kernel-menuconfig)
        cd /opt/linux
        if [ -f "${WORKSPACE}/configs/kernel.config" ]; then
            cp "${WORKSPACE}/configs/kernel.config" .config
        fi
        make menuconfig
        cp .config "${WORKSPACE}/configs/kernel.config"
        ;;
    
    test)
        echo "=== PanoramicData.Os QEMU Test ==="
        KERNEL="${OUTPUT}/vmlinuz"
        INITRD="${OUTPUT}/initramfs.cpio.gz"
        
        if [ ! -f "${KERNEL}" ] || [ ! -f "${INITRD}" ]; then
            echo "Error: Build outputs not found. Run 'build' first."
            exit 1
        fi
        
        echo "Starting QEMU (30 second timeout)..."
        # Use script to capture PTY output, timeout to limit duration
        script -q -c "timeout 30 qemu-system-x86_64 \
            -kernel ${KERNEL} \
            -initrd ${INITRD} \
            -append 'console=ttyS0 panic=1' \
            -nographic \
            -m 256M \
            -no-reboot" /dev/null || true
        
        echo ""
        echo "=== QEMU Test Complete ==="
        ;;
    
    shell)
        exec /bin/bash
        ;;
    
    *)
        exec "$@"
        ;;
esac

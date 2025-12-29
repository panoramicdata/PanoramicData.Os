#!/bin/bash
set -e

WORKSPACE="/workspace"
BUILD_DIR="${WORKSPACE}/build"
INITRAMFS_DIR="${BUILD_DIR}/initramfs"
MUSL_CROSS="/opt/x86_64-linux-musl-cross"

echo "Creating initramfs directory structure..."
rm -rf "${INITRAMFS_DIR}"
mkdir -p "${INITRAMFS_DIR}"/{bin,dev,etc,lib,proc,sys,tmp,var,run}

echo "Copying init binary..."
cp "${BUILD_DIR}/init/PanoramicData.Os.Init" "${INITRAMFS_DIR}/init"
chmod +x "${INITRAMFS_DIR}/init"

echo "Copying musl dynamic linker..."
# Copy the musl dynamic linker (ld-musl-x86_64.so.1)
if [ -f "${MUSL_CROSS}/x86_64-linux-musl/lib/ld-musl-x86_64.so.1" ]; then
    cp "${MUSL_CROSS}/x86_64-linux-musl/lib/ld-musl-x86_64.so.1" "${INITRAMFS_DIR}/lib/"
elif [ -f "${MUSL_CROSS}/x86_64-linux-musl/lib/libc.so" ]; then
    # The libc.so is also the dynamic linker in musl
    cp "${MUSL_CROSS}/x86_64-linux-musl/lib/libc.so" "${INITRAMFS_DIR}/lib/ld-musl-x86_64.so.1"
fi

# Also copy libc.so as it may be needed
if [ -f "${MUSL_CROSS}/x86_64-linux-musl/lib/libc.so" ]; then
    cp "${MUSL_CROSS}/x86_64-linux-musl/lib/libc.so" "${INITRAMFS_DIR}/lib/"
fi

echo "Creating device nodes..."
cd "${INITRAMFS_DIR}"
# These will be created by init at runtime, but we need basic console for kernel
mknod -m 622 dev/console c 5 1 || true
mknod -m 666 dev/null c 1 3 || true
mknod -m 666 dev/zero c 1 5 || true
mknod -m 666 dev/tty c 5 0 || true
# Serial console (used by QEMU)
mknod -m 666 dev/ttyS0 c 4 64 || true
mknod -m 666 dev/tty0 c 4 0 || true

echo "Creating /etc files..."
echo "panos" > "${INITRAMFS_DIR}/etc/hostname"

echo "Creating initramfs archive..."
cd "${INITRAMFS_DIR}"
find . -print0 | cpio --null -ov --format=newc | gzip -9 > "${BUILD_DIR}/initramfs.cpio.gz"

echo "Initramfs created: ${BUILD_DIR}/initramfs.cpio.gz"
ls -lh "${BUILD_DIR}/initramfs.cpio.gz"

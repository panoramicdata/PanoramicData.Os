#!/bin/bash
# Wrapper script for x86_64-linux-musl-gcc that filters out clang-style flags
# NativeAOT passes --target=x86_64-linux-musl which gcc doesn't understand

# Filter out unsupported flags
ARGS=()
for arg in "$@"; do
    case "$arg" in
        --target=*)
            # Skip clang-style target flag - musl-gcc already targets musl
            ;;
        --sysroot=*)
            # Skip sysroot - musl-gcc has its own sysroot configured
            ;;
        *)
            ARGS+=("$arg")
            ;;
    esac
done

# Call the real musl-gcc
exec /opt/x86_64-linux-musl-cross/bin/x86_64-linux-musl-gcc "${ARGS[@]}"

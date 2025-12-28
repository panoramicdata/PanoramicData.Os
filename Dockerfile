# PanoramicData.Os Build Environment
# Multi-stage Dockerfile for building the OS image

FROM ubuntu:24.04 AS builder

# Prevent interactive prompts
ENV DEBIAN_FRONTEND=noninteractive

# Install build dependencies
RUN apt-get update && apt-get install -y \
    build-essential \
    git \
    wget \
    curl \
    unzip \
    bc \
    bison \
    flex \
    libssl-dev \
    libelf-dev \
    libncurses-dev \
    cpio \
    rsync \
    python3 \
    python3-pip \
    file \
    qemu-system-x86 \
    qemu-utils \
    expect \
    && rm -rf /var/lib/apt/lists/*

# Install .NET 10 SDK
RUN wget https://dot.net/v1/dotnet-install.sh -O dotnet-install.sh \
    && chmod +x dotnet-install.sh \
    && ./dotnet-install.sh --channel 10.0 --install-dir /usr/share/dotnet \
    && rm dotnet-install.sh

ENV PATH="/usr/share/dotnet:${PATH}"
ENV DOTNET_ROOT="/usr/share/dotnet"

# Install musl cross-compiler for NativeAOT
# Download and install x86_64-linux-musl toolchain
WORKDIR /opt
RUN wget https://musl.cc/x86_64-linux-musl-cross.tgz \
    && tar xzf x86_64-linux-musl-cross.tgz \
    && rm x86_64-linux-musl-cross.tgz

ENV PATH="/opt/x86_64-linux-musl-cross/bin:${PATH}"

# Create symlinks for clang-style cross-compilation
RUN ln -sf /opt/x86_64-linux-musl-cross/bin/x86_64-linux-musl-gcc /usr/local/bin/x86_64-linux-musl-gcc \
    && ln -sf /opt/x86_64-linux-musl-cross/bin/x86_64-linux-musl-g++ /usr/local/bin/x86_64-linux-musl-g++ \
    && ln -sf /opt/x86_64-linux-musl-cross/bin/x86_64-linux-musl-ar /usr/local/bin/x86_64-linux-musl-ar \
    && ln -sf /opt/x86_64-linux-musl-cross/bin/x86_64-linux-musl-ld /usr/local/bin/x86_64-linux-musl-ld \
    && ln -sf /opt/x86_64-linux-musl-cross/bin/x86_64-linux-musl-strip /usr/local/bin/x86_64-linux-musl-strip

# Download and extract Buildroot
ARG BUILDROOT_VERSION=2024.02.8
WORKDIR /opt
RUN wget https://buildroot.org/downloads/buildroot-${BUILDROOT_VERSION}.tar.xz \
    && tar xf buildroot-${BUILDROOT_VERSION}.tar.xz \
    && mv buildroot-${BUILDROOT_VERSION} buildroot \
    && rm buildroot-${BUILDROOT_VERSION}.tar.xz

# Download Linux kernel
ARG KERNEL_VERSION=6.6.68
WORKDIR /opt
RUN wget https://cdn.kernel.org/pub/linux/kernel/v6.x/linux-${KERNEL_VERSION}.tar.xz \
    && tar xf linux-${KERNEL_VERSION}.tar.xz \
    && mv linux-${KERNEL_VERSION} linux \
    && rm linux-${KERNEL_VERSION}.tar.xz

# Set up workspace
WORKDIR /workspace

# Create output directory
RUN mkdir -p /output

# Copy entrypoint and wrapper script
COPY scripts/docker-entrypoint.sh /usr/local/bin/
COPY scripts/musl-gcc-wrapper.sh /usr/local/bin/
RUN chmod +x /usr/local/bin/docker-entrypoint.sh /usr/local/bin/musl-gcc-wrapper.sh

ENTRYPOINT ["/usr/local/bin/docker-entrypoint.sh"]
CMD ["build"]

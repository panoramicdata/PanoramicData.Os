#!/bin/bash
# Build script for PanoramicData.Os
# Run this from the repository root

set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(dirname "${SCRIPT_DIR}")"

# Configuration
IMAGE_NAME="panos-builder"
CONTAINER_NAME="panos-build"
OUTPUT_DIR="${REPO_ROOT}/output"

# Parse arguments
BUILD_CMD="${1:-build}"

echo "=== PanoramicData.Os Build System ==="
echo "Repository: ${REPO_ROOT}"
echo "Command: ${BUILD_CMD}"

# Create output directory
mkdir -p "${OUTPUT_DIR}"

# Build Docker image if needed
if [[ "$(docker images -q ${IMAGE_NAME} 2>/dev/null)" == "" ]] || [[ "$2" == "--rebuild" ]]; then
    echo "Building Docker image..."
    docker build -t "${IMAGE_NAME}" "${REPO_ROOT}"
fi

# Create build directory for intermediate files
BUILD_DIR="${REPO_ROOT}/build"
mkdir -p "${BUILD_DIR}"

# Run build in container
echo "Running build in container..."
docker run --rm \\
    -v "${REPO_ROOT}/src:/workspace/src" \\
    -v "${REPO_ROOT}/configs:/workspace/configs:ro" \\
    -v "${REPO_ROOT}/scripts:/workspace/scripts:ro" \\
    -v "${BUILD_DIR}:/workspace/build" \\
    -v "${OUTPUT_DIR}:/output" \
    --name "${CONTAINER_NAME}" \
    "${IMAGE_NAME}" \
    "${BUILD_CMD}"

echo ""
echo "=== Build Output ==="
ls -la "${OUTPUT_DIR}"

#!/usr/bin/env bash
set -euo pipefail

usage() {
  cat <<EOF
Usage: $(basename "$0") [options]

Options:
  -p|--platform PLATFORM            Target platform: win32 | macos | macos-arm64 | linux (default: win32)
  -c|--config CONFIG                Debug | Release (default: Debug)

  -s|--source-dir DIR               PROJECT_SOURCE_DIR
     --project-source-dir DIR
     --PROJECT_SOURCE_DIR DIR       (these 3 are the same)

  -b|--binary-dir DIR               PROJECT_BINARY_DIR (default: ./build)
  -n|--dll-name NAME                DLL filename (default: CSharpLibrary.dll)
  -f|--framework FRAME              Target framework (default: net9.0)

  --self-contained [true|false]     Make publish self-contained (default: false)
  -o|--output DIR                   Force publish output dir (optional)

  -h|--help                         Show this help

Examples:
  ./publish_native_aot.sh --platform win32
  ./publish_native_aot.sh --project-source-dir ~/myproj --binary-dir ./out
EOF
  exit 1
}

# Defaults
PLATFORM="win32"
CONFIG="Debug"
PROJECT_SOURCE_DIR="$(pwd)"
PROJECT_BINARY_DIR="$(pwd)"
DLL_NAME="CSharpLibrary.dll"
FRAMEWORK="net10.0"
SELF_CONTAINED="false"
FORCE_OUTPUT_DIR=""

# Parse args
while [[ $# -gt 0 ]]; do
  case "$1" in
    -p|--platform) PLATFORM="$2"; shift 2 ;;
    -c|--config) CONFIG="$2"; shift 2 ;;
    
    -s|--source-dir|--project-source-dir|--PROJECT_SOURCE_DIR)
      PROJECT_SOURCE_DIR="$2"; shift 2 ;;
    
    -b|--binary-dir) PROJECT_BINARY_DIR="$2"; shift 2 ;;
    -n|--dll-name) DLL_NAME="$2"; shift 2 ;;
    -f|--framework) FRAMEWORK="$2"; shift 2 ;;
    
    --self-contained)
      if [[ -n "${2:-}" && "${2:0:1}" != "-" ]]; then
        SELF_CONTAINED="$2"; shift 2
      else
        SELF_CONTAINED="true"; shift 1
      fi
      ;;
    
    -o|--output) FORCE_OUTPUT_DIR="$2"; shift 2 ;;
    -h|--help) usage ;;
    *) echo "Unknown arg: $1"; usage ;;
  esac
done

# Lowercase platform safely
PLATFORM_LOWER="$(printf '%s' "$PLATFORM" | tr '[:upper:]' '[:lower:]')"

case "$PLATFORM_LOWER" in
  win32|win|windows) RID="win-x64"; RID_FOLDER="win-x64" ;;
  macos|osx)         RID="osx-x64"; RID_FOLDER="osx-x64" ;;
  macos-arm64|osx-arm64)
                    RID="osx-arm64"; RID_FOLDER="osx-arm64" ;;
  linux|linux-x64)  RID="linux-x64"; RID_FOLDER="linux-x64" ;;
  *) echo "Unsupported platform: $PLATFORM"; exit 2 ;;
esac

# Compute publish output
if [[ -n "$FORCE_OUTPUT_DIR" ]]; then
  PUBLISH_OUTPUT="$FORCE_OUTPUT_DIR"
else
  PUBLISH_OUTPUT="${PROJECT_SOURCE_DIR}/bin/${CONFIG}/${FRAMEWORK}/${RID_FOLDER}"
fi

MY_DLL_SOURCE="${PUBLISH_OUTPUT}/publish/"
MY_DLL_DESTINATION="${PROJECT_BINARY_DIR}/${DLL_NAME}"

echo "Platform:        $PLATFORM (RID: $RID)"
echo "Source dir:      $PROJECT_SOURCE_DIR"
echo "Binary dir:      $PROJECT_BINARY_DIR"
echo "Publish output:  $PUBLISH_OUTPUT"
echo "Self-contained:  $SELF_CONTAINED"
echo

# dotnet check
if ! command -v dotnet >/dev/null 2>&1; then
  echo "Error: dotnet not found in PATH."
  exit 3
fi

pushd "${PROJECT_SOURCE_DIR}" >/dev/null

# Build command
DOTNET_CMD=(dotnet publish -r "$RID" -c "$CONFIG" -f "$FRAMEWORK" )

SELF_LOWER="$(printf '%s' "$SELF_CONTAINED" | tr '[:upper:]' '[:lower:]')"
if [[ "$SELF_LOWER" = "true" || "$SELF_LOWER" = "1" ]]; then
  DOTNET_CMD+=(--self-contained true)
fi

if [[ -n "$FORCE_OUTPUT_DIR" ]]; then
  mkdir -p "$FORCE_OUTPUT_DIR"
  DOTNET_CMD+=(-o "$FORCE_OUTPUT_DIR")
fi

echo "Running: ${DOTNET_CMD[*]}"
"${DOTNET_CMD[@]}"

popd >/dev/null

# Copy the published files to the build dir
mkdir -p "$(dirname "$MY_DLL_DESTINATION")"
cp -r "$PUBLISH_OUTPUT/publish/"* "$PROJECT_BINARY_DIR/build"

echo "Done."

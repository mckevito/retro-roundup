#!/bin/bash
# Cloud sessions only: installs the .NET 10 SDK and Godot 4.7.2 .NET so a session can build, test and capture
# pictures before pushing. Runs in the background and is safe to rerun. Progress: /tmp/session-start.log.
set -euo pipefail
[ "${CLAUDE_CODE_REMOTE:-}" = "true" ] || exit 0
echo '{"async": true, "asyncTimeout": 900000}'
V=4.7.2; DIR="Godot_v${V}-stable_mono_linux_x86_64"; BIN="/opt/godot-mono/${DIR}/Godot_v${V}-stable_mono_linux.x86_64"
LOG=/tmp/session-start.log
SUDO=""; [ "$(id -u)" = "0" ] || SUDO="sudo -n"
[ -n "${CLAUDE_ENV_FILE:-}" ] && echo "export GODOT=\"$BIN\"" >> "$CLAUDE_ENV_FILE"
{
  echo "start $(date -u +%H:%M:%S)"
  if ! command -v dotnet >/dev/null 2>&1; then
    $SUDO apt-get update -q && DEBIAN_FRONTEND=noninteractive $SUDO apt-get install -y -q dotnet-sdk-10.0
  fi
  if [ ! -x "$BIN" ]; then
    tmp="$(mktemp -d)"
    curl -fsSL --retry 4 -o "$tmp/g.zip" "https://github.com/godotengine/godot/releases/download/${V}-stable/${DIR}.zip"
    $SUDO mkdir -p /opt/godot-mono && $SUDO unzip -q -o "$tmp/g.zip" -d /opt/godot-mono && rm -rf "$tmp"
  fi
  $SUDO ln -sf "$BIN" /usr/local/bin/godot
  command -v xvfb-run >/dev/null 2>&1 || { $SUDO apt-get update -q && DEBIAN_FRONTEND=noninteractive $SUDO apt-get install -y -q --no-install-recommends xvfb; }
  echo "ready $(date -u +%H:%M:%S): $(godot --version 2>/dev/null | head -1), dotnet $(dotnet --version)"
} >> "$LOG" 2>&1

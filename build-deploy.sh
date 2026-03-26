#!/bin/bash
# Build the deploy/ folder with only production files.
# Usage: bash build-deploy.sh

set -e
SCRIPT_DIR="$(cd "$(dirname "$0")" && pwd)"
DEPLOY="$SCRIPT_DIR/deploy"

# Clean previous build
rm -rf "$DEPLOY"
mkdir -p "$DEPLOY"

# Root files
cp "$SCRIPT_DIR/index.html"         "$DEPLOY/"
cp "$SCRIPT_DIR/resume.html"        "$DEPLOY/"
cp "$SCRIPT_DIR/one-pager.html"     "$DEPLOY/"
cp "$SCRIPT_DIR/404.html"           "$DEPLOY/"
cp "$SCRIPT_DIR/styles.css"         "$DEPLOY/"
cp "$SCRIPT_DIR/main.js"            "$DEPLOY/"
cp "$SCRIPT_DIR/manifest.json"      "$DEPLOY/"
cp "$SCRIPT_DIR/robots.txt"         "$DEPLOY/"
cp "$SCRIPT_DIR/sitemap.xml"        "$DEPLOY/"
cp "$SCRIPT_DIR/.htaccess"          "$DEPLOY/"
cp "$SCRIPT_DIR/apple-touch-icon.png" "$DEPLOY/"
cp "$SCRIPT_DIR/fisher-sovereign.html" "$DEPLOY/"

# Directories (full copy)
cp -r "$SCRIPT_DIR/img"             "$DEPLOY/img"
cp -r "$SCRIPT_DIR/fonts"           "$DEPLOY/fonts"
cp -r "$SCRIPT_DIR/thumbs"          "$DEPLOY/thumbs"
cp -r "$SCRIPT_DIR/brand"           "$DEPLOY/brand"
cp -r "$SCRIPT_DIR/noco"            "$DEPLOY/noco"
cp -r "$SCRIPT_DIR/concepts"        "$DEPLOY/concepts"
cp -r "$SCRIPT_DIR/dashboard"       "$DEPLOY/dashboard"
cp -r "$SCRIPT_DIR/playbook"        "$DEPLOY/playbook"
cp -r "$SCRIPT_DIR/ftda"            "$DEPLOY/ftda"
cp -r "$SCRIPT_DIR/harmony"         "$DEPLOY/harmony"
cp -r "$SCRIPT_DIR/jumpquest"       "$DEPLOY/jumpquest"
cp -r "$SCRIPT_DIR/api"             "$DEPLOY/api"
[ -d "$SCRIPT_DIR/bloodlines" ] && cp -r "$SCRIPT_DIR/bloodlines" "$DEPLOY/bloodlines"
[ -d "$SCRIPT_DIR/projects" ] && cp -r "$SCRIPT_DIR/projects" "$DEPLOY/projects"
# one-three is a sibling project; copy from FisherSovereign if available
ONE_THREE_SRC="$(dirname "$SCRIPT_DIR")/one-three-net"
[ -d "$ONE_THREE_SRC" ] && cp -r "$ONE_THREE_SRC" "$DEPLOY/one-three"

echo "Deploy folder built at: $DEPLOY"
du -sh "$DEPLOY"

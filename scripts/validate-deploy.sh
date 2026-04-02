#!/bin/bash
# validate-deploy.sh — Pre-upload validation for lancewfisher.com deploy folder
# Run BEFORE uploading to Hostinger. Catches common deployment issues.
# Usage: bash scripts/validate-deploy.sh

DEPLOY_DIR="$(dirname "$0")/../deploy"
ERRORS=0
WARNINGS=0

echo "=== Deploy Folder Validation ==="
echo "Target: $DEPLOY_DIR"
echo ""

# Check deploy directory exists
if [ ! -d "$DEPLOY_DIR" ]; then
    echo "FATAL: deploy/ directory not found at $DEPLOY_DIR"
    exit 1
fi

# 1. Check for development artifacts that should not be deployed
echo "--- Checking for development artifacts ---"
for pattern in ".git" ".claude" "CLAUDE.md" ".env" ".gitignore" "launch.bat" "node_modules" "__pycache__" ".DS_Store"; do
    found=$(find "$DEPLOY_DIR" -name "$pattern" 2>/dev/null)
    if [ -n "$found" ]; then
        echo "ERROR: Found $pattern in deploy:"
        echo "$found"
        ERRORS=$((ERRORS + 1))
    fi
done

# 2. Check robots.txt has no /deploy/ line
echo ""
echo "--- Checking robots.txt ---"
if [ -f "$DEPLOY_DIR/robots.txt" ]; then
    if grep -q "/deploy/" "$DEPLOY_DIR/robots.txt"; then
        echo "ERROR: robots.txt contains /deploy/ disallow (stale rule)"
        ERRORS=$((ERRORS + 1))
    else
        echo "OK: robots.txt clean"
    fi
else
    echo "WARNING: No robots.txt found in deploy"
    WARNINGS=$((WARNINGS + 1))
fi

# 3. Check for nested deploy/ directories
echo ""
echo "--- Checking for nested deploy/ directories ---"
nested=$(find "$DEPLOY_DIR" -mindepth 1 -type d -name "deploy" 2>/dev/null)
if [ -n "$nested" ]; then
    echo "ERROR: Found nested deploy/ directory:"
    echo "$nested"
    ERRORS=$((ERRORS + 1))
else
    echo "OK: No nested deploy/ directories"
fi

# 4. Check for sensitive files
echo ""
echo "--- Checking for sensitive files ---"
for pattern in "*.env" "*.pem" "*.key" "credentials*" "secrets*" ".env.*"; do
    found=$(find "$DEPLOY_DIR" -name "$pattern" 2>/dev/null)
    if [ -n "$found" ]; then
        echo "ERROR: Found sensitive file in deploy:"
        echo "$found"
        ERRORS=$((ERRORS + 1))
    fi
done

# 5. Check internal docs not in deploy
echo ""
echo "--- Checking for internal documents ---"
for pattern in "HANDOFF.md" "STOPPOINT.md" "SESSION_STATUS.md" "todo.md" "lessons.md" "_archive"; do
    found=$(find "$DEPLOY_DIR" -name "$pattern" 2>/dev/null)
    if [ -n "$found" ]; then
        echo "ERROR: Found internal document in deploy:"
        echo "$found"
        ERRORS=$((ERRORS + 1))
    fi
done

# 6. Validate all HTML files exist and are non-empty
echo ""
echo "--- Checking HTML files ---"
html_count=0
empty_html=0
for f in $(find "$DEPLOY_DIR" -name "*.html" 2>/dev/null); do
    html_count=$((html_count + 1))
    if [ ! -s "$f" ]; then
        echo "ERROR: Empty HTML file: $f"
        empty_html=$((empty_html + 1))
        ERRORS=$((ERRORS + 1))
    fi
done
echo "Found $html_count HTML files ($empty_html empty)"

# 7. Check href targets exist (top-level links only)
echo ""
echo "--- Checking top-level href targets ---"
if [ -f "$DEPLOY_DIR/index.html" ]; then
    # Extract relative href values from main index
    hrefs=$(grep -oP 'href="(?!https?://|mailto:|tel:|#|javascript:)[^"]*"' "$DEPLOY_DIR/index.html" 2>/dev/null | tr -d '"' | sed 's/href=//')
    missing_links=0
    for href in $hrefs; do
        target="$DEPLOY_DIR/$href"
        if [ ! -e "$target" ] && [ ! -e "${target%/}" ]; then
            # Skip anchors and query strings
            clean=$(echo "$href" | cut -d'#' -f1 | cut -d'?' -f1)
            if [ -n "$clean" ] && [ ! -e "$DEPLOY_DIR/$clean" ]; then
                echo "WARNING: href target not found: $href"
                missing_links=$((missing_links + 1))
                WARNINGS=$((WARNINGS + 1))
            fi
        fi
    done
    if [ $missing_links -eq 0 ]; then
        echo "OK: All checked href targets exist"
    fi
else
    echo "ERROR: No index.html in deploy root"
    ERRORS=$((ERRORS + 1))
fi

# 8. Check source/deploy index.html alignment
echo ""
echo "--- Checking source/deploy alignment ---"
SOURCE_INDEX="$(dirname "$0")/../index.html"
DEPLOY_INDEX="$DEPLOY_DIR/index.html"
if [ -f "$SOURCE_INDEX" ] && [ -f "$DEPLOY_INDEX" ]; then
    if diff -q "$SOURCE_INDEX" "$DEPLOY_INDEX" > /dev/null 2>&1; then
        echo "OK: index.html source and deploy are in sync"
    else
        echo "WARNING: index.html differs between source and deploy (run build-deploy.sh)"
        WARNINGS=$((WARNINGS + 1))
    fi
fi

# Summary
echo ""
echo "=== Validation Summary ==="
echo "Errors:   $ERRORS"
echo "Warnings: $WARNINGS"

if [ $ERRORS -gt 0 ]; then
    echo ""
    echo "DEPLOY BLOCKED — fix errors before uploading to Hostinger"
    exit 1
elif [ $WARNINGS -gt 0 ]; then
    echo ""
    echo "DEPLOY OK WITH WARNINGS — review before uploading"
    exit 0
else
    echo ""
    echo "DEPLOY CLEAN — ready for Hostinger upload"
    exit 0
fi

# overnight-build.ps1
# Bloodlines unattended overnight development loop.
# Uses Claude Code CLI in non-interactive mode to continuously implement
# Unity ECS sub-slices, validate, commit, and push -- no human input required.
#
# USAGE:
#   powershell -ExecutionPolicy Bypass -File scripts\overnight-build.ps1
#   powershell -ExecutionPolicy Bypass -File scripts\overnight-build.ps1 -Hours 7 -Model opus
#
# PREREQUISITES:
#   - Claude Code installed at C:\Users\lance\.local\bin\claude.exe
#   - Git configured with push credentials for lance-fisher/lancewfisher-v2
#   - Node.js in PATH (for data-validation.mjs and runtime-bridge.mjs)
#   - dotnet CLI in PATH (for Assembly-CSharp.csproj build gate)
#   - Unity batch mode reachable from the validation wrapper scripts

param(
    [int]$Hours = 7,
    [string]$Model = "opus",
    [int]$PauseBetweenSessionsSec = 90,
    [string]$PromptFile = "03_PROMPTS\CODEX_CONTINUE.md"
)

$rootDir    = "D:\ProjectsHome\Bloodlines"
$claudeExe  = "C:\Users\lance\.local\bin\claude.exe"
$logDir     = Join-Path $rootDir "logs"
$logFile    = Join-Path $logDir "overnight-$(Get-Date -Format 'yyyy-MM-dd-HHmm').log"
$endTime    = (Get-Date).AddHours($Hours)
$iteration  = 0
$successCount = 0
$failCount  = 0

# ---------------------------------------------------------------------------
# Helpers
# ---------------------------------------------------------------------------

function Write-Log {
    param([string]$Message, [string]$Color = "White")
    $line = "[$(Get-Date -Format 'HH:mm:ss')] $Message"
    Write-Host $line -ForegroundColor $Color
    Add-Content -Path $logFile -Value $line -Encoding UTF8
}

function Get-GitHead {
    try { git -C $rootDir rev-parse --short HEAD 2>$null } catch { "unknown" }
}

# ---------------------------------------------------------------------------
# Pre-flight checks
# ---------------------------------------------------------------------------

if (-not (Test-Path $logDir)) { New-Item -ItemType Directory -Path $logDir | Out-Null }
if (-not (Test-Path $claudeExe)) {
    Write-Host "ERROR: Claude Code not found at $claudeExe" -ForegroundColor Red
    exit 1
}
$promptPath = Join-Path $rootDir $PromptFile
if (-not (Test-Path $promptPath)) {
    Write-Host "ERROR: Prompt file not found: $promptPath" -ForegroundColor Red
    exit 1
}

# Ensure we are working from the Bloodlines root
Set-Location $rootDir

# Pull latest master state before starting
Write-Host "Fetching origin/master before starting..." -ForegroundColor Gray
git fetch origin 2>&1 | Out-Null

# ---------------------------------------------------------------------------
# Banner
# ---------------------------------------------------------------------------

Write-Log "================================================" "Green"
Write-Log "  BLOODLINES OVERNIGHT BUILD" "Green"
Write-Log "================================================" "Green"
Write-Log "  Start:    $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')" "Green"
Write-Log "  Stop by:  $($endTime.ToString('HH:mm:ss'))" "Green"
Write-Log "  Duration: $Hours hours" "Green"
Write-Log "  Model:    $Model" "Green"
Write-Log "  Git HEAD: $(Get-GitHead)" "Green"
Write-Log "  Log:      $logFile" "Green"
Write-Log "  Prompt:   $promptPath" "Green"
Write-Log "================================================" "Green"
Write-Log "" "White"

# ---------------------------------------------------------------------------
# Main loop
# ---------------------------------------------------------------------------

while ((Get-Date) -lt $endTime) {
    $iteration++
    $sessionStart = Get-Date
    $headBefore   = Get-GitHead

    Write-Log "--- Session $iteration | HEAD: $headBefore ---" "Cyan"

    # Re-read the prompt each iteration so any mid-run updates are picked up
    $prompt = Get-Content $promptPath -Raw -Encoding UTF8

    try {
        # Run Claude Code in non-interactive mode.
        # -p              : print mode -- runs the full agentic loop then exits
        # bypassPermissions: no confirmation prompts (required for unattended use)
        # --model         : use the specified model
        #
        # stdout+stderr are both captured and appended to the log file in real time.
        & $claudeExe `
            -p $prompt `
            --permission-mode bypassPermissions `
            --model $Model `
            2>&1 | Tee-Object -FilePath $logFile -Append

        $exitCode = $LASTEXITCODE
    }
    catch {
        $exitCode = 1
        Write-Log "Session $iteration threw an exception: $_" "Red"
    }

    $headAfter  = Get-GitHead
    $duration   = [math]::Round(((Get-Date) - $sessionStart).TotalMinutes, 1)
    $newCommits = if ($headBefore -ne $headAfter) { " | new commits landed" } else { " | no new commits" }

    if ($exitCode -eq 0) {
        $successCount++
        Write-Log "Session $iteration done in ${duration}min (exit 0)$newCommits" "Green"
    } else {
        $failCount++
        Write-Log "Session $iteration done in ${duration}min (exit $exitCode)$newCommits" "Yellow"
    }

    # Fetch remote state so next session's handoff read is current
    git fetch origin 2>&1 | Out-Null

    $remaining = [math]::Round(($endTime - (Get-Date)).TotalMinutes, 0)
    if ((Get-Date) -lt $endTime -and $remaining -gt 2) {
        Write-Log "Next session in ${PauseBetweenSessionsSec}s. ~${remaining}min remaining tonight." "Gray"
        Start-Sleep -Seconds $PauseBetweenSessionsSec
    }
}

# ---------------------------------------------------------------------------
# Summary
# ---------------------------------------------------------------------------

Write-Log "" "White"
Write-Log "================================================" "Green"
Write-Log "  OVERNIGHT BUILD COMPLETE" "Green"
Write-Log "================================================" "Green"
Write-Log "  Sessions run:        $iteration" "Green"
Write-Log "  Successful:          $successCount" "Green"
Write-Log "  Non-zero exit:       $failCount" "Green"
Write-Log "  Final HEAD:          $(Get-GitHead)" "Green"
Write-Log "  Log:                 $logFile" "Green"
Write-Log "================================================" "Green"

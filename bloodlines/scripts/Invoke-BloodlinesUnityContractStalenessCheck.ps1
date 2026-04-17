#Requires -Version 5.1
<#
.SYNOPSIS
Checks whether docs/unity/CONCURRENT_SESSION_CONTRACT.md is current relative
to the newest per-slice handoff under docs/unity/session-handoffs/.

.DESCRIPTION
Exit 0  -- contract Last Updated date >= newest handoff date, or no dated
           handoffs exist.
Exit 1  -- contract is missing, Revision field is not a positive integer, Last
           Updated date cannot be parsed, or the contract is older than the
           newest handoff.

Any resume session should run this check before doing Unity work. If it exits
non-zero, the session must STOP, bump the Revision, set Last Updated to today,
update affected lane subsections, and re-run until green.
#>

[CmdletBinding()]
param()

$ErrorActionPreference = 'Stop'

$repoRoot     = Split-Path $PSScriptRoot -Parent
$docsUnityDir = Join-Path (Join-Path $repoRoot 'docs') 'unity'
$contractPath = Join-Path $docsUnityDir 'CONCURRENT_SESSION_CONTRACT.md'
$handoffDir   = Join-Path $docsUnityDir 'session-handoffs'

# ── 1. Contract must exist ────────────────────────────────────────────────────
if (-not (Test-Path $contractPath)) {
    Write-Error ("STALENESS CHECK FAILED: Contract file not found at '{0}'." -f $contractPath)
    exit 1
}

$contractText = Get-Content $contractPath -Raw -Encoding UTF8

# ── 2. Revision must be a positive integer ───────────────────────────────────
if ($contractText -match '(?m)^- Revision:\s+(\d+)') {
    $revision = [int]$Matches[1]
    if ($revision -lt 1) {
        Write-Error ("STALENESS CHECK FAILED: Revision field is '{0}', must be a positive integer." -f $revision)
        exit 1
    }
} else {
    Write-Error "STALENESS CHECK FAILED: Could not parse 'Revision:' field from contract metadata block."
    exit 1
}

# ── 3. Last Updated must be a parseable YYYY-MM-DD date ──────────────────────
if ($contractText -match '(?m)^- Last Updated:\s+(\d{4}-\d{2}-\d{2})') {
    try {
        $contractDate = [DateTime]::ParseExact($Matches[1], 'yyyy-MM-dd', $null)
    } catch {
        Write-Error ("STALENESS CHECK FAILED: Could not parse Last Updated date '{0}'." -f $Matches[1])
        exit 1
    }
} else {
    Write-Error "STALENESS CHECK FAILED: Could not parse 'Last Updated:' field from contract metadata block."
    exit 1
}

# ── 4. Find all dated handoff files ──────────────────────────────────────────
if (-not (Test-Path $handoffDir)) {
    Write-Host "STALENESS CHECK PASSED: Handoff directory not found at '$handoffDir'. Contract revision=$revision, last-updated=$($contractDate.ToString('yyyy-MM-dd'))."
    exit 0
}

$handoffs = Get-ChildItem -Path $handoffDir -Filter '*.md' -ErrorAction SilentlyContinue

if (-not $handoffs -or $handoffs.Count -eq 0) {
    Write-Host "STALENESS CHECK PASSED: No handoff files found in '$handoffDir'. Contract revision=$revision, last-updated=$($contractDate.ToString('yyyy-MM-dd'))."
    exit 0
}

# ── 5. Extract dates from filenames (YYYY-MM-DD-*) ───────────────────────────
$latestHandoffDate = [DateTime]::MinValue
$latestHandoffName = ''

foreach ($file in $handoffs) {
    if ($file.Name -match '^(\d{4}-\d{2}-\d{2})-') {
        try {
            $d = [DateTime]::ParseExact($Matches[1], 'yyyy-MM-dd', $null)
            if ($d -gt $latestHandoffDate) {
                $latestHandoffDate = $d
                $latestHandoffName = $file.Name
            }
        } catch {
            # filename has a date-like prefix that won't parse; skip it
        }
    }
}

if ($latestHandoffDate -eq [DateTime]::MinValue) {
    Write-Host "STALENESS CHECK PASSED: No dated handoffs found (no YYYY-MM-DD prefix). Contract revision=$revision, last-updated=$($contractDate.ToString('yyyy-MM-dd'))."
    exit 0
}

# ── 6. Compare dates ──────────────────────────────────────────────────────────
$contractDateStr  = $contractDate.ToString('yyyy-MM-dd')
$latestDateStr    = $latestHandoffDate.ToString('yyyy-MM-dd')

if ($contractDate -ge $latestHandoffDate) {
    Write-Host "STALENESS CHECK PASSED: Contract revision=$revision, last-updated=$contractDateStr is current. Latest handoff: $latestHandoffName ($latestDateStr)."
    exit 0
} else {
    Write-Error ("STALENESS CHECK FAILED: Contract revision=$revision, last-updated=$contractDateStr " +
                 "is OLDER than the newest handoff '$latestHandoffName' ($latestDateStr). " +
                 "Resume sessions must not proceed until the contract is updated: " +
                 "bump Revision, set Last Updated to today, set Last Updated By to the " +
                 "current session identifier, and amend affected lane subsections. " +
                 "Then re-run this check.")
    exit 1
}

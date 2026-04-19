param()

$ErrorActionPreference = 'Stop'

$unityPath = 'C:\Program Files\Unity\Hub\Editor\6000.3.13f1\Editor\Unity.exe'
$rootPath = Split-Path -Parent $PSScriptRoot
$projectPath = Join-Path $rootPath 'unity'
$logPath = Join-Path $rootPath 'artifacts\unity-imminent-engagement-smoke.log'

if (-not (Test-Path -LiteralPath $unityPath)) {
    throw "Unity editor not found at $unityPath"
}

if (-not (Test-Path -LiteralPath $projectPath)) {
    throw "Unity project not found at $projectPath"
}

$logDirectory = Split-Path -Parent $logPath
if (-not (Test-Path -LiteralPath $logDirectory)) {
    New-Item -ItemType Directory -Path $logDirectory | Out-Null
}

$arguments = @(
    '-batchmode'
    '-projectPath', $projectPath
    '-logFile', $logPath
    '-executeMethod', 'Bloodlines.EditorTools.BloodlinesImminentEngagementSmokeValidation.RunBatchBloodlinesImminentEngagementSmokeValidation'
)

function Get-ValidationOutcome {
    if (-not (Test-Path -LiteralPath $logPath)) {
        return 'unknown'
    }

    $content = Get-Content -Path $logPath -Raw
    if ($content -match 'BLOODLINES_IMMINENT_ENGAGEMENT_SMOKE PASS') {
        return 'passed'
    }

    if ($content -match 'BLOODLINES_IMMINENT_ENGAGEMENT_SMOKE FAIL' -or
        $content -match 'errored' -or
        $content -match 'timed out') {
        return 'failed'
    }

    return 'unknown'
}

function Wait-ForValidationOutcome {
    param(
        [int]$TimeoutSeconds = 180,
        [int]$PollSeconds = 2
    )

    $deadline = (Get-Date).AddSeconds($TimeoutSeconds)
    while ((Get-Date) -lt $deadline) {
        $outcome = Get-ValidationOutcome
        if ($outcome -ne 'unknown') {
            return $outcome
        }

        Start-Sleep -Seconds $PollSeconds
    }

    return Get-ValidationOutcome
}

function Invoke-UnityValidationPass {
    if (Test-Path -LiteralPath $logPath) {
        Remove-Item -LiteralPath $logPath -Force
    }

    $process = Start-Process -FilePath $unityPath -ArgumentList $arguments -PassThru -NoNewWindow -Wait
    return $process.ExitCode
}

Write-Host 'Running Bloodlines Unity imminent engagement smoke validation...'
Write-Host "Unity:   $unityPath"
Write-Host "Project: $projectPath"
Write-Host "Log:     $logPath"

$exitCode = Invoke-UnityValidationPass
$outcome = Get-ValidationOutcome

if ($outcome -eq 'unknown') {
    Write-Host 'Imminent engagement smoke validation did not report an explicit outcome immediately. Waiting for the batch editor to finish.'
    $outcome = Wait-ForValidationOutcome
}

if ($outcome -eq 'unknown') {
    Write-Host 'First imminent engagement batch pass ended without an explicit outcome. Rerunning once after compilation/import.'
    $exitCode = Invoke-UnityValidationPass
    $outcome = Wait-ForValidationOutcome
}

if ($outcome -eq 'passed') {
    Write-Host 'Imminent engagement smoke validation passed.'
    exit 0
}

if ($outcome -eq 'failed') {
    Write-Host "Unity exited with code $exitCode"
    exit 1
}

Write-Host 'Imminent engagement smoke validation produced no pass/fail marker. Check the log.'
exit 1

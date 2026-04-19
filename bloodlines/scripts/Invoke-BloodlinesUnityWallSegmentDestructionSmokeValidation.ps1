param()

$ErrorActionPreference = 'Stop'

$unityPath = 'C:\Program Files\Unity\Hub\Editor\6000.3.13f1\Editor\Unity.exe'
$rootPath = Split-Path -Parent $PSScriptRoot
$projectPath = Join-Path $rootPath 'unity'
$logPath = Join-Path $rootPath 'artifacts\unity-wall-segment-destruction-smoke.log'

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
    '-quit'
    '-projectPath', $projectPath
    '-logFile', $logPath
    '-executeMethod', 'Bloodlines.EditorTools.BloodlinesWallSegmentDestructionSmokeValidation.RunBatchWallSegmentDestructionSmokeValidation'
)

function Get-ValidationOutcome {
    if (-not (Test-Path -LiteralPath $logPath)) {
        return 'unknown'
    }

    $content = Get-Content -Path $logPath -Raw
    if ($content -match 'BLOODLINES_WALL_SEGMENT_DESTRUCTION_SMOKE PASS') {
        return 'passed'
    }

    if ($content -match 'BLOODLINES_WALL_SEGMENT_DESTRUCTION_SMOKE FAIL' -or
        $content -match 'errored' -or
        $content -match 'timed out') {
        return 'failed'
    }

    return 'unknown'
}

function Invoke-UnityValidationPass {
    if (Test-Path -LiteralPath $logPath) {
        Remove-Item -LiteralPath $logPath -Force
    }

    $process = Start-Process -FilePath $unityPath -ArgumentList $arguments -PassThru -Wait
    return $process.ExitCode
}

Write-Host 'Running Bloodlines Unity wall-segment destruction smoke validation...'
Write-Host "Unity:   $unityPath"
Write-Host "Project: $projectPath"
Write-Host "Log:     $logPath"

$exitCode = Invoke-UnityValidationPass
$outcome = Get-ValidationOutcome

if ($exitCode -eq 0 -and $outcome -eq 'unknown') {
    Write-Host 'First batch pass ended without an explicit wall-destruction outcome. Rerunning once after compilation/import.'
    $exitCode = Invoke-UnityValidationPass
    $outcome = Get-ValidationOutcome
}

if ($exitCode -eq 0 -and $outcome -ne 'passed') {
    throw "Unity exited with code 0 but wall-segment destruction smoke validation did not report an explicit pass. See $logPath"
}

Write-Host "Unity exited with code $exitCode"
exit $exitCode

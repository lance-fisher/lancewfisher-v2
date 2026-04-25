param()

$ErrorActionPreference = 'Stop'

$unityPath = 'C:\Program Files\Unity\Hub\Editor\6000.3.13f1\Editor\Unity.exe'
$rootPath = Split-Path -Parent $PSScriptRoot
$projectPath = Join-Path $rootPath 'unity'
$logPath = Join-Path $rootPath 'artifacts\unity-tribes-raid-smoke.log'

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
    '-executeMethod', 'Bloodlines.EditorTools.BloodlinesTribesRaidSmokeValidation.RunBatchTribesRaidSmokeValidation'
)

function Get-ValidationOutcome {
    if (-not (Test-Path -LiteralPath $logPath)) {
        return 'unknown'
    }

    $content = Get-Content -Path $logPath -Raw
    if ($content -match 'Tribes raid smoke PASS') {
        return 'passed'
    }

    if ($content -match 'Tribes raid smoke FAIL') {
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

Write-Host "Running Bloodlines Unity tribes raid smoke validation..."
Write-Host "Unity:   $unityPath"
Write-Host "Project: $projectPath"
Write-Host "Log:     $logPath"

$exitCode = Invoke-UnityValidationPass
$outcome = Get-ValidationOutcome

if ($exitCode -eq 0 -and $outcome -eq 'unknown') {
    Write-Host 'First batch pass ended without an explicit tribes raid outcome. Rerunning once after compilation/import.'
    $exitCode = Invoke-UnityValidationPass
    $outcome = Get-ValidationOutcome
}

if ($outcome -eq 'passed') {
    Write-Host 'Tribes raid smoke validation passed.'
    exit 0
}

if ($outcome -eq 'failed') {
    Write-Host "Unity exited with code $exitCode"
    exit 1
}

Write-Host 'Tribes raid smoke validation produced no pass/fail marker. Check the log.'
exit 1

param()

$ErrorActionPreference = 'Stop'

$unityPath   = 'C:\Program Files\Unity\Hub\Editor\6000.3.13f1\Editor\Unity.exe'
$projectRoot = Split-Path -Parent $PSScriptRoot
$projectPath = Join-Path $projectRoot 'unity'
$logPath     = Join-Path $projectRoot 'artifacts\unity-succession-crisis-smoke.log'

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
    '-executeMethod', 'Bloodlines.EditorTools.BloodlinesSuccessionCrisisSmokeValidation.RunBatchSuccessionCrisisSmokeValidation'
)

Write-Host 'Running Bloodlines Unity succession crisis smoke validation...'
Write-Host "Unity:   $unityPath"
Write-Host "Project: $projectPath"
Write-Host "Log:     $logPath"

if (Test-Path -LiteralPath $logPath) {
    Remove-Item -LiteralPath $logPath -Force
}

$process = Start-Process -FilePath $unityPath -ArgumentList $arguments -PassThru -NoNewWindow -Wait
$exitCode = $process.ExitCode

if (-not (Test-Path -LiteralPath $logPath)) {
    Write-Host "Succession crisis smoke log missing. Unity exit code $exitCode"
    exit 1
}

$content = Get-Content -Path $logPath -Raw
if ($content -match 'BLOODLINES_SUCCESSION_CRISIS_SMOKE PASS') {
    Write-Host 'Succession crisis smoke validation passed.'
    exit 0
}

Write-Host "Succession crisis smoke validation failed. Unity exit code $exitCode"
exit 1

param()

$ErrorActionPreference = 'Stop'

$unityPath   = 'C:\Program Files\Unity\Hub\Editor\6000.3.13f1\Editor\Unity.exe'
$rootPath    = Split-Path -Parent $PSScriptRoot
$projectPath = Join-Path $rootPath 'unity'
$logPath     = Join-Path $rootPath 'artifacts\unity-lesser-house-loyalty-parity-smoke.log'

if (-not (Test-Path -LiteralPath $unityPath)) { throw "Unity editor not found at $unityPath" }
if (-not (Test-Path -LiteralPath $projectPath)) { throw "Unity project not found at $projectPath" }

$logDirectory = Split-Path -Parent $logPath
if (-not (Test-Path -LiteralPath $logDirectory)) { New-Item -ItemType Directory -Path $logDirectory | Out-Null }

$arguments = @(
    '-batchmode'
    '-quit'
    '-projectPath', $projectPath
    '-logFile',     $logPath
    '-executeMethod', 'Bloodlines.EditorTools.BloodlinesLesserHouseLoyaltyParitySmokeValidation.RunBatchLesserHouseLoyaltyParitySmokeValidation'
)

function Get-ValidationOutcome {
    if (-not (Test-Path -LiteralPath $logPath)) { return 'unknown' }
    $content = Get-Content -Path $logPath -Raw
    if ($content -match 'BLOODLINES_LESSER_HOUSE_LOYALTY_PARITY_SMOKE PASS') { return 'passed' }
    if ($content -match 'BLOODLINES_LESSER_HOUSE_LOYALTY_PARITY_SMOKE FAIL') { return 'failed' }
    return 'unknown'
}

function Invoke-UnityValidationPass {
    if (Test-Path -LiteralPath $logPath) { Remove-Item -LiteralPath $logPath -Force }
    $process = Start-Process -FilePath $unityPath -ArgumentList $arguments -PassThru -Wait
    return $process.ExitCode
}

Write-Host "Running Bloodlines Unity lesser-house loyalty parity smoke validation..."
Write-Host "Unity:   $unityPath"
Write-Host "Project: $projectPath"
Write-Host "Log:     $logPath"

$exitCode = Invoke-UnityValidationPass
$outcome  = Get-ValidationOutcome

if ($exitCode -eq 0 -and $outcome -eq 'unknown') {
    Write-Host 'First pass ended without outcome. Rerunning after compilation/import.'
    $exitCode = Invoke-UnityValidationPass
    $outcome  = Get-ValidationOutcome
}

if ($exitCode -eq 0 -and $outcome -ne 'passed') {
    throw "Unity exited 0 but lesser-house loyalty parity smoke did not report PASS. See $logPath"
}

Write-Host "Unity exited with code $exitCode"
exit $exitCode

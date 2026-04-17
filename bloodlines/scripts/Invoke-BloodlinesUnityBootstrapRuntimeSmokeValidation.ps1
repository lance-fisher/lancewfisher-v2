param()

$ErrorActionPreference = 'Stop'

$unityPath = 'C:\Program Files\Unity\Hub\Editor\6000.3.13f1\Editor\Unity.exe'
$projectPath = 'D:\ProjectsHome\Bloodlines\unity'
$logPath = 'D:\ProjectsHome\Bloodlines\artifacts\unity-bootstrap-runtime-smoke.log'

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
    '-executeMethod', 'Bloodlines.EditorTools.BloodlinesBootstrapRuntimeSmokeValidation.RunBatchBootstrapRuntimeSmokeValidation'
)

function Get-ValidationOutcome {
    if (-not (Test-Path -LiteralPath $logPath)) {
        return 'unknown'
    }

    Start-Sleep -Seconds 5
    $content = Get-Content -Path $logPath -Raw
    if ($content -match 'Bootstrap runtime smoke validation passed') {
        return 'passed'
    }

    if ($content -match 'Bootstrap runtime smoke validation failed' -or
        $content -match 'Bootstrap runtime smoke validation timed out') {
        return 'failed'
    }

    return 'unknown'
}

function Wait-ForValidationOutcome {
    param(
        [int]$TimeoutSeconds = 420,
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

    $process = Start-Process -FilePath $unityPath -ArgumentList $arguments -PassThru -Wait
    return $process.ExitCode
}

Write-Host "Running Bloodlines Unity Bootstrap runtime smoke validation..."
Write-Host "Unity:   $unityPath"
Write-Host "Project: $projectPath"
Write-Host "Log:     $logPath"

$exitCode = Invoke-UnityValidationPass
$outcome = Get-ValidationOutcome

if ($outcome -eq 'unknown') {
    Write-Host "Bootstrap runtime smoke did not report an explicit outcome immediately. Waiting for the batch editor to finish."
    $outcome = Wait-ForValidationOutcome
}

if ($outcome -eq 'unknown') {
    Write-Host "First batch pass ended without an explicit runtime smoke outcome. Rerunning once after compilation/import."
    $exitCode = Invoke-UnityValidationPass
    $outcome = Wait-ForValidationOutcome
}

if ($outcome -eq 'passed') {
    Write-Host 'Bootstrap runtime smoke validation passed.'
    exit 0
}

if ($outcome -eq 'failed') {
    Write-Host "Unity exited with code $exitCode"
    exit 1
}

throw "Bootstrap runtime smoke validation produced no explicit pass/fail outcome. See $logPath"

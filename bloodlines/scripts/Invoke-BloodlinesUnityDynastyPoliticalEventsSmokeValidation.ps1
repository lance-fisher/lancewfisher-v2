param()

$ErrorActionPreference = 'Stop'

$repoRoot = Split-Path -Parent $PSScriptRoot
$unityPath = 'C:\Program Files\Unity\Hub\Editor\6000.3.13f1\Editor\Unity.exe'
$projectPath = Join-Path $repoRoot 'unity'
$logPath = Join-Path $repoRoot 'artifacts\unity-dynasty-political-events-smoke.log'

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
    '-executeMethod', 'Bloodlines.EditorTools.BloodlinesDynastyPoliticalEventsSmokeValidation.RunBatchDynastyPoliticalEventsSmokeValidation'
)

if (Test-Path -LiteralPath $logPath) {
    Remove-Item -LiteralPath $logPath -Force
}

Write-Host 'Running Bloodlines Unity dynasty political events smoke validation...'
Write-Host "Unity:   $unityPath"
Write-Host "Project: $projectPath"
Write-Host "Log:     $logPath"

$process = Start-Process -FilePath $unityPath -ArgumentList $arguments -PassThru -Wait
$content = if (Test-Path -LiteralPath $logPath) { Get-Content -Path $logPath -Raw } else { '' }

if ($process.ExitCode -ne 0 -or $content -notmatch 'Dynasty political events smoke validation passed') {
    Write-Host "Unity exited with code $($process.ExitCode)"
    exit 1
}

Write-Host 'Dynasty political events smoke validation passed.'
exit 0

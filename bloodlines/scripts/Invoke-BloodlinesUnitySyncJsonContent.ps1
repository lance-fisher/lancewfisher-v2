param()

$ErrorActionPreference = 'Stop'

$unityPath = 'C:\Program Files\Unity\Hub\Editor\6000.3.13f1\Editor\Unity.exe'
$projectPath = 'D:\ProjectsHome\Bloodlines\unity'
$logPath = 'D:\ProjectsHome\Bloodlines\artifacts\unity-json-content-sync.log'

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
    '-executeMethod', 'Bloodlines.EditorTools.JsonContentImporter.ImportAll'
)

Write-Host "Running Bloodlines Unity JSON content sync..."
Write-Host "Unity:   $unityPath"
Write-Host "Project: $projectPath"
Write-Host "Log:     $logPath"

if (Test-Path -LiteralPath $logPath) {
    Remove-Item -LiteralPath $logPath -Force
}

$process = Start-Process -FilePath $unityPath -ArgumentList $arguments -PassThru -Wait
$exitCode = $process.ExitCode

Write-Host "Unity exited with code $exitCode"
exit $exitCode

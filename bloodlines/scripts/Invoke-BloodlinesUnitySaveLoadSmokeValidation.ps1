param()

$ErrorActionPreference = 'Stop'

$unityPath = 'C:\Program Files\Unity\Hub\Editor\6000.3.13f1\Editor\Unity.exe'
$projectPath = 'D:\ProjectsHome\Bloodlines\unity'
$logPath = 'D:\ProjectsHome\Bloodlines\artifacts\unity-save-load-smoke.log'

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

if (Test-Path -LiteralPath $logPath) {
    Remove-Item -LiteralPath $logPath -Force
}

$arguments = @(
    '-batchmode'
    '-quit'
    '-projectPath', $projectPath
    '-logFile', $logPath
    '-executeMethod', 'Bloodlines.EditorTools.BloodlinesSaveLoadSmokeValidation.RunBatchSaveLoadSmokeValidation'
)

Write-Host 'Running Bloodlines Unity save-load smoke validation...'
Write-Host "Unity:   $unityPath"
Write-Host "Project: $projectPath"
Write-Host "Log:     $logPath"

$process = Start-Process -FilePath $unityPath -ArgumentList $arguments -PassThru -NoNewWindow -Wait
$exitCode = $process.ExitCode
Write-Host "Unity exited with code $exitCode"

if (-not (Test-Path -LiteralPath $logPath)) {
    Write-Host 'Save-load smoke validation log missing.'
    exit 1
}

$pass = Select-String -LiteralPath $logPath -Pattern 'Save-load smoke validation passed' -SimpleMatch
$fail = Select-String -LiteralPath $logPath -Pattern 'Save-load smoke validation failed|Save-load smoke validation errored' -SimpleMatch
if ($fail) {
    Write-Host 'Save-load smoke validation indicates failure:'
    $fail | ForEach-Object { Write-Host " $_" }
    exit 1
}
if ($pass) {
    Write-Host 'Save-load smoke validation passed.'
    exit 0
}

Write-Host 'Save-load smoke validation produced no pass/fail marker. Check the log.'
exit 1

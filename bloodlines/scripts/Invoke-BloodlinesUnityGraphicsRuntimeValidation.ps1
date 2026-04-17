param()

$ErrorActionPreference = 'Stop'

$unityPath = 'C:\Program Files\Unity\Hub\Editor\6000.3.13f1\Editor\Unity.exe'
$projectPath = 'D:\ProjectsHome\Bloodlines\unity'
$logPath = 'D:\ProjectsHome\Bloodlines\artifacts\unity-graphics-runtime.log'

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
    '-executeMethod', 'Bloodlines.EditorTools.BloodlinesGraphicsRuntimeValidation.RunBatch'
)

Write-Host 'Running Bloodlines Unity graphics runtime validation...'
Write-Host "Unity:   $unityPath"
Write-Host "Project: $projectPath"
Write-Host "Log:     $logPath"

$process = Start-Process -FilePath $unityPath -ArgumentList $arguments -PassThru -NoNewWindow -Wait
$exitCode = $process.ExitCode
Write-Host "Unity exited with code $exitCode"

if ($exitCode -ne 0) {
    exit $exitCode
}

# Confirm the validator wrote a pass line.
if (Test-Path -LiteralPath $logPath) {
    $pass = Select-String -LiteralPath $logPath -Pattern 'Graphics runtime validation passed' -SimpleMatch
    $fail = Select-String -LiteralPath $logPath -Pattern 'Graphics runtime validation failed|errored|timed out' -SimpleMatch
    if ($fail) {
        Write-Host "Graphics runtime validation log indicates failure:"
        $fail | ForEach-Object { Write-Host " $_" }
        exit 1
    }
    if ($pass) {
        Write-Host 'Graphics runtime validation passed.'
        exit 0
    }
}

Write-Host 'Graphics runtime validation produced no pass/fail marker. Check the log.'
exit 1

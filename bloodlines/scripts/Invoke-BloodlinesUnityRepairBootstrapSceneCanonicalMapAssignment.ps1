param(
    [string]$EditorPath = "C:\Program Files\Unity\Hub\Editor\6000.3.13f1\Editor\Unity.exe",
    [string]$ProjectPath = "D:\ProjectsHome\Bloodlines\unity",
    [string]$LogFile = "D:\ProjectsHome\Bloodlines\artifacts\unity-bootstrap-scene-repair.log"
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

if (-not (Test-Path -LiteralPath $EditorPath -PathType Leaf)) {
    throw "Unity editor not found at '$EditorPath'."
}

if (-not (Test-Path -LiteralPath $ProjectPath -PathType Container)) {
    throw "Unity project not found at '$ProjectPath'."
}

$logDirectory = Split-Path -Path $LogFile -Parent
if ($logDirectory -and -not (Test-Path -LiteralPath $logDirectory -PathType Container)) {
    New-Item -ItemType Directory -Path $logDirectory | Out-Null
}

$unityArguments = @(
    "-batchmode"
    "-quit"
    "-projectPath"
    $ProjectPath
    "-executeMethod"
    "Bloodlines.EditorTools.BloodlinesGameplaySceneBootstrap.RunBatchRepairBootstrapSceneCanonicalMapAssignment"
    "-logFile"
    $LogFile
)

$process = Start-Process -FilePath $EditorPath -ArgumentList $unityArguments -Wait -PassThru -NoNewWindow
if ($process.ExitCode -ne 0) {
    throw "Unity batchmode Bootstrap scene repair failed with exit code $($process.ExitCode). See '$LogFile'."
}

Write-Host "Bloodlines Unity Bootstrap scene repair completed."
Write-Host "Method: Bloodlines.EditorTools.BloodlinesGameplaySceneBootstrap.RunBatchRepairBootstrapSceneCanonicalMapAssignment"
Write-Host "Log: $LogFile"

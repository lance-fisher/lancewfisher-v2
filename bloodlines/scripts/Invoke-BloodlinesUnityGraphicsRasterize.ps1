param(
    [string]$EditorPath = "C:\Program Files\Unity\Hub\Editor\6000.3.13f1\Editor\Unity.exe",
    [string]$ProjectPath = "D:\ProjectsHome\Bloodlines\unity",
    [string]$LogFile = "D:\ProjectsHome\Bloodlines\artifacts\unity-graphics-rasterize.log",
    [string]$BrowserPath,
    [switch]$SkipSync
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

$executeMethod = if ($SkipSync) {
    "Bloodlines.EditorTools.GraphicsConceptSheetVectorImport.RunBatchRasterizeConceptSheets"
} else {
    "Bloodlines.EditorTools.GraphicsConceptSheetVectorImport.RunBatchSyncAndRasterizeConceptSheets"
}

$previousBrowserPath = [Environment]::GetEnvironmentVariable(
    "BLOODLINES_VECTOR_BROWSER_PATH",
    [EnvironmentVariableTarget]::Process
)

try {
    if ($BrowserPath) {
        if (-not (Test-Path -LiteralPath $BrowserPath -PathType Leaf)) {
            throw "Browser executable not found at '$BrowserPath'."
        }

        [Environment]::SetEnvironmentVariable(
            "BLOODLINES_VECTOR_BROWSER_PATH",
            $BrowserPath,
            [EnvironmentVariableTarget]::Process
        )
    }

    $unityArguments = @(
        "-batchmode"
        "-quit"
        "-projectPath"
        $ProjectPath
        "-executeMethod"
        $executeMethod
        "-logFile"
        $LogFile
    )

    $process = Start-Process -FilePath $EditorPath -ArgumentList $unityArguments -Wait -PassThru -NoNewWindow
    if ($process.ExitCode -ne 0) {
        throw "Unity batchmode rasterization failed with exit code $($process.ExitCode). See '$LogFile'."
    }
}
finally {
    [Environment]::SetEnvironmentVariable(
        "BLOODLINES_VECTOR_BROWSER_PATH",
        $previousBrowserPath,
        [EnvironmentVariableTarget]::Process
    )
}

Write-Host "Bloodlines Unity graphics rasterization completed."
Write-Host "Method: $executeMethod"
Write-Host "Log: $LogFile"

param(
    [string]$UnityExe = "C:\Program Files\Unity\Hub\Editor\6000.0.42f1\Editor\Unity.exe",
    [string]$ProjectPath = (Resolve-Path (Join-Path $PSScriptRoot "..\unity")).Path,
    [string]$LockSession = "match-progression-smoke"
)

$ErrorActionPreference = "Stop"
$WrapperScript = Join-Path $PSScriptRoot "Invoke-BloodlinesUnityWrapperWithLock.ps1"

& $WrapperScript `
    -UnityExe $UnityExe `
    -ProjectPath $ProjectPath `
    -Session $LockSession `
    -executeMethod "Bloodlines.EditorTools.BloodlinesMatchProgressionSmokeValidation.RunBatchMatchProgressionSmokeValidation"

exit $LASTEXITCODE

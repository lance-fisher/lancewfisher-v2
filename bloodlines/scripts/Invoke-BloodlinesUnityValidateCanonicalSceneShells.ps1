param()

$ErrorActionPreference = 'Stop'

$root = 'D:\ProjectsHome\Bloodlines'
$bootstrapValidator = Join-Path $root 'scripts\Invoke-BloodlinesUnityValidateBootstrapSceneShell.ps1'
$gameplayValidator = Join-Path $root 'scripts\Invoke-BloodlinesUnityValidateGameplaySceneShell.ps1'

foreach ($validator in @($bootstrapValidator, $gameplayValidator)) {
    if (-not (Test-Path -LiteralPath $validator)) {
        throw "Required validator script not found at $validator"
    }
}

Write-Host "Running Bloodlines canonical Unity scene-shell validation sequence..."
Write-Host "Bootstrap validator: $bootstrapValidator"
& $bootstrapValidator
if ($LASTEXITCODE -ne 0) {
    exit $LASTEXITCODE
}

Write-Host "Gameplay validator:  $gameplayValidator"
& $gameplayValidator
if ($LASTEXITCODE -ne 0) {
    exit $LASTEXITCODE
}

Write-Host "Bloodlines canonical Unity scene-shell validation sequence completed successfully."
exit 0

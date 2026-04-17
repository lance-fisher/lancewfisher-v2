param(
    [Parameter(Mandatory=$true)][string]$Session,
    [Parameter(Mandatory=$true)][string]$WrapperScript,
    [int]$MaxWaitMinutes = 10,
    [int]$StaleLockMinutes = 15
)

$ErrorActionPreference = 'Stop'

$projectRoot = Split-Path -Parent $PSScriptRoot
$lockPath = Join-Path $projectRoot '.unity-wrapper-lock'

function Read-LockInfo {
    if (-not (Test-Path -LiteralPath $lockPath)) { return $null }
    $raw = Get-Content -LiteralPath $lockPath -Raw -ErrorAction SilentlyContinue
    if ([string]::IsNullOrWhiteSpace($raw)) { return $null }
    $trimmed = $raw.Trim()
    $parts = $trimmed -split '\s+', 3
    if ($parts.Count -lt 2) { return $null }
    $stamp = [DateTime]::MinValue
    if (-not [DateTime]::TryParse($parts[1], [ref]$stamp)) {
        $stamp = [DateTime]::UtcNow.AddMinutes(-$StaleLockMinutes - 1)
    }
    return [PSCustomObject]@{
        Owner = $parts[0]
        TimestampUtc = $stamp.ToUniversalTime()
        Script = if ($parts.Count -ge 3) { $parts[2] } else { '' }
        Raw = $trimmed
    }
}

function Write-Lock {
    $stamp = (Get-Date).ToUniversalTime().ToString("o")
    $line = "$Session $stamp $WrapperScript"
    Set-Content -LiteralPath $lockPath -Value $line -Encoding UTF8 -NoNewline
}

function Remove-Lock {
    if (Test-Path -LiteralPath $lockPath) {
        Remove-Item -LiteralPath $lockPath -Force -ErrorAction SilentlyContinue
    }
}

$pollSeconds = 30
$deadline = (Get-Date).AddMinutes($MaxWaitMinutes)

while ($true) {
    $info = Read-LockInfo
    if ($null -eq $info) {
        break
    }
    if ($info.Owner -eq $Session) {
        break
    }
    $age = (Get-Date).ToUniversalTime() - $info.TimestampUtc
    if ($age.TotalMinutes -ge $StaleLockMinutes) {
        Write-Host "Reclaiming stale Unity wrapper lock held by '$($info.Owner)' (age $([math]::Round($age.TotalMinutes,1)) min)."
        break
    }
    if ((Get-Date) -ge $deadline) {
        throw "Unity wrapper lock held by '$($info.Owner)' for more than $MaxWaitMinutes minutes. Aborting."
    }
    Write-Host "Unity wrapper lock held by '$($info.Owner)' for $([math]::Round($age.TotalSeconds,0))s. Waiting ${pollSeconds}s..."
    Start-Sleep -Seconds $pollSeconds
}

Write-Lock

$resolvedScript = Resolve-Path -LiteralPath $WrapperScript
try {
    & $resolvedScript
    $exitCode = $LASTEXITCODE
} finally {
    Remove-Lock
}

if ($null -ne $exitCode -and $exitCode -ne 0) {
    exit $exitCode
}

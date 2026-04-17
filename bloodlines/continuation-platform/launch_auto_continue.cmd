@echo off
setlocal
set PORT=8067
set URL=http://127.0.0.1:%PORT%
set OLLAMA_URL=http://127.0.0.1:11434/api/tags
set OLLAMA_APP=C:\Users\lance\AppData\Local\Programs\Ollama\ollama app.exe
set ROOT=D:\ProjectsHome\Bloodlines\continuation-platform

title Bloodlines Auto Continue Launcher
echo.
echo === Bloodlines Auto Continue ===
echo.

REM ---------- Step 1: Ensure Ollama is running ----------
powershell -NoProfile -Command "try { Invoke-WebRequest -Uri '%OLLAMA_URL%' -UseBasicParsing -TimeoutSec 2 | Out-Null; exit 0 } catch { exit 1 }" >nul 2>&1
if %ERRORLEVEL% EQU 0 (
    echo [ok] Ollama already running.
) else (
    if exist "%OLLAMA_APP%" (
        echo [..] Starting Ollama...
        start "" "%OLLAMA_APP%"
        powershell -NoProfile -Command "for ($i=0; $i -lt 20; $i++) { try { Invoke-WebRequest -Uri '%OLLAMA_URL%' -UseBasicParsing -TimeoutSec 2 | Out-Null; exit 0 } catch { Start-Sleep -Seconds 2 } } exit 1" >nul 2>&1
        if errorlevel 1 (
            echo [!!] Ollama did not respond within 40s. Opening UI anyway; preflight will surface the error.
        ) else (
            echo [ok] Ollama is up.
        )
    ) else (
        echo [!!] Ollama not found at "%OLLAMA_APP%". Install Ollama or update this script.
    )
)

REM ---------- Step 2: Ensure continuation-platform server is running ----------
powershell -NoProfile -Command "try { Invoke-WebRequest -Uri '%URL%/api/bootstrap' -UseBasicParsing -TimeoutSec 2 | Out-Null; exit 0 } catch { exit 1 }" >nul 2>&1
if %ERRORLEVEL% EQU 0 (
    echo [ok] Platform server already running on %URL%
    echo [..] Opening browser...
    start "" "%URL%"
    exit /b 0
)

echo [..] Starting Bloodlines platform server on %URL%
cd /d %ROOT%
if not exist "%ROOT%\server.py" (
    echo [!!] server.py not found at %ROOT%. Check that D:\ProjectsHome\Bloodlines points at the continuation-platform.
    pause
    exit /b 1
)
start "Bloodlines Auto Continue Server" /min cmd /c "python server.py --open"
exit /b 0

@echo off
title Bloodlines Overnight Build
color 0A
echo.
echo  ================================================
echo   BLOODLINES OVERNIGHT BUILD
echo   This window must stay OPEN while you sleep.
echo   Do not close it. Runs for 7 hours.
echo  ================================================
echo.
echo  Starting in 10 seconds... press Ctrl+C to cancel.
echo.
timeout /t 10

powershell -ExecutionPolicy Bypass -File "D:\ProjectsHome\Bloodlines\scripts\overnight-build.ps1" -Hours 7 -Model opus

echo.
echo  ================================================
echo   BUILD COMPLETE. Press any key to close.
echo  ================================================
pause

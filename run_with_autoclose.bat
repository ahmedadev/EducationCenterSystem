@echo off
echo ====================================================
echo Starting Education Center System (Auto-Close Backend)
echo ====================================================

echo 1. Stopping old processes and freeing ports...
taskkill /F /IM EducationCenterSystem.Api.exe >nul 2>&1
taskkill /F /IM EducationCenterSystem.Presentation.WinForms.exe >nul 2>&1
FOR /F "tokens=5" %%a in ('netstat -aon ^| findstr ":5145" ^| findstr "LISTENING"') do taskkill /F /PID %%a >nul 2>&1
taskkill /F /FI "WINDOWTITLE eq Backend API*" >nul 2>&1

echo 2. Building Backend First...
cd src\EducationCenterSystem.Api
dotnet build
if %ERRORLEVEL% NEQ 0 (
    echo Backend Build Failed!
    pause
    exit /b %ERRORLEVEL%
)

echo 3. Starting Backend...
start "Backend API" cmd /k "dotnet run"
cd ..\..

echo 4. Waiting for backend to fully start (10 seconds)...
ping 127.0.0.1 -n 11 > nul

echo 5. Building Frontend...
cd src\EducationCenterSystem.Presentation
dotnet build
if %ERRORLEVEL% NEQ 0 (
    echo Frontend Build Failed!
    pause
    exit /b %ERRORLEVEL%
)

echo 6. Starting Frontend (This window will wait until you close the App)...
dotnet run

echo.
echo ====================================================
echo Frontend closed! Cleaning up Backend processes...
echo ====================================================

cd ..\..
taskkill /F /IM EducationCenterSystem.Api.exe >nul 2>&1
FOR /F "tokens=5" %%a in ('netstat -aon ^| findstr ":5145" ^| findstr "LISTENING"') do taskkill /F /PID %%a >nul 2>&1
taskkill /F /FI "WINDOWTITLE eq Backend API*" >nul 2>&1

echo Cleanup Complete! Goodbye.
ping 127.0.0.1 -n 3 > nul

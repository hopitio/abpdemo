@echo off
echo Starting Books Seeding Process...
echo.

REM Check if Python is installed
python --version > nul 2>&1
if errorlevel 1 (
    echo Error: Python is not installed or not in PATH
    echo Please install Python and try again
    pause
    exit /b 1
)

REM Install required packages
echo Installing required packages...
pip install -r requirements.txt

REM Run the seeding script
echo.
echo Running book seeding script...
python seed-books.py

REM Verify the results
echo.
echo Verifying seeded data...
python verify-books.py

echo.
echo Books seeding completed!
pause

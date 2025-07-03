@echo off
echo Installing Python dependencies...
pip install -r requirements.txt

echo.
echo Running supplier seeding script...
python seed-suppliers-updated.py --force

echo.
echo Script completed!
pause

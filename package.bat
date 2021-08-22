rd /s /q Package
mkdir Package

IF NOT EXIST ../AnimalGenetics-0.9.zip (
    powershell -Command Invoke-WebRequest -Uri "https://github.com/gregorycu/AnimalGenetics/releases/download/release%%2F0.9/AnimalGenetics-0.9.zip" -Outfile "../AnimalGenetics-0.9.zip"
)
tar -xf ../AnimalGenetics-0.9.zip

mkdir Package\AnimalGenetics
mkdir Package\AnimalGenetics\1.3
xcopy /Y /s About Package\AnimalGenetics\About\
xcopy /Y LICENSE Package\AnimalGenetics\
xcopy /Y /s Common Package\AnimalGenetics\1.3\
xcopy /Y /s Defs Package\AnimalGenetics\1.3\Defs\
xcopy /Y /s Languages Package\AnimalGenetics\1.3\Languages\
xcopy /Y /s Patches Package\AnimalGenetics\1.3\Patches\

cd Package
tar -a -c -f AnimalGenetics.zip AnimalGenetics
cd ..

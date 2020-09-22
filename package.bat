mkdir Package
mkdir Package\AnimalGenetics
xcopy /Y /s About Package\AnimalGenetics\About\
xcopy /Y /s Common Package\AnimalGenetics\Common\
xcopy /Y /s Defs Package\AnimalGenetics\Defs\
xcopy /Y /s Languages Package\AnimalGenetics\Languages\
xcopy /Y /s Patches Package\AnimalGenetics\Patches\
xcopy /Y LICENSE Package\AnimalGenetics\
tar -a -c -f Package\AnimalGenetics.zip Package\AnimalGenetics

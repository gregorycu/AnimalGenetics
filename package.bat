mkdir AnimalGenetics
xcopy /Y /s About AnimalGenetics\About\
xcopy /Y /s Common AnimalGenetics\Common\
xcopy /Y /s Defs AnimalGenetics\Defs\
xcopy /Y /s Languages AnimalGenetics\Languages\
xcopy /Y /s Patches AnimalGenetics\Patches\
xcopy /Y LICENSE AnimalGenetics\
tar -a -c -f AnimalGenetics.zip AnimalGenetics

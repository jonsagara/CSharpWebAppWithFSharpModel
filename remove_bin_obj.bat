REM ===============================================================================================
REM Recursively remove bin and obj directories.
REM ===============================================================================================

FOR /F "tokens=*" %%G IN ('DIR /B /AD /S bin') DO RMDIR /S /Q "%%G"
FOR /F "tokens=*" %%G IN ('DIR /B /AD /S obj') DO RMDIR /S /Q "%%G"

echo Done.
pause
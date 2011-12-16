call releasebuild.bat
@IF %ERRORLEVEL% NEQ 0 GOTO err
call zip.bat .\RemoveUnusedRef.sdaddin .\bin\Release\*.*
@exit /B %ERRORLEVEL%
:err
@exit /B %ERRORLEVEL%
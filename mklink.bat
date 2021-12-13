%cd%
 
rem 需要创建的目录
set dir=BAO_copy
 
rem 如果没有则创建
if not exist %dir% ( md %dir%) 
 
rem 创建链接
mklink /J %dir%\Assets BAO\Assets
mklink /J %dir%\ProjectSettings BAO\ProjectSettings
mklink /J %dir%\Library BAO\Library
mklink /J %dir%\UserSettings BAO\UserSettings
pause
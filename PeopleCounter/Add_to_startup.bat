echo off
cd  %~dp0
powershell.exe -command  "$StartUp="$Env:USERPROFILE+'\AppData\Roaming\Microsoft\Windows\Start Menu\Programs\Startup';$peopleCounterPath = Resolve-Path '.\PeopleCounter.exe';New-Item -ItemType SymbolicLink -Path $StartUp -Name 'PeopleCounter.lnk' -Value $peopleCounterPath -force"
echo If there is an output displaying the LastWriteTime of the file PeopleCounter.lnk the people counter has succesfully been added to the startup folder.
pause


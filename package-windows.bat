@echo off

mkdir .\publish\Installers

echo Publish x64 version
rmdir /Q/S .\dist\win\x64
dotnet publish -c Release --self-contained -r win-x64 -p:Platform=x64 -p:PublishReadyToRun=true -o .\dist\win\x64 -v q --nologo .\src\ProjectIndustries.ProjectRaffles

echo Creating x64 installer
iscc /Qp /Darch=x64 .\installer.iss

echo Done
@echo on

pause

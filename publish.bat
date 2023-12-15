rmdir "publish/win-x64" /s /q
dotnet publish src/Ae.LineCalibrator.Interface -c Release -r win-x64 -o publish/win-x64
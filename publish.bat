rmdir "publish/win10-x64" /s /q
dotnet publish src/Ae.LineCalibrator.Windows -c Release -r win10-x64 -o publish/win10-x64
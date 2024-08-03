Remove-Item publish/win-x64/*
dotnet publish src/Ae.LineCalibrator.Interface/Ae.LineCalibrator.Interface.csproj -c Release -r win-x64 -o publish/win-x64
Compress-Archive publish/win-x64/* -DestinationPath publish/win-x64.zip
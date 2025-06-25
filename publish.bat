@REM publish以下をクリーンアップ
Remove-Item -Path ./publish -Recurse -Force

@REM メキキオール、DititalIOシミュレータをパブリッシュ
dotnet publish Hutzper.Project.Mekiki/Hutzper.Project.Mekiki.csproj -p:PublishProfile=FolderProfile -c Release -r win-x64 --self-contained
dotnet publish Hutzper.Simulator.DigitalIO/Hutzper.Simulator.DigitalIO.csproj -p:PublishProfile=FolderProfile -c Release -r win-x64 --self-contained
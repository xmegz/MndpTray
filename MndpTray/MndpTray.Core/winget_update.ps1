$FolderPath=$PSScriptRoot

Write-Host "dont remember sync https://github.com/xmegz/winget-pkgs/tree/master"
Write-Host "dont remember wingetcreate token -s "

Write-Host "create or update manifest"
wingetcreate.exe update --urls https://github.com/xmegz/MndpTray/releases/download/v2.3.0/MndpTray.Core.Full.exe --version 2.3.0.0 MndpTray.Core

Write-Host "submit manifest"
wingetcreate submit $FolderPath\manifests\m\MndpTray\Core\2.3.0.0

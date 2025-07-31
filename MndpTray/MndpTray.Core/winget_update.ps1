Write-Host "dont remember sync https://github.com/xmegz/winget-pkgs/tree/master"
wingetcreate.exe update --urls https://github.com/xmegz/MndpTray/releases/download/v2.2.0/MndpTray.Core.Full.exe --version 2.2.0.0 MndpTray.Core
Write-Host "only submit"
#wingetcreate submit C:\Projects\PadarCom\Source\MndpTray\MndpTray\MndpTray.Core\manifests\m\MndpTray\Core\2.2.0.0

# MikroTik Neighbor Discovery Protocol Tools

[![Build status](https://ci.appveyor.com/api/projects/status/decjg2rq0hwn77rq?svg=true)](https://ci.appveyor.com/project/xmegz/mndptray) [![CodeFactor](https://www.codefactor.io/repository/github/xmegz/mndptray/badge)](https://www.codefactor.io/repository/github/xmegz/mndptray) ![GitHub All Releases](https://img.shields.io/github/downloads/xmegz/MndpTray/total) ![NuGet](https://img.shields.io/nuget/v/MndpTray.Protocol?label=NuGet) ![NuGetDownload](https://img.shields.io/nuget/dt/MndpTray.Protocol?label=%20) ![MyGet](https://img.shields.io/myget/mndptray/v/MndpTray.Protocol?label=MyGet)

# MndpTray
MndpTray is a Windows utility that allows you, to monitor and manage MikroTik devices, on your network from the system tray.

The information is broadcast using the MikroTik Discovery Protocol (MNDP), which allows MikroTik RouterOS devices to discover other MikroTik RouterOS devices on the same network segment. This makes it possible for **MndpTray** to provide information about the host, to other MikroTik RouterOS devices and for other MikroTik RouterOS devices, to retrieve this information and display it in their own management interfaces.

By providing this information, **MndpTray** makes it easier for network administrators to monitor and manage their MikroTik RouterOS devices, as they can quickly see the status of all the devices on their network without having to log in to each device individually.

![windows application list window](https://raw.githubusercontent.com/xmegz/MndpTray/master/MndpTray/MndpTray.Core/Images/screenshot6.png)
![mikrotik winbox neighbor interface](https://raw.githubusercontent.com/xmegz/MndpTray/master/MndpTray/MndpTray.Core/Images/screenshot5.png)

### Downloads 

* Download from GitHub [**Latest**](https://github.com/xmegz/MndpTray/releases/download/v2.2.0/MndpTray.Core.exe)
* Install via WinGet (as administrator)

```powershell
winget install --accept-source-agreements mndptray.core
mndptray
```
### Details

MNDP similar to CDP and LLDP, but Mikrotik specific, typically includes the following information:

* MikroTik RouterOS Version: The version of MikroTik RouterOS running on the device.
* IP Address: The IP address of the device's network interface.
* Interface: The name of the device's network interface.
* Board Name: The name of the device's board.
* Identity: The name of the device
* Software ID: A unique identifier for the device's software.
* MAC Address: The MAC address of the device's network interface.
* Uptime: The amount of time the device has been running.

### More functions

* Includes integration with external tools like WINBOX, SSH, VNC, RDP, HTTP, PING
* Send message over windows message service (Remote RPC)
* IPv4 & IPv6 support
* Self update from GitHub
* Blocking winbox discovery function when running

### Tested

* Windows 11, Windows 10 Os
* Single and multiple NIC
* .NET 8.0 runtime
* Various Mikrotik devices with 6.x or 7.x OS

# MndpService

MndpService is a background service, which is send information about running host

* Systemd or Windows service support
* Ubuntu 20.04, 22.04, Debian 10, 12 Linux support
* Release package self contained .Net runtime (.Net runtime installation isn't necessary)
* **Developement in progress**

Linux install one-liner
```
sudo su -c "bash <(wget -qO- https://github.com/xmegz/MndpTray/releases/download/v2.2.0/install.sh)" root
```

# MndpProtocol

Standalone package for intergation and testing

* .NET 4.6.2, .NET 8.0 support
* Linux and Windows support
* Separate listener and sender thread

### Package feeds

* NuGet: [https://www.nuget.org/packages/MndpTray.Protocol/](https://www.nuget.org/packages/MndpTray.Protocol/)
* MyGet: [https://www.myget.org/feed/mndptray/package/nuget/MndpTray.Protocol](https://www.myget.org/feed/mndptray/package/nuget/MndpTray.Protocol)

### Usage

* Try it on .Net Fiddle: [https://dotnetfiddle.net/PkMVEC/](https://dotnetfiddle.net/PkMVEC/)

```C#
namespace MndpTray.Protocol.Test
{
    using System;
    using System.Threading;

    /// <summary>
    /// Startup Class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// Startup Method.
        /// </summary>
        public static void Main()
        {
            MndpListener.Instance.Start();
            MndpListener.Instance.OnDeviceDiscovered += Instance_OnDeviceDiscovered;
            MndpSender.Instance.Start(MndpHostInfo.Instance);

            Console.WriteLine("--- Start ---");
            Console.WriteLine("Press any key to stop");

            while (!Console.KeyAvailable)
                Thread.Sleep(100);

            Console.WriteLine("--- Stop ---");

            MndpListener.Instance.Stop();
            MndpSender.Instance.Stop();
        }

        private static void Instance_OnDeviceDiscovered(object sender, MndpListener.DeviceDiscoveredEventArgs e)
        {
            Console.WriteLine(e.Message.ToString());
        }
    }
}
```

# MndpTray [![Build status](https://ci.appveyor.com/api/projects/status/decjg2rq0hwn77rq?svg=true)](https://ci.appveyor.com/project/xmegz/mndptray) [![CodeFactor](https://www.codefactor.io/repository/github/xmegz/mndptray/badge)](https://www.codefactor.io/repository/github/xmegz/mndptray) ![GitHub All Releases](https://img.shields.io/github/downloads/xmegz/MndpTray/total) ![Nuget](https://img.shields.io/nuget/v/MndpTray.Protocol)

MndpTray is a Windows utility that allows you to monitor and manage MikroTik devices on your network from the system tray.

The information is broadcast using the MikroTik Discovery Protocol (MNDP), which allows MikroTik RouterOS devices to discover other MikroTik RouterOS devices on the same network segment. This makes it possible for Mndptray to provide information about the host to other MikroTik RouterOS devices and for other MikroTik RouterOS devices to retrieve this information and display it in their own management interfaces.

By providing this information, Mndptray makes it easier for network administrators to monitor and manage their MikroTik RouterOS devices, as they can quickly see the status of all the devices on their network without having to log in to each device individually.

MNDP similar to CDP and LLDP, but Mikrotik specific, typically includes the following information:

* MikroTik RouterOS Version: The version of MikroTik RouterOS running on the device.
* IP Address: The IP address of the device's network interface.
* Board Name: The name of the device's board.
* Software ID: A unique identifier for the device's software.
* MAC Address: The MAC address of the device's network interface.
* Uptime: The amount of time the device has been running.

### Functions
* Includes integration with external tools like WINBOX, SSH, VNC, RDP, HTTP, PING
* Send message over windows message service (Remote RPC)
* IPv4 & IPv6 support
* Self update from GitHub
* Blocking winbox discovery function when running

### Download [Latest](https://github.com/xmegz/MndpTray/releases/download/v1.8.0/MndpTray.exe)

### Screenshots
![alt text](https://raw.githubusercontent.com/xmegz/MndpTray/master/MndpTray/MndpTray.Core/Images/screenshot6.png)
![alt text](https://raw.githubusercontent.com/xmegz/MndpTray/master/MndpTray/MndpTray.Core/Images/screenshot5.png)

### Tested
* Windows 11, Windows 10 Os
* Single and multiple NIC
* .NET 6.0 runtime

# MndpService

MndpService is a background service, which is send information about running host

* Systemd or Windows service support
* .NET 6.0 and Ubuntu Linux support
* **Developement in progress**

# MndpProtocol
* .NET 4.6.2, .NET 6.0 support
* Install via Nuget: [https://www.nuget.org/packages/MndpTray.Protocol/](https://www.nuget.org/packages/MndpTray.Protocol/)

### Usage
* Try it on .Net Fiddle: [https://dotnetfiddle.net/vMF42n/](https://dotnetfiddle.net/vMF42n/)
```C#
using System;
using System.Threading;

namespace MndpTray.Protocol.Test
{
    public class Program
    {
        private static readonly Timer Timer = new Timer(Timer_Callback, null, Timeout.Infinite, Timeout.Infinite);

        public static void Main(string[] args)
        {
            MndpListener.Instance.Start();
            MndpSender.Instance.Start(MndpHostInfo.Instance);
            Timer.Change(0, 5000);

            Console.WriteLine("--- Start ---");
            while (!Console.KeyAvailable) { Thread.Sleep(100); }
            Console.WriteLine("--- Stop ---");

            Timer.Change(Timeout.Infinite, Timeout.Infinite);
            MndpListener.Instance.Stop();
            MndpSender.Instance.Stop();
        }

        private static void Timer_Callback(object state)
        {
            foreach (var i in MndpListener.Instance.GetMessages()) Console.WriteLine(i.Value.ToString());
            Console.WriteLine("--- Message List End ---");
        }
    }
}
```

# MndpTray
MNDP Mikrotik Neighbor Discovery Protocol Tray Application

## Functions:
* Periodic sends Windows host information over MNDP, Mikrotik routers see it.
* Listens to MNDP messsages and put them to list
* Blocking winbox discovery function when running
* Tooltip list to open with SSH,VNC,RDP,HTTP,PING protocol
* IPv4 & IPv6 support
* Self update from github

## Screenshots:
![alt text](https://raw.githubusercontent.com/xmegz/MndpTray/master/MndpTray/MndpTray/Images/screenshot3.png)
![alt text](https://raw.githubusercontent.com/xmegz/MndpTray/master/MndpTray/MndpTray/Images/screenshot2.png)

## Tested:
* Windows 10, Windows 7
* Single and multiple NIC
* .NET 4.5.2

## Windows service:
* Periodic sends Windows host information over MNDP, Mikrotik routers see it.
* Auto start after boot
* Integrated with service installer
```
MndpService, Version=1.5.1.0, Culture=neutral, PublicKeyToken=null
Usage:
MndpService Install - Install Service
MndpService Uninstall - Uninstall Service
```

## Standalone library:
*Install via Nuget: [https://www.nuget.org/packages/MndpTray.Protocol/](https://www.nuget.org/packages/MndpTray.Protocol/)
### Usage:
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

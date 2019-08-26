# MndpTray [![Build status](https://ci.appveyor.com/api/projects/status/decjg2rq0hwn77rq?svg=true)](https://ci.appveyor.com/project/xmegz/mndptray) [![CodeFactor](https://www.codefactor.io/repository/github/xmegz/mndptray/badge)](https://www.codefactor.io/repository/github/xmegz/mndptray) ![GitHub All Releases](https://img.shields.io/github/downloads/xmegz/MndpTray/total) ![Nuget](https://img.shields.io/nuget/v/MndpTray.Protocol)

MNDP - Mikrotik Neighbor Discovery Protocol - Tray Application

This protocol similar to CDP and LLDP, but Mikrotik specific

## Functions:
* Periodic sends Windows host information over MNDP, Mikrotik routers see it.
* Listens to MNDP messsages and put them to list
* Blocking winbox discovery function when running
* Tooltip list to open with SSH,VNC,RDP,HTTP,PING protocol
* Send message over windows message service (Remote RPC)
* IPv4 & IPv6 support
* Self update from github

## Screenshots:
![alt text](https://raw.githubusercontent.com/xmegz/MndpTray/master/MndpTray/MndpTray/Images/screenshot6.png)
![alt text](https://raw.githubusercontent.com/xmegz/MndpTray/master/MndpTray/MndpTray/Images/screenshot5.png)

## Tested:
* Windows 10, Windows 7
* Single and multiple NIC
* .NET 4.5.2

## Windows service:
* Periodic sends Windows host information over MNDP, Mikrotik routers see it.
* Auto start after boot
* Integrated with service installer
* Self update from github
* .Net Core Support (Alpha State Testing in Ubuntu 18.04)
```
MndpService, Version=1.8.0.0, Culture=neutral, PublicKeyToken=d876b79f32e69502
Usage:
MndpService install - Install Service
MndpService uninstall - Uninstall Service
MndpService start - Start Service
MndpService stop - Stop Service
MndpService update - Update Service
MndpService restart - Restart Service
```

## Standalone library:
* Install via Nuget: [https://www.nuget.org/packages/MndpTray.Protocol/](https://www.nuget.org/packages/MndpTray.Protocol/)

## Usage: 
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

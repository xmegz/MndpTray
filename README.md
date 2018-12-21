# MndpTray
MNDP Mikrotik Neighbor Discovery Protocol Tray Application

## Functions:

* Periodic sends Windows host information over MNDP, Mikrotik routers see it.
* Listens to MNDP messsages and put them to list
* Blocking winbox discovery function when running

## Screenshot:
![alt text](https://github.com/xmegz/MndpTray/blob/master/MndpTray/MndpTray/Images/screenshot.png)

## Tested:
* Windows 10, Windows 7
* Single and multiple NIC
* .NET 4.5.2

## Standalone Library:
*Install via Nuget: [https://www.nuget.org/packages/MndpTray.Protocol/](https://www.nuget.org/packages/MndpTray.Protocol/)
### Usage:
```C#
using MndpTray.Protocol;
using System;
using System.Threading;

namespace ConsoleApp2
{
    internal class Program
    {
        private static readonly Timer Timer = new Timer(Timer_Callback, null, Timeout.Infinite, Timeout.Infinite);

        private static void Timer_Callback(object state)
        {
            foreach (var i in MndpListener.Instance.GetMessages()) Console.WriteLine(i.Value.ToString());
            Console.WriteLine("--- Message List End ---");
        }

        private static void Main(string[] args)
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
    }
}
```

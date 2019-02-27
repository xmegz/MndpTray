using System;
using System.Threading;

namespace MndpTray.Protocol.Test
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

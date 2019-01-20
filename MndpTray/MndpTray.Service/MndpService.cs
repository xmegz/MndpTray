using MndpTray.Protocol;
using System.Reflection;
using System.ServiceProcess;

namespace MndpTray.Service
{
    public partial class MndpService : ServiceBase
    {
        public MndpService()
        {
            this.InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            EventLog.WriteEntry("Start:" + Assembly.GetEntryAssembly().ToString());
            Program.Log("------------------< START >------------------");
            MndpSender.Instance.Start(MndpHostInfo.Instance);
        }

        protected override void OnStop()
        {
            MndpSender.Instance.Stop();
            EventLog.WriteEntry("Stop:" + Assembly.GetEntryAssembly().ToString());
            Program.Log("------------------< STOP >------------------");
        }
    }
}
using MndpTray.Protocol;
using System.Reflection;
using System.ServiceProcess;

namespace MndpService
{
    public partial class MndpService : ServiceBase
    {
        public MndpService()
        {
            this.InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            this.EventLog.WriteEntry("Start:" + Assembly.GetEntryAssembly().ToString());
            Program.Log("------------------< START >------------------");
            MndpSender.Instance.Start(MndpHostInfo.Instance);
        }

        protected override void OnStop()
        {
            MndpSender.Instance.Stop();
            this.EventLog.WriteEntry("Stop:" + Assembly.GetEntryAssembly().ToString());
            Program.Log("------------------< STOP >------------------");
        }
    }
}
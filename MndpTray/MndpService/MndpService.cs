using MndpTray.Protocol;
using System.ComponentModel;
using System.Reflection;
using System.ServiceProcess;

namespace MndpService
{
    /// <summary>
    /// Mndp Service class
    /// </summary>
    [DesignerCategory("Code")]
    public class MndpService : ServiceBase
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public MndpService()
        {
            this.ServiceName = Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyTitleAttribute>().Title;
        }

        /// <summary>
        /// Start event
        /// </summary>
        /// <param name="args">Startup arguments</param>
        protected override void OnStart(string[] args)
        {
            this.EventLog.WriteEntry("Start:" + Assembly.GetEntryAssembly().ToString());
            Program.Log("------------------< START >------------------");
            MndpSender.Instance.Start(MndpHostInfo.Instance);
        }

        /// <summary>
        /// Stop event
        /// </summary>
        protected override void OnStop()
        {
            MndpSender.Instance.Stop();
            this.EventLog.WriteEntry("Stop:" + Assembly.GetEntryAssembly().ToString());
            Program.Log("------------------< STOP >------------------");
        }
    }
}
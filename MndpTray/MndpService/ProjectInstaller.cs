using System.ComponentModel;

namespace MndpService
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        public ProjectInstaller()
        {
            this.InitializeComponent();
        }
    }
}
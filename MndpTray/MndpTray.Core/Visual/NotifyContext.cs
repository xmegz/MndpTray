/*-----------------------------------------------------------------------------
 * Project:    MndpTray
 * Repository: https://github.com/xmegz/MndpTray
 * Author:     Pádár Tamás
 -----------------------------------------------------------------------------*/
namespace MndpTray.Core
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Threading;
    using System.Windows.Forms;
    using MndpTray.Protocol;

    public class NotifyContext : ApplicationContext
    {
        private readonly AboutBox _aboutBox;
        private readonly ListForm _listForm;
        private NotifyIcon _notifyIcon;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotifyContext"/> class.
        /// </summary>
        public NotifyContext()
        {
            this.InizializeComponets();
            this._listForm = new ListForm();
            this._aboutBox = new AboutBox();

            Log.SetInfoAction(Program.Log);

            MndpListener.Instance.Start();
            MndpSender.Instance.Start(MndpHostInfo.Instance);
        }

        #region Event Handlers

        private void About_Click(object sender, System.EventArgs e)
        {
            if (!this._aboutBox.Visible)
            {
                this._aboutBox.ShowDialog();
            }
            else
            {
                this._aboutBox.WindowState = FormWindowState.Normal;
            }

            this._aboutBox.BringToFront();
        }

        private void Exit_Click(object sender, EventArgs e)
        {
            MndpListener.Instance.Stop();
            MndpSender.Instance.Stop();

            this._notifyIcon.Dispose();
            this._listForm.Close();

            Thread.Sleep(100);
            Application.Exit();
        }

        private void List_Click(object sender, EventArgs e)
        {
            if (!this._listForm.Visible)
            {
                this._listForm.ShowDialog();
            }
            else
            {
                this._listForm.WindowState = FormWindowState.Normal;
            }

            this._listForm.BringToFront();
        }

        private void Send_Click(object sender, System.EventArgs e)
        {
            MndpSender.Instance.SendHostInfoNow();
        }

        private void Update_Click(object sender, System.EventArgs e)
        {
            string repositoryName = "MndpTray";
            string assetName = Assembly.GetExecutingAssembly().GetName().Name;
            Version version = Assembly.GetExecutingAssembly().GetName().Version;

            string url = Update.Methods.GetNextVersionDownloadUrl("xmegz", repositoryName, assetName, version);

            if (url != null)
            {
                var res = MessageBox.Show("New release found, would you like to update?", "Update", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (res == DialogResult.Yes)
                {
                    byte[] data = Update.Methods.DownloadBinary(url);
                    Update.Methods.UpdateProgram(Path.GetFullPath(System.AppContext.BaseDirectory), data);

                    MessageBox.Show("Update successful, please restart application!", "Update", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                MessageBox.Show("New release not found!", "Update", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        #endregion Event Handlers

        #region Init

        private void InizializeComponets()
        {
            var notifyIcon = new NotifyIcon
            {
                Icon = MndpTray.Core.Properties.Resources.favicon_ico,
                Text = nameof(MndpTray),
                Visible = true,
            };

            var contextMenuStrip = new ContextMenuStrip();

            var listMenuStrip = new ToolStripMenuItem
            {
                Text = "List",
            };
            listMenuStrip.Click += this.List_Click;
            contextMenuStrip.Items.Add(listMenuStrip);

            contextMenuStrip.Items.Add(new ToolStripSeparator());

            var sendMenuStrip = new ToolStripMenuItem
            {
                Text = "Send",
            };
            sendMenuStrip.Click += this.Send_Click;
            contextMenuStrip.Items.Add(sendMenuStrip);

            contextMenuStrip.Items.Add(new ToolStripSeparator());

            var aboutMenuStrip = new ToolStripMenuItem
            {
                Text = "About",
            };
            aboutMenuStrip.Click += this.About_Click;
            contextMenuStrip.Items.Add(aboutMenuStrip);

            contextMenuStrip.Items.Add(new ToolStripSeparator());

            var updateMenuStrip = new ToolStripMenuItem
            {
                Text = "Update",
            };
            updateMenuStrip.Click += this.Update_Click;
            contextMenuStrip.Items.Add(updateMenuStrip);

            contextMenuStrip.Items.Add(new ToolStripSeparator());

            var exitMenuStrip = new ToolStripMenuItem
            {
                Text = "Exit",
            };
            exitMenuStrip.Click += this.Exit_Click;
            contextMenuStrip.Items.Add(exitMenuStrip);

            notifyIcon.ContextMenuStrip = contextMenuStrip;

            this._notifyIcon = notifyIcon;
        }

        #endregion Init
    }
}
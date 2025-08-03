/*-----------------------------------------------------------------------------
 * Project:    MndpTray
 * Repository: https://github.com/xmegz/MndpTray
 * Author:     Pádár Tamás
 -----------------------------------------------------------------------------*/
namespace MndpTray.Core
{
    using MndpTray.Protocol;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Windows.Forms;

    /// <summary>
    /// List Form.
    /// </summary>
    public partial class ListForm : Form
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ListForm"/> class.
        /// </summary>
        public ListForm()
        {
            this.InitializeComponent();
            
            SetDoubleBuffering(this.dgvGrid);

            this.Text = string.Concat(this.Text, " Version: ", Assembly.GetEntryAssembly().GetName().Version.ToString());
        }

        #region Event Handlers

        private void DgvGrid_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var contextMenuStrip = new ContextMenuStrip();

                var pingMenuStrip = new ToolStripMenuItem
                {
                    Text = "Ping",
                };
                pingMenuStrip.Click += this.Ping_Click;
                contextMenuStrip.Items.Add(pingMenuStrip);

                var httpMenuStrip = new ToolStripMenuItem
                {
                    Text = "Http",
                };
                httpMenuStrip.Click += this.Http_Click;
                contextMenuStrip.Items.Add(httpMenuStrip);

                var sshMenuStrip = new ToolStripMenuItem
                {
                    Text = "Ssh",
                };
                sshMenuStrip.Click += this.Ssh_Click;
                contextMenuStrip.Items.Add(sshMenuStrip);

                var rdpMenuStrip = new ToolStripMenuItem
                {
                    Text = "Rdp",
                };
                rdpMenuStrip.Click += this.Rdp_Click;
                contextMenuStrip.Items.Add(rdpMenuStrip);

                var vncMenuStrip = new ToolStripMenuItem
                {
                    Text = "Vnc",
                };
                vncMenuStrip.Click += this.Vnc_Click;
                contextMenuStrip.Items.Add(vncMenuStrip);

                var winboxMenuStrip = new ToolStripMenuItem
                {
                    Text = "Winbox",
                };
                winboxMenuStrip.Click += this.Winbox_Click;
                contextMenuStrip.Items.Add(winboxMenuStrip);

                var messageMenuStrip = new ToolStripMenuItem
                {
                    Text = "Message",
                };
                messageMenuStrip.Click += this.MessageMenuStrip_Click;
                contextMenuStrip.Items.Add(messageMenuStrip);

                contextMenuStrip.Show(this, new Point(e.X, e.Y));
            }
        }

        private void MessageMenuStrip_Click(object sender, EventArgs e)
        {
            try
            {
                string ip = this.GetSelectedIpAddress();
                if (ip == null)
                    return;

                string path = GetMsgExePath();
                if (path == null)
                    return;

                MsgBoxForm form = new MsgBoxForm();

                if (form.ShowDialog() != DialogResult.OK)
                    return;


                string message = form.MsgText;

                System.Diagnostics.Process.Start(path, string.Format("/SERVER:{0} console \"{1}\"", ip, message));
            }
            catch (Exception ex)
            {
                Program.Log("Exception {0}", ex);
            }
        }

        private void Http_Click(object sender, EventArgs e)
        {
            try
            {
                string ip = this.GetSelectedIpAddress();
                if (ip == null)
                    return;

                System.Diagnostics.Process.Start(new ProcessStartInfo
                {
                    FileName = "http://" + ip,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                Program.Log("Exception {0}", ex);
            }
        }

        private void Ping_Click(object sender, EventArgs e)
        {
            this.StartProcessWithIpArgument("ping");
        }

        private void Rdp_Click(object sender, EventArgs e)
        {
            try
            {
                string ip = this.GetSelectedIpAddress();
                if (ip == null)
                    return;

                System.Diagnostics.Process.Start("mstsc", "/v:" + ip);
            }
            catch (Exception ex)
            {
                Program.Log("Exception {0}", ex);
            }
        }

        private void Receive_Timer(object sender, EventArgs e)
        {
            try
            {
                List<MndpMessageEx> listMsg = [.. MndpListener.Instance.GetMessages().Select(a => a.Value)];

                Dictionary<string, DataGridViewRow> dictRow = [];

                foreach (DataGridViewRow i in this.dgvGrid.Rows)
                {
                    if (i.Tag != null)
                        dictRow[(string)i.Tag] = i;
                }

                this.dgvGrid.SuspendLayout();

                foreach (MndpMessageEx i in listMsg)
                {
                    if (dictRow.TryGetValue(i.MacAddress, out DataGridViewRow value))
                    {
                        var row = value;

                        row.SetValues(i.UnicastAddress, i.MacAddressDelimited, i.Identity, i.Platform, i.Version, i.BoardName, i.InterfaceName, i.SoftwareId, i.Age.ToString("F0"), i.Uptime, i.UnicastIPv6Address);

                        dictRow.Remove(i.MacAddress);
                    }
                    else
                    {
                        var row = new DataGridViewRow();

                        row.CreateCells(this.dgvGrid, i.UnicastAddress, i.MacAddressDelimited, i.Identity, i.Platform, i.Version, i.BoardName, i.InterfaceName, i.SoftwareId, i.Age.ToString("F0"), i.Uptime, i.UnicastIPv6Address);
                        row.Tag = i.MacAddress;

                        this.dgvGrid.Rows.Add(row);
                    }
                }

                foreach (DataGridViewRow i in dictRow.Values)
                {
                    this.dgvGrid.Rows.Remove(i);
                }

                this.dgvGrid.ResumeLayout();
            }
            catch (Exception ex)
            {
                Program.Log("{0}, {1} Exception:{2}{3}", nameof(ListForm), nameof(this.Receive_Timer), Environment.NewLine, ex.ToString());
            }
        }

        private void Ssh_Click(object sender, EventArgs e)
        {
            string name = Environment.ExpandEnvironmentVariables(@"c:\Windows\System32\OpenSSH\ssh.exe");

            this.StartProcessWithIpArgument(name);
        }

        private void Vnc_Click(object sender, EventArgs e)
        {
            this.StartProcessWithIpArgument("tvnviewer");
        }

        private void Winbox_Click(object sender, EventArgs e)
        {
            this.StartProcessWithIpArgument("winbox");
        }

        #endregion Event Handlers

        #region Methods

        private static string GetMsgExePath()
        {
            string[] paths =
            [
                Environment.ExpandEnvironmentVariables(@"%windir%\system32\msg.exe"),
                Environment.ExpandEnvironmentVariables(@"%windir%\sysnative\msg.exe"),
            ];

            foreach (string i in paths)
            {
                if (File.Exists(i))
                    return i;
            }

            return null;
        }

        private string GetSelectedIpAddress()
        {
            if (this.dgvGrid.SelectedRows.Count > 0)
                return this.dgvGrid.SelectedRows[0].Cells[0].Value as string;

            return null;
        }

        private static void SetDoubleBuffering(DataGridView dataGridView, bool value = true)
        {
            Type type = dataGridView.GetType();
            PropertyInfo pi = type.GetProperty(
                "DoubleBuffered",
                BindingFlags.Instance | BindingFlags.NonPublic);
            pi.SetValue(dataGridView, value, null);
        }

        private void StartProcessWithIpArgument(string name)
        {
            try
            {
                string ip = this.GetSelectedIpAddress();

                if (ip == null)
                {
                    return;
                }

                try
                {
                    System.Diagnostics.Process.Start(name, ip);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + ":" + name);
                }
            }
            catch (Exception ex)
            {
                Program.Log("Exception {0}", ex);
            }
        }

        #endregion Methods
    }
}
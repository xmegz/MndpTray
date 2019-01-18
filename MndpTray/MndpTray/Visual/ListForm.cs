using MndpTray.Protocol;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace MndpTray
{
    public partial class ListForm : Form
    {
        public ListForm()
        {
            this.InitializeComponent();
            this.SetDoubleBuffering(this.dgvGrid);

            this.Text = String.Concat(this.Text, " Version: ", Assembly.GetEntryAssembly().GetName().Version.ToString());
        }

        private void DgvGrid_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var contextMenuStrip = new ContextMenuStrip();

                var pingMenuStrip = new ToolStripMenuItem();
                pingMenuStrip.Text = "Ping";
                pingMenuStrip.Click += this.Ping_Click;
                contextMenuStrip.Items.Add(pingMenuStrip);

                var httpMenuStrip = new ToolStripMenuItem();
                httpMenuStrip.Text = "Http";
                httpMenuStrip.Click += this.Http_Click;
                contextMenuStrip.Items.Add(httpMenuStrip);

                var sshMenuStrip = new ToolStripMenuItem();
                sshMenuStrip.Text = "Ssh";
                sshMenuStrip.Click += this.Ssh_Click;
                contextMenuStrip.Items.Add(sshMenuStrip);

                var rdpMenuStrip = new ToolStripMenuItem();
                rdpMenuStrip.Text = "Rdp";
                rdpMenuStrip.Click += this.Rdp_Click;
                contextMenuStrip.Items.Add(rdpMenuStrip);

                var vncMenuStrip = new ToolStripMenuItem();
                vncMenuStrip.Text = "Vnc";
                vncMenuStrip.Click += this.Vnc_Click;
                contextMenuStrip.Items.Add(vncMenuStrip);

                contextMenuStrip.Show(this, new Point(e.X, e.Y));
            }
        }

        private string GetSelectedIpAddress()
        {
            if (this.dgvGrid.SelectedRows.Count > 0)
            {
                return this.dgvGrid.SelectedRows[0].Cells[0].Value as string;
            }

            return null;
        }

        private void Http_Click(object sender, EventArgs e)
        {
            try
            {
                string ip = this.GetSelectedIpAddress();
                if (ip == null) return;
                System.Diagnostics.Process.Start("http://" + ip);
            }
            catch (Exception ex)
            {
                Program.Log("Exception {0}", ex);
            }
        }

        private void Ping_Click(object sender, EventArgs e)
        {
            try
            {
                string ip = this.GetSelectedIpAddress();
                if (ip == null) return;
                System.Diagnostics.Process.Start("ping", ip);
            }
            catch (Exception ex)
            {
                Program.Log("Exception {0}", ex);
            }
        }

        private void Rdp_Click(object sender, EventArgs e)
        {
            try
            {
                string ip = this.GetSelectedIpAddress();
                if (ip == null) return;
                System.Diagnostics.Process.Start("mstsc", "/v:" + ip);
            }
            catch (Exception ex)
            {
                Program.Log("Exception {0}", ex);
            }
        }

        private void Vnc_Click(object sender, EventArgs e)
        {
            try
            {
                string ip = this.GetSelectedIpAddress();
                if (ip == null) return;
                System.Diagnostics.Process.Start("tvnviewer", ip);
            }
            catch (Exception ex)
            {
                Program.Log("Exception {0}", ex);
            }
        }

        private void Ssh_Click(object sender, EventArgs e)
        {
            try
            {
                string ip = this.GetSelectedIpAddress();
                if (ip == null) return;
                System.Diagnostics.Process.Start("putty", ip);
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
                List<MndpMessageEx> listMsg = MndpListener.Instance.GetMessages().Select(a => a.Value).ToList();

                Dictionary<string, DataGridViewRow> dictRow = new Dictionary<string, DataGridViewRow>();

                foreach (DataGridViewRow i in this.dgvGrid.Rows)
                {
                    if (i.Tag != null)
                        dictRow[(string)i.Tag] = i;
                }

                this.dgvGrid.SuspendLayout();

                foreach (MndpMessageEx i in listMsg)
                {
                    if (dictRow.ContainsKey(i.MacAddress))
                    {
                        var row = dictRow[i.MacAddress];
                        row.SetValues(i.UnicastAddress, i.MacAddressDelimited, i.Identity, i.Platform, i.Version, i.BoardName, i.InterfaceName, i.SoftwareId, i.Age.ToString("F0"), i.Uptime);
                        dictRow.Remove(i.MacAddress);
                    }
                    else
                    {
                        var row = new DataGridViewRow();

                        row.CreateCells(this.dgvGrid, i.UnicastAddress, i.MacAddressDelimited, i.Identity, i.Platform, i.Version, i.BoardName, i.InterfaceName, i.SoftwareId, i.Age.ToString("F0"), i.Uptime);
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
                Program.Log("{0}, {1} Exception:{2}{3}", nameof(ListForm), nameof(Receive_Timer), Environment.NewLine, ex.ToString());
            }
        }

        private void SetDoubleBuffering(DataGridView dataGridView, bool value = true)
        {
            Type type = dataGridView.GetType();
            PropertyInfo pi = type.GetProperty("DoubleBuffered",
                    BindingFlags.Instance | BindingFlags.NonPublic);
            pi.SetValue(dataGridView, value, null);
        }
    }
}
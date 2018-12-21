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

        private void Ping_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("ping","127.0.0.1");
            }
            catch (Exception ex)
            {
                Program.Log("Exception {0}", ex);
            }
        }

        private void SetDoubleBuffering(DataGridView dataGridView, bool value = true)
        {
            Type type = dataGridView.GetType();
            PropertyInfo pi = type.GetProperty("DoubleBuffered",
                    BindingFlags.Instance | BindingFlags.NonPublic);
            pi.SetValue(dataGridView, value, null);
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
                Program.Log("{0}, {1} Exception:{2}{3}", nameof(ListForm), nameof(Receive_Timer),Environment.NewLine, ex.ToString());             
            }
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

                contextMenuStrip.Show(this,new Point(e.X, e.Y));

            }
        }
    }
}
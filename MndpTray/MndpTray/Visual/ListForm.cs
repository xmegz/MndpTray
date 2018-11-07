using MndpTray.Protocol;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace MndpTray
{
    public partial class ListForm : Form
    {
        public ListForm()
        {
            InitializeComponent();
            this.Text = String.Concat(this.Text, " Version: ", Assembly.GetEntryAssembly().GetName().Version.ToString());            
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
                        row.SetValues(i.SenderAddress, i.MacAddress, i.Identity, i.Platform, i.Version, i.BoardName, i.InterfaceName, i.SoftwareId, i.Age.ToString("F0"), i.Uptime);
                        dictRow.Remove(i.MacAddress);
                    }
                    else
                    {
                        var row = new DataGridViewRow();

                        row.CreateCells(this.dgvGrid, i.SenderAddress, i.MacAddress, i.Identity, i.Platform, i.Version, i.BoardName, i.InterfaceName, i.SoftwareId, i.Age.ToString("F0"), i.Uptime);
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
                Debug.Exception(nameof(ListForm), nameof(Receive_Timer), ex);
            }
        }
    }
}
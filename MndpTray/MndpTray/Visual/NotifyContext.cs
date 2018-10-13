using System;
using System.Windows.Forms;

namespace MndpTray
{
    public class NotifyContext : ApplicationContext
    {
        private ListForm _listForm;
        private NotifyIcon _notifyIcon;

        public NotifyContext()
        {
            this.InizializeComponets();
            this._listForm = new ListForm();
            MndpListener.Instance.Start();
            MndpHost.SendMessage();
        }

        #region Event Handlers

        private void Send_Click(object sender, System.EventArgs e)
        {
            MndpHost.SendMessage();
        }

        private void Exit_Click(object sender, EventArgs e)
        {
            MndpListener.Instance.Stop();
            this._notifyIcon.Dispose();
            this._listForm.Close();
            this.ExitThread();
        }

        private void List_Click(object sender, EventArgs e)
        {
            this._listForm.ShowDialog();
        }

        #endregion Event Handlers

        #region Init

        private void InizializeComponets()
        {
            this._notifyIcon = new NotifyIcon();
            this._notifyIcon.Icon = MndpTray.Properties.Resources.favicon;
            this._notifyIcon.Text = nameof(MndpTray);
            this._notifyIcon.Visible = true;

            var contextMenuStrip = new ContextMenuStrip();

            var listMenuStrip = new ToolStripMenuItem();
            listMenuStrip.Text = "List";
            listMenuStrip.Click += List_Click;
            contextMenuStrip.Items.Add(listMenuStrip);

            contextMenuStrip.Items.Add(new ToolStripSeparator());

            var sendMenuStrip = new ToolStripMenuItem();
            sendMenuStrip.Text = "Send";
            sendMenuStrip.Click += Send_Click;
            contextMenuStrip.Items.Add(sendMenuStrip);

            contextMenuStrip.Items.Add(new ToolStripSeparator());

            var exitMenuStrip = new ToolStripMenuItem();
            exitMenuStrip.Text = "Exit";
            exitMenuStrip.Click += Exit_Click;
            contextMenuStrip.Items.Add(exitMenuStrip);

            _notifyIcon.ContextMenuStrip = contextMenuStrip;
        }

        #endregion Init
    }
}
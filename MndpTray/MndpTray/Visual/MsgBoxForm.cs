using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MndpTray
{
    public partial class MsgBoxForm : Form
    {
        public MsgBoxForm()
        {
            InitializeComponent();
            this.tbMessage.Focus();
        }

        public string MsgText {get { return this.tbMessage.Text; } }

        private void btSend_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }
    }
}

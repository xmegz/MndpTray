/*-----------------------------------------------------------------------------
 * Project:    MndpTray
 * Repository: https://github.com/xmegz/MndpTray
 * Author:     Pádár Tamás
 -----------------------------------------------------------------------------*/
namespace MndpTray.Core
{
    using System;
    using System.Windows.Forms;

    /// <summary>
    /// Message Box.
    /// </summary>
    public partial class MsgBoxForm : Form
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MsgBoxForm"/> class.
        /// </summary>
        public MsgBoxForm()
        {
            this.InitializeComponent();
            this.tbMessage.Focus();
        }

        /// <summary>
        /// Gets message text.
        /// </summary>
        public string MsgText
        {
            get { return this.tbMessage.Text; }
        }

        private void BtSend_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }
    }
}

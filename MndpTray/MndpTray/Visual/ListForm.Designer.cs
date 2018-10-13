namespace MndpTray
{
    partial class ListForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.ReceiveTimer = new System.Windows.Forms.Timer(this.components);
            this.dgvGrid = new System.Windows.Forms.DataGridView();
            this.IpAddress = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MacAddress = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Identity = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Version = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Platform = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Uptime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SoftwareId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BoardName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.InterfaceName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SendTimer = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.dgvGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // ReceiveTimer
            // 
            this.ReceiveTimer.Enabled = true;
            this.ReceiveTimer.Interval = 5000;
            this.ReceiveTimer.Tick += new System.EventHandler(this.Receive_Timer);
            // 
            // dgvGrid
            // 
            this.dgvGrid.AllowUserToAddRows = false;
            this.dgvGrid.AllowUserToDeleteRows = false;
            this.dgvGrid.AllowUserToResizeRows = false;
            this.dgvGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.IpAddress,
            this.MacAddress,
            this.Identity,
            this.Version,
            this.Platform,
            this.Uptime,
            this.SoftwareId,
            this.BoardName,
            this.InterfaceName});
            this.dgvGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvGrid.Location = new System.Drawing.Point(0, 0);
            this.dgvGrid.Name = "dgvGrid";
            this.dgvGrid.ReadOnly = true;
            this.dgvGrid.RowHeadersVisible = false;
            this.dgvGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvGrid.ShowEditingIcon = false;
            this.dgvGrid.Size = new System.Drawing.Size(903, 478);
            this.dgvGrid.TabIndex = 0;
            // 
            // IpAddress
            // 
            this.IpAddress.HeaderText = "IpAddress";
            this.IpAddress.Name = "IpAddress";
            this.IpAddress.ReadOnly = true;
            // 
            // MacAddress
            // 
            this.MacAddress.HeaderText = "MacAddress";
            this.MacAddress.Name = "MacAddress";
            this.MacAddress.ReadOnly = true;
            // 
            // Identity
            // 
            this.Identity.HeaderText = "Identity";
            this.Identity.Name = "Identity";
            this.Identity.ReadOnly = true;
            // 
            // Version
            // 
            this.Version.HeaderText = "Version";
            this.Version.Name = "Version";
            this.Version.ReadOnly = true;
            // 
            // Platform
            // 
            this.Platform.HeaderText = "Platform";
            this.Platform.Name = "Platform";
            this.Platform.ReadOnly = true;
            // 
            // Uptime
            // 
            this.Uptime.HeaderText = "Uptime";
            this.Uptime.Name = "Uptime";
            this.Uptime.ReadOnly = true;
            // 
            // SoftwareId
            // 
            this.SoftwareId.HeaderText = "SoftwareId";
            this.SoftwareId.Name = "SoftwareId";
            this.SoftwareId.ReadOnly = true;
            // 
            // BoardName
            // 
            this.BoardName.HeaderText = "BoardName";
            this.BoardName.Name = "BoardName";
            this.BoardName.ReadOnly = true;
            // 
            // InterfaceName
            // 
            this.InterfaceName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.InterfaceName.HeaderText = "InterfaceName";
            this.InterfaceName.Name = "InterfaceName";
            this.InterfaceName.ReadOnly = true;
            // 
            // SendTimer
            // 
            this.SendTimer.Enabled = true;
            this.SendTimer.Interval = 60000;
            this.SendTimer.Tick += new System.EventHandler(this.SendTimer_Tick);
            // 
            // ListForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(903, 478);
            this.Controls.Add(this.dgvGrid);
            this.Name = "ListForm";
            this.Text = "Neighbor List";
            ((System.ComponentModel.ISupportInitialize)(this.dgvGrid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.DataGridView dgvGrid;
        private System.Windows.Forms.DataGridViewTextBoxColumn IpAddress;
        private System.Windows.Forms.DataGridViewTextBoxColumn MacAddress;
        private System.Windows.Forms.DataGridViewTextBoxColumn Identity;
        private System.Windows.Forms.DataGridViewTextBoxColumn Version;
        private System.Windows.Forms.DataGridViewTextBoxColumn Platform;
        private System.Windows.Forms.DataGridViewTextBoxColumn Uptime;
        private System.Windows.Forms.DataGridViewTextBoxColumn SoftwareId;
        private System.Windows.Forms.DataGridViewTextBoxColumn BoardName;
        private System.Windows.Forms.DataGridViewTextBoxColumn InterfaceName;
        public System.Windows.Forms.Timer SendTimer;
        public System.Windows.Forms.Timer ReceiveTimer;
    }
}
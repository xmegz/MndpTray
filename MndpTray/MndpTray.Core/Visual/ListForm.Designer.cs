namespace MndpTray.Core
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ListForm));
            this.ReceiveTimer = new System.Windows.Forms.Timer(this.components);
            this.dgvGrid = new System.Windows.Forms.DataGridView();
            this.IpAddress = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MacAddress = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Identity = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Platform = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Version = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BoardName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.InterfaceName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SoftwareId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Age = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Uptime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.IPv6Address = new System.Windows.Forms.DataGridViewTextBoxColumn();
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
            this.Platform,
            this.Version,
            this.BoardName,
            this.InterfaceName,
            this.SoftwareId,
            this.Age,
            this.Uptime,
            this.IPv6Address});
            this.dgvGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvGrid.Location = new System.Drawing.Point(0, 0);
            this.dgvGrid.Margin = new System.Windows.Forms.Padding(6);
            this.dgvGrid.Name = "dgvGrid";
            this.dgvGrid.ReadOnly = true;
            this.dgvGrid.RowHeadersVisible = false;
            this.dgvGrid.RowHeadersWidth = 82;
            this.dgvGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvGrid.ShowEditingIcon = false;
            this.dgvGrid.Size = new System.Drawing.Size(1850, 697);
            this.dgvGrid.TabIndex = 0;
            this.dgvGrid.MouseDown += new System.Windows.Forms.MouseEventHandler(this.DgvGrid_MouseDown);
            // 
            // IpAddress
            // 
            this.IpAddress.HeaderText = "IP Address";
            this.IpAddress.MinimumWidth = 10;
            this.IpAddress.Name = "IpAddress";
            this.IpAddress.ReadOnly = true;
            this.IpAddress.Width = 200;
            // 
            // MacAddress
            // 
            this.MacAddress.HeaderText = "MAC Address";
            this.MacAddress.MinimumWidth = 10;
            this.MacAddress.Name = "MacAddress";
            this.MacAddress.ReadOnly = true;
            this.MacAddress.Width = 200;
            // 
            // Identity
            // 
            this.Identity.HeaderText = "Identity";
            this.Identity.MinimumWidth = 10;
            this.Identity.Name = "Identity";
            this.Identity.ReadOnly = true;
            this.Identity.Width = 200;
            // 
            // Platform
            // 
            this.Platform.HeaderText = "Platform";
            this.Platform.MinimumWidth = 10;
            this.Platform.Name = "Platform";
            this.Platform.ReadOnly = true;
            this.Platform.Width = 200;
            // 
            // Version
            // 
            this.Version.HeaderText = "Version";
            this.Version.MinimumWidth = 10;
            this.Version.Name = "Version";
            this.Version.ReadOnly = true;
            this.Version.Width = 200;
            // 
            // BoardName
            // 
            this.BoardName.HeaderText = "Board Name";
            this.BoardName.MinimumWidth = 10;
            this.BoardName.Name = "BoardName";
            this.BoardName.ReadOnly = true;
            this.BoardName.Width = 200;
            // 
            // InterfaceName
            // 
            this.InterfaceName.HeaderText = "Interface Name";
            this.InterfaceName.MinimumWidth = 10;
            this.InterfaceName.Name = "InterfaceName";
            this.InterfaceName.ReadOnly = true;
            this.InterfaceName.Width = 125;
            // 
            // SoftwareId
            // 
            this.SoftwareId.HeaderText = "Software ID";
            this.SoftwareId.MinimumWidth = 10;
            this.SoftwareId.Name = "SoftwareId";
            this.SoftwareId.ReadOnly = true;
            this.SoftwareId.Width = 200;
            // 
            // Age
            // 
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.Age.DefaultCellStyle = dataGridViewCellStyle1;
            this.Age.FillWeight = 75F;
            this.Age.HeaderText = "Age (s)";
            this.Age.MinimumWidth = 10;
            this.Age.Name = "Age";
            this.Age.ReadOnly = true;
            this.Age.Width = 75;
            // 
            // Uptime
            // 
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.Uptime.DefaultCellStyle = dataGridViewCellStyle2;
            this.Uptime.HeaderText = "Uptime";
            this.Uptime.MinimumWidth = 10;
            this.Uptime.Name = "Uptime";
            this.Uptime.ReadOnly = true;
            this.Uptime.Width = 10;
            // 
            // IPv6Address
            // 
            this.IPv6Address.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.IPv6Address.HeaderText = "IPv6";
            this.IPv6Address.MinimumWidth = 10;
            this.IPv6Address.Name = "IPv6Address";
            this.IPv6Address.ReadOnly = true;
            // 
            // ListForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1850, 697);
            this.Controls.Add(this.dgvGrid);
            this.Font = new System.Drawing.Font("Segoe UI", 7.875F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(6);
            this.Name = "ListForm";
            this.Text = "MndpTray - Neighbor List";
            ((System.ComponentModel.ISupportInitialize)(this.dgvGrid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.DataGridView dgvGrid;
        public System.Windows.Forms.Timer ReceiveTimer;
        private System.Windows.Forms.DataGridViewTextBoxColumn IpAddress;
        private System.Windows.Forms.DataGridViewTextBoxColumn MacAddress;
        private System.Windows.Forms.DataGridViewTextBoxColumn Identity;
        private System.Windows.Forms.DataGridViewTextBoxColumn Platform;
        private System.Windows.Forms.DataGridViewTextBoxColumn Version;
        private System.Windows.Forms.DataGridViewTextBoxColumn BoardName;
        private System.Windows.Forms.DataGridViewTextBoxColumn InterfaceName;
        private System.Windows.Forms.DataGridViewTextBoxColumn SoftwareId;
        private System.Windows.Forms.DataGridViewTextBoxColumn Age;
        private System.Windows.Forms.DataGridViewTextBoxColumn Uptime;
        private System.Windows.Forms.DataGridViewTextBoxColumn IPv6Address;
    }
}
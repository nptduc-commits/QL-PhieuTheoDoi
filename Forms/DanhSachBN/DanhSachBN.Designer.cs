namespace project.Forms.DanhSachBN
{
    partial class DanhSachBN
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
            this.txtHoTenBN = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnXemChiTiet = new System.Windows.Forms.Button();
            this.btnTimKiem = new System.Windows.Forms.Button();
            this.dataGridViewDanhSachBN = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewDanhSachBN)).BeginInit();
            this.SuspendLayout();
            // 
            // txtHoTenBN
            // 
            this.txtHoTenBN.Location = new System.Drawing.Point(124, 17);
            this.txtHoTenBN.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtHoTenBN.Name = "txtHoTenBN";
            this.txtHoTenBN.Size = new System.Drawing.Size(279, 22);
            this.txtHoTenBN.TabIndex = 4;
            this.txtHoTenBN.TextChanged += new System.EventHandler(this.txtHoTenBN_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(39, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 16);
            this.label1.TabIndex = 3;
            this.label1.Text = "Họ Tên BN :";
            // 
            // btnXemChiTiet
            // 
            this.btnXemChiTiet.Location = new System.Drawing.Point(213, 110);
            this.btnXemChiTiet.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnXemChiTiet.Name = "btnXemChiTiet";
            this.btnXemChiTiet.Size = new System.Drawing.Size(143, 26);
            this.btnXemChiTiet.TabIndex = 7;
            this.btnXemChiTiet.Text = "Xem Chi Tiết";
            this.btnXemChiTiet.UseVisualStyleBackColor = true;
            this.btnXemChiTiet.Click += new System.EventHandler(this.btnXemChiTiet_Click);
            // 
            // btnTimKiem
            // 
            this.btnTimKiem.Location = new System.Drawing.Point(44, 110);
            this.btnTimKiem.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnTimKiem.Name = "btnTimKiem";
            this.btnTimKiem.Size = new System.Drawing.Size(75, 26);
            this.btnTimKiem.TabIndex = 6;
            this.btnTimKiem.Text = "Tìm Kiếm";
            this.btnTimKiem.UseVisualStyleBackColor = true;
            this.btnTimKiem.Click += new System.EventHandler(this.btnTimKiem_Click);
            // 
            // dataGridViewDanhSachBN
            // 
            this.dataGridViewDanhSachBN.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewDanhSachBN.Location = new System.Drawing.Point(41, 154);
            this.dataGridViewDanhSachBN.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.dataGridViewDanhSachBN.Name = "dataGridViewDanhSachBN";
            this.dataGridViewDanhSachBN.RowHeadersWidth = 51;
            this.dataGridViewDanhSachBN.RowTemplate.Height = 24;
            this.dataGridViewDanhSachBN.Size = new System.Drawing.Size(675, 354);
            this.dataGridViewDanhSachBN.TabIndex = 5;
            this.dataGridViewDanhSachBN.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewDanhSachBN_CellContentClick_1);
            // 
            // DanhSachBN
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1169, 620);
            this.Controls.Add(this.btnXemChiTiet);
            this.Controls.Add(this.btnTimKiem);
            this.Controls.Add(this.dataGridViewDanhSachBN);
            this.Controls.Add(this.txtHoTenBN);
            this.Controls.Add(this.label1);
            this.Name = "DanhSachBN";
            this.Text = "DanhSachBN";
            this.Load += new System.EventHandler(this.DanhSachBN_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewDanhSachBN)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtHoTenBN;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnXemChiTiet;
        private System.Windows.Forms.Button btnTimKiem;
        private System.Windows.Forms.DataGridView dataGridViewDanhSachBN;
    }
}
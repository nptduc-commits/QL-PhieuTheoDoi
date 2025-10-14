using project.Forms; // Thêm dòng này nếu DanhSachBN ở trong thư mục Forms
using project.Forms.DanhSachBN;
using System;
using System.Windows.Forms;

namespace project
{
    public partial class BVUB : Form
    {
        public BVUB()
        {
            InitializeComponent();
        }

        private void themmoi_Click(object sender, EventArgs e)
        {
        }

        private void DanhSachBN_Click(object sender, EventArgs e)
        {
            // Tạo một thể hiện của form DanhSachBN
            DanhSachBN frm = new DanhSachBN();

            // Hiển thị form DanhSachBN
            frm.Show();

            // Ẩn form hiện tại (BVUB)
            this.Hide();
        }
    }
}
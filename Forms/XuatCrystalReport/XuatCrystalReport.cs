using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CrystalDecisions.CrystalReports.Engine;
using System.Data;

namespace project.Forms.XuatCrystalReport
{
    public partial class XuatCrystalReport : Form
    {
        public XuatCrystalReport()
        {
            InitializeComponent();
        }
        private void XuatCrystalReport_Load(object sender, EventArgs e)
        {
            // --- PHẦN NÀY BẠN CẦN CHUẨN BỊ DỮ LIỆU ---
            // Ví dụ: Lấy dữ liệu từ một DataTable có tên là dt.
            // DataTable dt = new DataTable();
            // dt = ... // Code lấy dữ liệu từ database của bạn đổ vào đây.
            //---------------------------------------------


            // 1. Tạo một đối tượng của báo cáo bạn đã thiết kế
            // Giả sử file report của bạn tên là CrystalReport1.rpt
            CrystalReport1 report = new CrystalReport1();

            // 2. NẠP DỮ LIỆU VÀO BÁO CÁO (CỰC KỲ QUAN TRỌNG)
            // Nếu không có bước này, báo cáo sẽ trống không.
            // report.SetDataSource(dt); // dt là DataTable bạn đã chuẩn bị ở trên

            // 3. Gán báo cáo đã có dữ liệu cho Viewer để hiển thị
            crystalReportViewer1.ReportSource = report;
            crystalReportViewer1.Refresh(); // Làm mới viewer để hiển thị báo cáo
        }

        private void crystalReportViewer1_Load(object sender, EventArgs e)
        {

        }
    }
}

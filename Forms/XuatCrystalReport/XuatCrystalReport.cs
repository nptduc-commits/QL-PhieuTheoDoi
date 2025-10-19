// Trong file: XuatCrystalReport.cs

using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using CrystalDecisions.CrystalReports.Engine;

namespace project.Forms.XuatCrystalReport
{
    public partial class XuatCrystalReport : Form
    {
        // Thêm thuộc tính này. Nó sẽ hoạt động như một "cửa sổ"
        // để nhận mã bệnh nhân từ form ChamSocVaTheoDoi.
        public string MaNguoiBenh { get; set; }

        private const string connectionString = "Data Source=DESKTOP-S2SMBM8\\SQLEXPRESS03;Initial Catalog=project;Integrated Security=True";

        public XuatCrystalReport()
        {
            InitializeComponent();
        }

        private void XuatCrystalReport_Load(object sender, EventArgs e)
        {
            // Khi form được tải, nó sẽ dùng "MaNguoiBenh" đã được gán ở Bước 1
            // để truy vấn đúng dữ liệu.
            if (string.IsNullOrEmpty(MaNguoiBenh))
            {
                MessageBox.Show("Chưa có mã người bệnh để xuất báo cáo.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                return;
            }

            try
            {
                // Câu lệnh SQL lấy thông tin chi tiết của bệnh nhân và phiếu chăm sóc
                string query = @"
                    SELECT 
                        pcs.IDPhieu,
                        pcs.MaNguoiBenh,
                        pcs.ThoiGian,
                        pcs.MaKhoa,
                        pcs.MaGiuong,
                        pcs.ToSo,
                        pcs.TongNhap,
                        pcs.TongXuat,
                        pcs.PhanCapChamSoc,
                        bn.HoTen,
                        bn.Tuoi,
                        bn.GioiTinh,
                        bn.SoVaoVien,
                        bn.TienSuDiUng,
                        bn.GhiNhanDiUng,
                        bn.ChanDoan,
                        k.TenKhoa,
                        p.TenPhong,
                        g.TenGiuong
                    FROM PhieuChamSoc pcs
                    INNER JOIN BenhNhan bn ON pcs.MaNguoiBenh = bn.MaNguoiBenh
                    LEFT JOIN Khoa k ON pcs.MaKhoa = k.MaKhoa
                    LEFT JOIN Giuong g ON pcs.MaGiuong = g.MaGiuong
                    LEFT JOIN Phong p ON g.MaPhong = p.MaPhong
                    WHERE pcs.MaNguoiBenh = @MaNguoiBenh
                    ORDER BY pcs.ThoiGian DESC";

                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@MaNguoiBenh", this.MaNguoiBenh);

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }

                // Kiểm tra xem có dữ liệu không
                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("Không tìm thấy dữ liệu cho bệnh nhân có mã: " + this.MaNguoiBenh, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    this.Close();
                    return;
                }

                // Nạp dữ liệu vào report và hiển thị
                project.CrystalReport1 report = new CrystalReport1();
                report.SetDataSource(dt);
                crystalReportViewer1.ReportSource = report;
                crystalReportViewer1.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải báo cáo: " + ex.Message);
            }
        }
    }
}
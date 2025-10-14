using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace project.Forms.DanhSachBN
{
    public partial class DanhSachBN : Form
    {
        private const string connectionString = "Data Source=DESKTOP-S2SMBM8\\SQLEXPRESS03;Initial Catalog=project;Integrated Security=True";
        public DanhSachBN()
        {
            InitializeComponent();
            // Thiết lập chế độ chọn cả hàng để đảm bảo logic hoạt động đúng
            this.dataGridViewDanhSachBN.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }

        private void DanhSachBN_Load(object sender, EventArgs e)
        {
            LoadData();
            dataGridViewDanhSachBN.ClearSelection();
        }

        private void LoadData(string hoTenCanTim = "")
        {
            DataTable dataTable = new DataTable();
            string query = @"
SELECT
    pcs.IDPhieu, 
    pcs.MaNguoiBenh,
    bn.HoTen,
    bn.Tuoi,
    bn.GioiTinh,
    k.TenKhoa,
    pcs.ThoiGian
FROM
    dbo.PhieuChamSoc AS pcs
INNER JOIN
    dbo.BenhNhan AS bn ON pcs.MaNguoiBenh = bn.MaNguoiBenh
LEFT JOIN 
    dbo.Khoa AS k ON pcs.MaKhoa = k.MaKhoa";

            // SỬA LỖI: Dùng LIKE thay vì CONTAINS
            if (!string.IsNullOrWhiteSpace(hoTenCanTim))
            {
                query += " WHERE bn.HoTen LIKE @hoTen";
            }

            query += " ORDER BY pcs.ThoiGian DESC;";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        if (!string.IsNullOrWhiteSpace(hoTenCanTim))
                        {
                            // SỬA LỖI: Định dạng tham số cho LIKE
                            // Thêm ký tự '%' để tìm kiếm bất kỳ chuỗi nào chứa hoTenCanTim
                            command.Parameters.AddWithValue("@hoTen", "%" + hoTenCanTim + "%");
                        }

                        connection.Open();
                        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                        {
                            adapter.Fill(dataTable);
                        }
                    }
                }

                dataGridViewDanhSachBN.DataSource = dataTable;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi khi tải dữ liệu: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataGridViewDanhSachBN_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                try
                {
                    DataGridViewRow row = this.dataGridViewDanhSachBN.Rows[e.RowIndex];
                    object hoTenValue = row.Cells["HoTen"].Value;

                    if (hoTenValue != null)
                    {
                        // Convert an toàn từ bất kỳ kiểu dữ liệu nào sang string
                        txtHoTenBN.Text = hoTenValue.ToString();
                    }
                }
                catch (Exception ex)
                {
                    // Xử lý lỗi một cách im lặng hoặc log nếu cần
                    System.Diagnostics.Debug.WriteLine("Lỗi khi lấy dữ liệu từ DataGridView: " + ex.Message);
                }
            }
        }

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            LoadData(txtHoTenBN.Text.Trim());
        }

        private void txtHoTenBN_TextChanged(object sender, EventArgs e)
        {

        }

        private void dataGridViewDanhSachBN_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                try
                {
                    DataGridViewRow row = this.dataGridViewDanhSachBN.Rows[e.RowIndex];
                    object hoTenValue = row.Cells["HoTen"].Value;

                    if (hoTenValue != null)
                    {
                        // Convert an toàn từ bất kỳ kiểu dữ liệu nào sang string
                        txtHoTenBN.Text = hoTenValue.ToString();
                    }
                }
                catch (Exception ex)
                {
                    // Xử lý lỗi một cách im lặng hoặc log nếu cần
                    System.Diagnostics.Debug.WriteLine("Lỗi khi lấy dữ liệu từ DataGridView: " + ex.Message);
                }
            }
        }

        private void btnXemChiTiet_Click(object sender, EventArgs e)
        {
            if (dataGridViewDanhSachBN.SelectedRows.Count > 0)
            {
                try
                {
                    DataGridViewRow selectedRow = dataGridViewDanhSachBN.SelectedRows[0];

                    // Sửa lỗi casting: kiểm tra null và convert an toàn
                    object maNguoiBenhValue = selectedRow.Cells["MaNguoiBenh"].Value;
                    string maNguoiBenh = "";

                    if (maNguoiBenhValue != null)
                    {
                        // Convert an toàn từ bất kỳ kiểu dữ liệu nào sang string
                        maNguoiBenh = maNguoiBenhValue.ToString();
                    }
                    else
                    {
                        MessageBox.Show("Không thể lấy mã bệnh nhân từ dữ liệu được chọn.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // Kiểm tra mã bệnh nhân có hợp lệ không
                    if (string.IsNullOrEmpty(maNguoiBenh))
                    {
                        MessageBox.Show("Mã bệnh nhân không hợp lệ.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    // Tạo form chi tiết với mã bệnh nhân đã được xử lý an toàn
                    ChamSocVaTheoDoi formChiTiet = new ChamSocVaTheoDoi(maNguoiBenh);
                    formChiTiet.ShowDialog();
                }
                catch (Exception ex)
                {
                    // Hiển thị thông báo lỗi chi tiết hơn
                    MessageBox.Show("Đã xảy ra lỗi khi tải dữ liệu chi tiết: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một bệnh nhân để xem chi tiết.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}

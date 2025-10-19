using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;
using CrystalDecisions.Shared; // Thêm using này
using CrystalDecisions.CrystalReports.Engine;



// Gợi ý: Namespace nên là project.Forms để hợp lý hơn
namespace project.Forms.DanhSachBN
{
    public partial class ChamSocVaTheoDoi : Form
    {
        // Thay đổi chuỗi kết nối cho phù hợp với môi trường của bạn
        private const string connectionString = "Data Source=DESKTOP-S2SMBM8\\SQLEXPRESS03;Initial Catalog=project;Integrated Security=True";
        private readonly string _maNguoiBenh;

        public ChamSocVaTheoDoi(string maNguoiBenh)
        {
            InitializeComponent();
            _maNguoiBenh = maNguoiBenh;
        }

        private void ChamSocVaTheoDoi_Load(object sender, EventArgs e)
        {
            // Kiểm tra kết nối trước khi thực hiện các thao tác khác
            if (!TestConnection())
            {
                MessageBox.Show("Không thể kết nối đến cơ sở dữ liệu. Vui lòng kiểm tra lại chuỗi kết nối hoặc trạng thái SQL Server.", "Lỗi kết nối", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close(); // Đóng form nếu không kết nối được
                return;
            }

            // Tải tất cả thông tin cần thiết khi form được mở
            LoadThongTinBenhNhan();
            LoadAllDanhGiaData();
        }

        /// <summary>
        /// Tải tất cả dữ liệu đánh giá và hiển thị lên form.
        /// </summary>
        private void LoadAllDanhGiaData()
        {
            // Tải dữ liệu cho từng phần
            LoadDanhGiaToanTrangData();
            LoadDanhGiaChiSoCoTheData();
            LoadDanhGiaDauHieuSinhTonData();
            LoadDanhGiaTuanHoanData();
            LoadDanhGiaHoHapData();
            LoadDanhGiaTieuHoaData();
            LoadDanhGiaDinhDuongData();
            LoadDanhGiaTietNieuSinhDucData();
            LoadDanhGiaThanKinhData();
            LoadDanhGiaTinhThanGiacNguData();
            LoadDanhGiaCoXuongKhopData();
            LoadDanhGiaNhanDinhKhacData();
            LoadChuanDoanDieuDuongData();
            LoadPhanCapVaNhapXuatData();

        }



        private bool TestConnection()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    return true;
                }
            }
            catch (Exception ex)
            {
                // Ghi lại lỗi để dễ dàng gỡ rối (tùy chọn)
                Console.WriteLine("Lỗi kết nối DB: " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Tải và hiển thị thông tin cơ bản của bệnh nhân.
        /// </summary>
        private void LoadThongTinBenhNhan()
        {
            if (string.IsNullOrEmpty(_maNguoiBenh))
            {
                MessageBox.Show("Không có mã người bệnh để tải thông tin.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                return;
            }

            // Thêm lp.ThoiGian vào câu lệnh SELECT cuối cùng
            string query = @"
        WITH LatestPhieu AS (
            SELECT TOP 1 IDPhieu, MaKhoa, MaGiuong, ToSo, ThoiGian -- Lấy thêm ThoiGian ở đây
            FROM dbo.PhieuChamSoc
            WHERE MaNguoiBenh = @MaNguoiBenh
            ORDER BY ThoiGian DESC
        ),
        AggregatedChanDoan AS (
            SELECT 
                IDPhieu,
                STRING_AGG(NoiDungChanDoan, '; ') AS ChanDoanDieuDuong_TongHop
            FROM dbo.ChanDoanDieuDuong
            WHERE IDPhieu = (SELECT IDPhieu FROM LatestPhieu)
            GROUP BY IDPhieu
        )
        SELECT
            bn.HoTen, bn.Tuoi, bn.GioiTinh, bn.SoVaoVien,
            bn.TienSuDiUng, bn.GhiNhanDiUng,
            ISNULL(agg_cddd.ChanDoanDieuDuong_TongHop, bn.ChanDoan) AS ChanDoanHienThi,
            lp.ToSo,
            lp.ThoiGian, -- Và lấy ra ở đây
            k.TenKhoa,
            p.TenPhong,
            g.TenGiuong
        FROM
            dbo.BenhNhan AS bn
        LEFT JOIN LatestPhieu AS lp ON 1=1
        LEFT JOIN AggregatedChanDoan AS agg_cddd ON lp.IDPhieu = agg_cddd.IDPhieu
        LEFT JOIN dbo.Khoa AS k ON lp.MaKhoa = k.MaKhoa
        LEFT JOIN dbo.Giuong AS g ON lp.MaGiuong = g.MaGiuong
        LEFT JOIN dbo.Phong AS p ON g.MaPhong = p.MaPhong
        WHERE
            bn.MaNguoiBenh = @MaNguoiBenh";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@MaNguoiBenh", _maNguoiBenh);
                        connection.Open();

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Gán dữ liệu vào các controls
                                txtHoTen.Text = reader["HoTen"].ToString();
                                txtTuoi.Text = reader["Tuoi"].ToString();
                                txtSoVaoVien.Text = reader["SoVaoVien"].ToString();
                                txtMaNguoiBenh.Text = _maNguoiBenh;
                                txtChanDoan.Text = reader["ChanDoanHienThi"].ToString();

                                // MỚI: Xử lý và hiển thị ngày giờ
                                if (reader["ThoiGian"] != DBNull.Value)
                                {
                                    DateTime thoiGian = Convert.ToDateTime(reader["ThoiGian"]);

                                    // ================================================================
                                    // SỬA Ở ĐÂY: Thay 'yourDateTimePicker' bằng tên control của bạn
                                    // ================================================================
                                    yourDateTimePicker.Value = thoiGian;
                                }
                                else
                                {
                                    // Nếu không có phiếu chăm sóc, đặt ngày giờ về hiện tại
                                    yourDateTimePicker.Value = DateTime.Now;
                                }

                                // Xử lý giới tính
                                string gioiTinh = reader["GioiTinh"].ToString();
                                radNam.Checked = gioiTinh.Equals("Nam", StringComparison.OrdinalIgnoreCase);
                                radNu.Checked = gioiTinh.Equals("Nữ", StringComparison.OrdinalIgnoreCase);

                                // Xử lý Tiền sử dị ứng
                                bool coDiUng = reader["GhiNhanDiUng"] != DBNull.Value && Convert.ToBoolean(reader["GhiNhanDiUng"]);
                                if (coDiUng)
                                {
                                    radDiUngCo.Checked = true;
                                    txtTienSuDiUng.Text = reader["TienSuDiUng"].ToString();
                                }
                                else
                                {
                                    radDiUngChuaGhiNhan.Checked = true;
                                    txtTienSuDiUng.Text = "";
                                }

                                // Thông tin từ các bảng JOIN
                                txtToSo.Text = reader["ToSo"].ToString();
                                txtKhoa.Text = reader["TenKhoa"].ToString();
                                txtPhong.Text = reader["TenPhong"].ToString();
                                txtGiuong.Text = reader["TenGiuong"].ToString();
                            }
                            else
                            {
                                MessageBox.Show("Không tìm thấy thông tin cho bệnh nhân có mã: " + _maNguoiBenh, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                this.Close();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi khi tải thông tin bệnh nhân: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        // 1.Toàn Trạng
        private void LoadDanhGiaToanTrangData()
        {
            string query = @"
        SELECT TOP 1 
            TriGiac, DiemGlasgow, MauSacDa, TinhTrangDa, TinhTrangPhu, ViTriPhu
        FROM DanhGia_ToanTrang dgtt
        INNER JOIN PhieuChamSoc pcs ON dgtt.IDPhieu = pcs.IDPhieu
        WHERE pcs.MaNguoiBenh = @MaNguoiBenh
        ORDER BY pcs.ThoiGian DESC";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString)) // Sử dụng _connectionString
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@MaNguoiBenh", _maNguoiBenh);
                        connection.Open();

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read()) // Đảm bảo có dữ liệu để đọc
                            {
                                // === Xử lý Tri Giác ===
                                string triGiacDb = reader["TriGiac"]?.ToString();
                                switch (triGiacDb)
                                {
                                    case "Tỉnh táo":
                                        radTinhTao.Checked = true;
                                        break;
                                    case "Lú lẫn":
                                        radLuLan.Checked = true;
                                        break;
                                    case "Hôn mê":
                                        radHonMe.Checked = true;
                                        break;
                                    case "Kích động":
                                        radKichDong.Checked = true;
                                        break;
                                    default:
                                        radTriGiacKhac.Checked = true;
                                        txtTriGiacKhac.Text = triGiacDb;
                                        break;
                                }
                                txtDiemGlasgow.Text = reader["DiemGlasgow"]?.ToString();

                                // === Xử lý Màu sắc da ===
                                string mauSacDaDb = reader["MauSacDa"]?.ToString();
                                switch (mauSacDaDb)
                                {
                                    case "Hồng": radMauSacHong.Checked = true; break;
                                    case "Vàng": radMauSacVang.Checked = true; break;
                                    case "Nhợt nhạt": radMauSacNhotNhat.Checked = true; break;
                                    case "Tím tái": radMauSacTimTai.Checked = true; break;
                                    default:
                                        radMauSacKhac.Checked = true;
                                        txtMauSacKhac.Text = mauSacDaDb;
                                        break;
                                }

                                // === Xử lý Tình trạng da (ĐÃ SỬA LẠI LOGIC) ===
                                string tinhTrangDaDb = reader["TinhTrangDa"]?.ToString();
                                switch (tinhTrangDaDb)
                                {
                                    case "Bình thường": radDaBinhThuong.Checked = true; break;
                                    case "Râm, lở": radDaRamLo.Checked = true; break;
                                    case "Viêm Tại Chỗ": radDaViemTaiCho.Checked = true; break;
                                    case "Chuẩn Xuất Huyết": radDaXuatHuyet.Checked = true; break;
                                    case "Khối Máu Tụ": radDaKhoiMauTu.Checked = true; break;
                                    default:
                                        // Chỉ điền vào textbox "Khác" nếu giá trị không khớp
                                        radDaKhac.Checked = true;
                                        txtDaKhac.Text = tinhTrangDaDb;
                                        break;
                                }

                                // === Xử lý Tình trạng phù ===
                                if (reader["TinhTrangPhu"] != DBNull.Value)
                                {
                                    bool coPhu = Convert.ToBoolean(reader["TinhTrangPhu"]);
                                    radPhuCo.Checked = coPhu;
                                    radPhuKhong.Checked = !coPhu;
                                }
                                // Chỉ hiển thị vị trí phù nếu có phù
                                txtViTriPhu.Text = reader["ViTriPhu"]?.ToString();
                                txtViTriPhu.Visible = radPhuCo.Checked;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi khi tải dữ liệu toàn trạng: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void TriGiac_CheckedChanged(object sender, EventArgs e)
        {
            // Lấy RadioButton vừa được chọn để kiểm tra
            RadioButton rad = sender as RadioButton;
            if (rad == null || !rad.Checked)
            {
                return; // Không làm gì nếu RadioButton không được chọn
            }

            // Yêu cầu 1: Nếu chọn "Lú lẫn" (2), "Hôn mê" (3), hoặc "Kích động" (4) thì hiện ô Điểm Glasgow
            bool hienThiDiemGlasgow = radLuLan.Checked || radHonMe.Checked || radKichDong.Checked;
            txtDiemGlasgow.Visible = hienThiDiemGlasgow;

            // Yêu cầu 2: Nếu chọn "Khác" (5) thì hiện ô nhập Khác
            txtTriGiacKhac.Visible = radTriGiacKhac.Checked;
        }

        //2.Dấu hiệu sinh tồn
        private void LoadDanhGiaDauHieuSinhTonData()
        {
            // Sửa câu lệnh SQL để lấy thêm cột 'NhietDo'
            string query = @"
        SELECT TOP 1 TanSoMach, HuyetAp, TanSoTho, SpO2 FROM DanhGia_DauHieuSinhTon dgdst INNER JOIN PhieuChamSoc pcs ON dgdst.IDPhieu = pcs.IDPhieu WHERE pcs.MaNguoiBenh = @MaNguoiBenh ORDER BY pcs.ThoiGian DESC";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@MaNguoiBenh", _maNguoiBenh);
                        connection.Open();

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Sửa lại việc gán giá trị cho đúng các TextBox
                                txtTanSoMach.Text = reader["TanSoMach"]?.ToString();
                                txtHuyetAp.Text = reader["HuyetAp"]?.ToString();
                                txtTanSoTho.Text = reader["TanSoTho"]?.ToString(); // Gán cho txtTanSoTho
                                txtSpO2.Text = reader["SpO2"]?.ToString();       // Giữ lại dòng gán cho SpO2
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi khi tải dấu hiệu sinh tồn: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 3.Chỉ số cân nặng, chiều cao
        private void LoadDanhGiaChiSoCoTheData()
        {
            string query = @"
                SELECT TOP 1 
                    ChieuCao, CanNang, BMI
                FROM DanhGia_ChiSoCoThe dgcsc
                INNER JOIN PhieuChamSoc pcs ON dgcsc.IDPhieu = pcs.IDPhieu
                WHERE pcs.MaNguoiBenh = @MaNguoiBenh
                ORDER BY pcs.ThoiGian DESC";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@MaNguoiBenh", _maNguoiBenh);
                        connection.Open();

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Gán trực tiếp vào các TextBox
                                // **LƯU Ý**: Thay thế tên control (txtChieuCao,...) bằng tên thật.
                                txtChieuCao.Text = reader["ChieuCao"]?.ToString();
                                txtCanNang.Text = reader["CanNang"]?.ToString();
                                txtBMI.Text = reader["BMI"]?.ToString();
                            }
                            // Không cần thông báo nếu không có dữ liệu, các trường sẽ để trống.
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi khi tải chỉ số cơ thể: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 4.Tuần hoàn
        private void LoadDanhGiaTuanHoanData()
        {
            // Giả sử tên bảng là DanhGia_TuanHoan
            string query = @"
        SELECT TOP 1 
            TinhChatMach, DauHieuKhac
        FROM DanhGia_TuanHoan dgth
        INNER JOIN PhieuChamSoc pcs ON dgth.IDPhieu = pcs.IDPhieu
        WHERE pcs.MaNguoiBenh = @MaNguoiBenh
        ORDER BY pcs.ThoiGian DESC";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@MaNguoiBenh", _maNguoiBenh);
                        connection.Open();

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // === Xử lý phần Tính Chất Mạch ===
                                string tinhChatMachDb = reader["TinhChatMach"]?.ToString();
                                switch (tinhChatMachDb)
                                {
                                    case "Đều Rõ": radMachDeuRo.Checked = true; break;
                                    case "Không Đều": radMachKhongDeu.Checked = true; break;
                                    case "Rời Rạc": radMachRoiRac.Checked = true; break;
                                    case "Khó Bắt": radMachKhoBat.Checked = true; break;
                                    default:
                                        radMachKhac.Checked = true;
                                        txtMachKhac.Text = tinhChatMachDb;
                                        break;
                                }

                                // === Xử lý phần Dấu Hiệu Khác ===
                                string dauHieuKhacDb = reader["DauHieuKhac"]?.ToString();
                                switch (dauHieuKhacDb)
                                {
                                    case "Chưa Ghi Nhận": radDauHieuChuaGhiNhan.Checked = true; break;
                                    case "Hồi Hộp": radDauHieuHoiHop.Checked = true; break;
                                    case "Tức Ngực": radDauHieuTucNguc.Checked = true; break;
                                    case "Đánh Trống Ngực": radDauHieuDanhTrongNguc.Checked = true; break;
                                    case "Đau Ngực": radDauHieuDauNguc.Checked = true; break;
                                    default:
                                        radDauHieuKhac.Checked = true;
                                        txtDauHieuKhac.Text = dauHieuKhacDb;
                                        break;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi khi tải dữ liệu tuần hoàn: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 5.Hô hấp
        private void LoadDanhGiaHoHapData()
        {
            // Sửa lỗi câu query:
            // 1. Bỏ dấu phẩy thừa ở cuối.
            // 2. Đảm bảo tất cả các cột cần đọc đều có trong SELECT.
            string query = @"
        SELECT TOP 1 
            TinhTrangHoHap, 
            KieuTho, 
            Ho, 
            Dom_MauSac, 
            Dom_SoLuong,
            ThongKhiCoHoc_CheDoTho,
            ThongKhiCoHoc_Vt,
            ThongKhiCoHoc_Ps,
            ThongKhiCoHoc_PEEP,
            ThongKhiCoHoc_TanSoTho,
            ThongKhiCoHoc_FiO2
        FROM DanhGia_HoHap dghh
        INNER JOIN PhieuChamSoc pcs ON dghh.IDPhieu = pcs.IDPhieu
        WHERE pcs.MaNguoiBenh = @MaNguoiBenh
        ORDER BY pcs.ThoiGian DESC";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@MaNguoiBenh", _maNguoiBenh);
                        connection.Open();

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // === Xử lý Tình Trạng Hô Hấp ===
                                string tinhTrangDb = reader["TinhTrangHoHap"]?.ToString() ?? "";
                                if (tinhTrangDb.StartsWith("Thở Oxi qua canula"))
                                {
                                    radThoOxiCanula.Checked = true;
                                    txtThoOxiCanula.Text = tinhTrangDb.Replace("Thở Oxi qua canula:", "").Trim();
                                }
                                else if (tinhTrangDb.StartsWith("Thở Oxi qua mask"))
                                {
                                    radThoOxiMask.Checked = true;
                                    txtThoOxiMask.Text = tinhTrangDb.Replace("Thở Oxi qua mask:", "").Trim();
                                }
                                else
                                {
                                    switch (tinhTrangDb)
                                    {
                                        case "Bình thường": radHoHapBinhThuong.Checked = true; break;
                                        case "Thở máy qua nội khí quản": radHoHapNoiKhiQuan.Checked = true; break;
                                        case "Mở Khí Quản": radHoHapMoKhiQuan.Checked = true; break;
                                        case "Khò Sạch": radHoHapKhoSach.Checked = true; break;
                                        case "Rõ Đờm": radHoHapRoDom.Checked = true; break;
                                        case "Thẩm Dịch Tiết": radHoHapThamDichTiet.Checked = true; break;
                                        default:
                                            radTinhTrangHoHapKhac.Checked = true;
                                            txtTinhTrangHoHapKhac.Text = tinhTrangDb;
                                            break;
                                    }
                                }

                                // === Xử lý Kiểu Thở ===
                                string kieuThoDb = reader["KieuTho"]?.ToString();
                                switch (kieuThoDb)
                                {
                                    case "Bình Thường": radKieuThoBinhThuong.Checked = true; break;
                                    case "Râm, lở": radKieuThoRamLo.Checked = true; break;
                                    case "Viêm Tại Chỗ": radKieuThoViemTaiCho.Checked = true; break;
                                    case "Chuẩn Xuất Huyết": radKieuThoChuanXuatHuyet.Checked = true; break;
                                    case "Khối Máu Tụ": radKieuThoKhoiMauTu.Checked = true; break;
                                    default:
                                        radKieuThoKhac.Checked = true;
                                        txtKieuThoKhac.Text = kieuThoDb;
                                        break;
                                }

                                // === Xử lý Thông Khí Cơ Học (ĐÃ SỬA LỖI) ===
                                txtCheDoTho.Text = reader["ThongKhiCoHoc_CheDoTho"]?.ToString();
                                txtDungTichSongVt.Text = reader["ThongKhiCoHoc_Vt"]?.ToString();
                                txtApLucHoTroPs.Text = reader["ThongKhiCoHoc_Ps"]?.ToString();
                                txtApLucTuongDoi.Text = reader["ThongKhiCoHoc_PEEP"]?.ToString();
                                txtTanSoThoMay.Text = reader["ThongKhiCoHoc_TanSoTho"]?.ToString();
                                txtFiO2.Text = reader["ThongKhiCoHoc_FiO2"]?.ToString();

                                // === Xử lý Ho - Tình Trạng ===
                                string tinhTrangHoDb = reader["Ho"]?.ToString();
                                switch (tinhTrangHoDb)
                                {
                                    case "Không Ho": radHoKhong.Checked = true; break;
                                    case "Ho Khan": radHoKhan.Checked = true; break;
                                    case "Ho Có Đờm": radHoCoDom.Checked = true; break;
                                    case "Ho Ra Máu": radHoRaMau.Checked = true; break;
                                    default:
                                        radHoKhac.Checked = true;
                                        txtHoKhac.Text = tinhTrangHoDb;
                                        break;
                                }

                                // === Xử lý Ho - Số Lượng Đờm ===
                                string soLuongDomDb = reader["Dom_SoLuong"]?.ToString();
                                switch (soLuongDomDb)
                                {
                                    case "Ít": radDomIt.Checked = true; break;
                                    case "Nhiều": radDomNhieu.Checked = true; break;
                                    case "Trung Bình": radDomTrungBinh.Checked = true; break;
                                    default:
                                        radDomKhac.Checked = true;
                                        txtDomKhac.Text = soLuongDomDb;
                                        break;
                                }

                                // === Xử lý Ho - Màu Sắc Đờm ===
                                string mauSacDomDb = reader["Dom_MauSac"]?.ToString();
                                switch (mauSacDomDb)
                                {
                                    case "Trắng Đục": radDomTrangDuc.Checked = true; break;
                                    case "Vàng": radDomVang.Checked = true; break;
                                    case "Đỏ": radDomDo.Checked = true; break;
                                    default:
                                        radDomMauKhac.Checked = true;
                                        txtDomMauKhac.Text = mauSacDomDb;
                                        break;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Thêm chi tiết về lỗi SQL vào thông báo để dễ debug hơn
                MessageBox.Show("Đã xảy ra lỗi khi tải dữ liệu hô hấp: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void TinhTrangHoHap_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rad = sender as RadioButton;
            if (rad == null || !rad.Checked)
            {
                return;
            }

            bool showThongKhiCoHoc = radHoHapNoiKhiQuan.Checked || radHoHapMoKhiQuan.Checked;
            pnlThongKhiCoHoc.Visible = showThongKhiCoHoc;

            txtThoOxiCanula.Visible = radThoOxiCanula.Checked;
            txtThoOxiMask.Visible = radThoOxiMask.Checked;
            txtTinhTrangHoHapKhac.Visible = radTinhTrangHoHapKhac.Checked;
        }

        // 6.Tiêu Hóa
        private void LoadDanhGiaTieuHoaData()
        {
            // Câu truy vấn đã được cập nhật để lấy tất cả các cột chi tiết mới
            string query = @"
        SELECT TOP 1 
            -- Các cột cũ
            TinhTrangBung, RoiLoanNuot, DayBungKhoTieu, Non,
            NhuDongRuot, DaiTien, HauMonNhanTao,
            HMNT_NiemMac, HMNT_DaXungQuanh,
            
            -- Các cột mới cho Nôn
            Non_SoLan, Non_SoLuong, Non_ChatNon, Non_ChiTiet,

            -- Các cột mới cho Đại tiện
            DaiTien_SoLuong_ChiTiet, DaiTien_MauSac, DaiTien_Khac_ChiTiet, DaiTien_ChiTiet,
            DaiTien_SoNgayChuaDaiTien,
            DaiTien_SoLan, DaiTien_SoLuong_Phan, DaiTien_TinhChatPhan
        FROM DanhGia_TieuHoa dgth
        INNER JOIN PhieuChamSoc pcs ON dgth.IDPhieu = pcs.IDPhieu
        WHERE pcs.MaNguoiBenh = @MaNguoiBenh
        ORDER BY pcs.ThoiGian DESC";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@MaNguoiBenh", _maNguoiBenh);
                        connection.Open();

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // === Xử lý Tình trạng bụng (giữ nguyên) ===
                                string tinhTrangBungDb = reader["TinhTrangBung"]?.ToString();
                                switch (tinhTrangBungDb)
                                {
                                    case "Bụng Mềm": radBungMem.Checked = true; break;
                                    case "Bụng Báng": radBungBang.Checked = true; break;
                                    case "Bụng Chướng": radBungChuong.Checked = true; break;
                                    default:
                                        radBungKhac.Checked = true;
                                        txtBungKhac.Text = tinhTrangBungDb;
                                        break;
                                }

                                // === Xử lý Rối loạn nuốt (giữ nguyên) ===
                                if (reader["RoiLoanNuot"] != DBNull.Value)
                                {
                                    bool roiLoanNuot = Convert.ToBoolean(reader["RoiLoanNuot"]);
                                    radRoiLoanNuotCo.Checked = roiLoanNuot;
                                    radRoiLoanNuotKhong.Checked = !roiLoanNuot;
                                }

                                // === Xử lý Đầy bụng, khó tiêu (giữ nguyên) ===
                                if (reader["DayBungKhoTieu"] != DBNull.Value)
                                {
                                    bool dayBungKhoTieu = Convert.ToBoolean(reader["DayBungKhoTieu"]);
                                    radDayBungCo.Checked = dayBungKhoTieu;
                                    radDayBungKhong.Checked = !dayBungKhoTieu;
                                }

                                // === CẬP NHẬT: Xử lý Nôn và các chi tiết ===
                                if (reader["Non"] != DBNull.Value)
                                {
                                    bool coNon = Convert.ToBoolean(reader["Non"]);
                                    radNonCo.Checked = coNon;
                                    radNonKhong.Checked = !coNon;
                                }
                                // Gán dữ liệu vào các textbox chi tiết của Nôn
                                txtNonSoLan.Text = reader["Non_SoLan"]?.ToString();
                                txtNonSoLuong.Text = reader["Non_SoLuong"]?.ToString();
                                txtChatNon.Text = reader["Non_ChatNon"]?.ToString();
                                txtKhac.Text = reader["Non_ChiTiet"]?.ToString();

                                // === Xử lý Nhu động ruột (giữ nguyên) ===
                                if (reader["NhuDongRuot"] != DBNull.Value)
                                {
                                    bool nhuDongRuot = Convert.ToBoolean(reader["NhuDongRuot"]);
                                    radNhuDongRuotCo.Checked = nhuDongRuot;
                                    radNhuDongRuotKhong.Checked = !nhuDongRuot;
                                }

                                // === CẬP NHẬT: Xử lý Đại tiện và các chi tiết ===
                                string daiTienDb = reader["DaiTien"]?.ToString();
                                switch (daiTienDb)
                                {
                                    case "Bình thường": radDaiTienBinhThuong.Checked = true; break;
                                    case "Phân lẫn máu": radDaiTienPhanLanMau.Checked = true; break;
                                    case "Táo bón": radDaiTienTaoBon.Checked = true; break;
                                    case "Tiêu chảy": radDaiTienTieuChay.Checked = true; break;
                                    default:
                                        radDaiTienKhac.Checked = true;
                                        txtDaiTienKhac.Text = daiTienDb; // Ô khác của nhóm radio button
                                        break;
                                }
                                // Gán dữ liệu vào các textbox chi tiết của Đại tiện
                                txtDaiTienSoLuong.Text = reader["DaiTien_SoLuong_ChiTiet"]?.ToString();
                                txtDaiTienMauSac.Text = reader["DaiTien_MauSac"]?.ToString();
                                txtdaitienslkhac.Text = reader["DaiTien_ChiTiet"]?.ToString();
                                txtSoNgayChuaDaiTien.Text = reader["DaiTien_SoNgayChuaDaiTien"]?.ToString();
                                txtDaiTienSoLan2.Text = reader["DaiTien_SoLan"]?.ToString();
                                txtDaiTienSoLuong2.Text = reader["DaiTien_SoLuong_Phan"]?.ToString();
                                txtDaiTienTinhChatPhan.Text = reader["DaiTien_TinhChatPhan"]?.ToString();
                                txtDaiTienKhac2.Text = reader["DaiTien_Khac_ChiTiet"]?.ToString(); // Dùng chung cột Khác cho cả 2 ô


                                // === Xử lý Hậu môn nhân tạo (giữ nguyên) ===
                                string hauMonNhanTaoDb = reader["HauMonNhanTao"]?.ToString();
                                switch (hauMonNhanTaoDb)
                                {
                                    case "Không": radHMNTKhong.Checked = true; break;
                                    case "Đại Tràng": radHMNTDaiTrang.Checked = true; break;
                                    case "Hồi Tràng": radHMNTHoiTrang.Checked = true; break;
                                    default:
                                        radHMNTKhac.Checked = true;
                                        txtHMNTKhac.Text = hauMonNhanTaoDb;
                                        break;
                                }
                                // ... (giữ nguyên phần code xử lý Niêm Mạc và Da xung quanh HMNT)
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi khi tải dữ liệu tiêu hóa: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void DaiTien_CheckedChanged(object sender, EventArgs e)
        {
            // 1. Ẩn tất cả các panel chi tiết trước
            pnlDaiTien_PhanLanMau.Visible = false;
            pnlDaiTien_TaoBon.Visible = false;
            pnlDaiTien_TieuChay.Visible = false;

            // 2. Lấy radio button đang được chọn
            RadioButton rad = sender as RadioButton;
            if (rad == null || !rad.Checked)
            {
                return; // Không làm gì nếu không phải là radio button hoặc không được chọn
            }

            // 3. Hiện panel tương ứng dựa trên lựa chọn
            // Giả sử tên các RadioButton của bạn là:
            // radDaiTienPhanLanMau, radDaiTienTaoBon, radDaiTienTieuChay, radDaiTienKhac

            if (rad == radDaiTienPhanLanMau)
            {
                pnlDaiTien_PhanLanMau.Visible = true;
            }
            else if (rad == radDaiTienTaoBon)
            {
                pnlDaiTien_TaoBon.Visible = true;
            }
            else if (rad == radDaiTienTieuChay)
            {
                pnlDaiTien_TieuChay.Visible = true;
            }
            else if (rad == radDaiTienKhac)
            {
                // Khi chọn "Khác", thường sẽ hiện panel chung nhất
                pnlDaiTien_PhanLanMau.Visible = true;
            }
            // Nếu chọn "Bình Thường" thì không có panel nào hiện ra cả.
        }



        // 7.Dinh Dưỡng
        private void LoadDanhGiaDinhDuongData()
        {
            string query = @"
        SELECT TOP 1 
            NguyCoSuyDinhDuong, CheDoAn, TinhTrangAn, DuongAn
        FROM DanhGia_DinhDuong dgdd
        INNER JOIN PhieuChamSoc pcs ON dgdd.IDPhieu = pcs.IDPhieu
        WHERE pcs.MaNguoiBenh = @MaNguoiBenh
        ORDER BY pcs.ThoiGian DESC";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@MaNguoiBenh", _maNguoiBenh);
                        connection.Open();

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            // Luôn phải gọi reader.Read() để di chuyển đến dòng dữ liệu đầu tiên
                            if (reader.Read())
                            {
                                // === Xử lý Nguy cơ suy dinh dưỡng ===
                                if (reader["NguyCoSuyDinhDuong"] != DBNull.Value)
                                {
                                    bool coNguyCo = Convert.ToBoolean(reader["NguyCoSuyDinhDuong"]);
                                    if (coNguyCo)
                                    {
                                        radNguyCoCo.Checked = true;
                                    }
                                    else
                                    {
                                        radNguyCoKhong.Checked = true;
                                    }
                                }

                                // === Xử lý Chế độ ăn ===
                                txtCheDoAn.Text = reader["CheDoAn"]?.ToString();

                                // === Xử lý Tình trạng ăn ===
                                string tinhTrangAnDb = reader["TinhTrangAn"]?.ToString();
                                switch (tinhTrangAnDb)
                                {
                                    case "Ăn Hết Suất":
                                        radAnHetSuat.Checked = true;
                                        break;
                                    case "Ăn 1/2 Suất":
                                        radAnNuaSuat.Checked = true;
                                        break;
                                    case "Không Ăn":
                                        radKhongAn.Checked = true;
                                        break;
                                }

                                // === Xử lý Đường ăn ===
                                string duongAnDb = reader["DuongAn"]?.ToString();
                                switch (duongAnDb)
                                {
                                    case "Ăn Qua Miệng":
                                        radAnQuaMieng.Checked = true;
                                        break;
                                    case "Ăn Qua Mũi, Dạ Dày":
                                        radAnQuaMuiDaDay.Checked = true;
                                        break;
                                    case "Ăn Quan Mở Thông Dạ Dày":
                                        radAnQuaMoThongDaDay.Checked = true;
                                        break;
                                    case "Ăn Qua Mở Thông Hỗng Tràng":
                                        radAnQuaMoThongHongTrang.Checked = true;
                                        break;
                                    case "Dinh Dưỡng Tĩnh Mạch":
                                        radDinhDuongTinhMach.Checked = true;
                                        break;
                                    case "Nhịn Ăn":
                                        radNhinAn.Checked = true;
                                        break;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi khi tải dữ liệu dinh dưỡng: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 8.Tiếu Niệu
        private void LoadDanhGiaTietNieuSinhDucData()
        {
            string query = @"
        SELECT TOP 1 
            HinhThucTieu, MauSacNuocTieu, SoLuongNuocTieu, TieuRatBuot, BoPhanSinhDuc
        FROM DanhGia_TietNieuSinhDuc dgtnsd
        INNER JOIN PhieuChamSoc pcs ON dgtnsd.IDPhieu = pcs.IDPhieu
        WHERE pcs.MaNguoiBenh = @MaNguoiBenh
        ORDER BY pcs.ThoiGian DESC";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@MaNguoiBenh", _maNguoiBenh);
                        connection.Open();

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // === Xử lý Hình Thức Đi Tiểu ===
                                string hinhThucDiTieuDb = reader["HinhThucTieu"]?.ToString();
                                switch (hinhThucDiTieuDb)
                                {
                                    case "Nhịn Tiểu": radNhinTieu.Checked = true; break;
                                    case "Bí Tiểu": radBiTieu.Checked = true; break;
                                    case "Sonde Tiểu": radTieuQuaOngThong.Checked = true; radSondeTieu.Checked = true; break;
                                    case "Dẫn Lưu Niệu Quản Ra Da": radTieuQuaOngThong.Checked = true; radDanLuuNieuQuan.Checked = true; break;
                                    case "Dẫn Lưu Bàng Quang Trên Xương Mu": radTieuQuaOngThong.Checked = true; radDanLuuBangQuang.Checked = true; break;
                                    case "Tiểu Qua Ông Thông": radTieuQuaOngThong.Checked = true; break;
                                }

                                // === Xử lý Màu Sắc Nước Tiểu ===
                                string mauSacNuocTieuDb = reader["MauSacNuocTieu"]?.ToString();
                                switch (mauSacNuocTieuDb)
                                {
                                    case "Trong": radMauNuocTieuTrong.Checked = true; break;
                                    case "Vàng": radMauNuocTieuVang.Checked = true; break;
                                    case "Đỏ": radMauNuocTieuDo.Checked = true; break;
                                    default: radMauNuocTieuKhac.Checked = true; txtMauNuocTieuKhac.Text = mauSacNuocTieuDb; break;
                                }

                                // === Xử lý Số Lượng Nước Tiểu ===
                                txtSoLuongNuocTieu.Text = reader["SoLuongNuocTieu"]?.ToString();

                                // === Xử lý Tiểu Rắt/Buốt ===
                                if (reader["TieuRatBuot"] != DBNull.Value)
                                {
                                    bool coTieuRat = Convert.ToBoolean(reader["TieuRatBuot"]);
                                    radTieuRatCo.Checked = coTieuRat;
                                    radTieuRatKhong.Checked = !coTieuRat;
                                }

                                // === Xử lý Bộ Phận Sinh Dục ===
                                string boPhanSinhDucDb = reader["BoPhanSinhDuc"]?.ToString();
                                switch (boPhanSinhDucDb)
                                {
                                    case "Sạch": radBoPhanSinhDucSach.Checked = true; break;
                                    case "Bẩn": radBoPhanSinhDucBan.Checked = true; break;
                                    default: radBoPhanSinhDucKhac.Checked = true; txtBoPhanSinhDucKhac.Text = boPhanSinhDucDb; break;
                                }

                                // KHỐI SWITCH THỨ HAI ĐÃ ĐƯỢC XÓA BỎ
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Sửa lại thông báo lỗi cho đúng ngữ cảnh
                MessageBox.Show("Đã xảy ra lỗi khi tải dữ liệu Tiết Niệu: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void HinhThucDiTieu_CheckedChanged(object sender, EventArgs e)
        {
            if (pnlTieuQuaOngThong != null)
            {
                pnlTieuQuaOngThong.Visible = radTieuQuaOngThong.Checked;
            }
        }

        // 9.Thần Kinh
        private void LoadDanhGiaThanKinhData()
        {
            // CẬP NHẬT: Đã loại bỏ 'ThoiGianRoiLoanVanDong' khỏi câu SELECT
            string query = @"
        SELECT TOP 1 
            LoiNoi, YeuLiet, RoiLoanVanDong, DauHieuKhac
        FROM DanhGia_ThanKinh dgtk
        INNER JOIN PhieuChamSoc pcs ON dgtk.IDPhieu = pcs.IDPhieu
        WHERE pcs.MaNguoiBenh = @MaNguoiBenh
        ORDER BY pcs.ThoiGian DESC";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@MaNguoiBenh", _maNguoiBenh);
                        connection.Open();

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // === Xử lý Lời Nói ===
                                string loiNoiDb = reader["LoiNoi"]?.ToString();
                                switch (loiNoiDb)
                                {
                                    case "Rõ Ràng": radLoiNoiRoRang.Checked = true; break;
                                    case "Nói Đớ": radLoiNoiDo.Checked = true; break;
                                    case "Nói Nhảm": radLoiNoiNham.Checked = true; break;
                                }

                                // === Xử lý Yếu Liệt ===
                                string yeuLietDb = reader["YeuLiet"]?.ToString();
                                switch (yeuLietDb)
                                {
                                    case "Không": radYeuLietKhong.Checked = true; break;
                                    case "Toàn Thân": radYeuLietToanThan.Checked = true; break;
                                    case "1/2 Người (T)": radYeuLietNuaNguoiT.Checked = true; break;
                                    case "1/2 Người (P)": radYeuLietNuaNguoiP.Checked = true; break;
                                    case "Hai Chi Dưới": radYeuLietHaiChiDuoi.Checked = true; break;
                                    default:
                                        radYeuLietKhac.Checked = true;
                                        txtYeuLietKhac.Text = yeuLietDb;
                                        break;
                                }

                                // === Xử lý Rối Loạn Vận Động ===
                                string roiLoanVanDongDb = reader["RoiLoanVanDong"]?.ToString();

                                radRoiLoanVanDongKhong.Checked = true; // Mặc định chọn "Không" và ẩn panel

                                if (!string.IsNullOrEmpty(roiLoanVanDongDb) && roiLoanVanDongDb != "Không")
                                {
                                    radRoiLoanVanDongCo.Checked = true; // Sẽ kích hoạt sự kiện để hiện panel
                                    switch (roiLoanVanDongDb)
                                    {
                                        case "Động Kinh": radDongKinh.Checked = true; break;
                                        case "Múa Giật": radMuaGiat.Checked = true; break;
                                        case "Co Giật": radCoGiat.Checked = true; break;
                                        default:
                                            radRoiLoanVanDongKhac.Checked = true;
                                            txtRoiLoanVanDongKhac.Text = roiLoanVanDongDb;
                                            break;
                                    }
                                }

                                // CẬP NHẬT: Dòng này phải được giữ ở dạng ghi chú hoặc xóa đi
                                // txtThoiGianRoiLoanVanDong.Text = reader["ThoiGianRoiLoanVanDong"]?.ToString();

                                // === Xử lý Dấu Hiệu Khác ===
                                string dauHieuKhacDb = reader["DauHieuKhac"]?.ToString();
                                switch (dauHieuKhacDb)
                                {
                                    case "Chưa Ghi Nhận": radDauHieuKhacChuaGhiNhan.Checked = true; break;
                                    case "Đau Đầu": radDauDau.Checked = true; break;
                                    case "Chóng Mặt": radChongMat.Checked = true; break;
                                    case "Nôn Vọt": radNonVot.Checked = true; break;
                                    case "Sụp Mi": radSupMi.Checked = true; break;
                                    case "Liệt Mặt": radLietMat.Checked = true; break;
                                    case "Kích Động": radDauHieuKhacKichDong.Checked = true; break;
                                    default:
                                        radDauHieuKhacKhac.Checked = true;
                                        txtDauHieuKhacKhac.Text = dauHieuKhacDb;
                                        break;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi khi tải dữ liệu Thần Kinh: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void RoiLoanVanDong_CheckedChanged(object sender, EventArgs e)
        {
            // Chỉ hiện panel chi tiết khi người dùng chọn "Có"
            if (pnlRoiLoanVanDong != null)
            {
                pnlRoiLoanVanDong.Visible = radRoiLoanVanDongCo.Checked;
            }
        }

        // 10.Tinh Thần Giấc Ngủ
        private void LoadDanhGiaTinhThanGiacNguData()
        {
            string query = @"
        SELECT TOP 1 
            TinhThan, GiacNgu, ThoiGianNgu
        FROM DanhGia_TinhThanGiacNgu dgttgn
        INNER JOIN PhieuChamSoc pcs ON dgttgn.IDPhieu = pcs.IDPhieu
        WHERE pcs.MaNguoiBenh = @MaNguoiBenh
        ORDER BY pcs.ThoiGian DESC";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@MaNguoiBenh", _maNguoiBenh);
                        connection.Open();

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // === Xử lý Tinh Thần ===
                                string tinhThanDb = reader["TinhThan"]?.ToString();
                                switch (tinhThanDb)
                                {
                                    case "Bình Thường":
                                        radTinhThanBinhThuong.Checked = true;
                                        break;
                                    case "Lo Lắng":
                                        radLoLang.Checked = true;
                                        break;
                                    case "Trầm Cảm":
                                        radTramCam.Checked = true;
                                        break;
                                    default:
                                        radTinhThanKhac.Checked = true;
                                        txtTinhThanKhac.Text = tinhThanDb; // Điền vào ô "Khác"
                                        break;
                                }

                                // === Xử lý Giấc Ngủ ===
                                string giacNguDb = reader["GiacNgu"]?.ToString();
                                switch (giacNguDb)
                                {
                                    case "Bình Thường":
                                        radGiacNguBinhThuong.Checked = true;
                                        break;
                                    case "Khó Ngủ":
                                        radKhoNgu.Checked = true;
                                        break;
                                    case "Ngủ Ít":
                                        radNguIt.Checked = true;
                                        break;
                                    case "Mất Ngủ":
                                        radMatNgu.Checked = true;
                                        break;
                                    case "Thời Gian Ngủ/Ngày":
                                        radThoiGianNgu.Checked = true;
                                        txtThoiGianNgu.Text = reader["ThoiGianNgu"]?.ToString();
                                        break;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi khi tải dữ liệu Tinh Thần - Giấc Ngủ: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 11.Cơ Sương Khớp
        private void LoadDanhGiaCoXuongKhopData()
        {
            string query = @"
        SELECT TOP 1 
            VanDong, VanDeKhac, VanDeKhac_ViTri
        FROM DanhGia_CoXuongKhop dgcxk
        INNER JOIN PhieuChamSoc pcs ON dgcxk.IDPhieu = pcs.IDPhieu
        WHERE pcs.MaNguoiBenh = @MaNguoiBenh
        ORDER BY pcs.ThoiGian DESC";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@MaNguoiBenh", _maNguoiBenh);
                        connection.Open();

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // === Xử lý Vận Động (phần trên) ===
                                string vanDongDb = reader["VanDong"]?.ToString();
                                switch (vanDongDb)
                                {
                                    case "Bình Thường":
                                        radVanDongBinhThuong.Checked = true;
                                        break;
                                    case "Hạn Chế":
                                        radVanDongHanChe.Checked = true;
                                        break;
                                    case "Bất Động":
                                        radVanDongBatDong.Checked = true;
                                        break;
                                    default:
                                        radVanDongKhac.Checked = true;
                                        txtVanDongKhac.Text = vanDongDb;
                                        break;
                                }

                                // === Xử lý Vấn Đề Khác (phần dưới) ===
                                string vanDeKhacDb = reader["VanDeKhac"]?.ToString();

                                // Mặc định chọn "Không" và ẩn panel chi tiết
                                radVanDeKhong.Checked = true;

                                if (!string.IsNullOrEmpty(vanDeKhacDb) && vanDeKhacDb != "Không")
                                {
                                    radVanDeCo.Checked = true; // Kích hoạt sự kiện để hiện panel
                                    switch (vanDeKhacDb)
                                    {
                                        case "Sưng Khớp": radSungKhop.Checked = true; break;
                                        case "Trật Khớp": radTratKhop.Checked = true; break;
                                        case "Gãy Xương": radGayXuong.Checked = true; break;
                                        case "Đau Khớp": radDauKhop.Checked = true; break;
                                        case "Cứng Khớp": radCungKhop.Checked = true; break;
                                        case "Teo Cơ": radTeoCo.Checked = true; break;
                                        default:
                                            radVanDeKhac_ChiTiet.Checked = true; // RadioButton "Khác" trong panel
                                            txtVanDeKhac_ChiTiet.Text = vanDeKhacDb; // TextBox "Khác" trong panel
                                            break;

                                    }
                                    txtVanDeKhac_ViTri.Text = reader["VanDeKhac_ViTri"]?.ToString();
                                }



                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi khi tải dữ liệu Cơ Xương Khớp: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void VanDeCoXuongKhop_CheckedChanged(object sender, EventArgs e)
        {
            // Chỉ hiện panel chi tiết khi người dùng chọn "Có"
            if (pnlVanDeCoXuongKhop != null)
            {
                pnlVanDeCoXuongKhop.Visible = radVanDeCo.Checked;
            }
        }

        // 12.Nhận Định Khác
        private void LoadDanhGiaNhanDinhKhacData()
        {
            // CẬP NHẬT: Câu truy vấn đã được viết lại để lấy tất cả các cột cần thiết
            string query = @"
        SELECT TOP 1 
            Dau_ViTri, Dau_ThangDiem, Dau_Khac,
            VetThuongLoet_ViTri, VetThuongLoet_KichThuoc, VetThuongLoet_MoTa,
            DanLuu_ViTri, DanLuu_MauSac, DanLuu_SoLuong, DanLuu_TinhChat,
            NguyCoNga,
            CanhBaoSom
        FROM DanhGia_NhanDinhKhac dgndk
        INNER JOIN PhieuChamSoc pcs ON dgndk.IDPhieu = pcs.IDPhieu
        WHERE pcs.MaNguoiBenh = @MaNguoiBenh
        ORDER BY pcs.ThoiGian DESC";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@MaNguoiBenh", _maNguoiBenh);
                        connection.Open();

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // === Xử lý phần Đau ===
                                txtDauViTri.Text = reader["Dau_ViTri"]?.ToString();
                                txtThangDiemDau.Text = reader["Dau_ThangDiem"]?.ToString();
                                txtDauKhac.Text = reader["Dau_Khac"]?.ToString(); // Giả sử ô cuối là "Đau Khác"

                                // === Xử lý Vết Thương Loét ===
                                txtVetThuongLoetViTri.Text = reader["VetThuongLoet_ViTri"]?.ToString();
                                txtVetThuongLoetKichThuoc.Text = reader["VetThuongLoet_KichThuoc"]?.ToString();
                                txtVetThuongLoetMoTa.Text = reader["VetThuongLoet_MoTa"]?.ToString();

                                // === Xử lý Dẫn Lưu ===
                                txtDanLuuViTri.Text = reader["DanLuu_ViTri"]?.ToString();
                                txtDanLuuMauSac.Text = reader["DanLuu_MauSac"]?.ToString();
                                txtDanLuuSoLuong.Text = reader["DanLuu_SoLuong"]?.ToString();
                                txtDanLuuTinhChat.Text = reader["DanLuu_TinhChat"]?.ToString();

                                // === Xử lý Nguy Cơ Ngã ===
                                if (reader["NguyCoNga"] != DBNull.Value)
                                {
                                    bool coNguyCo = Convert.ToBoolean(reader["NguyCoNga"]);
                                    radNguyCoNgaCo.Checked = coNguyCo;
                                    radNguyCoNgaKhong.Checked = !coNguyCo;
                                }

                                // === Xử lý Cảnh Báo Sớm ===
                                string canhBaoSomDb = reader["CanhBaoSom"]?.ToString();
                                if (string.IsNullOrEmpty(canhBaoSomDb) || canhBaoSomDb.Equals("Không", StringComparison.OrdinalIgnoreCase))
                                {
                                    radCanhBaoSomKhong.Checked = true;
                                }
                                else
                                {
                                    radCanhBaoSomCo.Checked = true;
                                    txtCanhBaoSom.Text = canhBaoSomDb; // Điền nội dung vào ô textbox bên cạnh
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi khi tải dữ liệu Nhận Định: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 13 & 14. Theo Dõi Nhập/Xuất và Phân Cấp Chăm Sóc
        private void LoadPhanCapVaNhapXuatData()
        {
            // Câu truy vấn lấy các cột TongNhap, TongXuat, PhanCapChamSoc từ phiếu chăm sóc mới nhất
            string query = @"
        SELECT TOP 1 
            TongNhap, TongXuat, PhanCapChamSoc
        FROM dbo.PhieuChamSoc
        WHERE MaNguoiBenh = @MaNguoiBenh
        ORDER BY ThoiGian DESC";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@MaNguoiBenh", _maNguoiBenh);
                        connection.Open();

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // === Gán dữ liệu cho phần Theo Dõi Nhập/Xuất ===
                                // LƯU Ý: Thay txtTongNhap, txtTongXuat bằng tên control đúng của bạn
                                txtTongNhap.Text = reader["TongNhap"]?.ToString();
                                txtTongXuat.Text = reader["TongXuat"]?.ToString();

                                // === Gán dữ liệu cho phần Phân Cấp Chăm Sóc ===
                                // LƯU Ý: Thay radCap1, radCap2, radCap3 bằng tên control đúng của bạn
                                string phanCapDb = reader["PhanCapChamSoc"]?.ToString();
                                switch (phanCapDb)
                                {
                                    case "Cấp I":
                                        radCap1.Checked = true;
                                        break;
                                    case "Cấp II":
                                        radCap2.Checked = true;
                                        break;
                                    case "Cấp III":
                                        radCap3.Checked = true;
                                        break;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi khi tải dữ liệu Phân Cấp & Nhập/Xuất: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 15.Chuẩn Đoán Điều Dưỡng
        private void LoadChuanDoanDieuDuongData()
        {
            // Câu truy vấn của bạn đã rất tốt và tối ưu, giữ nguyên nó.
            string query = @"
        SELECT 
            cddd.NoiDungChanDoan, 
            cddd.MucTieu
        FROM ChanDoanDieuDuong cddd
        WHERE cddd.IDPhieu = (
            SELECT TOP 1 IDPhieu
            FROM PhieuChamSoc
            WHERE MaNguoiBenh = @MaNguoiBenh
            ORDER BY ThoiGian DESC
        )";

            try
            {
                // Xóa tất cả các control cũ trong panel trước khi tải dữ liệu mới
                // Điều này rất quan trọng để tránh dữ liệu bị trùng lặp khi gọi lại hàm này.
                flpChuanDoan.Controls.Clear();

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@MaNguoiBenh", _maNguoiBenh);
                        connection.Open();

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            int stt = 1; // Biến để đếm số thứ tự chuẩn đoán
                            while (reader.Read())
                            {
                                // Lấy dữ liệu từ database
                                string noiDungChanDoan = reader["NoiDungChanDoan"]?.ToString();
                                string mucTieu = reader["MucTieu"]?.ToString();

                                // ---- TẠO CONTROL MỘT CÁCH LINH HOẠT ----

                                // 1. Tạo Label cho mỗi cặp (ví dụ: CĐĐD 1, CĐĐD 2, ...)
                                Label lblChuanDoan = new Label();
                                lblChuanDoan.Text = $"CĐĐD {stt}:";
                                lblChuanDoan.Width = 80; // Chiều rộng của Label
                                lblChuanDoan.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

                                // 2. Tạo TextBox cho Nội dung Chuẩn đoán
                                TextBox txtChanDoan = new TextBox();
                                txtChanDoan.Text = noiDungChanDoan;
                                txtChanDoan.Width = 400; // Chiều rộng của TextBox
                                txtChanDoan.Margin = new Padding(3, 3, 3, 6); // Tạo khoảng cách với control bên dưới

                                // 3. Tạo Label cho Mục tiêu
                                Label lblMucTieu = new Label();
                                lblMucTieu.Text = "Mục Tiêu:";
                                lblMucTieu.Width = 80;
                                lblMucTieu.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

                                // 4. Tạo TextBox cho Mục tiêu
                                TextBox txtMucTieu = new TextBox();
                                txtMucTieu.Text = mucTieu;
                                txtMucTieu.Width = 400;
                                txtMucTieu.Margin = new Padding(3, 3, 3, 12); // Khoảng cách lớn hơn để phân tách các cặp

                                // 5. Thêm tất cả control vừa tạo vào FlowLayoutPanel
                                // FlowLayoutPanel sẽ tự động sắp xếp chúng!
                                flpChuanDoan.Controls.Add(lblChuanDoan);
                                flpChuanDoan.Controls.Add(txtChanDoan);
                                flpChuanDoan.Controls.Add(lblMucTieu);
                                flpChuanDoan.Controls.Add(txtMucTieu);

                                stt++; // Tăng số thứ tự
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi khi tải dữ liệu Chuẩn Đoán Điều Dưỡng: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnExportPDF_Click(object sender, EventArgs e)
        {
            CrystalReport1 rpt = new CrystalReport1();

            
            // 3. Mở hộp thoại để người dùng chọn nơi lưu file
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "PDF Files (*.pdf)|*.pdf";
            saveFileDialog.Title = "Lưu file PDF";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // 4. Xuất báo cáo ra đĩa
                    rpt.ExportToDisk(ExportFormatType.PortableDocFormat, saveFileDialog.FileName);
                    MessageBox.Show("Xuất file PDF thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi xuất file: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}



    
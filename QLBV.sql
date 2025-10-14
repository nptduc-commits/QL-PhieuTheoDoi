

-- Bảng Khoa
IF OBJECT_ID(N'dbo.Khoa', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Khoa (
    MaKhoa INT IDENTITY(1,1) PRIMARY KEY,
    TenKhoa NVARCHAR(255) NOT NULL UNIQUE
);
END
GO

-- (Tuỳ chọn) Bảng Phòng
IF OBJECT_ID(N'dbo.Phong', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Phong (
    MaPhong INT IDENTITY(1,1) PRIMARY KEY,
    TenPhong NVARCHAR(100) NOT NULL,
    MaKhoa INT NOT NULL,
        CONSTRAINT FK_Phong_Khoa FOREIGN KEY (MaKhoa) REFERENCES dbo.Khoa(MaKhoa) ON DELETE CASCADE
);
END
GO

-- (Tuỳ chọn) Bảng Giường
IF OBJECT_ID(N'dbo.Giuong', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Giuong (
    MaGiuong INT IDENTITY(1,1) PRIMARY KEY,
    TenGiuong NVARCHAR(100) NOT NULL,
    MaPhong INT NOT NULL,
        CONSTRAINT FK_Giuong_Phong FOREIGN KEY (MaPhong) REFERENCES dbo.Phong(MaPhong) ON DELETE CASCADE
    );
END
GO

-- Bảng Bệnh nhân
IF OBJECT_ID(N'dbo.BenhNhan', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.BenhNhan (
        MaNguoiBenh NVARCHAR(50) PRIMARY KEY,
    SoVaoVien VARCHAR(255) NULL,
    HoTen NVARCHAR(255) NOT NULL,
    Tuoi INT NULL,
        GioiTinh NVARCHAR(3) NULL, -- Nam/Nữ
        MaKhoa INT NULL,
    ChanDoan NVARCHAR(MAX) NULL,
    TienSuDiUng NVARCHAR(MAX) NULL,
    GhiNhanDiUng BIT DEFAULT 0,
    CONSTRAINT CHK_BenhNhan_GioiTinh CHECK (GioiTinh IN (N'Nam', N'Nữ')),
        CONSTRAINT FK_BenhNhan_Khoa FOREIGN KEY (MaKhoa) REFERENCES dbo.Khoa(MaKhoa)
    );
    CREATE INDEX IX_BenhNhan_HoTen ON dbo.BenhNhan(HoTen);
END
GO

-- Phiếu chăm sóc (trung tâm)
IF OBJECT_ID(N'dbo.PhieuChamSoc', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.PhieuChamSoc (
    IDPhieu INT IDENTITY(1,1) PRIMARY KEY,
        MaNguoiBenh NVARCHAR(50) NOT NULL,
        ThoiGian DATETIME2(0) NOT NULL DEFAULT (sysdatetime()),
    MaKhoa INT NULL,
        MaGiuong INT NULL,
    ToSo INT NULL,
    TongNhap DECIMAL(10, 2) NULL,
    TongXuat DECIMAL(10, 2) NULL,
    PhanCapChamSoc NVARCHAR(10) NULL,
    BanGiao NVARCHAR(MAX) NULL,
    DieuDuongThucHien NVARCHAR(255) NULL,
    CONSTRAINT CHK_PhieuChamSoc_PhanCap CHECK (PhanCapChamSoc IN (N'Cấp I', N'Cấp II', N'Cấp III')),
        CONSTRAINT FK_PCS_BenhNhan FOREIGN KEY (MaNguoiBenh) REFERENCES dbo.BenhNhan(MaNguoiBenh),
        CONSTRAINT FK_PCS_Khoa FOREIGN KEY (MaKhoa) REFERENCES dbo.Khoa(MaKhoa),
        CONSTRAINT FK_PCS_Giuong FOREIGN KEY (MaGiuong) REFERENCES dbo.Giuong(MaGiuong)
    );
    CREATE INDEX IX_PCS_MaNguoiBenh_ThoiGian ON dbo.PhieuChamSoc(MaNguoiBenh, ThoiGian DESC);
    CREATE INDEX IX_PCS_ThoiGian ON dbo.PhieuChamSoc(ThoiGian DESC);
END
GO

-- 1. DanhGia_ToanTrang
IF OBJECT_ID(N'dbo.DanhGia_ToanTrang', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.DanhGia_ToanTrang (
        ID INT IDENTITY(1,1) PRIMARY KEY,
        IDPhieu INT NOT NULL UNIQUE,
        TriGiac NVARCHAR(255) NULL,
        DiemGlasgow INT NULL,
        MauSacDa NVARCHAR(255) NULL,
        TinhTrangDa NVARCHAR(255) NULL,
        TinhTrangPhu BIT NULL,
        ViTriPhu NVARCHAR(MAX) NULL,
        CONSTRAINT FK_DGTT_PCS FOREIGN KEY (IDPhieu) REFERENCES dbo.PhieuChamSoc(IDPhieu) ON DELETE CASCADE
    );
    CREATE INDEX IX_DGTT_IDPhieu ON dbo.DanhGia_ToanTrang(IDPhieu);
END
GO

-- 2. DanhGia_DauHieuSinhTon
IF OBJECT_ID(N'dbo.DanhGia_DauHieuSinhTon', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.DanhGia_DauHieuSinhTon (
        ID INT IDENTITY(1,1) PRIMARY KEY,
        IDPhieu INT NOT NULL UNIQUE,
        TanSoMach INT NULL,
        HuyetAp VARCHAR(20) NULL,
        TanSoTho INT NULL,
        SpO2 INT NULL,
        CONSTRAINT FK_DHDST_PCS FOREIGN KEY (IDPhieu) REFERENCES dbo.PhieuChamSoc(IDPhieu) ON DELETE CASCADE
    );
    CREATE INDEX IX_DHDST_IDPhieu ON dbo.DanhGia_DauHieuSinhTon(IDPhieu);
END
GO

-- 3. DanhGia_ChiSoCoThe
IF OBJECT_ID(N'dbo.DanhGia_ChiSoCoThe', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.DanhGia_ChiSoCoThe (
        ID INT IDENTITY(1,1) PRIMARY KEY,
        IDPhieu INT NOT NULL UNIQUE,
        ChieuCao DECIMAL(5, 2) NULL,
        CanNang DECIMAL(5, 2) NULL,
        BMI DECIMAL(4, 2) NULL,
        CONSTRAINT FK_CSCT_PCS FOREIGN KEY (IDPhieu) REFERENCES dbo.PhieuChamSoc(IDPhieu) ON DELETE CASCADE
    );
    CREATE INDEX IX_CSCT_IDPhieu ON dbo.DanhGia_ChiSoCoThe(IDPhieu);
END
GO

-- 4. DanhGia_TuanHoan
IF OBJECT_ID(N'dbo.DanhGia_TuanHoan', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.DanhGia_TuanHoan (
        ID INT IDENTITY(1,1) PRIMARY KEY,
        IDPhieu INT NOT NULL UNIQUE,
        TinhChatMach NVARCHAR(255) NULL,
        DauHieuKhac NVARCHAR(MAX) NULL,
        CONSTRAINT FK_TH_PCS FOREIGN KEY (IDPhieu) REFERENCES dbo.PhieuChamSoc(IDPhieu) ON DELETE CASCADE
    );
    CREATE INDEX IX_TH_IDPhieu ON dbo.DanhGia_TuanHoan(IDPhieu);
END
GO

-- 5. DanhGia_HoHap
IF OBJECT_ID(N'dbo.DanhGia_HoHap', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.DanhGia_HoHap (
        ID INT IDENTITY(1,1) PRIMARY KEY,
        IDPhieu INT NOT NULL UNIQUE,
        TinhTrangHoHap NVARCHAR(MAX) NULL,
        KieuTho NVARCHAR(255) NULL,
        Ho NVARCHAR(255) NULL,
        Dom_SoLuong NVARCHAR(255) NULL,
        Dom_MauSac NVARCHAR(255) NULL,
        ThongKhiCoHoc_CheDoTho NVARCHAR(255) NULL,
        ThongKhiCoHoc_Vt INT NULL,
        ThongKhiCoHoc_PS INT NULL,
        ThongKhiCoHoc_PEEP INT NULL,
        ThongKhiCoHoc_TanSoTho INT NULL,
        ThongKhiCoHoc_FiO2 INT NULL,
        CONSTRAINT FK_HoHap_PCS FOREIGN KEY (IDPhieu) REFERENCES dbo.PhieuChamSoc(IDPhieu) ON DELETE CASCADE
    );
    CREATE INDEX IX_HoHap_IDPhieu ON dbo.DanhGia_HoHap(IDPhieu);
END
GO

-- 6. DanhGia_TieuHoa
IF OBJECT_ID(N'dbo.DanhGia_TieuHoa', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.DanhGia_TieuHoa (
        ID INT IDENTITY(1,1) PRIMARY KEY,
        IDPhieu INT NOT NULL UNIQUE,
        TinhTrangBung NVARCHAR(255) NULL,
        RoiLoanNuot BIT NULL,
        DayBungKhoTieu BIT NULL,
        Non BIT NULL,
        Non_ChiTiet NVARCHAR(MAX) NULL,
        NhuDongRuot BIT NULL,
        DaiTien NVARCHAR(MAX) NULL,
        DaiTien_ChiTiet NVARCHAR(MAX) NULL,
        HauMonNhanTao NVARCHAR(MAX) NULL,
        HMNT_NiemMac NVARCHAR(MAX) NULL,
        HMNT_DaXungQuanh NVARCHAR(MAX) NULL,
        CONSTRAINT FK_TieuHoa_PCS FOREIGN KEY (IDPhieu) REFERENCES dbo.PhieuChamSoc(IDPhieu) ON DELETE CASCADE
    );
    CREATE INDEX IX_TieuHoa_IDPhieu ON dbo.DanhGia_TieuHoa(IDPhieu);
END
GO

-- 7. DanhGia_DinhDuong
IF OBJECT_ID(N'dbo.DanhGia_DinhDuong', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.DanhGia_DinhDuong (
        ID INT IDENTITY(1,1) PRIMARY KEY,
        IDPhieu INT NOT NULL UNIQUE,
        NguyCoSuyDinhDuong BIT NULL,
        CheDoAn NVARCHAR(MAX) NULL,
        TinhTrangAn NVARCHAR(255) NULL,
        DuongAn NVARCHAR(MAX) NULL,
        CONSTRAINT FK_DinhDuong_PCS FOREIGN KEY (IDPhieu) REFERENCES dbo.PhieuChamSoc(IDPhieu) ON DELETE CASCADE
    );
    CREATE INDEX IX_DinhDuong_IDPhieu ON dbo.DanhGia_DinhDuong(IDPhieu);
END
GO

-- 8. DanhGia_TietNieuSinhDuc
IF OBJECT_ID(N'dbo.DanhGia_TietNieuSinhDuc', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.DanhGia_TietNieuSinhDuc (
        ID INT IDENTITY(1,1) PRIMARY KEY,
        IDPhieu INT NOT NULL UNIQUE,
        HinhThucTieu NVARCHAR(MAX) NULL,
        MauSacNuocTieu NVARCHAR(255) NULL,
        SoLuongNuocTieu VARCHAR(100) NULL,
        TieuRatBuot BIT NULL,
        BoPhanSinhDuc NVARCHAR(255) NULL,
        CONSTRAINT FK_TNSD_PCS FOREIGN KEY (IDPhieu) REFERENCES dbo.PhieuChamSoc(IDPhieu) ON DELETE CASCADE
    );
    CREATE INDEX IX_TNSD_IDPhieu ON dbo.DanhGia_TietNieuSinhDuc(IDPhieu);
END
GO

-- 9. DanhGia_ThanKinh
IF OBJECT_ID(N'dbo.DanhGia_ThanKinh', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.DanhGia_ThanKinh (
    ID INT IDENTITY(1,1) PRIMARY KEY,
    IDPhieu INT NOT NULL UNIQUE,
    LoiNoi NVARCHAR(255) NULL,
    YeuLiet NVARCHAR(MAX) NULL,
    RoiLoanVanDong NVARCHAR(MAX) NULL,
    RoiLoanVanDong_ThoiGian VARCHAR(100) NULL,
    DauHieuKhac NVARCHAR(MAX) NULL,
        CONSTRAINT FK_ThanKinh_PCS FOREIGN KEY (IDPhieu) REFERENCES dbo.PhieuChamSoc(IDPhieu) ON DELETE CASCADE
    );
    CREATE INDEX IX_ThanKinh_IDPhieu ON dbo.DanhGia_ThanKinh(IDPhieu);
END
GO

-- 10. DanhGia_TinhThanGiacNgu
IF OBJECT_ID(N'dbo.DanhGia_TinhThanGiacNgu', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.DanhGia_TinhThanGiacNgu (
        ID INT IDENTITY(1,1) PRIMARY KEY,
        IDPhieu INT NOT NULL UNIQUE,
        TinhThan NVARCHAR(255) NULL,
        GiacNgu NVARCHAR(255) NULL,
        ThoiGianNgu VARCHAR(100) NULL,
        CONSTRAINT FK_TTGN_PCS FOREIGN KEY (IDPhieu) REFERENCES dbo.PhieuChamSoc(IDPhieu) ON DELETE CASCADE
    );
    CREATE INDEX IX_TTGN_IDPhieu ON dbo.DanhGia_TinhThanGiacNgu(IDPhieu);
END
GO

-- 11. DanhGia_CoXuongKhop
IF OBJECT_ID(N'dbo.DanhGia_CoXuongKhop', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.DanhGia_CoXuongKhop (
        ID INT IDENTITY(1,1) PRIMARY KEY,
        IDPhieu INT NOT NULL UNIQUE,
        VanDong NVARCHAR(255) NULL,
        VanDeKhac NVARCHAR(MAX) NULL,
        VanDeKhac_ViTri NVARCHAR(255) NULL,
        CONSTRAINT FK_CXK_PCS FOREIGN KEY (IDPhieu) REFERENCES dbo.PhieuChamSoc(IDPhieu) ON DELETE CASCADE
    );
    CREATE INDEX IX_CXK_IDPhieu ON dbo.DanhGia_CoXuongKhop(IDPhieu);
END
GO

-- 12. DanhGia_NhanDinhKhac
IF OBJECT_ID(N'dbo.DanhGia_NhanDinhKhac', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.DanhGia_NhanDinhKhac (
        ID INT IDENTITY(1,1) PRIMARY KEY,
        IDPhieu INT NOT NULL UNIQUE,
        Dau_ViTri NVARCHAR(MAX) NULL,
        Dau_ThangDiem INT NULL,
        Dau_Khac NVARCHAR(MAX) NULL,
        VetThuongLoet_ViTri NVARCHAR(MAX) NULL,
        VetThuongLoet_KichThuoc VARCHAR(255) NULL,
        VetThuongLoet_MoTa NVARCHAR(MAX) NULL,
        DanLuu_ViTri NVARCHAR(MAX) NULL,
        DanLuu_MauSac NVARCHAR(255) NULL,
        DanLuu_SoLuong VARCHAR(255) NULL,
        DanLuu_TinhChat NVARCHAR(255) NULL,
        NguyCoNga BIT NULL,
        CanhBaoSom NVARCHAR(MAX) NULL,
        CONSTRAINT FK_NDK_PCS FOREIGN KEY (IDPhieu) REFERENCES dbo.PhieuChamSoc(IDPhieu) ON DELETE CASCADE
    );
    CREATE INDEX IX_NDK_IDPhieu ON dbo.DanhGia_NhanDinhKhac(IDPhieu);
END
GO

-- 13. ChanDoanDieuDuong (nhiều dòng/phiếu)
IF OBJECT_ID(N'dbo.ChanDoanDieuDuong', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.ChanDoanDieuDuong (
        IDChanDoan INT IDENTITY(1,1) PRIMARY KEY,
        IDPhieu INT NOT NULL,
        NoiDungChanDoan NVARCHAR(MAX) NOT NULL,
        MucTieu NVARCHAR(MAX) NULL,
        TrangThai NVARCHAR(20) NOT NULL,
        CONSTRAINT CHK_ChanDoanDieuDuong_TrangThai CHECK (TrangThai IN (N'Bắt đầu', N'Tiếp tục', N'Kết thúc')),
        CONSTRAINT FK_CDDD_PCS FOREIGN KEY (IDPhieu) REFERENCES dbo.PhieuChamSoc(IDPhieu) ON DELETE CASCADE
    );
    CREATE INDEX IX_CDDD_IDPhieu ON dbo.ChanDoanDieuDuong(IDPhieu);
END
GO

-- 14. CanThiepDieuDuong (tuỳ chọn)
IF OBJECT_ID(N'dbo.CanThiepDieuDuong', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.CanThiepDieuDuong (
        IDCanThiep INT IDENTITY(1,1) PRIMARY KEY,
        IDPhieu INT NOT NULL,
        LoaiCanThiep NVARCHAR(50) NOT NULL,
        NoiDungCanThiep NVARCHAR(MAX) NOT NULL,
        CONSTRAINT CHK_CanThiepDieuDuong_Loai CHECK (LoaiCanThiep IN (N'Thực hiện y lệnh', N'Chỉ định CLS', N'Chăm sóc điều dưỡng')),
        CONSTRAINT FK_CTDD_PCS FOREIGN KEY (IDPhieu) REFERENCES dbo.PhieuChamSoc(IDPhieu) ON DELETE CASCADE
    );
    CREATE INDEX IX_CTDD_IDPhieu ON dbo.CanThiepDieuDuong(IDPhieu);
END
GO

-- Gợi ý dữ liệu mẫu (chạy nếu cần)
-- INSERT INTO dbo.Khoa(TenKhoa) VALUES (N'HSCC'), (N'Nội tổng hợp');
-- INSERT INTO dbo.BenhNhan(MaNguoiBenh, HoTen, Tuoi, GioiTinh, MaKhoa, ChanDoan) VALUES
-- (N'BN001', N'Nguyễn Văn A', 60, N'Nam', 1, N'Viêm phổi');
-- INSERT INTO dbo.PhieuChamSoc(MaNguoiBenh, MaKhoa, ThoiGian) VALUES (N'BN001', 1, SYSUTCDATETIME());

-- ===============================
-- SEED DỮ LIỆU MẪU (FAKE DATA)
-- ===============================
-- Có thể chạy nhiều lần, đã có IF NOT EXISTS để tránh trùng

-- Khoa
IF NOT EXISTS (SELECT 1 FROM dbo.Khoa)
BEGIN
    INSERT INTO dbo.Khoa(TenKhoa)
    VALUES (N'Hồi sức cấp cứu'), (N'Nội tổng hợp'), (N'Ngoại tổng hợp');
END
GO

-- Phòng (tuỳ chọn)
IF OBJECT_ID(N'dbo.Phong', 'U') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.Phong)
BEGIN
    INSERT INTO dbo.Phong(TenPhong, MaKhoa)
    SELECT N'Phòng A1', MaKhoa FROM dbo.Khoa WHERE TenKhoa = N'Hồi sức cấp cứu';
    INSERT INTO dbo.Phong(TenPhong, MaKhoa)
    SELECT N'Phòng B2', MaKhoa FROM dbo.Khoa WHERE TenKhoa = N'Nội tổng hợp';
END
GO

-- Giường (tuỳ chọn)
IF OBJECT_ID(N'dbo.Giuong', 'U') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.Giuong)
BEGIN
    INSERT INTO dbo.Giuong(TenGiuong, MaPhong)
    SELECT N'Giường 01', MaPhong FROM dbo.Phong WHERE TenPhong = N'Phòng A1';
    INSERT INTO dbo.Giuong(TenGiuong, MaPhong)
    SELECT N'Giường 02', MaPhong FROM dbo.Phong WHERE TenPhong = N'Phòng B2';
END
GO

-- Bệnh nhân
IF NOT EXISTS (SELECT 1 FROM dbo.BenhNhan WHERE MaNguoiBenh = N'BN001')
BEGIN
    INSERT INTO dbo.BenhNhan(MaNguoiBenh, SoVaoVien, HoTen, Tuoi, GioiTinh, MaKhoa, ChanDoan)
    SELECT N'BN001', N'SVV001', N'Nguyễn Văn A', 65, N'Nam', MaKhoa, N'Viêm phổi nặng'
    FROM dbo.Khoa WHERE TenKhoa = N'Hồi sức cấp cứu';
END
IF NOT EXISTS (SELECT 1 FROM dbo.BenhNhan WHERE MaNguoiBenh = N'BN002')
BEGIN
    INSERT INTO dbo.BenhNhan(MaNguoiBenh, SoVaoVien, HoTen, Tuoi, GioiTinh, MaKhoa, ChanDoan)
    SELECT N'BN002', N'SVV002', N'Trần Thị B', 54, N'Nữ', MaKhoa, N'Đái tháo đường type 2'
    FROM dbo.Khoa WHERE TenKhoa = N'Nội tổng hợp';
END
GO

-- Một phiếu chăm sóc cho mỗi bệnh nhân (đủ để các form lấy TOP 1)
IF NOT EXISTS (SELECT 1 FROM dbo.PhieuChamSoc WHERE MaNguoiBenh = N'BN001')
BEGIN
    INSERT INTO dbo.PhieuChamSoc(MaNguoiBenh, ThoiGian, MaKhoa, MaGiuong, ToSo, PhanCapChamSoc, BanGiao, DieuDuongThucHien)
    SELECT N'BN001', DATEADD(minute, -10, SYSUTCDATETIME()), k.MaKhoa, g.MaGiuong, 1, N'Cấp I', N'Ổn định, theo dõi sát', N'Điều dưỡng A'
    FROM dbo.Khoa k
    LEFT JOIN dbo.Giuong g ON 1=0
    WHERE k.TenKhoa = N'Hồi sức cấp cứu';
END
IF NOT EXISTS (SELECT 1 FROM dbo.PhieuChamSoc WHERE MaNguoiBenh = N'BN002')
BEGIN
    INSERT INTO dbo.PhieuChamSoc(MaNguoiBenh, ThoiGian, MaKhoa, MaGiuong, ToSo, PhanCapChamSoc, BanGiao, DieuDuongThucHien)
    SELECT N'BN002', DATEADD(minute, -5, SYSUTCDATETIME()), k.MaKhoa, g.MaGiuong, 1, N'Cấp II', N'Kiểm soát đường huyết', N'Điều dưỡng B'
    FROM dbo.Khoa k
    LEFT JOIN dbo.Giuong g ON 1=0
    WHERE k.TenKhoa = N'Nội tổng hợp';
END
GO

-- Lấy IDPhieu mới nhất cho từng bệnh nhân để seed các bảng đánh giá
DECLARE @IDPhieu_BN001 INT = (SELECT TOP 1 IDPhieu FROM dbo.PhieuChamSoc WHERE MaNguoiBenh = N'BN001' ORDER BY ThoiGian DESC);
DECLARE @IDPhieu_BN002 INT = (SELECT TOP 1 IDPhieu FROM dbo.PhieuChamSoc WHERE MaNguoiBenh = N'BN002' ORDER BY ThoiGian DESC);

-- 1) DanhGia_ToanTrang
IF @IDPhieu_BN001 IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.DanhGia_ToanTrang WHERE IDPhieu = @IDPhieu_BN001)
BEGIN
    INSERT INTO dbo.DanhGia_ToanTrang(IDPhieu, TriGiac, DiemGlasgow, MauSacDa, TinhTrangDa, TinhTrangPhu, ViTriPhu)
    VALUES(@IDPhieu_BN001, N'Tỉnh táo', 15, N'Hồng', N'Bình thường', 0, NULL);
END
IF @IDPhieu_BN002 IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.DanhGia_ToanTrang WHERE IDPhieu = @IDPhieu_BN002)
BEGIN
    INSERT INTO dbo.DanhGia_ToanTrang(IDPhieu, TriGiac, DiemGlasgow, MauSacDa, TinhTrangDa, TinhTrangPhu, ViTriPhu)
    VALUES(@IDPhieu_BN002, N'Lú lẫn', 13, N'Nhợt nhạt', N'Râm, lở', 1, N'Chi dưới');
END

-- 2) Dấu hiệu sinh tồn
IF @IDPhieu_BN001 IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.DanhGia_DauHieuSinhTon WHERE IDPhieu = @IDPhieu_BN001)
BEGIN
    INSERT INTO dbo.DanhGia_DauHieuSinhTon(IDPhieu, TanSoMach, HuyetAp, TanSoTho, SpO2)
    VALUES(@IDPhieu_BN001, 88, N'120/80', 18, 97);
END
IF @IDPhieu_BN002 IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.DanhGia_DauHieuSinhTon WHERE IDPhieu = @IDPhieu_BN002)
BEGIN
    INSERT INTO dbo.DanhGia_DauHieuSinhTon(IDPhieu, TanSoMach, HuyetAp, TanSoTho, SpO2)
    VALUES(@IDPhieu_BN002, 92, N'140/90', 20, 95);
END

-- 3) Chỉ số cơ thể
IF @IDPhieu_BN001 IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.DanhGia_ChiSoCoThe WHERE IDPhieu = @IDPhieu_BN001)
BEGIN
    INSERT INTO dbo.DanhGia_ChiSoCoThe(IDPhieu, ChieuCao, CanNang, BMI)
    VALUES(@IDPhieu_BN001, 170.0, 65.0, 22.5);
END
IF @IDPhieu_BN002 IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.DanhGia_ChiSoCoThe WHERE IDPhieu = @IDPhieu_BN002)
BEGIN
    INSERT INTO dbo.DanhGia_ChiSoCoThe(IDPhieu, ChieuCao, CanNang, BMI)
    VALUES(@IDPhieu_BN002, 158.0, 70.0, 28.0);
END

-- 4) Tuần hoàn
IF @IDPhieu_BN001 IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.DanhGia_TuanHoan WHERE IDPhieu = @IDPhieu_BN001)
BEGIN
    INSERT INTO dbo.DanhGia_TuanHoan(IDPhieu, TinhChatMach, DauHieuKhac)
    VALUES(@IDPhieu_BN001, N'Đều Rõ', N'Chưa Ghi Nhận');
END
IF @IDPhieu_BN002 IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.DanhGia_TuanHoan WHERE IDPhieu = @IDPhieu_BN002)
BEGIN
    INSERT INTO dbo.DanhGia_TuanHoan(IDPhieu, TinhChatMach, DauHieuKhac)
    VALUES(@IDPhieu_BN002, N'Rời Rạc', N'Đau Ngực');
END

-- 5) Hô hấp
IF @IDPhieu_BN001 IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.DanhGia_HoHap WHERE IDPhieu = @IDPhieu_BN001)
BEGIN
    INSERT INTO dbo.DanhGia_HoHap(IDPhieu, TinhTrangHoHap, KieuTho, Ho, Dom_SoLuong, Dom_MauSac, ThongKhiCoHoc_CheDoTho, ThongKhiCoHoc_Vt, ThongKhiCoHoc_PS, ThongKhiCoHoc_PEEP, ThongKhiCoHoc_TanSoTho, ThongKhiCoHoc_FiO2)
    VALUES(@IDPhieu_BN001, N'Bình thường', N'Bình Thường', N'Không Ho', N'Ít', N'Trắng Đục', NULL, NULL, NULL, NULL, NULL, NULL);
END
IF @IDPhieu_BN002 IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.DanhGia_HoHap WHERE IDPhieu = @IDPhieu_BN002)
BEGIN
    INSERT INTO dbo.DanhGia_HoHap(IDPhieu, TinhTrangHoHap, KieuTho, Ho, Dom_SoLuong, Dom_MauSac, ThongKhiCoHoc_CheDoTho, ThongKhiCoHoc_Vt, ThongKhiCoHoc_PS, ThongKhiCoHoc_PEEP, ThongKhiCoHoc_TanSoTho, ThongKhiCoHoc_FiO2)
    VALUES(@IDPhieu_BN002, N'Thở Oxi qua mask: 5 l/p', N'Bình Thường', N'Ho Có Đờm', N'Nhiều', N'Vàng', NULL, NULL, NULL, NULL, NULL, NULL);
END

-- 6) Tiêu hoá
IF @IDPhieu_BN001 IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.DanhGia_TieuHoa WHERE IDPhieu = @IDPhieu_BN001)
BEGIN
    INSERT INTO dbo.DanhGia_TieuHoa(IDPhieu, TinhTrangBung, RoiLoanNuot, DayBungKhoTieu, Non, Non_ChiTiet, NhuDongRuot, DaiTien, DaiTien_ChiTiet, HauMonNhanTao, HMNT_NiemMac, HMNT_DaXungQuanh)
    VALUES(@IDPhieu_BN001, N'Bụng Mềm', 0, 0, 0, NULL, 1, N'Bình thường', NULL, N'Không', N'Hồng', N'Bình Thường');
END
IF @IDPhieu_BN002 IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.DanhGia_TieuHoa WHERE IDPhieu = @IDPhieu_BN002)
BEGIN
    INSERT INTO dbo.DanhGia_TieuHoa(IDPhieu, TinhTrangBung, RoiLoanNuot, DayBungKhoTieu, Non, Non_ChiTiet, NhuDongRuot, DaiTien, DaiTien_ChiTiet, HauMonNhanTao, HMNT_NiemMac, HMNT_DaXungQuanh)
    VALUES(@IDPhieu_BN002, N'Bụng Chướng', 1, 1, 0, NULL, 0, N'Táo bón', N'3 ngày', N'Không', N'Tím Tái', N'Viêm Đỏ');
END

-- 7) Dinh dưỡng
IF @IDPhieu_BN001 IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.DanhGia_DinhDuong WHERE IDPhieu = @IDPhieu_BN001)
BEGIN
    INSERT INTO dbo.DanhGia_DinhDuong(IDPhieu, NguyCoSuyDinhDuong, CheDoAn, TinhTrangAn, DuongAn)
    VALUES(@IDPhieu_BN001, 0, N'Ăn mềm', N'Ăn Hết Suất', N'Ăn Qua Miệng');
END
IF @IDPhieu_BN002 IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.DanhGia_DinhDuong WHERE IDPhieu = @IDPhieu_BN002)
BEGIN
    INSERT INTO dbo.DanhGia_DinhDuong(IDPhieu, NguyCoSuyDinhDuong, CheDoAn, TinhTrangAn, DuongAn)
    VALUES(@IDPhieu_BN002, 1, N'Ăn kiêng đường', N'Ăn 1/2 Suất', N'Ăn Qua Miệng');
END

-- 8) Tiết niệu - sinh dục
IF @IDPhieu_BN001 IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.DanhGia_TietNieuSinhDuc WHERE IDPhieu = @IDPhieu_BN001)
BEGIN
    INSERT INTO dbo.DanhGia_TietNieuSinhDuc(IDPhieu, HinhThucTieu, MauSacNuocTieu, SoLuongNuocTieu, TieuRatBuot, BoPhanSinhDuc)
    VALUES(@IDPhieu_BN001, N'Nhịn Tiểu', N'Trong', N'~1500ml/ngày', 0, N'Sạch');
END
IF @IDPhieu_BN002 IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.DanhGia_TietNieuSinhDuc WHERE IDPhieu = @IDPhieu_BN002)
BEGIN
    INSERT INTO dbo.DanhGia_TietNieuSinhDuc(IDPhieu, HinhThucTieu, MauSacNuocTieu, SoLuongNuocTieu, TieuRatBuot, BoPhanSinhDuc)
    VALUES(@IDPhieu_BN002, N'Tiểu Qua Ông Thông', N'Vàng', N'~1200ml/ngày', 1, N'Bẩn');
END

-- 9) Thần kinh
IF @IDPhieu_BN001 IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.DanhGia_ThanKinh WHERE IDPhieu = @IDPhieu_BN001)
BEGIN
    INSERT INTO dbo.DanhGia_ThanKinh(IDPhieu, LoiNoi, YeuLiet, RoiLoanVanDong, RoiLoanVanDong_ThoiGian, DauHieuKhac)
    VALUES(@IDPhieu_BN001, N'Rõ Ràng', N'Không', N'Không', NULL, N'Chưa Ghi Nhận');
END
IF @IDPhieu_BN002 IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.DanhGia_ThanKinh WHERE IDPhieu = @IDPhieu_BN002)
BEGIN
    INSERT INTO dbo.DanhGia_ThanKinh(IDPhieu, LoiNoi, YeuLiet, RoiLoanVanDong, RoiLoanVanDong_ThoiGian, DauHieuKhac)
    VALUES(@IDPhieu_BN002, N'Nói Đớ', N'1/2 Người (P)', N'Co Giật', N'05 phút', N'Đau Đầu');
END

-- 10) Tinh thần - giấc ngủ
IF @IDPhieu_BN001 IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.DanhGia_TinhThanGiacNgu WHERE IDPhieu = @IDPhieu_BN001)
BEGIN
    INSERT INTO dbo.DanhGia_TinhThanGiacNgu(IDPhieu, TinhThan, GiacNgu, ThoiGianNgu)
    VALUES(@IDPhieu_BN001, N'Bình Thường', N'Bình Thường', NULL);
END
IF @IDPhieu_BN002 IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.DanhGia_TinhThanGiacNgu WHERE IDPhieu = @IDPhieu_BN002)
BEGIN
    INSERT INTO dbo.DanhGia_TinhThanGiacNgu(IDPhieu, TinhThan, GiacNgu, ThoiGianNgu)
    VALUES(@IDPhieu_BN002, N'Lo Lắng', N'Mất Ngủ', N'3 giờ');
END

-- 11) Cơ xương khớp
IF @IDPhieu_BN001 IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.DanhGia_CoXuongKhop WHERE IDPhieu = @IDPhieu_BN001)
BEGIN
    INSERT INTO dbo.DanhGia_CoXuongKhop(IDPhieu, VanDong, VanDeKhac, VanDeKhac_ViTri)
    VALUES(@IDPhieu_BN001, N'Bình Thường', N'Không', NULL);
END
IF @IDPhieu_BN002 IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.DanhGia_CoXuongKhop WHERE IDPhieu = @IDPhieu_BN002)
BEGIN
    INSERT INTO dbo.DanhGia_CoXuongKhop(IDPhieu, VanDong, VanDeKhac, VanDeKhac_ViTri)
    VALUES(@IDPhieu_BN002, N'Hạn Chế', N'Sưng Khớp', N'Gối (P)');
END

-- 12) Nhận định khác
IF @IDPhieu_BN001 IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.DanhGia_NhanDinhKhac WHERE IDPhieu = @IDPhieu_BN001)
BEGIN
    INSERT INTO dbo.DanhGia_NhanDinhKhac(IDPhieu, Dau_ViTri, Dau_ThangDiem, Dau_Khac, VetThuongLoet_ViTri, VetThuongLoet_KichThuoc, VetThuongLoet_MoTa, DanLuu_ViTri, DanLuu_MauSac, DanLuu_SoLuong, DanLuu_TinhChat, NguyCoNga, CanhBaoSom)
    VALUES(@IDPhieu_BN001, N'Không', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0, N'Không');
END
IF @IDPhieu_BN002 IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.DanhGia_NhanDinhKhac WHERE IDPhieu = @IDPhieu_BN002)
BEGIN
    INSERT INTO dbo.DanhGia_NhanDinhKhac(IDPhieu, Dau_ViTri, Dau_ThangDiem, Dau_Khac, VetThuongLoet_ViTri, VetThuongLoet_KichThuoc, VetThuongLoet_MoTa, DanLuu_ViTri, DanLuu_MauSac, DanLuu_SoLuong, DanLuu_TinhChat, NguyCoNga, CanhBaoSom)
    VALUES(@IDPhieu_BN002, N'Bụng', 5, N'Âm ỉ', N'Gót chân (T)', N'2x3 cm', N'Loét độ 2', N'Màng phổi', N'Đỏ sẫm', N'Ít', N'Dịch máu', 1, N'Cảnh báo tụt huyết áp');
END

-- 13) Chuẩn đoán điều dưỡng
IF @IDPhieu_BN001 IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.ChanDoanDieuDuong WHERE IDPhieu = @IDPhieu_BN001)
BEGIN
    INSERT INTO dbo.ChanDoanDieuDuong(IDPhieu, NoiDungChanDoan, MucTieu, TrangThai)
    VALUES(@IDPhieu_BN001, N'Nguy cơ suy hô hấp', N'Duy trì SpO2 > 95%', N'Bắt đầu');
END
IF @IDPhieu_BN002 IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.ChanDoanDieuDuong WHERE IDPhieu = @IDPhieu_BN002)
BEGIN
    INSERT INTO dbo.ChanDoanDieuDuong(IDPhieu, NoiDungChanDoan, MucTieu, TrangThai)
    VALUES(@IDPhieu_BN002, N'Rối loạn đường huyết', N'Duy trì đường huyết 6-10 mmol/L', N'Tiếp tục');
END
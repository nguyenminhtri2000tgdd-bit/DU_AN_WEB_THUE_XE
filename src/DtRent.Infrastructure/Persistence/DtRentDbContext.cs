using System;
using System.Collections.Generic;
using DtRent.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace DtRent.Infrastructure.Persistence;

public partial class DtRentDbContext : DbContext
{
    public DtRentDbContext(DbContextOptions<DtRentDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AnhXe> AnhXes { get; set; }

    public virtual DbSet<BaoTri> BaoTris { get; set; }

    public virtual DbSet<GplxKhachHang> GplxKhachHangs { get; set; }

    public virtual DbSet<HangGplx> HangGplxes { get; set; }

    public virtual DbSet<HangThanhVien> HangThanhViens { get; set; }

    public virtual DbSet<KhachHang> KhachHangs { get; set; }

    public virtual DbSet<NhanVien> NhanViens { get; set; }

    public virtual DbSet<NhomXe> NhomXes { get; set; }

    public virtual DbSet<PhieuThue> PhieuThues { get; set; }

    public virtual DbSet<TaiKhoan> TaiKhoans { get; set; }

    public virtual DbSet<ThanhToan> ThanhToans { get; set; }

    public virtual DbSet<TraXe> TraXes { get; set; }

    public virtual DbSet<Xe> Xes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("btree_gist");

        modelBuilder.Entity<AnhXe>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("anh_xe_pkey");

            entity.ToTable("anh_xe");

            entity.HasIndex(e => e.XeId, "uq_anh_xe_anh_chinh")
                .IsUnique()
                .HasFilter("la_anh_chinh");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.DuongDan)
                .HasMaxLength(500)
                .HasColumnName("duong_dan");
            entity.Property(e => e.LaAnhChinh).HasColumnName("la_anh_chinh");
            entity.Property(e => e.XeId).HasColumnName("xe_id");

            entity.HasOne(d => d.Xe).WithOne(p => p.AnhXe)
                .HasForeignKey<AnhXe>(d => d.XeId)
                .HasConstraintName("anh_xe_xe_id_fkey");
        });

        modelBuilder.Entity<BaoTri>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("bao_tri_pkey");

            entity.ToTable("bao_tri");

            entity.HasIndex(e => e.XeId, "ix_bao_tri_xe");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.ChiPhi)
                .HasPrecision(14)
                .HasColumnName("chi_phi");
            entity.Property(e => e.Loai)
                .HasMaxLength(15)
                .HasColumnName("loai");
            entity.Property(e => e.MoTa).HasColumnName("mo_ta");
            entity.Property(e => e.NgayBatDau).HasColumnName("ngay_bat_dau");
            entity.Property(e => e.NgayKetThuc).HasColumnName("ngay_ket_thuc");
            entity.Property(e => e.NhanVienId).HasColumnName("nhan_vien_id");
            entity.Property(e => e.TrangThai)
                .HasMaxLength(15)
                .HasDefaultValueSql("'DANG_THUC_HIEN'::character varying")
                .HasColumnName("trang_thai");
            entity.Property(e => e.XeId).HasColumnName("xe_id");

            entity.HasOne(d => d.NhanVien).WithMany(p => p.BaoTris)
                .HasForeignKey(d => d.NhanVienId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("bao_tri_nhan_vien_id_fkey");

            entity.HasOne(d => d.Xe).WithMany(p => p.BaoTris)
                .HasForeignKey(d => d.XeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("bao_tri_xe_id_fkey");
        });

        modelBuilder.Entity<GplxKhachHang>(entity =>
        {
            entity.HasKey(e => new { e.KhachHangId, e.HangGplxMa }).HasName("gplx_khach_hang_pkey");

            entity.ToTable("gplx_khach_hang");

            entity.Property(e => e.KhachHangId).HasColumnName("khach_hang_id");
            entity.Property(e => e.HangGplxMa)
                .HasMaxLength(10)
                .HasColumnName("hang_gplx_ma");
            entity.Property(e => e.NgayHetHan).HasColumnName("ngay_het_han");
            entity.Property(e => e.SoGplx)
                .HasMaxLength(20)
                .HasColumnName("so_gplx");

            entity.HasOne(d => d.HangGplxMaNavigation).WithMany(p => p.GplxKhachHangs)
                .HasForeignKey(d => d.HangGplxMa)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("gplx_khach_hang_hang_gplx_ma_fkey");

            entity.HasOne(d => d.KhachHang).WithMany(p => p.GplxKhachHangs)
                .HasForeignKey(d => d.KhachHangId)
                .HasConstraintName("gplx_khach_hang_khach_hang_id_fkey");
        });

        modelBuilder.Entity<HangGplx>(entity =>
        {
            entity.HasKey(e => e.Ma).HasName("hang_gplx_pkey");

            entity.ToTable("hang_gplx");

            entity.Property(e => e.Ma)
                .HasMaxLength(10)
                .HasColumnName("ma");
            entity.Property(e => e.MoTa)
                .HasMaxLength(150)
                .HasColumnName("mo_ta");
        });

        modelBuilder.Entity<HangThanhVien>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("hang_thanh_vien_pkey");

            entity.ToTable("hang_thanh_vien");

            entity.HasIndex(e => e.Ma, "hang_thanh_vien_ma_key").IsUnique();

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.Ma)
                .HasMaxLength(20)
                .HasColumnName("ma");
            entity.Property(e => e.PhanTramGiam)
                .HasPrecision(5, 2)
                .HasColumnName("phan_tram_giam");
            entity.Property(e => e.Ten)
                .HasMaxLength(50)
                .HasColumnName("ten");
        });

        modelBuilder.Entity<KhachHang>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("khach_hang_pkey");

            entity.ToTable("khach_hang");

            entity.HasIndex(e => e.SoDienThoai, "ix_khach_hang_sdt");

            entity.HasIndex(e => e.MaKhach, "khach_hang_ma_khach_key").IsUnique();

            entity.HasIndex(e => e.SoCccd, "khach_hang_so_cccd_key").IsUnique();

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.DiaChi)
                .HasMaxLength(255)
                .HasColumnName("dia_chi");
            entity.Property(e => e.HangThanhVienId).HasColumnName("hang_thanh_vien_id");
            entity.Property(e => e.HoTen)
                .HasMaxLength(100)
                .HasColumnName("ho_ten");
            entity.Property(e => e.MaKhach)
                .HasMaxLength(20)
                .HasColumnName("ma_khach");
            entity.Property(e => e.NgayTao)
                .HasDefaultValueSql("now()")
                .HasColumnName("ngay_tao");
            entity.Property(e => e.SoCccd)
                .HasMaxLength(12)
                .HasColumnName("so_cccd");
            entity.Property(e => e.SoDienThoai)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("so_dien_thoai");

            entity.HasOne(d => d.HangThanhVien).WithMany(p => p.KhachHangs)
                .HasForeignKey(d => d.HangThanhVienId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("khach_hang_hang_thanh_vien_id_fkey");
        });

        modelBuilder.Entity<NhanVien>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("nhan_vien_pkey");

            entity.ToTable("nhan_vien");

            entity.HasIndex(e => e.MaNhanVien, "nhan_vien_ma_nhan_vien_key").IsUnique();

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.DangHoatDong)
                .HasDefaultValue(true)
                .HasColumnName("dang_hoat_dong");
            entity.Property(e => e.HoTen)
                .HasMaxLength(100)
                .HasColumnName("ho_ten");
            entity.Property(e => e.MaNhanVien)
                .HasMaxLength(20)
                .HasColumnName("ma_nhan_vien");
            entity.Property(e => e.NgayTao)
                .HasDefaultValueSql("now()")
                .HasColumnName("ngay_tao");
            entity.Property(e => e.SoDienThoai)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("so_dien_thoai");
        });

        modelBuilder.Entity<NhomXe>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("nhom_xe_pkey");

            entity.ToTable("nhom_xe");

            entity.HasIndex(e => e.Ma, "nhom_xe_ma_key").IsUnique();

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.ChinhSachGia)
                .HasMaxLength(40)
                .HasColumnName("chinh_sach_gia");
            entity.Property(e => e.GiaThueNgay)
                .HasPrecision(14)
                .HasColumnName("gia_thue_ngay");
            entity.Property(e => e.KmToiDaMoiNgay).HasColumnName("km_toi_da_moi_ngay");
            entity.Property(e => e.LoaiNhienLieu)
                .HasMaxLength(10)
                .HasColumnName("loai_nhien_lieu");
            entity.Property(e => e.Ma)
                .HasMaxLength(30)
                .HasColumnName("ma");
            entity.Property(e => e.PhiTreGio)
                .HasPrecision(14)
                .HasColumnName("phi_tre_gio");
            entity.Property(e => e.PhiVuotKm)
                .HasPrecision(14)
                .HasColumnName("phi_vuot_km");
            entity.Property(e => e.SoCho).HasColumnName("so_cho");
            entity.Property(e => e.Ten)
                .HasMaxLength(100)
                .HasColumnName("ten");
            entity.Property(e => e.TienCoc)
                .HasPrecision(14)
                .HasColumnName("tien_coc");

            entity.HasMany(d => d.HangGplxMas).WithMany(p => p.NhomXes)
                .UsingEntity<Dictionary<string, object>>(
                    "NhomXeGplx",
                    r => r.HasOne<HangGplx>().WithMany()
                        .HasForeignKey("HangGplxMa")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("nhom_xe_gplx_hang_gplx_ma_fkey"),
                    l => l.HasOne<NhomXe>().WithMany()
                        .HasForeignKey("NhomXeId")
                        .HasConstraintName("nhom_xe_gplx_nhom_xe_id_fkey"),
                    j =>
                    {
                        j.HasKey("NhomXeId", "HangGplxMa").HasName("nhom_xe_gplx_pkey");
                        j.ToTable("nhom_xe_gplx");
                        j.IndexerProperty<short>("NhomXeId").HasColumnName("nhom_xe_id");
                        j.IndexerProperty<string>("HangGplxMa")
                            .HasMaxLength(10)
                            .HasColumnName("hang_gplx_ma");
                    });
        });

        modelBuilder.Entity<PhieuThue>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("phieu_thue_pkey");

            entity.ToTable("phieu_thue");

            entity.HasIndex(e => e.KhachHangId, "ix_phieu_thue_khach");

            entity.HasIndex(e => e.ThoiDiemNhan, "ix_phieu_thue_nhan");

            entity.HasIndex(e => e.TrangThai, "ix_phieu_thue_trang_thai");

            entity.HasIndex(e => e.XeId, "ix_phieu_thue_xe");

            entity.HasIndex(e => e.MaPhieu, "phieu_thue_ma_phieu_key").IsUnique();

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.DonGiaNgay)
                .HasPrecision(14)
                .HasColumnName("don_gia_ngay");
            entity.Property(e => e.GhiChu).HasColumnName("ghi_chu");
            entity.Property(e => e.KhachHangId).HasColumnName("khach_hang_id");
            entity.Property(e => e.KmToiDaMoiNgay).HasColumnName("km_toi_da_moi_ngay");
            entity.Property(e => e.MaPhieu)
                .HasMaxLength(20)
                .HasColumnName("ma_phieu");
            entity.Property(e => e.NgayTao)
                .HasDefaultValueSql("now()")
                .HasColumnName("ngay_tao");
            entity.Property(e => e.NhanVienId).HasColumnName("nhan_vien_id");
            entity.Property(e => e.PhanTramGiam)
                .HasPrecision(5, 2)
                .HasColumnName("phan_tram_giam");
            entity.Property(e => e.PhiTreGio)
                .HasPrecision(14)
                .HasColumnName("phi_tre_gio");
            entity.Property(e => e.PhiVuotKm)
                .HasPrecision(14)
                .HasColumnName("phi_vuot_km");
            entity.Property(e => e.SoKmNhan).HasColumnName("so_km_nhan");
            entity.Property(e => e.SoNgayThue).HasColumnName("so_ngay_thue");
            entity.Property(e => e.ThoiDiemHenTra).HasColumnName("thoi_diem_hen_tra");
            entity.Property(e => e.ThoiDiemNhan).HasColumnName("thoi_diem_nhan");
            entity.Property(e => e.TienCoc)
                .HasPrecision(14)
                .HasColumnName("tien_coc");
            entity.Property(e => e.TinhTrangPinNhan)
                .HasPrecision(5, 2)
                .HasColumnName("tinh_trang_pin_nhan");
            entity.Property(e => e.TrangThai)
                .HasMaxLength(15)
                .HasDefaultValueSql("'DANG_THUE'::character varying")
                .HasColumnName("trang_thai");
            entity.Property(e => e.XeId).HasColumnName("xe_id");

            entity.HasOne(d => d.KhachHang).WithMany(p => p.PhieuThues)
                .HasForeignKey(d => d.KhachHangId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("phieu_thue_khach_hang_id_fkey");

            entity.HasOne(d => d.NhanVien).WithMany(p => p.PhieuThues)
                .HasForeignKey(d => d.NhanVienId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("phieu_thue_nhan_vien_id_fkey");

            entity.HasOne(d => d.Xe).WithMany(p => p.PhieuThues)
                .HasForeignKey(d => d.XeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("phieu_thue_xe_id_fkey");
        });

        modelBuilder.Entity<TaiKhoan>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("tai_khoan_pkey");

            entity.ToTable("tai_khoan");

            entity.HasIndex(e => e.NhanVienId, "tai_khoan_nhan_vien_id_key").IsUnique();

            entity.HasIndex(e => e.TenDangNhap, "tai_khoan_ten_dang_nhap_key").IsUnique();

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.DangHoatDong)
                .HasDefaultValue(true)
                .HasColumnName("dang_hoat_dong");
            entity.Property(e => e.MatKhauHash).HasColumnName("mat_khau_hash");
            entity.Property(e => e.NgayTao)
                .HasDefaultValueSql("now()")
                .HasColumnName("ngay_tao");
            entity.Property(e => e.NhanVienId).HasColumnName("nhan_vien_id");
            entity.Property(e => e.TenDangNhap)
                .HasMaxLength(50)
                .HasColumnName("ten_dang_nhap");
            entity.Property(e => e.VaiTro)
                .HasMaxLength(20)
                .HasColumnName("vai_tro");

            entity.HasOne(d => d.NhanVien).WithOne(p => p.TaiKhoan)
                .HasForeignKey<TaiKhoan>(d => d.NhanVienId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("tai_khoan_nhan_vien_id_fkey");
        });

        modelBuilder.Entity<ThanhToan>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("thanh_toan_pkey");

            entity.ToTable("thanh_toan");

            entity.HasIndex(e => e.PhieuThueId, "ix_thanh_toan_phieu");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.ChiTiet)
                .HasDefaultValueSql("'{}'::jsonb")
                .HasColumnType("jsonb")
                .HasColumnName("chi_tiet");
            entity.Property(e => e.LoaiGiaoDich)
                .HasMaxLength(15)
                .HasColumnName("loai_giao_dich");
            entity.Property(e => e.NhanVienId).HasColumnName("nhan_vien_id");
            entity.Property(e => e.PhieuThueId).HasColumnName("phieu_thue_id");
            entity.Property(e => e.PhuongThuc)
                .HasMaxLength(15)
                .HasColumnName("phuong_thuc");
            entity.Property(e => e.SoTien)
                .HasPrecision(14)
                .HasColumnName("so_tien");
            entity.Property(e => e.ThoiDiem)
                .HasDefaultValueSql("now()")
                .HasColumnName("thoi_diem");

            entity.HasOne(d => d.NhanVien).WithMany(p => p.ThanhToans)
                .HasForeignKey(d => d.NhanVienId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("thanh_toan_nhan_vien_id_fkey");

            entity.HasOne(d => d.PhieuThue).WithMany(p => p.ThanhToans)
                .HasForeignKey(d => d.PhieuThueId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("thanh_toan_phieu_thue_id_fkey");
        });

        modelBuilder.Entity<TraXe>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("tra_xe_pkey");

            entity.ToTable("tra_xe");

            entity.HasIndex(e => e.PhieuThueId, "tra_xe_phieu_thue_id_key").IsUnique();

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.GhiChu).HasColumnName("ghi_chu");
            entity.Property(e => e.KmDaDi).HasColumnName("km_da_di");
            entity.Property(e => e.KmVuot).HasColumnName("km_vuot");
            entity.Property(e => e.NgayTao)
                .HasDefaultValueSql("now()")
                .HasColumnName("ngay_tao");
            entity.Property(e => e.NhanVienId).HasColumnName("nhan_vien_id");
            entity.Property(e => e.PhiTre)
                .HasPrecision(14)
                .HasColumnName("phi_tre");
            entity.Property(e => e.PhiVuotKm)
                .HasPrecision(14)
                .HasColumnName("phi_vuot_km");
            entity.Property(e => e.PhieuThueId).HasColumnName("phieu_thue_id");
            entity.Property(e => e.SoGioTre).HasColumnName("so_gio_tre");
            entity.Property(e => e.SoKmTra).HasColumnName("so_km_tra");
            entity.Property(e => e.ThoiDiemTraThucTe).HasColumnName("thoi_diem_tra_thuc_te");
            entity.Property(e => e.TienGiam)
                .HasPrecision(14)
                .HasColumnName("tien_giam");
            entity.Property(e => e.TienThue)
                .HasPrecision(14)
                .HasColumnName("tien_thue");
            entity.Property(e => e.TinhTrangPinTra)
                .HasPrecision(5, 2)
                .HasColumnName("tinh_trang_pin_tra");
            entity.Property(e => e.TongPhaiTra)
                .HasPrecision(14)
                .HasColumnName("tong_phai_tra");
            entity.Property(e => e.TongTruocGiam)
                .HasPrecision(14)
                .HasColumnName("tong_truoc_giam");
            entity.Property(e => e.XeHuHong).HasColumnName("xe_hu_hong");

            entity.HasOne(d => d.NhanVien).WithMany(p => p.TraXes)
                .HasForeignKey(d => d.NhanVienId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("tra_xe_nhan_vien_id_fkey");

            entity.HasOne(d => d.PhieuThue).WithOne(p => p.TraXe)
                .HasForeignKey<TraXe>(d => d.PhieuThueId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("tra_xe_phieu_thue_id_fkey");
        });

        modelBuilder.Entity<Xe>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("xe_pkey");

            entity.ToTable("xe");

            entity.HasIndex(e => e.NhomXeId, "ix_xe_nhom_xe");

            entity.HasIndex(e => e.TrangThai, "ix_xe_trang_thai");

            entity.HasIndex(e => e.BienSo, "xe_bien_so_key").IsUnique();

            entity.HasIndex(e => e.MaXe, "xe_ma_xe_key").IsUnique();

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.BienSo)
                .HasMaxLength(15)
                .HasColumnName("bien_so");
            entity.Property(e => e.DongXe)
                .HasMaxLength(50)
                .HasColumnName("dong_xe");
            entity.Property(e => e.GhiChu).HasColumnName("ghi_chu");
            entity.Property(e => e.HangSanXuat)
                .HasMaxLength(50)
                .HasColumnName("hang_san_xuat");
            entity.Property(e => e.MaXe)
                .HasMaxLength(20)
                .HasColumnName("ma_xe");
            entity.Property(e => e.NamSanXuat).HasColumnName("nam_san_xuat");
            entity.Property(e => e.NgayTao)
                .HasDefaultValueSql("now()")
                .HasColumnName("ngay_tao");
            entity.Property(e => e.NhomXeId).HasColumnName("nhom_xe_id");
            entity.Property(e => e.SoKmHienTai).HasColumnName("so_km_hien_tai");
            entity.Property(e => e.TinhTrangPin)
                .HasPrecision(5, 2)
                .HasColumnName("tinh_trang_pin");
            entity.Property(e => e.TrangThai)
                .HasMaxLength(20)
                .HasDefaultValueSql("'SAN_SANG'::character varying")
                .HasColumnName("trang_thai");

            entity.HasOne(d => d.NhomXe).WithMany(p => p.Xes)
                .HasForeignKey(d => d.NhomXeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("xe_nhom_xe_id_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

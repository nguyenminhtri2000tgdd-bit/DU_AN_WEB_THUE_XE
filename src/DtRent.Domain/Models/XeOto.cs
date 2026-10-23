using DtRent.Domain.Enums;
using DtRent.Domain.Exceptions;

namespace DtRent.Domain.Models;

public sealed class XeOto
{
    public XeOto(string ma, string bienSo, NhomXeOto nhom, TrangThaiXe trangThai = TrangThaiXe.SanSang)
    {
        if (string.IsNullOrWhiteSpace(ma))
            throw new ArgumentException("Mã xe không được để trống.", nameof(ma));
        if (string.IsNullOrWhiteSpace(bienSo))
            throw new ArgumentException("Biển số không được để trống.", nameof(bienSo));
        ArgumentNullException.ThrowIfNull(nhom);

        Ma = ma;
        BienSo = bienSo;
        Nhom = nhom;
        TrangThai = trangThai;
    }

    public string Ma { get; }
    public string BienSo { get; }
    public NhomXeOto Nhom { get; }
    public TrangThaiXe TrangThai { get; private set; }

    public bool SanSangChoThue => TrangThai == TrangThaiXe.SanSang;

    /// <summary>Sẵn sàng → Đang cho thuê (khi lập phiếu thuê).</summary>
    public void BatDauChoThue()
    {
        if (!SanSangChoThue)
            throw new XeKhongSanSangException(BienSo, TrangThai);
        TrangThai = TrangThaiXe.DangChoThue;
    }

    /// <summary>Đang cho thuê → Sẵn sàng, hoặc Đang bảo trì nếu xe bị hư hỏng (khi trả xe).</summary>
    public void NhanXeVe(bool biHuHong)
    {
        if (TrangThai != TrangThaiXe.DangChoThue)
            throw new InvalidOperationException("Chỉ nhận lại được xe đang ở trạng thái \"Đang cho thuê\".");
        TrangThai = biHuHong ? TrangThaiXe.DangBaoTri : TrangThaiXe.SanSang;
    }

    /// <summary>Sẵn sàng → Đang bảo trì.</summary>
    public void BatDauBaoTri()
    {
        if (TrangThai != TrangThaiXe.SanSang)
            throw new InvalidOperationException("Chỉ đưa vào bảo trì được xe đang ở trạng thái \"Sẵn sàng\".");
        TrangThai = TrangThaiXe.DangBaoTri;
    }

    /// <summary>Đang bảo trì → Sẵn sàng.</summary>
    public void HoanThanhBaoTri()
    {
        if (TrangThai != TrangThaiXe.DangBaoTri)
            throw new InvalidOperationException("Xe không ở trạng thái \"Đang bảo trì\".");
        TrangThai = TrangThaiXe.SanSang;
    }
}
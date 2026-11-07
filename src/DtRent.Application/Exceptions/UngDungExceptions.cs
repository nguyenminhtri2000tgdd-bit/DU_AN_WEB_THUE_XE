using DtRent.Domain.Exceptions;

namespace DtRent.Application.Exceptions;

public sealed class KhongTimThayException : NghiepVuThueXeException
{
    public KhongTimThayException(string doiTuong, object ma)
        : base("KHONG_TIM_THAY", $"Không tìm thấy {doiTuong} (mã/Id: {ma}).")
    {
    }
}

public sealed class XeTrungLichException : NghiepVuThueXeException
{
    public XeTrungLichException(string bienSo)
        : base("XE_TRUNG_LICH", $"Xe {bienSo} đã có phiếu thuê trùng khoảng thời gian này.")
    {
    }
}

public sealed class PhieuThueKhongConHieuLucException : NghiepVuThueXeException
{
    public PhieuThueKhongConHieuLucException(string maPhieu, string trangThai)
        : base("PHIEU_KHONG_CON_HIEU_LUC",
               $"Phiếu thuê {maPhieu} đang ở trạng thái \"{trangThai}\" nên không thể trả xe.")
    {
    }
}

public sealed class SoKmTraKhongHopLeException : NghiepVuThueXeException
{
    public SoKmTraKhongHopLeException(int soKmNhan, int soKmTra)
        : base("SO_KM_TRA_KHONG_HOP_LE",
               $"Số km khi trả ({soKmTra}) không được nhỏ hơn số km khi nhận ({soKmNhan}).")
    {
    }
}
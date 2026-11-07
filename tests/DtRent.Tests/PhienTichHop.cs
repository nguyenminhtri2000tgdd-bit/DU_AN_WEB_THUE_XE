using DtRent.Application.ThueXe;
using DtRent.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace DtRent.Tests;

/// <summary>
/// Một phiên kiểm thử tích hợp: mở giao dịch trên PostgreSQL thật và LUÔN hoàn tác khi kết thúc,
/// nên dữ liệu trong CSDL không bị thay đổi.
/// </summary>
internal sealed class PhienTichHop : IAsyncDisposable
{
    private readonly IDbContextTransaction _giaoDich;

    private PhienTichHop(DtRentDbContext db, IDbContextTransaction giaoDich)
    {
        Db = db;
        _giaoDich = giaoDich;
        DichVu = new ThueXeDichVu(db);
    }

    public DtRentDbContext Db { get; }
    public ThueXeDichVu DichVu { get; }

    public static async Task<PhienTichHop> TaoAsync()
    {
        string? chuoiKetNoi = Environment.GetEnvironmentVariable("DTRENT_CONNECTION");
        if (string.IsNullOrWhiteSpace(chuoiKetNoi))
            throw new InvalidOperationException(
                "Chưa đặt biến môi trường DTRENT_CONNECTION. Trong PowerShell, chạy: " +
                "$env:DTRENT_CONNECTION = 'Host=localhost;Port=5432;Database=dt_rent;Username=postgres;Password=...' " +
                "rồi chạy lại dotnet test trong CÙNG cửa sổ.");

        var options = new DbContextOptionsBuilder<DtRentDbContext>().UseNpgsql(chuoiKetNoi).Options;
        var db = new DtRentDbContext(options);
        var giaoDich = await db.Database.BeginTransactionAsync();
        return new PhienTichHop(db, giaoDich);
    }

    public Task<long> KhachIdAsync(string maKhach) =>
        Db.KhachHangs.Where(k => k.MaKhach == maKhach).Select(k => k.Id).SingleAsync();

    public Task<int> XeIdAsync(string maXe) =>
        Db.Xes.Where(x => x.MaXe == maXe).Select(x => x.Id).SingleAsync();

    public Task<int> NhanVienIdAsync(string maNhanVien) =>
        Db.NhanViens.Where(n => n.MaNhanVien == maNhanVien).Select(n => n.Id).SingleAsync();

    /// <summary>Đặt trạng thái và số km của xe bằng SQL (trong giao dịch, sẽ bị hoàn tác).</summary>
    public async Task DatXeAsync(string maXe, string trangThai, int soKm)
    {
        await Db.Database.ExecuteSqlInterpolatedAsync(
            $"UPDATE xe SET trang_thai = {trangThai}, so_km_hien_tai = {soKm} WHERE ma_xe = {maXe}");
        Db.ChangeTracker.Clear();
    }

    public async ValueTask DisposeAsync()
    {
        await _giaoDich.RollbackAsync();
        await _giaoDich.DisposeAsync();
        await Db.DisposeAsync();
    }
}
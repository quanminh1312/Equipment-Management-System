using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NuGet.DependencyResolver;
using System.Reflection.Emit;
using System;
namespace CNPM.Models
{
    public class AppDbContext : IdentityDbContext<TaiKhoan>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Bỏ tiền tố AspNet của các bảng: mặc định các bảng trong IdentityDbContext có
            // tên với tiền tố AspNet như: AspNetUserRoles, AspNetUser ...
            // Đoạn mã sau chạy khi khởi tạo DbContext, tạo database sẽ loại bỏ tiền tố đó
            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                var tableName = entityType.GetTableName();
                if (tableName!.StartsWith("AspNet"))
                {
                    entityType.SetTableName(tableName.Substring(6));
                }
            }
            builder.Entity<NguoiDung>().ToTable("NguoiDung");
            builder.Entity<SinhVien>().ToTable("SinhVien");
            builder.Entity<GiangVien>().ToTable("GiangVien");
            builder.Entity<ThuKho>().ToTable("ThuKho");
        }
        public DbSet<ChiTietPhieuMuon> ChiTietPhieuMuons { get; set; }
        public DbSet<PhieuMuon> PhieuMuons { get; set; }
        public DbSet<LoaiThietBi> LoaiThietBis { get; set; }
        public DbSet<ThietBi> ThietBis { get; set; }
        public DbSet<SinhVien> sinhViens { get; set; }
        public DbSet<GiangVien> GiangViens { get; set; }
        public DbSet<ThuKho> ThuKhos { get; set; }
        public DbSet<NguoiDung> NguoiDungs { get; set; }
        public DbSet<TinhTrang> TinhTrang { get; set; }
    }
}

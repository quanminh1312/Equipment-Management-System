using CNPM.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Security.AccessControl;

namespace CNPM
{
    public static class Utils
    {
        public enum Status
        {
            Pending,
            Accepted,
            Finished,
            Borrowed,
            All
        }
        public static string rootPath = "";
        public static bool CheckMaNguoiDung(string maNguoiDung)
        {
            if (!maNguoiDung.ToLower().Contains("sv") && !maNguoiDung.ToLower().Contains("gv") && !maNguoiDung.ToLower().Contains("tk"))
            {
                return false;
            }
            return true;
        }
        public static bool CheckEmail(string email)
        {
            if (!email.ToLower().Contains("siu.edu.vn"))
            {
                return false;
            }
            return true;
        }
        public static void SeedData(AppDbContext context, IConfiguration Configuration, IUserStore<TaiKhoan> _userStore, UserManager<TaiKhoan> _userManager)
        {
            context.Database.EnsureCreated();
            IUserEmailStore<TaiKhoan> _emailStore = GetEmailStore(_userManager,_userStore);
            if (context.Roles.Count() == 0)
            {
                context.Roles.Add(new Microsoft.AspNetCore.Identity.IdentityRole
                {
                    Name = "ad",
                    NormalizedName = "AD"
                });
                context.Roles.Add(new Microsoft.AspNetCore.Identity.IdentityRole
                {
                    Name = "tk",
                    NormalizedName = "TK"
                });
                context.Roles.Add(new Microsoft.AspNetCore.Identity.IdentityRole
                {
                    Name = "gv",
                    NormalizedName = "GV"
                });
                context.Roles.Add(new Microsoft.AspNetCore.Identity.IdentityRole
                {
                    Name = "sv",
                    NormalizedName = "SV"
                });
                context.SaveChanges();
            }
            //add admin from appsettings.json
            if (context.Users.FirstOrDefault(u => u.Email == Configuration["AdminAccount:Email"]) == null)
            {
                var user = new TaiKhoan()
                {
                    TenDangNhap = Configuration["AdminAccount:Username"]!,
                    Email = Configuration["AdminAccount:Email"],
                    UserName = Configuration["AdminAccount:Email"],
                    EmailConfirmed = true
                };
                _userStore.SetUserNameAsync(user, user.Email, CancellationToken.None).Wait();
                _emailStore.SetEmailAsync(user, user.Email, CancellationToken.None).Wait();
                var result = _userManager.CreateAsync(user, Configuration["AdminAccount:Password"]!).Result;
                if (result.Succeeded)
                {
                    _userManager.AddToRoleAsync(user, "ad").Wait();
                }
            }
        }
        public static bool ValidateRegister(ModelStateDictionary pageModel, dynamic Input)
        {
            if (!CheckMaNguoiDung(Input.MaNguoiDung))
            {
                pageModel.AddModelError("Input.MaNguoiDung", "Mã người dùng không hợp lệ");
                return false;
            }
            if (!CheckEmail(Input.Email))
            {
                pageModel.AddModelError("Input.Email", "Email không hợp lệ");
                return false;
            }
            return true;
        }
        public static async Task CreateUserData(AppDbContext context, TaiKhoan user, dynamic Input)
        {
            if (Input.MaNguoiDung.ToLower().Contains("sv"))
            {
                await context.sinhViens.AddAsync(new SinhVien {Email = Input.Email, IdTaiKhoan = user.Id, MaNguoiDung= Input.MaNguoiDung });
            }
            else if (Input.MaNguoiDung.ToLower().Contains("gv"))
            {
                await context.GiangViens.AddAsync(new GiangVien { Email = Input.Email, IdTaiKhoan = user.Id, MaNguoiDung = Input.MaNguoiDung });
            }
            else if (Input.MaNguoiDung.ToLower().Contains("tk"))
            {
                await context.ThuKhos.AddAsync(new ThuKho{ Email = Input.Email, IdTaiKhoan = user.Id, MaNguoiDung = Input.MaNguoiDung });
            }
            await context.SaveChangesAsync();
        }
        public static async Task<ModelStateDictionary> Upload(ModelStateDictionary model, List<string> validTypes, IFormFile file, string modelError, string path)
        {
            if (file == null || file.Length == 0)
            {
                model.AddModelError(modelError,"chưa chọn file!");
                return model;
            }

            if (!validTypes.Contains(file.ContentType))
            {
                model.AddModelError(modelError, "File không phải là ảnh hợp lệ!");
                return model;
            }
            await SaveFile(file, path);
            return model;

        }
        public static async Task SaveFile(IFormFile file, string path)
        {
            path = Path.Combine(rootPath, path);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            path = Path.Combine(path, file.FileName);
            using (var stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
        }
        public static void DeleteFile(string path)
        {
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }
        }
        public static bool CheckCanDeleteLoaiThietBi(AppDbContext context, int id)
        {
            return context.ThietBis.FirstOrDefault(t => t.IdLoaiThietBi == id) == null;
        }
        public static bool CheckCanDeleteThietBi(AppDbContext context, int id)
        {
            return context.ThietBis.FirstOrDefault(t => t.Id == id).idChiTietPhieuMuon == null;
        }
        public static bool CheckCanDeletePhieuMuon(AppDbContext context, int id)
        {
            return context.ChiTietPhieuMuons.FirstOrDefault(p => p.IdPhieuMuon == id && (p.NgayMuonThucTe != null || p.NgayTraThucTe != null)) == null;
        }
        public static bool CheckCanDeleteTinhTrang(AppDbContext context, int id)
        {
            return context.ThietBis.FirstOrDefault(t => t.IdTinhTrang == id) == null;
        }
        private static IUserEmailStore<TaiKhoan> GetEmailStore(UserManager<TaiKhoan> _userManager, IUserStore<TaiKhoan> _userStore)
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<TaiKhoan>)_userStore;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CNPM.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Data;

namespace CNPM.Controllers
{
    [Authorize]
    public class GiangViensController : Controller
    {
        private readonly AppDbContext _context;
        private readonly SignInManager<TaiKhoan> _signInManager;   
        private readonly UserManager<TaiKhoan> _userManager;

        public GiangViensController(AppDbContext context, UserManager<TaiKhoan> userManager, SignInManager<TaiKhoan> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // GET: GiangViens
        public async Task<IActionResult> Index()
        {
            if (User.IsInRole("ad"))
            {
               var appDbContext = await _context.GiangViens.Include(g => g.TaiKhoan).ToListAsync();
                appDbContext.Reverse();
                return View(appDbContext);
            } else if (User.IsInRole("gv"))
            {
                int id = _context.GiangViens.Where(s => s.IdTaiKhoan == _userManager.GetUserId(User)).First().Id;
                return RedirectToAction("Details", new { id = id });

            } else return NotFound();
        }

        // GET: GiangViens/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var giangVien = await _context.GiangViens.FirstOrDefaultAsync(m => m.Id == id);
            if (giangVien == null && User.IsInRole("gv"))
            {
                return RedirectToAction("Create");
            }
            if (giangVien == null)
            {
                return NotFound();
            }

            return View(giangVien);
        }

        // GET: GiangViens/Create
        public IActionResult Create()
        {
            if (User.IsInRole("gv"))
            {
                return View();
            }
            else return NotFound();
        }

        // POST: GiangViens/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Khoa,HocVi,ChuyenNganh,Id,MaNguoiDung,Email,TenNguoiDung,GioiTinh,DiaChi,SoDienThoai,NgaySinh")] GiangVien giangVien, IFormFile file)
        {
            giangVien.IdTaiKhoan = _userManager.GetUserId(User);
            ModelState.Remove("IdTaiKhoan");
            var path = giangVien.IdTaiKhoan+"\\images";
            List<string> validTypes = new List<string> { "image/jpeg", "image/png" };
            if (Utils.Upload(ModelState, validTypes, file, "AnhDaiDien", path).Result.IsValid)
            {
                giangVien.AnhDaiDien = Path.Combine(path, file.FileName);
                _context.Add(giangVien);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(giangVien);
        }

        // GET: GiangViens/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            if (id == null)
            {
                return NotFound();
            }
            if (!User.IsInRole("gv"))
            {
                return NotFound();
            }
            var giangVien = await _context.GiangViens.FirstOrDefaultAsync(m => m.Id == id);
            if (giangVien == null && User.IsInRole("gv"))
            {
                return RedirectToAction("Create");
            }
            if (giangVien == null)
            {
                return NotFound();
            }
            return View(giangVien);
        }

        // POST: GiangViens/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Khoa,HocVi,KhoaHoc,HeDaoTao,ChuyenNganh,Id,MaNguoiDung,Email,TenNguoiDung,GioiTinh,DiaChi,SoDienThoai,NgaySinh")] GiangVien giangVien, IFormFile file)
        {
            giangVien.IdTaiKhoan = _userManager.GetUserId(User);
            ModelState.Remove("IdTaiKhoan");
            //giangVien.Id = _context.GiangViens.Where(s => s.IdTaiKhoan == giangVien.IdTaiKhoan).First().Id;
            var path = giangVien.IdTaiKhoan + "\\images";
            Utils.DeleteFile(giangVien.AnhDaiDien!);
            List<string> validTypes = new List<string> { "image/jpeg", "image/png" };
            if (Utils.Upload(ModelState, validTypes, file, "AnhDaiDien", path).Result.IsValid)
            {
                try
                {
                    giangVien.AnhDaiDien = Path.Combine(path, file.FileName);
                    _context.Update(giangVien);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GiangVienExists(giangVien.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(giangVien);
        }

        // GET: GiangViens/Delete/5
        public async Task<IActionResult> Delete(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var giangVien = await _context.GiangViens.FirstOrDefaultAsync(m => m.IdTaiKhoan == id);
            if (giangVien == null)
            {
                return NotFound();
            }

            return View(giangVien);
        }

        // POST: GiangViens/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var giangVien = await _context.GiangViens.FirstOrDefaultAsync(m => m.IdTaiKhoan == id); ;
            if (giangVien != null)
            {
                _context.GiangViens.Remove(giangVien);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GiangVienExists(int id)
        {
            return _context.GiangViens.Any(e => e.Id == id);
        }
    }
}

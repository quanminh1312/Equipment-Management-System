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

namespace CNPM.Controllers
{
    [Authorize]
    public class SinhViensController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<TaiKhoan> _userManager;

        public SinhViensController(AppDbContext context, UserManager<TaiKhoan> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: SinhViens
        public async Task<IActionResult> Index()
        {
            if (User.IsInRole("ad"))
            {
                var appDbContext = await _context.sinhViens.Include(g => g.TaiKhoan).ToListAsync();
                appDbContext.Reverse();
                return View(appDbContext);
            }
            else if (User.IsInRole("sv"))
            {
                int id = _context.sinhViens.Where(s => s.IdTaiKhoan == _userManager.GetUserId(User)).First().Id;
                return RedirectToAction("Details", new { id = id });

            }
            else return NotFound();
        }

        // GET: SinhViens/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sinhVien = await _context.sinhViens.FirstOrDefaultAsync(m => m.Id == id);
            if (sinhVien == null && User.IsInRole("sv"))
            {
                return RedirectToAction("Create");
            }
            if (sinhVien == null)
            {
                return NotFound();
            }

            return View(sinhVien);
        }

        // GET: SinhViens/Create
        public IActionResult Create()
        {
            if (!User.IsInRole("sv"))
            {
                return NotFound();
            }
            return View();
        }

        // POST: SinhViens/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Lop,Khoa,HeDaoTao,ChuyenNganh,Id,MaNguoiDung,Email,TenNguoiDung,GioiTinh,DiaChi,SoDienThoai,NgaySinh")] SinhVien sinhVien, IFormFile file)
        {
            sinhVien.IdTaiKhoan = _userManager.GetUserId(User);
            ModelState.Remove("IdTaiKhoan");
            var path = sinhVien.IdTaiKhoan + "\\images";
            List<string> validTypes = new List<string> { "image/jpeg", "image/png" };
            if (Utils.Upload(ModelState, validTypes, file, "AnhDaiDien", path).Result.IsValid)
            {
                sinhVien.AnhDaiDien = Path.Combine(path, file.FileName);
                _context.Add(sinhVien);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(sinhVien);
        }

        // GET: SinhViens/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            if (!User.IsInRole("sv"))
            {
                return NotFound();
            }
            if (id == null)
            {
                return NotFound();
            }

            var sinhVien = await _context.sinhViens.Where(s => s.Id == id).FirstAsync();
            if (sinhVien == null && User.IsInRole("sv"))
            {
                return RedirectToAction("Create");
            }
            if (sinhVien == null)
            {
                return NotFound();
            }
            return View(sinhVien);
        }

        // POST: SinhViens/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Lop,Khoa,HeDaoTao,ChuyenNganh,MaNguoiDung,Email,TenNguoiDung,GioiTinh,DiaChi,SoDienThoai,NgaySinh")] SinhVien sinhVien, IFormFile file)
        {
            sinhVien.IdTaiKhoan = _userManager.GetUserId(User);
            ModelState.Remove("IdTaiKhoan");
            //sinhVien.Id = _context.sinhViens.Where(s => s.IdTaiKhoan == sinhVien.IdTaiKhoan).First().Id;
            var path = sinhVien.IdTaiKhoan + "\\images";
            Utils.DeleteFile(sinhVien.AnhDaiDien!);
            List<string> validTypes = new List<string> { "image/jpeg", "image/png" };
            var model = Utils.Upload(ModelState, validTypes, file, "AnhDaiDien", path).Result;
            if (model.IsValid)
            {
                try
                {
                    sinhVien.AnhDaiDien = Path.Combine(path, file.FileName);
                    _context.Update(sinhVien);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SinhVienExists(sinhVien.Id))
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
            return View(sinhVien);
        }

        // GET: SinhViens/Delete/5
        public async Task<IActionResult> Delete(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sinhVien = await _context.sinhViens.FirstOrDefaultAsync(m => m.IdTaiKhoan == id);
            if (sinhVien == null)
            {
                return NotFound();
            }

            return View(sinhVien);
        }

        // POST: SinhViens/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var sinhVien = await _context.sinhViens.FirstOrDefaultAsync(m => m.IdTaiKhoan == id);
            if (sinhVien != null)
            {
                _context.sinhViens.Remove(sinhVien);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SinhVienExists(int id)
        {
            return _context.sinhViens.Any(e => e.Id == id);
        }
    }
}

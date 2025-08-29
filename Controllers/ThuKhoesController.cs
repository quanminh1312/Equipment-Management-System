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
    public class ThuKhoesController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<TaiKhoan> _userManager;

        public ThuKhoesController(AppDbContext context, UserManager<TaiKhoan> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: ThuKhoes
        public async Task<IActionResult> Index()
        {
            if (User.IsInRole("ad"))
            {
                var appDbContext = await _context.ThuKhos.Include(g => g.TaiKhoan).ToListAsync();
                return View(appDbContext);
            }
            else if (User.IsInRole("tk"))
            {
                int id = _context.ThuKhos.Where(s => s.IdTaiKhoan == _userManager.GetUserId(User)).First().Id;
                return RedirectToAction("Details", new { id = id });

            }
            else return NotFound();
        }

        // GET: ThuKhoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var thuKho = await _context.ThuKhos.FirstOrDefaultAsync(m => m.Id == id);
            if (thuKho == null && User.IsInRole("tk"))
            {
                return RedirectToAction("Create");
            }
            if (thuKho == null)
            {
                return NotFound();
            }

            return View(thuKho);
        }

        // GET: ThuKhoes/Create
        public IActionResult Create()
        {
            if (!User.IsInRole("tk"))
            {
                return NotFound();
            }
            return View();
        }

        // POST: ThuKhoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("heSoLuong,Id,MaNguoiDung,Email,TenNguoiDung,GioiTinh,DiaChi,SoDienThoai,NgaySinh")] ThuKho thuKho, IFormFile file)
        {
            thuKho.IdTaiKhoan = _userManager.GetUserId(User);
            ModelState.Remove("IdTaiKhoan");
            var path = thuKho.IdTaiKhoan + "\\images";
            List<string> validTypes = new List<string> { "image/jpeg", "image/png" };
            if (Utils.Upload(ModelState, validTypes, file, "AnhDaiDien", path).Result.IsValid)
            {
                thuKho.AnhDaiDien = Path.Combine(path, file.FileName);
                _context.Add(thuKho);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(thuKho);
        }

        // GET: ThuKhoes/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            if (!User.IsInRole("tk"))
            {
                return NotFound();
            }
            if (id == null)
            {
                return NotFound();
            }

            var thuKho = await _context.ThuKhos.FirstOrDefaultAsync(s => s.Id==id);
            if (thuKho == null && User.IsInRole("tk"))
            {
                return RedirectToAction("Create");
            }
            if (thuKho == null)
            {
                return NotFound();
            }
            return View(thuKho);
        }

        // POST: ThuKhoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("heSoLuong,Id,MaNguoiDung,Email,TenNguoiDung,GioiTinh,DiaChi,SoDienThoai,NgaySinh")] ThuKho thuKho, IFormFile file)
        {
            thuKho.IdTaiKhoan = _userManager.GetUserId(User);
            ModelState.Remove("IdTaiKhoan");
            var path = thuKho.IdTaiKhoan + "\\images";
            Utils.DeleteFile(thuKho.AnhDaiDien!);
            List<string> validTypes = new List<string> { "image/jpeg", "image/png" };
            if (Utils.Upload(ModelState, validTypes, file, "AnhDaiDien", path).Result.IsValid)
            {
                try
                {
                    thuKho.AnhDaiDien = Path.Combine(path, file.FileName);
                    _context.Update(thuKho);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ThuKhoExists(thuKho.Id))
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
            return View(thuKho);
        }

        // GET: ThuKhoes/Delete/5
        public async Task<IActionResult> Delete(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var thuKho = await _context.ThuKhos.FirstOrDefaultAsync(m => m.IdTaiKhoan == id);
            if (thuKho == null)
            {
                return NotFound();
            }

            return View(thuKho);
        }

        // POST: ThuKhoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var thuKho = await _context.ThuKhos.FirstOrDefaultAsync(m => m.IdTaiKhoan == id);
            if (thuKho != null)
            {
                _context.ThuKhos.Remove(thuKho);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ThuKhoExists(int id)
        {
            return _context.ThuKhos.Any(e => e.Id == id);
        }
    }
}

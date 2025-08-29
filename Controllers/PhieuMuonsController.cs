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
    public class PhieuMuonsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly SignInManager<TaiKhoan> _signInManager;
        private readonly UserManager<TaiKhoan> _userManager;
        public PhieuMuonsController(AppDbContext context, UserManager<TaiKhoan> userManager, SignInManager<TaiKhoan> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<IActionResult> Index(Utils.Status status = Utils.Status.All, bool manage = false)
        {
            if (manage) ViewBag.manage = true; else ViewBag.manage = false;
            if ((User.IsInRole("ad") || User.IsInRole("tk")) && manage)
            {
                var tam = await PhieuMuons(status);
                tam.Reverse();
                return View(tam);
            } else
            {
                var tam = await PhieuMuonsById(status);
                tam.Reverse();
                return View(tam);
            }    
        }
        // GET: PhieuMuons/Details/5
        public async Task<IActionResult> Details(int? id, bool manage = false)
        {
            ViewBag.TrangThai = TrangThai(id);
            if (manage) ViewBag.manage = true; else ViewBag.manage = false;
            if (id == null)
            {
                return NotFound();
            }

            var phieuMuon = await _context.PhieuMuons
                .Include(p => p.NguoiMuon)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (phieuMuon == null)
            {
                return NotFound();
            }
            if (!User.IsInRole("ad") && !User.IsInRole("tk") && !manage)
            {
                var user = await _userManager.GetUserAsync(User);
                if (phieuMuon.NguoiMuon!.IdTaiKhoan != user!.Id)
                {
                    return NotFound();
                }
            }
            return View(phieuMuon);
        }
        private string TrangThai(int? id)
        {
            if (id == null) return "";
            string trangThai = "";
            var chiTietPhieuMuons = _context.ChiTietPhieuMuons.Where(c => c.IdPhieuMuon == id);
            if (chiTietPhieuMuons.Any(c => c.XacNhan == null))
            {
                trangThai = "Chờ xác nhận";
            }
            if (chiTietPhieuMuons.Any(c => c.XacNhan != null))
            {
                trangThai = "Đã xác nhận";
            }
            if (chiTietPhieuMuons.Any(c => c.NgayTraThucTe == null && c.NgayMuonThucTe != null))
            {
                trangThai = "Đang mượn";
            }
            if (chiTietPhieuMuons.All(c => c.NgayTraThucTe != null))
            {
                trangThai = "Đã hoàn thành";
            }
            return trangThai;
        }

        // GET: PhieuMuons/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: PhieuMuons/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdNguoiMuon,Id,NgayMuon,NgayHenTra")] PhieuMuon phieuMuon)
        {
            if (phieuMuon.NgayMuon < phieuMuon.NgayLapPhieuMuon)
            {
                ModelState.AddModelError("NgayLapPhieuMuon", "Ngày mượn không hợp lệ");
            }
            if (phieuMuon.NgayHenTra < phieuMuon.NgayMuon)
            {
                ModelState.AddModelError("NgayHenTra", "Ngày hẹn trả không hợp lệ");
            }
            if (phieuMuon.NgayMuon > phieuMuon.NgayHenTra)
            {
                ModelState.AddModelError("NgayHenTra", "Ngày hẹn trả không hợp lệ");
            }
            var user = await _userManager.GetUserAsync(User);
            var nguoiMuon = await _context.NguoiDungs.FirstOrDefaultAsync(p => p.IdTaiKhoan == user.Id);
            phieuMuon.IdNguoiMuon = nguoiMuon.Id;
            if (ModelState.IsValid)
            {
                _context.Add(phieuMuon);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(phieuMuon);
        }

        // GET: PhieuMuons/Edit/5
        public async Task<IActionResult> Edit(int? id, bool manage = false)
        {
            if (manage) ViewBag.manage = true; else ViewBag.manage = false;
            if (manage) return NotFound();
            if (id == null)
            {
                return NotFound();
            }

            var phieuMuon = await _context.PhieuMuons.Include(p => p.NguoiMuon).FirstOrDefaultAsync(p => p.Id == id);
            if (phieuMuon == null)
            {
                return NotFound();
            }
            if (!User.IsInRole("ad") && !User.IsInRole("tk") && !manage)
            {
                var user = await _userManager.GetUserAsync(User);
                if (phieuMuon.NguoiMuon!.IdTaiKhoan != user!.Id || _context.ChiTietPhieuMuons.Where(c => c.XacNhan != null).Any(c => c.IdPhieuMuon == phieuMuon.Id))
                {
                    return NotFound();
                }
            }
            return View(phieuMuon);
        }

        // POST: PhieuMuons/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,NgayMuon,NgayHenTra,IdNguoiMuon")] PhieuMuon phieuMuon, bool manage = false)
        {
            if (manage) ViewBag.manage = true; else ViewBag.manage = false;
            if (manage) return NotFound();
            var user = await _userManager.GetUserAsync(User);
            var ngayLapPhieuMuon = _context.PhieuMuons
                                    .Where(p => p.Id == id)
                                    .Select(p => p.NgayLapPhieuMuon)
                                    .FirstOrDefault();
            phieuMuon.NgayLapPhieuMuon = ngayLapPhieuMuon;
            if (id != phieuMuon.Id)
            {
                return NotFound();
            }
            if (!User.IsInRole("ad") && !User.IsInRole("tk") && !manage)
            {
                var nguoiMuon = await _context.NguoiDungs.FirstOrDefaultAsync(p => p.Id == phieuMuon.IdNguoiMuon);
                if (nguoiMuon!.IdTaiKhoan != user!.Id)
                {
                    ModelState.AddModelError("IdNguoiMuon", "Không thể sửa phiếu mượn của người khác");
                }
                if (_context.ChiTietPhieuMuons.Where(c => c.IdPhieuMuon == phieuMuon.Id).Any(c => c.XacNhan != null))
                {
                    ModelState.AddModelError("Id", "Không thể sửa phiếu mượn đã xác nhận");
                }
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(phieuMuon);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PhieuMuonExists(phieuMuon.Id))
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
            return View(phieuMuon);
        }

        // GET: PhieuMuons/Delete/5
        public async Task<IActionResult> Delete(int? id, bool manage = false)
        {
            if (manage) ViewBag.manage = true; else ViewBag.manage = false;
            if (manage) return NotFound();
            if (id == null)
            {
                return NotFound();
            }

            var phieuMuon = await _context.PhieuMuons
                .Include(p => p.NguoiMuon)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (phieuMuon == null)
            {
                return NotFound();
            }
            if (!User.IsInRole("ad") && !User.IsInRole("tk") && !manage)
            {
                var user = await _userManager.GetUserAsync(User);
                if (phieuMuon.NguoiMuon!.IdTaiKhoan != user!.Id || _context.ChiTietPhieuMuons.Where(c => c.NgayMuonThucTe != null || c.NgayTraThucTe != null).Any(c => c.IdPhieuMuon == phieuMuon.Id))
                {
                    return NotFound();
                }
            }
            return View(phieuMuon);
        }

        // POST: PhieuMuons/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, bool manage = false)
        {
            if (manage) return NotFound();
            if (manage) ViewBag.manage = true; else ViewBag.manage = false;
            var phieuMuon = await _context.PhieuMuons.Include(c => c.NguoiMuon).FirstAsync(c => c.Id == id);
            if (!User.IsInRole("ad") && !User.IsInRole("tk") && !manage)
            {
                var user = await _userManager.GetUserAsync(User);
                if (phieuMuon.NguoiMuon!.IdTaiKhoan != user!.Id || _context.ChiTietPhieuMuons.Where(c => c.NgayMuonThucTe != null || c.NgayTraThucTe != null).Any(c => c.IdPhieuMuon == phieuMuon.Id))
                {
                    return NotFound();
                }
            }
            if (phieuMuon != null)
            {
                if (!Utils.CheckCanDeletePhieuMuon(_context, (int)id))
                {
                    ModelState.AddModelError("Id", "Không thể xóa phiếu mượn đã đến lấy thiết bị/đã hoàn thành");
                    return View(phieuMuon);
                }
                _context.PhieuMuons.Remove(phieuMuon);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<List<PhieuMuon>> PhieuMuons(Utils.Status status)
        {
            switch (status)
            {
                case Utils.Status.Pending:
                    return await _context.ChiTietPhieuMuons.Include(c => c.PhieuMuon).GroupBy(c => c.PhieuMuon).Where(g=> g.Any(c => c.XacNhan == null)).Select(g => g.Key).ToListAsync();
                case Utils.Status.Accepted:
                    return await _context.ChiTietPhieuMuons.Include(c => c.PhieuMuon).GroupBy(c => c.PhieuMuon).Where(g => g.Any(c => c.NgayTraThucTe == null && c.XacNhan == true)).Select(c => c.Key).ToListAsync();
                case Utils.Status.Finished:
                    return await _context.ChiTietPhieuMuons.Include(c => c.PhieuMuon).GroupBy(c => c.PhieuMuon).Where(g => !g.Any(c => c.NgayTraThucTe != null && c.XacNhan == true) && g.All(c => c.XacNhan == false)).Select(c => c.Key).ToListAsync();
                default:
                    return await _context.PhieuMuons.Include(p => p.NguoiMuon).ToListAsync();
            }
        }
        public async Task<List<PhieuMuon>> PhieuMuonsById(Utils.Status status)
        {
            var user = await _context.NguoiDungs.FirstOrDefaultAsync(p => p.IdTaiKhoan == _userManager.GetUserId(User));
            switch (status)
            {
                case Utils.Status.Pending:
                    return await _context.ChiTietPhieuMuons.Include(c => c.PhieuMuon).GroupBy(c => c.PhieuMuon).Where(g => g.Any(c => c.XacNhan == null)).Select(g => g.Key).Where(p => p.IdNguoiMuon == user.Id).ToListAsync();
                case Utils.Status.Accepted:
                    return await _context.ChiTietPhieuMuons.Include(c => c.PhieuMuon).GroupBy(c => c.PhieuMuon).Where(g => g.Any(c => c.NgayTraThucTe == null && c.XacNhan == true)).Select(c => c.Key).Where(p => p.IdNguoiMuon == user.Id).ToListAsync();
                case Utils.Status.Finished:
                    return await _context.ChiTietPhieuMuons.Include(c => c.PhieuMuon).GroupBy(c => c.PhieuMuon).Where(g => !g.Any(c => c.NgayTraThucTe != null && c.XacNhan == true) && g.All(c => c.XacNhan == false)).Select(c => c.Key).Where(p => p.IdNguoiMuon == user.Id).ToListAsync();
                default:
                    return await _context.PhieuMuons.Include(p => p.NguoiMuon).Where(p => p.IdNguoiMuon == user.Id).ToListAsync();
            }
        }
        private bool PhieuMuonExists(int id)
        {
            return _context.PhieuMuons.Any(e => e.Id == id);
        }
    }
}

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
    public class ChiTietPhieuMuonModel
    {
        public int thietbi { get; set; }
        public int? soluong { get; set; }
        public DateOnly start { get; set; }
        public DateOnly end { get; set; }
    }
    public class thietBiHong
    {
        public int id { get; set; }
        public int sl { get; set; }
    }
    [Authorize]
    public class LoaiThietBisController : Controller
    {
        private readonly AppDbContext _context;
        private readonly SignInManager<TaiKhoan> _signInManager;
        private readonly UserManager<TaiKhoan> _userManager;

        public LoaiThietBisController(AppDbContext context, SignInManager<TaiKhoan> signInManager, UserManager<TaiKhoan> userManager)
        {
            _context = context;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        // GET: LoaiThietBis
        public async Task<IActionResult> Index()
        {
            //get list chi tiet phieu muon so luong phieu muon ngay muon ngay tra
            var chiTietPhieuMuon = _context.ChiTietPhieuMuons.Include(c => c.PhieuMuon).Where(c => c.XacNhan == true).ToList();
            var list = from phieumuon in chiTietPhieuMuon
                       select new ChiTietPhieuMuonModel
                       {
                           thietbi = phieumuon.IdThietBi,
                           soluong = phieumuon.SoLuongMuon,
                           start = phieumuon.PhieuMuon.NgayMuon,
                           end = phieumuon.PhieuMuon.NgayHenTra
                       };
            var thietBiHong = _context.ThietBis.Where(tb => tb.TinhTrang.Id > 1).AsEnumerable()
                .GroupBy(ThietBi => ThietBi.IdLoaiThietBi)
                .Select(g => new thietBiHong
                {
                    id = g.Key,
                    sl = g.Count()
                }).ToList();

            ViewData["list"] = list.ToList();
            ViewData["thietBiHong"] = thietBiHong;
            var tam = await _context.LoaiThietBis.ToListAsync();
            tam.Reverse();
            return View(tam);
        }

        // GET: LoaiThietBis/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loaiThietBi = await _context.LoaiThietBis
                .FirstOrDefaultAsync(m => m.Id == id);
            if (loaiThietBi == null)
            {
                return NotFound();
            }

            return View(loaiThietBi);
        }

        // GET: LoaiThietBis/Create
        public IActionResult Create()
        {
            if (!User.IsInRole("ad") && !User.IsInRole("tk"))
            {
                return NotFound();
            }
            return View();
        }

        // POST: LoaiThietBis/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,TenLoaiThietBi,MoTa")] LoaiThietBi loaiThietBi)
        {
            if (!User.IsInRole("ad") && !User.IsInRole("tk"))
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                _context.Add(loaiThietBi);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(loaiThietBi);
        }

        // GET: LoaiThietBis/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (!User.IsInRole("ad") && !User.IsInRole("tk"))
            {
                return NotFound();
            }
            if (id == null)
            {
                return NotFound();
            }

            var loaiThietBi = await _context.LoaiThietBis.FindAsync(id);
            if (loaiThietBi == null)
            {
                return NotFound();
            }
            return View(loaiThietBi);
        }

        // POST: LoaiThietBis/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,TenLoaiThietBi,MoTa")] LoaiThietBi loaiThietBi)
        {
            if (!User.IsInRole("ad") && !User.IsInRole("tk"))
            {
                return NotFound();
            }
            if (id != loaiThietBi.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(loaiThietBi);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LoaiThietBiExists(loaiThietBi.Id))
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
            return View(loaiThietBi);
        }

        // GET: LoaiThietBis/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (!User.IsInRole("ad") && !User.IsInRole("tk"))
            {
                return NotFound();
            }
            if (id == null)
            {
                return NotFound();
            }

            var loaiThietBi = await _context.LoaiThietBis
                .FirstOrDefaultAsync(m => m.Id == id);
            if (loaiThietBi == null)return NotFound();
            return View(loaiThietBi);
        }

        // POST: LoaiThietBis/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            if (id == null) return NotFound();
            if (!User.IsInRole("ad") && !User.IsInRole("tk")) return NotFound();
            var loaiThietBi = await _context.LoaiThietBis.FindAsync(id);
            if (!Utils.CheckCanDeleteLoaiThietBi(_context,(int)id))
            {
                ModelState.AddModelError("TenLoaiThietBi", "Không thể xóa loại thiết bị đã được sử dụng");
                return View(loaiThietBi);
            }
            if (loaiThietBi != null)
            {
                _context.LoaiThietBis.Remove(loaiThietBi);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> DatLich()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DatLich(List<LoaiThietBi> loaiThietBis)
        {

            return View();
        }
        private bool LoaiThietBiExists(int id)
        {
            return _context.LoaiThietBis.Any(e => e.Id == id);
        }
    }
}

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
using Microsoft.CodeAnalysis.Elfie.Model.Structures;
using NuGet.Protocol;

namespace CNPM.Controllers
{
    [Authorize]
    public class ChiTietPhieuMuonsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly SignInManager<TaiKhoan> _signInManager;
        private readonly UserManager<TaiKhoan> _userManager;

        public ChiTietPhieuMuonsController(AppDbContext context, SignInManager<TaiKhoan> signInManager, UserManager<TaiKhoan> userManager)
        {
            _context = context;
            _signInManager = signInManager;
            _userManager = userManager;
        }
        async Task<List<ChiTietPhieuMuon>> getChiTietPhieuMuonByIdPhieuMuon(int idPhieuMuon, TaiKhoan user)
        {
            return await _context.ChiTietPhieuMuons.Include(c => c.PhieuMuon).ThenInclude(p => p.NguoiMuon).Include(c => c.ThietBi).Where(c => c.PhieuMuon.NguoiMuon.IdTaiKhoan == user.Id && c.IdPhieuMuon == idPhieuMuon).ToListAsync();
        }
        async Task<List<ChiTietPhieuMuon>> getChiTietPhieuMuonByIdPhieuMuon(int idPhieuMuon)
        {
            return await _context.ChiTietPhieuMuons.Include(c => c.PhieuMuon).ThenInclude(p => p.NguoiMuon).Include(c => c.ThietBi).Where(c => c.IdPhieuMuon == idPhieuMuon).ToListAsync();
        }
        async Task<ChiTietPhieuMuon?> getChiTietPhieuMuonById(int id)
        {
            return await _context.ChiTietPhieuMuons.Include(c => c.PhieuMuon).ThenInclude(p => p.NguoiMuon).Include(c => c.ThietBi).FirstOrDefaultAsync(c => c.Id == id);
        }
        bool checkRoleUser(bool manage)
        {
            if (!User.IsInRole("ad") && !User.IsInRole("tk") && !manage) return true;
            return false;
        }
        // GET: ChiTietPhieuMuons
        public async Task<IActionResult> Index(int idPhieuMuon,bool manage = false)
        {
            ViewBag.idPhieuMuon= idPhieuMuon;
            if (manage) ViewBag.manage = true; else ViewBag.manage = false;
            dynamic appDbContext;
            if (checkRoleUser(manage))
            {
                var user = await _userManager.GetUserAsync(User);
                appDbContext = await getChiTietPhieuMuonByIdPhieuMuon(idPhieuMuon, user!);
            }
            else
            {
                appDbContext = await getChiTietPhieuMuonByIdPhieuMuon(idPhieuMuon);
            }
            appDbContext.Reverse();
            return View(appDbContext);
        }

        // GET: ChiTietPhieuMuons/Details/5
        public async Task<IActionResult> Details(int? id, bool manage = false)
        {
            if (id == null) return NotFound();
            if (manage) ViewBag.manage = true; else ViewBag.manage = false;

            var chiTietPhieuMuon = await getChiTietPhieuMuonById((int)id);
            if (chiTietPhieuMuon == null) return NotFound();

            if (checkRoleUser(manage))
            {
                var user = await _userManager.GetUserAsync(User);
                if (chiTietPhieuMuon.PhieuMuon.NguoiMuon!.IdTaiKhoan != user!.Id) return NotFound();
            }
            return View(chiTietPhieuMuon);
        }
        // GET: ChiTietPhieuMuons/Create
        public IActionResult Create(int idPhieuMuon)
        {
            if (ViewBag.manage == true) ViewBag.manage = true; else ViewBag.manage = false;
            ViewBag.IdThietBi = new SelectList(_context.LoaiThietBis, "Id", "TenLoaiThietBi");
            ViewBag.idPhieuMuon = idPhieuMuon;
            return View();
        }
        private bool xacNhanNgay(ChiTietPhieuMuon chiTietPhieuMuon)
        {
            int soluong = _context.LoaiThietBis.Where(l => l.Id == chiTietPhieuMuon.IdThietBi).FirstOrDefault().SoLuong;
            soluong -= _context.ThietBis.Where(t => t.IdLoaiThietBi == chiTietPhieuMuon.IdThietBi && t.IdTinhTrang > 1).Count();
            soluong -= _context.ChiTietPhieuMuons.Include(c => c.PhieuMuon).Where(c => 
            c.Id != chiTietPhieuMuon.Id
            && c.IdThietBi == chiTietPhieuMuon.IdThietBi 
            && c.XacNhan == true 
            && ((c.PhieuMuon.NgayMuon >= chiTietPhieuMuon.PhieuMuon.NgayMuon && c.PhieuMuon.NgayMuon <= chiTietPhieuMuon.PhieuMuon.NgayHenTra) || (c.PhieuMuon.NgayHenTra <= chiTietPhieuMuon.PhieuMuon.NgayHenTra && c.PhieuMuon.NgayHenTra >= chiTietPhieuMuon.PhieuMuon.NgayMuon))).Select(c => c.SoLuongMuon).Sum();
            if (soluong < chiTietPhieuMuon.SoLuongMuon) return true;
            return false;
        }
        // POST: ChiTietPhieuMuons/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<IActionResult> Create([Bind("Id,IdThietBi,SoLuongMuon,NgayMuonThucTe,NgayTraThucTe,XacNhan,moTa")] ChiTietPhieuMuon chiTietPhieuMuon, int idPhieuMuon)
        {
            ViewBag.idPhieuMuon = idPhieuMuon;
            chiTietPhieuMuon.PhieuMuon = _context.PhieuMuons.Include(p => p.NguoiMuon).FirstOrDefault(p => p.Id == idPhieuMuon);
            if (xacNhanNgay(chiTietPhieuMuon))
            {
                ModelState.AddModelError("SoLuongMuon", "Số lượng mượn vượt quá số lượng hiện có!");
            }
            if (ViewBag.manage == true)
            {
                chiTietPhieuMuon.IdNguoiDuyet = (await _context.NguoiDungs.FirstOrDefaultAsync(n => n.IdTaiKhoan == _userManager.GetUserId(User)))!.Id;
            }    
            chiTietPhieuMuon.IdPhieuMuon = idPhieuMuon;
            if (ModelState.IsValid)
            {
                _context.Add(chiTietPhieuMuon);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index),new {idPhieuMuon = idPhieuMuon,manage = ViewBag.manage });
            }
            ViewBag.IdThietBi = new SelectList(_context.LoaiThietBis, "Id", "TenLoaiThietBi", chiTietPhieuMuon.IdThietBi);
            return View(chiTietPhieuMuon);
        }

        // GET: ChiTietPhieuMuons/Edit/5
        public async Task<IActionResult> Edit(int? id, bool manage = false)
        {
            if (manage) ViewBag.manage = true; else ViewBag.manage = false;
            if (id == null)
            {
                return NotFound();
            }

            var chiTietPhieuMuon = await _context.ChiTietPhieuMuons.Include(c => c.PhieuMuon).ThenInclude(p => p.NguoiMuon)
                .Include(c => c.ThietBi)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (chiTietPhieuMuon == null)
            {
                return NotFound();
            }
            ViewBag.IdThietBi = new SelectList(_context.LoaiThietBis, "Id", "TenLoaiThietBi", chiTietPhieuMuon.IdThietBi);
            ViewBag.TenLoaiThietBi = _context.LoaiThietBis.Find(chiTietPhieuMuon.IdThietBi).TenLoaiThietBi;
            if (!User.IsInRole("ad") && !User.IsInRole("tk") && !manage)
            {
                var user = await _userManager.GetUserAsync(User);
                if (chiTietPhieuMuon.PhieuMuon.NguoiMuon!.IdTaiKhoan != user!.Id || chiTietPhieuMuon.XacNhan != null)
                {
                    return NotFound();
                }
                return View(chiTietPhieuMuon);
            }
            return View(chiTietPhieuMuon);
        }

        // POST: ChiTietPhieuMuons/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,IdThietBi,SoLuongMuon,NgayMuonThucTe,NgayTraThucTe,XacNhan,moTa")] ChiTietPhieuMuon chiTietPhieuMuon, int idPhieuMuon, bool manage =false)
        {
            int idChiTietPhieuMuon = chiTietPhieuMuon.Id;
            chiTietPhieuMuon.PhieuMuon = _context.PhieuMuons.Include(p => p.NguoiMuon).FirstOrDefault(p => p.Id == idPhieuMuon);
            if (xacNhanNgay(chiTietPhieuMuon) && chiTietPhieuMuon.XacNhan == true)
            {
                ModelState.AddModelError("XacNhan", "Số lượng mượn vượt quá số lượng hiện có!");
                ModelState.AddModelError("SoLuongMuon", "Số lượng mượn vượt quá số lượng hiện có!");
                chiTietPhieuMuon.NgayMuonThucTe = null;
                chiTietPhieuMuon.NgayTraThucTe = null;
                chiTietPhieuMuon.XacNhan = null;
            }
            if (chiTietPhieuMuon.XacNhan == false && chiTietPhieuMuon.NgayMuonThucTe != null)
            {
                ModelState.AddModelError("NgayMuonThucTe", "Không thể xác nhận mượn thiết bị khi không chấp nhận phiếu mượn");
                chiTietPhieuMuon.NgayMuonThucTe = null;
            }
            if (chiTietPhieuMuon.XacNhan == false && chiTietPhieuMuon.NgayTraThucTe != null)
            {
                ModelState.AddModelError("NgayTraThucTe", "Không thể xác nhận trả thiết bị khi không chấp nhận phiếu mượn");
                chiTietPhieuMuon.NgayTraThucTe = null;
            }
            if (chiTietPhieuMuon.NgayMuonThucTe == null && chiTietPhieuMuon.NgayTraThucTe != null)
            {
                ModelState.AddModelError("NgayTraThucTe", "Chưa xác nhận mượn thiết bị");
                chiTietPhieuMuon.NgayTraThucTe = null;
            }
            if (manage) ViewBag.manage = true; else ViewBag.manage = false;
            if (manage)
            {
                chiTietPhieuMuon.IdNguoiDuyet = (await _context.NguoiDungs.FirstOrDefaultAsync(n => n.IdTaiKhoan == _userManager.GetUserId(User))).Id;
            }
            chiTietPhieuMuon.PhieuMuon = _context.PhieuMuons.Include(p => p.NguoiMuon).FirstOrDefault(p => p.Id == idPhieuMuon);
            if (!User.IsInRole("ad") && !User.IsInRole("tk") && !manage)
            {
                var user = await _userManager.GetUserAsync(User);
                if (chiTietPhieuMuon.PhieuMuon.NguoiMuon!.IdTaiKhoan != user!.Id || chiTietPhieuMuon.XacNhan != null)
                {
                    return NotFound();
                }
            }
            chiTietPhieuMuon.IdPhieuMuon = idPhieuMuon;

            if (id != chiTietPhieuMuon.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    int countSoluong = _context.ThietBis.Where(t => t.IdTinhTrang != 1 && t.idChiTietPhieuMuon == chiTietPhieuMuon.Id).Count();
                    //if (chiTietPhieuMuon.XacNhan == true && chiTietPhieuMuon.NgayTraThucTe == null) CapNhatLoaiThietBi(chiTietPhieuMuon.Id, chiTietPhieuMuon.IdThietBi, chiTietPhieuMuon.SoLuongMuon - countSoluong, 2);
                    //if (chiTietPhieuMuon.NgayTraThucTe != null) CapNhatLoaiThietBi(chiTietPhieuMuon.Id, chiTietPhieuMuon.IdThietBi, chiTietPhieuMuon.SoLuongMuon, 1);
                    _context.Update(chiTietPhieuMuon);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ChiTietPhieuMuonExists(chiTietPhieuMuon.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                if (!(!User.IsInRole("ad") && !User.IsInRole("tk") && !manage))
                {
                    return RedirectToAction(nameof(Edit), new { id = idChiTietPhieuMuon, manage = ViewBag.manage });
                }
                return RedirectToAction(nameof(Index), new { idPhieuMuon = idPhieuMuon, manage = ViewBag.manage });
            }
            ViewBag.IdThietBi = new SelectList(_context.LoaiThietBis, "Id", "TenLoaiThietBi", chiTietPhieuMuon.IdThietBi);
            return View(chiTietPhieuMuon);
        }

        // GET: ChiTietPhieuMuons/Delete/5
        public async Task<IActionResult> Delete(int? id,bool manage =false)
        {
            if (manage) return NotFound();
            if (manage) ViewBag.manage = true; else ViewBag.manage = false;
            if (id == null)
            {
                return NotFound();
            }

            var chiTietPhieuMuon = await _context.ChiTietPhieuMuons
                .Include(c => c.PhieuMuon).ThenInclude(p => p.NguoiMuon)
                .Include(c => c.ThietBi)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (chiTietPhieuMuon == null)
            {
                return NotFound();
            }
            if (!User.IsInRole("ad") && !User.IsInRole("tk") && !manage)
            {
                var user = await _userManager.GetUserAsync(User);
                if (chiTietPhieuMuon.PhieuMuon.NguoiMuon!.IdTaiKhoan != user!.Id || chiTietPhieuMuon.NgayMuonThucTe != null)
                {
                    return NotFound();
                }
            }
            return View(chiTietPhieuMuon);
        }

        // POST: ChiTietPhieuMuons/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, bool manage =false)
        {
            if (manage) return NotFound();
            var idPhieuMuon = _context.ChiTietPhieuMuons.Find(id).IdPhieuMuon;
            if (manage) ViewBag.manage = true; else ViewBag.manage = false;
            var chiTietPhieuMuon = await _context.ChiTietPhieuMuons.Include(c => c.PhieuMuon).ThenInclude(c => c.NguoiMuon).FirstAsync(c => c.Id == id);
            if (!User.IsInRole("ad") && !User.IsInRole("tk") && !manage)
            {
                var user = await _userManager.GetUserAsync(User);
                if (chiTietPhieuMuon.PhieuMuon.NguoiMuon!.IdTaiKhoan != user!.Id || chiTietPhieuMuon.NgayMuonThucTe != null)
                {
                    ModelState.AddModelError("XacNhan", "Không thể xóa thiết bị đã được xác nhận mượn");
                    return View(chiTietPhieuMuon);
                }
            }
            if (chiTietPhieuMuon != null)
            {
                //if (chiTietPhieuMuon.XacNhan == true && chiTietPhieuMuon.NgayTraThucTe == null) CapNhatLoaiThietBi(chiTietPhieuMuon.Id, chiTietPhieuMuon.IdThietBi, chiTietPhieuMuon.SoLuongMuon, 1);
                _context.ChiTietPhieuMuons.Remove(chiTietPhieuMuon);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { idPhieuMuon = idPhieuMuon, manage = ViewBag.manage });
        }
        private async void CapNhatLoaiThietBi(int idChiTietPhieuMuon, int idLoaiThietBi, int soLuong, int tinhtrang)
        {
            if (tinhtrang == 2)
                await _context.ThietBis.Where(t => t.IdTinhTrang == 1 && t.IdLoaiThietBi == idLoaiThietBi).Take(soLuong).ForEachAsync(t => { t.IdTinhTrang = 2; t.idChiTietPhieuMuon = idChiTietPhieuMuon; });
            else if (tinhtrang == 1)
                await _context.ThietBis.Where(t => t.idChiTietPhieuMuon == idChiTietPhieuMuon).Take(soLuong).ForEachAsync(t => { t.IdTinhTrang = 1; t.idChiTietPhieuMuon = null; });
        }

        private bool ChiTietPhieuMuonExists(int id)
        {
            return _context.ChiTietPhieuMuons.Any(e => e.Id == id);
        }
    }
}

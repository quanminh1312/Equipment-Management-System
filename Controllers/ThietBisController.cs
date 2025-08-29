using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CNPM.Models;
using Microsoft.AspNetCore.Authorization;

namespace CNPM.Controllers
{
    [Authorize]
    public class ThietBisController : Controller
    {
        private readonly AppDbContext _context;

        public ThietBisController(AppDbContext context)
        {
            _context = context;
        }

        // GET: ThietBis
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.ThietBis.Include(t => t.LoaiThietBi).Include(t => t.TinhTrang).ToList();
            appDbContext.Reverse();
            return View(appDbContext);
        }

        // GET: ThietBis/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var thietBi = await _context.ThietBis
                .Include(t => t.LoaiThietBi)
                .Include(t => t.TinhTrang)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (thietBi == null)
            {
                return NotFound();
            }

            return View(thietBi);
        }

        // GET: ThietBis/Create
        public IActionResult Create()
        {
            ViewData["IdLoaiThietBi"] = new SelectList(_context.LoaiThietBis, "Id", "TenLoaiThietBi");
            ViewData["IdTinhTrang"] = new SelectList(_context.TinhTrang, "Id", "TenTinhTrang");
            return View();
        }

        // POST: ThietBis/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,TenThietBi,MaThietBi,NhaSanXuat,IdLoaiThietBi,IdTinhTrang")] ThietBi thietBi)
        {
            if (ModelState.IsValid)
            {
                var loaiThietBi = _context.LoaiThietBis.Find(thietBi.IdLoaiThietBi);
                loaiThietBi.SoLuong++;
                _context.Add(thietBi);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdLoaiThietBi"] = new SelectList(_context.LoaiThietBis, "Id", "TenLoaiThietBi", thietBi.IdLoaiThietBi);
            ViewData["IdTinhTrang"] = new SelectList(_context.TinhTrang, "Id", "TenTinhTrang", thietBi.IdTinhTrang);
            return View(thietBi);
        }

        // GET: ThietBis/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var thietBi = await _context.ThietBis.FindAsync(id);
            if (thietBi == null)
            {
                return NotFound();
            }
            ViewData["IdLoaiThietBi"] = new SelectList(_context.LoaiThietBis, "Id", "TenLoaiThietBi", thietBi.IdLoaiThietBi);
            ViewData["IdTinhTrang"] = new SelectList(_context.TinhTrang, "Id", "TenTinhTrang", thietBi.IdTinhTrang);
            return View(thietBi);
        }

        // POST: ThietBis/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, [Bind("Id,TenThietBi,MaThietBi,NhaSanXuat,IdLoaiThietBi,IdTinhTrang")] ThietBi thietBi)
        {
            if (id != thietBi.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var thietbicu = _context.ThietBis.Find(thietBi.Id);
                    var loaiThietBi = _context.LoaiThietBis.Find(thietBi.IdLoaiThietBi);
                    loaiThietBi.SoLuong++;
                    var loaiThietBiCu = _context.LoaiThietBis.Find(thietbicu.IdLoaiThietBi);
                    loaiThietBiCu.SoLuong--;
                    _context.Entry(thietbicu).State = EntityState.Detached;
                    _context.Entry(thietBi).State = EntityState.Modified;
                    _context.Update(thietBi);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ThietBiExists(thietBi.Id))
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
            ViewData["IdLoaiThietBi"] = new SelectList(_context.LoaiThietBis, "Id", "TenLoaiThietBi", thietBi.IdLoaiThietBi);
            ViewData["IdTinhTrang"] = new SelectList(_context.TinhTrang, "Id", "TenTinhTrang", thietBi.IdTinhTrang);
            return View(thietBi);
        }

        // GET: ThietBis/Delete/5
        //[Authorize(Roles = "ad")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var thietBi = await _context.ThietBis
                .Include(t => t.LoaiThietBi)
                .Include(t => t.TinhTrang)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (thietBi == null) return NotFound();
            return View(thietBi);
        }

        // POST: ThietBis/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            if (id == null) return NotFound();
            var thietBi = await _context.ThietBis.FindAsync(id);
            if (Utils.CheckCanDeleteThietBi(_context, thietBi.Id))
            {
                if (thietBi != null)
                {
                    var loaiThietBiCu = _context.LoaiThietBis.Find(_context.ThietBis.FirstOrDefault(t => t.Id == id).IdLoaiThietBi);
                    loaiThietBiCu.SoLuong--;
                    _context.ThietBis.Remove(thietBi);
                }
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ThietBiExists(int? id)
        {
            return _context.ThietBis.Any(e => e.Id == id);
        }
    }
}

using CNPM.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace CNPM.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;

        public HomeController(ILogger<HomeController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            //check if user is logged in
            if (User.Identity.IsAuthenticated)
            {
                if (User.IsInRole("ad"))
                {
                    return RedirectToAction("AdminPage", "Home");
                }
                if (User.IsInRole("tk"))
                {
                    return RedirectToAction("Index", "phieumuons", new { status = Utils.Status.All.ToString(), manage = true });
                }
                return RedirectToAction("Index", "phieumuons", new { status = Utils.Status.All.ToString() });
            }
            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }
        public async Task<IActionResult> AdminPage()
        {
            if (User.Identity.IsAuthenticated)
            {
                if (User.IsInRole("ad"))
                {
                    //lấy count số lượng người dùng
                    var count = await _context.NguoiDungs.CountAsync();
                    ViewBag.NguoiDung = count;
                    //lấy count số lượng thiết bị
                    count = await _context.ThietBis.CountAsync();
                    ViewBag.ThietBi = count;
                    //lấy count số lượng phiếu mượn
                    count = await _context.PhieuMuons.CountAsync();
                    ViewBag.PhieuMuon = count;
                    //lấy count số lượng loại thiết bị
                    count = await _context.LoaiThietBis.CountAsync();
                    ViewBag.LoaiThietBi = count;
                    ViewBag.Pending = await PhieuMuons(Utils.Status.Pending);
                    ViewBag.Accepted = await PhieuMuons(Utils.Status.Accepted);
                    ViewBag.Finished = await PhieuMuons(Utils.Status.Finished);
                    ViewBag.Borrowed = await PhieuMuons(Utils.Status.Borrowed);
                    ViewBag.CTPending = await CTPhieuMuon(Utils.Status.Pending);
                    ViewBag.CTAccepted = await CTPhieuMuon(Utils.Status.Accepted);
                    ViewBag.CTFinished = await CTPhieuMuon(Utils.Status.Finished);
                    ViewBag.CTBorrowed = await CTPhieuMuon(Utils.Status.Borrowed);
                    //get list chi tiet phieu muon so luong phieu muon ngay muon ngay tra
                    var chiTietPhieuMuon = _context.ChiTietPhieuMuons.Include(c => c.PhieuMuon).ToList();
                    var list = from phieumuon in chiTietPhieuMuon
                               select new ChiTietPhieuMuonModel
                               {
                                   thietbi = phieumuon.IdThietBi,
                                   soluong = phieumuon.SoLuongMuon,
                                   start = phieumuon.PhieuMuon.NgayMuon,
                                   end = phieumuon.PhieuMuon.NgayHenTra
                               };
                    ViewData["list"] = list.OrderByDescending(c => c.end).ToList();
                    return View();
                }
            }
            return NotFound();
        }
        public async Task<int> PhieuMuons(Utils.Status status)
        {
            switch (status)
            {
                case Utils.Status.Pending:
                    return await _context.ChiTietPhieuMuons.Include(c => c.PhieuMuon).GroupBy(c => c.PhieuMuon).Where(g => g.Any(c => c.XacNhan == null)).CountAsync();
                case Utils.Status.Accepted:
                    return await _context.ChiTietPhieuMuons.Include(c => c.PhieuMuon).GroupBy(c => c.PhieuMuon).Where(g => g.Any(c => c.NgayTraThucTe == null && c.NgayMuonThucTe == null && c.XacNhan != null)).CountAsync();
                case Utils.Status.Borrowed:
                    return await _context.ChiTietPhieuMuons.Include(c => c.PhieuMuon).GroupBy(c => c.PhieuMuon).Where(g => g.Any(c => c.NgayTraThucTe == null && c.NgayMuonThucTe != null)).CountAsync();
                case Utils.Status.Finished:
                    return await _context.ChiTietPhieuMuons.Include(c => c.PhieuMuon).GroupBy(c => c.PhieuMuon).Where(g => !g.Any(c => c.NgayTraThucTe != null) && g.All(c => c.XacNhan == false)).CountAsync();
                default:
                    return await _context.PhieuMuons.Include(p => p.NguoiMuon).CountAsync();
            }
        }
        public async Task<int> CTPhieuMuon(Utils.Status status)
        {
            switch (status)
            {
                case Utils.Status.Pending:
                    return await _context.ChiTietPhieuMuons.Where(c => c.XacNhan == null).CountAsync();
                case Utils.Status.Accepted:
                    return await _context.ChiTietPhieuMuons.Where(c => c.NgayTraThucTe == null && c.NgayMuonThucTe == null && c.XacNhan != null).CountAsync();
                case Utils.Status.Borrowed:
                    return await _context.ChiTietPhieuMuons.Where(c => c.NgayTraThucTe == null && c.NgayMuonThucTe != null).CountAsync();
                case Utils.Status.Finished:
                    return await _context.ChiTietPhieuMuons.Where(c => c.NgayTraThucTe != null && c.XacNhan == false).CountAsync();
                default:
                    return await _context.PhieuMuons.CountAsync();
            }
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

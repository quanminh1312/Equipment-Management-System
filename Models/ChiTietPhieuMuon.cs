using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CNPM.Models
{
    public class ChiTietPhieuMuon
    {
        public ChiTietPhieuMuon()
        {
        }
        public ChiTietPhieuMuon(int idPhieuMuon, int idThietBi, int soLuongMuon, int? idNguoiDuyet, DateOnly? ngayMuonThucTe, DateOnly? ngayTraThucTe, bool? xacNhan, string? moTa)
        {
            IdPhieuMuon = idPhieuMuon;
            IdThietBi = idThietBi;
            SoLuongMuon = soLuongMuon;
            IdNguoiDuyet = idNguoiDuyet;
            NgayMuonThucTe = ngayMuonThucTe;
            NgayTraThucTe = ngayTraThucTe;
            XacNhan = xacNhan;
            this.moTa = moTa;
        }
        [Key]
        public int Id { get; set; }
        [Display(Name = "Phiếu mượn")]
        public int IdPhieuMuon { get; set; }
        [ForeignKey("IdPhieuMuon")]
        [Display(Name = "Phiếu mượn")]
        public virtual PhieuMuon? PhieuMuon { get; set; }
        [Display(Name = "Thiết bị")]
        public int IdThietBi { get; set; }
        [ForeignKey("IdThietBi")]
        [Display(Name = "Thiết bị")]
        public virtual LoaiThietBi? ThietBi { get; set; }
        [Display(Name = "Số lượng thiết bị mượn")]
        public int SoLuongMuon { get; set; }
        [Display(Name = "Người duyệt")]
        public int? IdNguoiDuyet { get; set; }
        [ForeignKey("IdNguoiDuyet")]
        [DeleteBehavior(DeleteBehavior.Restrict)]
        [Display(Name = "Người duyệt")]
        public virtual NguoiDung? NguoiDuyet { get; set; }
        [Display(Name = "Ngày mượn thực tế")]
        [DataType(DataType.Date)]
        public DateOnly? NgayMuonThucTe { get; set; }
        [Display(Name = "Ngày trả thực tế")]
        [DataType(DataType.Date)]
        public DateOnly? NgayTraThucTe { get; set; }
        [Display(Name = "Duyệt xác nhận")]
        public bool? XacNhan { get; set; }
        [Display(Name = "Mô tả")]
        public string? moTa { get; set; }
    }
}

using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CNPM.Models
{
    public class PhieuMuon
    {
        public PhieuMuon(int idNguoiMuon, DateOnly ngayMuon, DateOnly ngayHenTra)
        {
            IdNguoiMuon = idNguoiMuon;
            NgayMuon = ngayMuon;
            NgayHenTra = ngayHenTra;
        }
        public PhieuMuon()
        {
        }
        [Key]
        public int Id { get; set; }


        [Display(Name = "Người mượn")]
        public int IdNguoiMuon { get; set; }
        [ForeignKey("IdNguoiMuon")]
        [Display(Name = "Người mượn")]
        public virtual NguoiDung? NguoiMuon { get; set; }
        [Display(Name = "Ngày lập phiếu")]
        public DateOnly NgayLapPhieuMuon { get; set; } = DateOnly.FromDateTime(DateTime.Now);
        [Display(Name = "Ngày mượn")]
        [DataType(DataType.Date)]
        public DateOnly NgayMuon { get; set; }
        [Display(Name = "Ngày hẹn trả")]
        [DataType(DataType.Date)]
        public DateOnly NgayHenTra { get; set; }
        
    }
}

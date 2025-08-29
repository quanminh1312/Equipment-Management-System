using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CNPM.Models
{
    public class ThietBi
    {
        public ThietBi(string tenThietBi, string maThietBi, string nhaSanXuat, int idLoaiThietBi, int idTinhTrang)
        {
            TenThietBi = tenThietBi;
            MaThietBi = maThietBi;
            NhaSanXuat = nhaSanXuat;
            IdLoaiThietBi = idLoaiThietBi;
            IdTinhTrang = idTinhTrang;
        }
        public ThietBi()
        {
        }
        [Key]
        public int Id { get; set; }
        [Display(Name = "Tên thiết bị")]
        public string TenThietBi { get; set; }
        [Required]
        [Display(Name = "Mã thiết bị")]
        public string MaThietBi { get; set; }
        [Display(Name = "Năm sản xuất")]
        public string NhaSanXuat { get; set; }
        [Display(Name = "Loại thiết bị")]
        public int IdLoaiThietBi { get; set; }
        [ForeignKey("IdLoaiThietBi")]
        [Display(Name = "Loại thiết bị")]
        public virtual LoaiThietBi? LoaiThietBi { get; set; }
        [Display(Name = "Tình trạng")]
        public int IdTinhTrang { get; set; }
        [ForeignKey("IdTinhTrang")]
        [Display(Name = "Tình trạng")]
        public virtual TinhTrang? TinhTrang { get; set; }
        public int? idChiTietPhieuMuon { get; set; }
    }
}

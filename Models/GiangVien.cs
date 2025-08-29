using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace CNPM.Models
{
    public class GiangVien : NguoiDung
    {
        [Display(Name = "Khoa")]
        public string? Khoa { get; set; }
        [Display(Name = "Học vị")]
        public string? HocVi { get; set; }
        [Display(Name = "Chuyên ngành")]
        public string? ChuyenNganh { get; set; }
    }
}

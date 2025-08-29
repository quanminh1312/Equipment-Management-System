using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CNPM.Models
{
    public class SinhVien : NguoiDung
    {
        [Display(Name = "Lớp")]
        public string? Lop { get; set; }
        [Display(Name = "Khoa")]
        public string? Khoa { get; set; }
        [Display(Name = "Hệ đào tạo")]
        public string? HeDaoTao { get; set; }
        [Display(Name = "Chuyên ngành")]
        public string? ChuyenNganh { get; set; }

    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CNPM.Models
{
    public class ThuKho : NguoiDung
    {
        [Display(Name = "Hệ số lương")]
        public int HeSoLuong { get; set; }
    }
}

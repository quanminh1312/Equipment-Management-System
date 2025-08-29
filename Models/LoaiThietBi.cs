using System.ComponentModel.DataAnnotations;

namespace CNPM.Models
{
    public class LoaiThietBi
    {
        public LoaiThietBi()
        {
            SoLuong = 0;
        }
        [Key]
        public int Id { get; set; }
        [Required]
        [Display(Name = "Tên loại thiết bị")]
        public string TenLoaiThietBi { get; set; }
        [Display(Name = "Số lượng")]
        public int SoLuong { get; set; }
        [Display(Name = "Mô tả")]
        public string? MoTa { get; set; }
    }
}

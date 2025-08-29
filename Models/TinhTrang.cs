using System.ComponentModel.DataAnnotations;

namespace CNPM.Models
{
    public class TinhTrang
    {
        public TinhTrang(string tenTinhTrang)
        {
            TenTinhTrang = tenTinhTrang;
        }
        public TinhTrang()
        {
        }
        [Key]
        public int Id { get; set; }
        [Required]
        [Display(Name = "Tên tình trạng")]
        public string TenTinhTrang { get; set; }
    }
}

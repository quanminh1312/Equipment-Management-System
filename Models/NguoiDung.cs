using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CNPM.Models
{
    public class NguoiDung
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Mã người dùng")]
        public string MaNguoiDung { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Display(Name = "Tên người dùng")]
        public string? TenNguoiDung { get; set; }
        [Display(Name = "Giới tính")]
        public string? GioiTinh { get; set; }
        [Display(Name = "Địa chỉ")]
        public string? DiaChi { get; set; }
        [Phone]
        [Display(Name = "Số điện thoại")]
        public string? SoDienThoai { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Ngày sinh")]
        public string? NgaySinh { get; set; }
        [DataType(DataType.ImageUrl)]
        [Display(Name = "Ảnh đại diện")]
        public string? AnhDaiDien { get; set; }
        [Display(Name = "Tài khoản")]
        public string IdTaiKhoan { get; set; }
        [ForeignKey("IdTaiKhoan")]
        [Display(Name = "Tài khoản")]
        public virtual TaiKhoan? TaiKhoan { get; set; }
    }
}

using KulupYonetimi.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace KulupYonetimi.Models.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Ad alanı zorunludur.")]
        public string Ad { get; set; } = string.Empty;

        [Required(ErrorMessage = "Soyad alanı zorunludur.")]
        public string Soyad { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email alanı zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçersiz email adresi.")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@dogus\.edu\.tr$", ErrorMessage = "Sadece @dogus.edu.tr uzantılı mail adresleri kabul edilmektedir.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifre alanı zorunludur.")]
        [DataType(DataType.Password)]
        public string Sifre { get; set; } = string.Empty;

        [Required]
        public Rol Rol { get; set; }

        public int? KulupId { get; set; }
    }
}

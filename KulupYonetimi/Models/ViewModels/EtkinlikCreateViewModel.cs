using System.ComponentModel.DataAnnotations;

namespace KulupYonetimi.Models.ViewModels
{
    public class EtkinlikCreateViewModel
    {
        [Required(ErrorMessage = "Ad alanı zorunludur.")]
        public string Ad { get; set; } = string.Empty;

        public string? Aciklama { get; set; }

        [Required(ErrorMessage = "Tarih alanı zorunludur.")]
        [DataType(DataType.Date)]
        public DateTime EtkinlikGunu { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Saat alanı zorunludur.")]
        public string EtkinlikSaati { get; set; } = string.Empty;

        public string? Konum { get; set; }
    }
}

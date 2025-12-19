using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KulupYonetimi.Models.Entities
{
    public class Etkinlik
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Ad { get; set; } = string.Empty;

        public string? Aciklama { get; set; }

        public DateTime Tarih { get; set; }

        public string? Konum { get; set; }

        public int KulupId { get; set; }

        [ForeignKey("KulupId")]
        public virtual Kulup Kulup { get; set; } = null!;

        public virtual ICollection<EtkinlikKayit> Kayitlar { get; set; } = new List<EtkinlikKayit>();
        public virtual ICollection<Degerlendirme> Degerlendirmeler { get; set; } = new List<Degerlendirme>();
    }
}

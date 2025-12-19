using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KulupYonetimi.Models.Entities
{
    public class Kulup
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Ad { get; set; } = string.Empty;

        public string? Aciklama { get; set; }

        public int? YoneticiId { get; set; }

        [ForeignKey("YoneticiId")]
        public virtual Kullanici? Yonetici { get; set; }

        public virtual ICollection<Etkinlik> Etkinlikler { get; set; } = new List<Etkinlik>();
        public virtual ICollection<KullaniciKulup> Uyeler { get; set; } = new List<KullaniciKulup>();
        public virtual ICollection<Duyuru> Duyurular { get; set; } = new List<Duyuru>();
    }
}

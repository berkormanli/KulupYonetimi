using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KulupYonetimi.Models.Entities
{
    public class Degerlendirme
    {
        [Key]
        public int Id { get; set; }

        public int EtkinlikId { get; set; }
        public int KullaniciId { get; set; }

        [Range(1, 5)]
        public int Puan { get; set; }

        public string? Yorum { get; set; }

        public DateTime Tarih { get; set; } = DateTime.Now;

        [ForeignKey("EtkinlikId")]
        public virtual Etkinlik? Etkinlik { get; set; }

        [ForeignKey("KullaniciId")]
        public virtual Kullanici? Kullanici { get; set; }
    }
}

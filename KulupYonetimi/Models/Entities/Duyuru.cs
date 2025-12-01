using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KulupYonetimi.Models.Entities
{
    public class Duyuru
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Baslik { get; set; }

        [Required]
        public string Icerik { get; set; }

        public DateTime YayinTarihi { get; set; } = DateTime.Now;

        public DateTime? GecerlilikBitis { get; set; }

        [NotMapped]
        public int? GecerlilikSuresiGun { get; set; }

        public int KulupId { get; set; }

        [ForeignKey("KulupId")]
        public virtual Kulup Kulup { get; set; }
    }
}

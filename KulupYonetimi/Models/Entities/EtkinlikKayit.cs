using System.ComponentModel.DataAnnotations.Schema;

namespace KulupYonetimi.Models.Entities
{
    public class EtkinlikKayit
    {
        public int KullaniciId { get; set; }
        public int EtkinlikId { get; set; }

        [ForeignKey("KullaniciId")]
        public virtual Kullanici Kullanici { get; set; }

        [ForeignKey("EtkinlikId")]
        public virtual Etkinlik Etkinlik { get; set; }
    }
}

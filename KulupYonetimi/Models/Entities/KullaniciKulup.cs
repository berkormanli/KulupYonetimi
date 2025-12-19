using System.ComponentModel.DataAnnotations.Schema;

namespace KulupYonetimi.Models.Entities
{
    public class KullaniciKulup
    {
        public int KullaniciId { get; set; }
        public int KulupId { get; set; }

        [ForeignKey("KullaniciId")]
        public virtual Kullanici Kullanici { get; set; } = null!;

        [ForeignKey("KulupId")]
        public virtual Kulup Kulup { get; set; } = null!;
    }
}

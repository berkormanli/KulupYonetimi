using System.ComponentModel.DataAnnotations;

namespace KulupYonetimi.Models.Entities
{
    public enum Rol
    {
        Ogrenci,
        KulupYoneticisi
    }

    public class Kullanici
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Ad { get; set; }

        [Required]
        public string Soyad { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Sifre { get; set; } // Not: Gerçek projede şifre hash'lenerek saklanmalıdır.

        public Rol Rol { get; set; }

        public virtual ICollection<Kulup> YonetilenKulupler { get; set; } = new List<Kulup>();
        public virtual ICollection<KullaniciKulup> UyeOlunanKulupler { get; set; } = new List<KullaniciKulup>();
        public virtual ICollection<EtkinlikKayit> KayitliEtkinlikler { get; set; } = new List<EtkinlikKayit>();
    }
}

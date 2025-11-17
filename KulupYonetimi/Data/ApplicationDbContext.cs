using KulupYonetimi.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace KulupYonetimi.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Kullanici> Kullanicilar { get; set; }
        public DbSet<Kulup> Kulupler { get; set; }
        public DbSet<Etkinlik> Etkinlikler { get; set; }
        public DbSet<KullaniciKulup> KullaniciKulupler { get; set; }
        public DbSet<EtkinlikKayit> EtkinlikKayitlari { get; set; }
        public DbSet<Degerlendirme> Degerlendirmeler { get; set; }
        public DbSet<Duyuru> Duyurular { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // KullaniciKulup (Many-to-Many)
            modelBuilder.Entity<KullaniciKulup>()
                .HasKey(kk => new { kk.KullaniciId, kk.KulupId });

            modelBuilder.Entity<KullaniciKulup>()
                .HasOne(kk => kk.Kullanici)
                .WithMany(k => k.UyeOlunanKulupler)
                .HasForeignKey(kk => kk.KullaniciId);

            modelBuilder.Entity<KullaniciKulup>()
                .HasOne(kk => kk.Kulup)
                .WithMany(k => k.Uyeler)
                .HasForeignKey(kk => kk.KulupId);

            // EtkinlikKayit (Many-to-Many)
            modelBuilder.Entity<EtkinlikKayit>()
                .HasKey(ek => new { ek.KullaniciId, ek.EtkinlikId });

            modelBuilder.Entity<EtkinlikKayit>()
                .HasOne(ek => ek.Kullanici)
                .WithMany(k => k.KayitliEtkinlikler)
                .HasForeignKey(ek => ek.KullaniciId);

            modelBuilder.Entity<EtkinlikKayit>()
                .HasOne(ek => ek.Etkinlik)
                .WithMany(e => e.Kayitlar)
                .HasForeignKey(ek => ek.EtkinlikId);
        }
    }
}

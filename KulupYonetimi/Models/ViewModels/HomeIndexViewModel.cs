using KulupYonetimi.Models.Entities;

namespace KulupYonetimi.Models.ViewModels
{
    public class YoneticiDashboardViewModel
    {
        public Kulup YonetilenKulup { get; set; }
        public List<Etkinlik> Etkinlikler { get; set; }
        public List<Duyuru> Duyurular { get; set; }
    }

    public class OgrenciDashboardViewModel
    {
        public List<Etkinlik> Etkinlikler { get; set; }
    }

    public class HomeIndexViewModel
    {
        public bool IsYonetici { get; set; }
        public YoneticiDashboardViewModel YoneticiDashboard { get; set; }
        public OgrenciDashboardViewModel OgrenciDashboard { get; set; }
    }
}

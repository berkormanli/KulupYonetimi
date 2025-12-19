using System;
using System.Collections.Generic;
using KulupYonetimi.Models.Entities;

namespace KulupYonetimi.Models.ViewModels
{
    public class EtkinlikAnalizViewModel
    {
        public int EtkinlikId { get; set; }
        public string Ad { get; set; } = string.Empty;
        public DateTime Tarih { get; set; }
        public int KatilimciSayisi { get; set; }
        public int DegerlendirmeSayisi { get; set; }
        public int YorumSayisi { get; set; }
        public double? OrtalamaPuan { get; set; }
    }

    public class YoneticiDashboardViewModel
    {
        public Kulup? YonetilenKulup { get; set; }
        public List<Etkinlik> Etkinlikler { get; set; } = new();
        public List<Duyuru> Duyurular { get; set; } = new();
        public List<EtkinlikAnalizViewModel> GecmisEtkinlikAnalizleri { get; set; } = new();
    }

    public class OgrenciDashboardViewModel
    {
        public List<Etkinlik> Etkinlikler { get; set; } = new();
    }

    public class HomeIndexViewModel
    {
        public bool IsYonetici { get; set; }
        public YoneticiDashboardViewModel YoneticiDashboard { get; set; } = new();
        public OgrenciDashboardViewModel OgrenciDashboard { get; set; } = new();
    }
}

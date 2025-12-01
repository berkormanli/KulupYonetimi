namespace KulupYonetimi.Models
{
    public class EmailSettings
    {
        public string MailServer { get; set; }
        public int MailPort { get; set; }
        public string SenderName { get; set; }
        public string SenderEmail { get; set; }
        public string SenderPassword { get; set; }
        public bool EnableSsl { get; set; }
        public bool TrustServerCertificate { get; set; } = false;
    }
}

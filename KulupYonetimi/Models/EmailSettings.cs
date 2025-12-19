namespace KulupYonetimi.Models
{
    public class EmailSettings
    {
        public string MailServer { get; set; } = string.Empty;
        public int MailPort { get; set; }
        public string SenderName { get; set; } = string.Empty;
        public string SenderEmail { get; set; } = string.Empty;
        public string SenderPassword { get; set; } = string.Empty;
        public bool EnableSsl { get; set; }
        public bool TrustServerCertificate { get; set; } = false;
    }
}

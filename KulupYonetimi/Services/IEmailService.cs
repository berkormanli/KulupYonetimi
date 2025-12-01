using System.Threading.Tasks;

namespace KulupYonetimi.Services
{
    public interface IEmailService
    {
        Task SendAsync(string toEmail, string subject, string body);
    }
}

using PADMA.Core.Models;

namespace PADMA.Core.Services
{
    using System.Text;
    using System.Text.Json;

    public class EmailJsFeedbackService : IFeedbackService
    {
        private readonly HttpClient _httpClient = new();

        private const string ServiceId = "service_pssu2i8";
        private const string TemplateId = "template_a49aqst";
        private const string PublicKey = "jcWHLIKr_QOgybDKM";
        private const string Endpoint = "https://api.emailjs.com/api/v1.0/email/send";

        public async Task<bool> SendAsync(FeedbackMessage message)
        {
            try
            {
                var payload = new
                {
                    service_id = ServiceId,
                    template_id = TemplateId,
                    user_id = PublicKey,
                    template_params = new
                    {
                        category = message.Category,
                        message = message.Message,
                        email = string.IsNullOrWhiteSpace(message.Email) ? "not-provided" : message.Email,
                        date = message.Date,
                        language = message.Language,
                        app_version = message.AppVersion
                    }
                };

                var json = JsonSerializer.Serialize(payload);

                using var content = new StringContent(json, Encoding.UTF8, "application/json");
                using var response = await _httpClient.PostAsync(Endpoint, content);

                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

    }
}

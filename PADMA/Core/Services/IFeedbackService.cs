using PADMA.Core.Models;

namespace PADMA.Core.Services
{
    public interface IFeedbackService
    {
        Task<bool> SendAsync(FeedbackMessage message);
    }
}

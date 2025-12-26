using System.Threading;
using System.Threading.Tasks;

namespace PADMA.UI.Services
{
    public interface IDayComputationService
    {
        Task<DayOverviewData> GetOverviewAsync(DayKey key, DayItem baseDay, CancellationToken ct = default);
        Task<DayDetailsData> GetDetailsAsync(DayKey key, DayItem baseDay, CancellationToken ct = default);

        void InvalidateProfile(int profileId);
        void InvalidateAll();
    }
}

using System.Diagnostics;

namespace PADMA.Core.Services
{
    public sealed class UserEventRetentionService
    {
        private readonly DatabaseService _db;

        public UserEventRetentionService(DatabaseService db)
        {
            _db = db;
        }

        public Task ApplyAsync()
        {
            try
            {
                var active = _db.GetActiveSettingCode("RETENTION");

                if (string.IsNullOrWhiteSpace(active) ||
                    active.Equals("OFF", StringComparison.OrdinalIgnoreCase))
                    return Task.CompletedTask;

                DateTime cutoffLocal;

                switch (active.ToUpperInvariant())
                {
                    case "DAY30":
                        cutoffLocal = DateTime.Now.AddDays(-30);
                        break;

                    case "MONTH3":
                        cutoffLocal = DateTime.Now.AddMonths(-3);
                        break;

                    case "MONTH6":
                        cutoffLocal = DateTime.Now.AddMonths(-6);
                        break;

                    default:
                        return Task.CompletedTask;
                }

                var deleted = _db.PurgeUserEventsEndedBefore(cutoffLocal);

                if (deleted > 0)
                    Debug.WriteLine($"[PADMA] Retention cleanup deleted {deleted} user events.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[PADMA] Retention cleanup failed: {ex.Message}");
            }

            return Task.CompletedTask;
        }
    }
}
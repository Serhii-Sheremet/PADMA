namespace PADMA.Core.Services;

public interface IUserNoteReminderService
{
    Task RefreshAsync(CancellationToken ct = default);
    Task CancelAllAsync(CancellationToken ct = default);
}

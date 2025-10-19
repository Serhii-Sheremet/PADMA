namespace PADMA.Core.Models;

public class Profile
{
    public int Id { get; set; }

    public string ProfileName { get; set; } = string.Empty;

    public string PersonName { get; set; } = string.Empty;

    public string PersonSurname { get; set; } = string.Empty;

    public DateTime DateOfBirth { get; set; }

    public int? PlaceOfBirthId { get; set; }

    public int? PlaceOfLivingId { get; set; }

    public string? Message { get; set; }

    public bool Checked { get; set; }

    // дополнительные свойства для привязки (не из БД)
    public string PlaceOfBirthLocality { get; set; } = string.Empty;

    public string PlaceOfLivingLocality { get; set; } = string.Empty;
}

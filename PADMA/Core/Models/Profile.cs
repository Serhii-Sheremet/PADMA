using System;
using SQLite;

namespace PADMA.Core.Models
{
    [Table("PROFILE")]
    public class Profile
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Column("PROFILENAME")]
        public string ProfileName { get; set; } = string.Empty;

        [Column("PERSONNAME")]
        public string PersonName { get; set; } = string.Empty;

        [Column("PERSONSURNAME")]
        public string PersonSurname { get; set; } = string.Empty;

        // Дата рождения хранится как TEXT в формате yyyy-MM-dd HH:mm:ss
        [Column("DATEOFBIRTH")]
        public string DateOfBirthString
        {
            get => DateOfBirth.ToString("yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
            set
            {
                if (DateTime.TryParseExact(value,
                        "yyyy-MM-dd HH:mm:ss",
                        System.Globalization.CultureInfo.InvariantCulture,
                        System.Globalization.DateTimeStyles.None,
                        out var parsed))
                    DateOfBirth = parsed;
                else
                    DateOfBirth = DateTime.MinValue;
            }
        }

        [Ignore] // не сохраняется в БД напрямую
        public DateTime DateOfBirth { get; set; } = DateTime.Now;

        [Column("PLACEOFBIRTHID")]
        public int? PlaceOfBirthId { get; set; }

        [Column("PLACEOFLIVINGID")]
        public int? PlaceOfLivingId { get; set; }

        [Column("MESSAGE")]
        public string Message { get; set; } = string.Empty;

        [Column("CHECKED")]
        public int CheckedInt
        {
            get => Checked ? 1 : 0;
            set => Checked = value == 1;
        }

        [Ignore]
        public bool Checked { get; set; }

        // локальные (вычисляемые) поля
        [Ignore]
        public string PlaceOfBirthLocality { get; set; } = string.Empty;

        [Ignore]
        public string PlaceOfLivingLocality { get; set; } = string.Empty;
    }
}

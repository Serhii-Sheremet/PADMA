using System;
using System.Globalization;
using SQLite;

namespace PADMA.Core.Models
{
    [Table("USER_EVENTS")]
    public sealed class UserEvent
    {
        [PrimaryKey, AutoIncrement]
        [Column("ID")]
        public int Id { get; set; }

        [Column("PROFILEID")]
        public int ProfileId { get; set; }

        // Stored in DB as TEXT/VARCHAR(20) in format "yyyy-MM-dd HH:mm:ss"
        [Column("DATESTART")]
        public string DateStartString
        {
            get => StartLocal.ToString(UserEventDateHelper.DbFormat, CultureInfo.InvariantCulture);
            set => StartLocal = UserEventDateHelper.TryParse(value);
        }

        // Stored in DB as TEXT/VARCHAR(20) in format "yyyy-MM-dd HH:mm:ss"
        [Column("DATEEND")]
        public string DateEndString
        {
            get => EndLocal.ToString(UserEventDateHelper.DbFormat, CultureInfo.InvariantCulture);
            set => EndLocal = UserEventDateHelper.TryParse(value);
        }

        [Ignore]
        public DateTime StartLocal { get; set; } = DateTime.MinValue;

        [Ignore]
        public DateTime EndLocal { get; set; } = DateTime.MinValue;

        [Column("NAME")]
        public string Name { get; set; } = string.Empty;

        [Column("MESSAGE")]
        public string Message { get; set; } = string.Empty;

        [Column("ARGBVALUE")]
        public int ArgbValue { get; set; }
    }

    public static class UserEventDateHelper
    {
        public const string DbFormat = "yyyy-MM-dd HH:mm:ss";

        public static DateTime TryParse(string? s)
        {
            if (string.IsNullOrWhiteSpace(s))
                return DateTime.MinValue;

            return DateTime.TryParseExact(
                s.Trim(),
                DbFormat,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var dt)
                ? dt
                : DateTime.MinValue;
        }

        public static string ToDbString(DateTime dt) =>
            dt.ToString(DbFormat, CultureInfo.InvariantCulture);
    }
}

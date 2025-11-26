using SQLite;

namespace CalendrierDeLavent
{
    public class AdventPlayer
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Indexed]
        public string SteamId { get; set; } = "";

        // Stocke les jours sous format "1;2;5;..."
        public string OpenedDaysRaw { get; set; } = ""; 
    }
}
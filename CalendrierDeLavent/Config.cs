namespace CalendrierDeLavent
{
    public class Config
    {
        public bool DebugMode { get; set; } = false;
        public bool Debug_UnlimitedGifts { get; set; } = false;
        public string DiscordWebhookUrl { get; set; } = "";
        
        // Plage de gains pour les jours normaux
        public int MoneyRewardDailyMin { get; set; } = 1000;
        public int MoneyRewardDailyMax { get; set; } = 10000;
        
        // Gain fixe pour le 24 décembre
        public int MoneyRewardChristmas { get; set; } = 25000;
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Life;
using Life.Network;
using Life.UI;
using ModKit.Helper;
using ModKit.Internal;
using ModKit.Interfaces;
using ModKit.Helper.DiscordHelper;
using SQLite;
using UnityEngine; 
using Newtonsoft.Json;
using _menu = AAMenu.Menu;
using Logger = ModKit.Internal.Logger;
using mk = ModKit.Helper.TextFormattingHelper;

namespace CalendrierDeLavent
{
    public class AdventCalendar : ModKit.ModKit
    {
        public Config Config { get; private set; } = new Config();
        private SQLiteConnection _db = null!; 
        private string _dbPath = string.Empty;
        
        public AdventCalendar(IGameAPI api) : base(api)
        {
            PluginInformations = new PluginInformations("CalendrierAvent", "1.0.0", "Robocnop");
        }

        public override void OnPluginInit()
        {
            base.OnPluginInit();

            string directoryPath = Path.Combine(pluginsPath, "CalendrierAvent");
            if (!Directory.Exists(directoryPath)) Directory.CreateDirectory(directoryPath);

            CreateConfig(directoryPath);

            _dbPath = Path.Combine(directoryPath, "AdventDatabase.db");
            try
            {
                _db = new SQLiteConnection(_dbPath);
                _db.CreateTable<AdventPlayer>();
                Logger.LogSuccess("is ready: yes", "Christmas Calendar DB");
            }
            catch (Exception e)
            {
                Logger.LogError($"is ready: no ({e.Message})", "Christmas Calendar DB");
            }

            InsertMenu();
            Logger.LogSuccess($"Plugin {PluginInformations.SourceName} v{PluginInformations.Version} créé par {PluginInformations.Author}", "Succès");
        }

        public void InsertMenu()
        {
            
            _menu.AddInteractionTabLine(PluginInformations, "Ouvrir le Calendrier de l'Avent", (ui) =>
            {
                Player player = PanelHelper.ReturnPlayerFromPanel(ui);
                OpenCalendarPanel(player);
            });

           
            _menu.AddAdminPluginTabLine(PluginInformations, 1, "Calendrier Avent", (ui) =>
            {
                Player player = PanelHelper.ReturnPlayerFromPanel(ui);
                OpenAdminPanel(player);
            }, 0);
        }

        // --- PANEL ADMIN ---
        public void OpenAdminPanel(Player player)
        {
            Panel panel = PanelHelper.Create("Administration Calendrier", UIPanel.PanelType.Tab, player, () => OpenAdminPanel(player));

            // Infos générales
            panel.AddTabLine($"{mk.Color("Plugin", mk.Colors.Info)} : {PluginInformations.SourceName}", _ => { });
            panel.AddTabLine($"{mk.Color("Version", mk.Colors.Info)} : {PluginInformations.Version}", _ => { });
            panel.AddTabLine($"{mk.Color("Auteur", mk.Colors.Info)} : {PluginInformations.Author}", _ => { });
            
            panel.AddTabLine("--- Configuration Actuelle ---", _ => { });

          
            string debugState = Config.DebugMode ? mk.Color("ACTIVÉ", mk.Colors.Warning) : mk.Color("Désactivé", mk.Colors.Success);
            panel.AddTabLine($"Mode Debug : {debugState}", _ => { });
            
            string spamState = Config.Debug_UnlimitedGifts ? mk.Color("OUI", mk.Colors.Warning) : "Non";
            panel.AddTabLine($"Spam Cadeaux (Debug) : {spamState}", _ => { });

            panel.AddTabLine($"Gains Journaliers : {Config.MoneyRewardDailyMin}€ - {Config.MoneyRewardDailyMax}€", _ => { });
            panel.AddTabLine($"Gains Noël (24) : {Config.MoneyRewardChristmas}€", _ => { });

            // Bouton Retour vers AAMenu (Admin Plugin Panel)
            panel.AddButton("Retour", ui => AAMenu.AAMenu.menu.AdminPluginPanel(player));
            
            panel.AddButton("Fermer", ui => player.ClosePanel(panel));

            player.ShowPanelUI(panel);
        }

        // --- PANEL JOUEUR ---
        public void OpenCalendarPanel(Player player)
        {
            Panel panel = PanelHelper.Create("Calendrier de l'Avent", UIPanel.PanelType.Tab, player, () => OpenCalendarPanel(player));

            DateTime date = DateTime.Now;
            
            bool isDebug = Config.DebugMode && player.IsAdmin;
            bool canSpam = isDebug && Config.Debug_UnlimitedGifts; 
            bool isDecember = date.Month == 12;

            if (isDebug)
            {
                isDecember = true;
                string spamStatus = canSpam ? mk.Color("SPAM AUTORISÉ", mk.Colors.Success) : mk.Color("SÉCURITÉ ACTIVE", mk.Colors.Warning);
                panel.AddTabLine($"[DEBUG] Accès hors-saison. {spamStatus}", _ => { });
            }

            if (!isDecember)
            {
                panel.AddTabLine($"{mk.Color("Erreur", mk.Colors.Error)} : Le calendrier n'est disponible qu'en Décembre !", _ => { });
                panel.AddButton("Retour", ui => AAMenu.AAMenu.menu.InteractionPanel(player));
                player.ShowPanelUI(panel);
                return;
            }

            int currentDay = date.Day;
            if (isDebug && date.Month != 12) currentDay = 1; 

            panel.SetTitle($"Calendrier - {date.ToShortDateString()}");

            List<int> openedDays = GetOpenedDays(player.steamId.ToString());
            bool alreadyOpened = openedDays.Contains(currentDay);
            

            if (alreadyOpened && !canSpam) 
            {
                panel.AddTabLine($"Vous avez déjà ouvert la case du {currentDay} Décembre.", _ => { });
                panel.AddTabLine($"{mk.Color("Revenez demain !", mk.Colors.Info)}", _ => { });
                panel.AddButton("Retour", ui => AAMenu.AAMenu.menu.InteractionPanel(player));
            }
            else if (currentDay > 24 && !isDebug)
            {
                panel.AddTabLine("L'événement de Noël est terminé. Joyeuses fêtes !", _ => { });
                panel.AddButton("Retour", ui => AAMenu.AAMenu.menu.InteractionPanel(player));
            }
            else
            {
                panel.AddTabLine($"Nous sommes le {mk.Color(currentDay.ToString(), mk.Colors.Success)} Décembre.", _ => { });

                if (currentDay == 24)
                {
                    panel.AddTabLine($"Cadeau de Noël : {mk.Color(Config.MoneyRewardChristmas + "€", mk.Colors.Warning)}", _ => { });
                }
                else
                {
                    panel.AddTabLine($"Cadeau du jour : {mk.Color("Mystère", mk.Colors.Warning)}", _ => { });
                    panel.AddTabLine($"Valeur possible : Entre {mk.Color(Config.MoneyRewardDailyMin + "€", mk.Colors.Info)} et {mk.Color(Config.MoneyRewardDailyMax + "€", mk.Colors.Info)}", _ => { });
                }
                
                panel.AddButton("Récupérer mon cadeau", async ui => 
                {
                    player.ClosePanel(panel);
                    
                    if (HasAlreadyOpened(player.steamId.ToString(), currentDay) && !canSpam)
                    {
                        player.Notify("Erreur", "Cadeau déjà récupéré !", NotificationManager.Type.Error);
                        return;
                    }
                    
                    int rewardAmount = GetRewardForDay(currentDay);
                    await GiveReward(player, currentDay, rewardAmount, canSpam);
                });
                
                panel.AddButton("Retour", ui => AAMenu.AAMenu.menu.InteractionPanel(player));
            }

            player.ShowPanelUI(panel);
        }

        private int GetRewardForDay(int day)
        {
            if (day == 24) return Config.MoneyRewardChristmas;
            return UnityEngine.Random.Range(Config.MoneyRewardDailyMin, Config.MoneyRewardDailyMax + 1);
        }

        private async Task GiveReward(Player player, int day, int amount, bool canSpam)
        {
            AddDayToHistory(player.steamId.ToString(), day);

            player.AddBankMoney(amount);
            player.Notify("Cadeau", $"Vous avez reçu {mk.Color(amount + "€", mk.Colors.Success)} !", NotificationManager.Type.Success);

            if (!string.IsNullOrEmpty(Config.DiscordWebhookUrl))
            {
                DiscordWebhookClient client = new DiscordWebhookClient(Config.DiscordWebhookUrl);
                string debugTag = canSpam ? " [TEST]" : "";
                await DiscordHelper.SendMsg(client, $"🎁 **[CALENDRIER]{debugTag}** Le joueur **{player.FullName} ({player.steamId})** a ouvert la case du **{day}** et a gagné **{amount}€**.");
            }
        }

        // --- ORM ---
        private List<int> GetOpenedDays(string steamId)
        {
            var profile = _db.Table<AdventPlayer>().FirstOrDefault(p => p.SteamId == steamId);
            if (profile == null || string.IsNullOrEmpty(profile.OpenedDaysRaw)) return new List<int>();

            return profile.OpenedDaysRaw.Split(';')
                .Where(x => int.TryParse(x, out _))
                .Select(int.Parse)
                .ToList();
        }

        private bool HasAlreadyOpened(string steamId, int day)
        {
            return GetOpenedDays(steamId).Contains(day);
        }

        private void AddDayToHistory(string steamId, int day)
        {
            var profile = _db.Table<AdventPlayer>().FirstOrDefault(p => p.SteamId == steamId);

            if (profile == null)
            {
                profile = new AdventPlayer { SteamId = steamId, OpenedDaysRaw = day.ToString() };
                _db.Insert(profile);
            }
            else
            {
                List<int> days = GetOpenedDays(steamId);
                if (!days.Contains(day))
                {
                    days.Add(day);
                    profile.OpenedDaysRaw = string.Join(";", days);
                    _db.Update(profile);
                }
            }
        }

        // --- CONFIG ---
        public void CreateConfig(string directoryPath)
        {
            string configFilePath = Path.Combine(directoryPath, "config.json");

            if (!File.Exists(configFilePath))
            {
                var defaultConfig = new Config();
                string jsonContent = JsonConvert.SerializeObject(defaultConfig, Formatting.Indented);
                File.WriteAllText(configFilePath, jsonContent);
            }

            string fileContent = File.ReadAllText(configFilePath);
            Config = JsonConvert.DeserializeObject<Config>(fileContent) ?? new Config();
        }
    }
}
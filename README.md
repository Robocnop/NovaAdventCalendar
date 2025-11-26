# 🎄 AdventCalendar 🎁

Ce plugin permet à vos joueurs d'ouvrir une case chaque jour en décembre pour gagner de l'argent. Simple, sécurisé et entièrement configurable (enfin il n'est seulement possible de faire gagner de l'argent pour l'instant).

  

-----

## ✨ Fonctionnalités

  * 📅 **Système Automatique** : Le calendrier ne s'ouvre qu'en **Décembre**. Le reste de l'année, il est fermé \!
  * 💰 **Récompenses Aléatoires** : Définissez une tranche de gains (ex: entre 1000€ et 5000€) pour pimenter l'ouverture quotidienne.
  * 🎅 **Jackpot de Noël** : Une récompense spéciale et fixe pour le **24 Décembre** (Grosse somme d'argent).
  * 🔒 **Sécurisé** : Utilise une base de données **SQLite** locale sur le serveur. Impossible de prendre deux fois le même cadeau, même en se déconnectant.
  * 📢 **Logs Discord** : Chaque ouverture de cadeau est envoyée sur votre Discord via Webhook (avec le Nom et SteamID du joueur).
  * ⚙️ **Mode Debug** : Testez votre calendrier même en plein été grâce aux debug dans la config.

-----

## 📥 Installation

1.  **Téléchargez** le fichier `CalendrierDeLavent.dll`.
2.  Placez-le dans le dossier **`Plugins`** de votre serveur Nova Life.
3.  **Lancez votre serveur**.
4.  Un dossier `Plugins/CalendrierAvent/` sera créé automatiquement avec la configuration.

-----

## 🎮 Comment l'utiliser ?

### Pour les Joueurs 👤

Il suffit d'ouvrir le menu Interaction :

1.  Allez dans l'onglet **Interactions**.
2.  Cliquez sur **"Ouvrir le Calendrier de l'Avent"**.
3.  Si nous sommes le bon jour, cliquez sur **"Récupérer mon cadeau"** \!


-----

## ⚙️ Configuration (`config.json`)

Le fichier se trouve dans `Plugins/CalendrierAvent/config.json`.
Voici à quoi il ressemble et comment le modifier :

```json
{
  "DebugMode": false,
  "Debug_UnlimitedGifts": false,
  "DiscordWebhookUrl": "https://discord.com/api/webhooks/VOTRE_URL_ICI",
  "MoneyRewardDailyMin": 1000,
  "MoneyRewardDailyMax": 5000,
  "MoneyRewardChristmas": 25000
}
```

| Option | Description |
| :--- | :--- |
| **DebugMode** | Mettre sur `true` pour tester le calendrier hors du mois de Décembre (Admin uniquement). |
| **Debug\_UnlimitedGifts** | Si `true` (et Debug actif), permet de récupérer les cadeaux à l'infini pour tester les logs/gains. |
| **DiscordWebhookUrl** | L'URL de votre Webhook Discord pour recevoir les logs. Laisser vide pour désactiver. |
| **MoneyRewardDailyMin** | Le montant minimum d'argent gagné pour un jour normal (1-23). |
| **MoneyRewardDailyMax** | Le montant maximum d'argent gagné pour un jour normal. |
| **MoneyRewardChristmas** | Le montant fixe gagné le **24 Décembre** (Gros lot \!). |

-----


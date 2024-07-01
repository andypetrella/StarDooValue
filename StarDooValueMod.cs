using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System.Collections.Generic;

public class StarDooValueMod : Mod
{
    private const string IntroductionFlag = "DataObservabilityOfficerIntroduction";
    private const string MailFlag = "DataObservabilityOfficerInvitation";
    private const string NextQuestFlag = "DataObservabilityOfficerNextQuest";
    private Dictionary<string, string> mailContents;
    private QuestManager questManager;

    public override void Entry(IModHelper helper)
    {
        questManager = new QuestManager(helper, Monitor);

        helper.Events.GameLoop.DayStarted += OnDayStarted;
        helper.Events.Input.ButtonPressed += questManager.OnButtonPressed;
        helper.Events.Content.AssetRequested += OnAssetRequested;
        helper.Events.Content.AssetReady += OnAssetReady;


        // Load mail content from assets/mail.json
        mailContents = helper.ModContent.Load<Dictionary<string, string>>("assets/mail.json");
    }

    private void OnAssetRequested(object? sender, AssetRequestedEventArgs e)
    {
        if (e.NameWithoutLocale.IsEquivalentTo("Data/mail"))
        {
            e.Edit(asset =>
            {
                var data = asset.AsDictionary<string, string>().Data;
                foreach (var entry in mailContents)
                {
                    data[entry.Key] = entry.Value;
                }
            });
        }
    }

    private void OnAssetReady(object? sender, AssetReadyEventArgs e)
    {
        if (e.NameWithoutLocale.IsEquivalentTo("Data/mail"))
        {
            Monitor.Log("Mail data ready and injected.", LogLevel.Debug);
        }
    }

    private void OnDayStarted(object? sender, DayStartedEventArgs e)
    {
        Monitor.Log("Checking if mail should be sent...", LogLevel.Debug);

        if (!Game1.player.mailReceived.Contains(MailFlag))
        {
            Game1.addMailForTomorrow(MailFlag);
            Monitor.Log("Added DataObservabilityOfficerInvitation mail for tomorrow.", LogLevel.Debug);
        }

        if (Game1.player.mailReceived.Contains(IntroductionFlag) && !Game1.player.mailReceived.Contains(NextQuestFlag))
        {
            Game1.addMailForTomorrow(NextQuestFlag);
            Monitor.Log("Added DataObservabilityOfficerNextQuest mail for tomorrow.", LogLevel.Debug);
        }
    }
}

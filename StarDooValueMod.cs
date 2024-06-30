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
    private DialogueManager dialogueManager;
    private QuestManager questManager;

    public override void Entry(IModHelper helper)
    {
        questManager = new QuestManager(helper, Monitor);
        dialogueManager = new DialogueManager(helper, Monitor, questManager);

        helper.Events.GameLoop.DayStarted += OnDayStarted;
        helper.Events.Input.ButtonPressed += OnButtonPressed;
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

    private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
    {
        if (!Context.IsWorldReady || Game1.activeClickableMenu != null)
            return;

        NPC npc = Game1.currentLocation.isCharacterAtTile(e.Cursor.Tile);
        if (e.Button.IsActionButton() && npc != null && npc.Name == "Lewis")
        {
            Monitor.Log("Interacting with Lewis.", LogLevel.Debug);

            if (Game1.player.mailReceived.Contains(MailFlag) && !Game1.player.eventsSeen.Contains(IntroductionFlag))
            {
                dialogueManager.StartInitialDialogue(npc);
            }
            else if (Game1.player.mailReceived.Contains(NextQuestFlag))
            {
                dialogueManager.StartNextQuestDialogue(npc);
            }
        }

        if (e.Button.IsActionButton())
        {
            // Interactions with other NPCs for gathering feedback
            if (npc != null && new List<string> { "Robin", "Pierre", "Morris", "Marnie", "Gus" }.Contains(npc.Name))
            {
                dialogueManager.GatherFeedback(npc);
            }

            // Interactions with engineers and analysts
            if (npc != null && new List<string> { "Clint", "Maru", "Emily", "Leah" }.Contains(npc.Name))
            {
                dialogueManager.EngineerAnalystDialogue(npc);
            }
        }
    }
}

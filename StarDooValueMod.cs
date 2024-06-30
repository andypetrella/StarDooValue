using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System.Collections.Generic;

public class StarDooValueMod : Mod
{
    private const string IntroductionFlag = "DataObservabilityOfficerIntroduction";
    private const string MailFlag = "DataObservabilityOfficerInvitation";
    private Dictionary<string, string> mailContents;

    public override void Entry(IModHelper helper)
    {
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
    }

    private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
    {
        if (!Context.IsWorldReady || Game1.activeClickableMenu != null)
            return;

        // Check if the player is interacting with Lewis
        NPC npc = Game1.currentLocation.isCharacterAtTile(e.Cursor.Tile);
        if (e.Button.IsActionButton() && npc != null)
        {
            if (npc != null && npc.Name == "Lewis")
            {
                Monitor.Log("Interacting with Lewis.", LogLevel.Debug);

                // Trigger dialogue if the player has received the mail
                if (Game1.player.mailReceived.Contains(MailFlag) && !Game1.player.eventsSeen.Contains(IntroductionFlag))
                {
                    string dialogueText = Helper.Translation.Get("DataObservabilityOfficerIntroduction");
                    Monitor.Log($"Fetched translation: {dialogueText}", LogLevel.Debug);

                    if (!string.IsNullOrEmpty(dialogueText))
                    {
                        npc.CurrentDialogue.Push(new Dialogue(npc, IntroductionFlag, dialogueText));
                        Game1.drawDialogue(npc);
                        Game1.player.eventsSeen.Add(IntroductionFlag);
                    }
                    else
                    {
                        Monitor.Log("No translation found for key: DataObservabilityOfficerIntroduction", LogLevel.Error);
                    }
                }
            }
        }
    }
}
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System.Collections.Generic;
using StardewValley.Locations;
using StardewValley.Tools;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;

public class StarDooValueMod : Mod
{
    private const string IntroductionFlag = "DataObservabilityOfficerIntroduction";
    private const string MailFlag = "DataObservabilityOfficerInvitation";
    private const string NextQuestFlag = "DataObservabilityOfficerNextQuest";
    private Dictionary<string, string> mailContents;
    private QuestManager questManager;

    public static StarDooValueMod Instance;
    private int collectedFiles = 0;
    private int targetFiles = 10;

    private int placedComputers = 0;


    public override void Entry(IModHelper helper)
    {
        questManager = new QuestManager(helper, Monitor);

        helper.Events.GameLoop.DayStarted += OnDayStarted;
        helper.Events.Input.ButtonPressed += questManager.OnButtonPressed;
        helper.Events.Content.AssetRequested += OnAssetRequested;
        helper.Events.Content.AssetReady += OnAssetReady;
        helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
        helper.Events.Display.RenderedHud += OnRenderedHud;
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
        if (Context.IsWorldReady)
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


    private void OnUpdateTicked(object? sender, EventArgs e)
    {
       
         if (Context.IsWorldReady && Game1.player?.currentLocation is MineShaft mine)
            {
                // Placer des ordinateurs cassés dynamiquement
                PlaceBrokenComputers(mine);
            }
    }


        private void PlaceBrokenComputers(MineShaft mine)
        {
            Random random = new Random();
            while (placedComputers < 5) {
                Vector2 position = new Vector2(random.Next(mine.Map.DisplayWidth / 64), random.Next(mine.Map.DisplayHeight / 64));
                if (mine.isTileClearForMineObjects(position) && mine.isTileOnClearAndSolidGround(position))
                {
                    StardewValley.Object computer = ItemRegistry.Create<StardewValley.Object>("(O)9001", 1);
                    computer.CanBeSetDown = true;
                    computer.CanBeGrabbed = false;
                    mine.objects[position] = computer;

                    int mineLevel = mine.mineLevel;
                    Monitor.Log($"BrokenComputer placed at level {mineLevel}, position ({position.X}, {position.Y})", LogLevel.Info);
                    placedComputers++;
                } else {
                    Monitor.Log($"Cannot place computer on {position}, retrying", LogLevel.Info);
                }
            }
        }

    private void OnRenderedHud(object? sender, RenderedHudEventArgs e)
    {
        // Afficher le compteur de fichiers collectés
        if (collectedFiles < targetFiles)
        {
            string text = $"Files collected: {collectedFiles}/{targetFiles}";
            Vector2 position = new Vector2(Game1.viewport.Width - 500, Game1.viewport.Height - 50);
            Game1.spriteBatch.DrawString(Game1.dialogueFont, text, position, Color.White);
        }
    }
}

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using Microsoft.Xna.Framework;
using System;

public class StarDooValueMod : Mod
{
    private const string IntroductionFlag = "DataObservabilityOfficerIntroduction";
    private const string MailFlag = "DataObservabilityOfficerInvitation";
    private const string NextQuestFlag = "DataObservabilityOfficerNextQuest";
    private QuestManager questManager;
    private int placedComputers = 0;


    public override void Entry(IModHelper helper)
    {
        questManager = new QuestManager(helper, Monitor);

        helper.Events.GameLoop.DayStarted += OnDayStarted;
        helper.Events.Input.ButtonPressed += questManager.OnButtonPressed;
        helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
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

}

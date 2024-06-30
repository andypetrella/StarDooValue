using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System.Collections.Generic;
using System.Text.Json.Nodes;

public class QuestManager
{
    private readonly IModHelper helper;
    private readonly IMonitor monitor;
    private readonly Quest1 quest1;
    private readonly Quest2 quest2;

    public QuestManager(IModHelper helper, IMonitor monitor)
    {
        this.helper = helper;
        this.monitor = monitor;
        this.quest1 = new Quest1(helper, monitor, this);
        this.quest2 = new Quest2(helper, monitor, this);
    }

    public void OnDayStarted(object? sender, DayStartedEventArgs e)
    {
        quest1.CheckMail();
        quest2.CheckMail();
    }

    public void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
    {
        NPC npc = Game1.currentLocation.isCharacterAtTile(e.Cursor.Tile);
        if (e.Button.IsActionButton() && npc != null)
        {
            if (quest1.IsActive)
            {
                quest1.HandleButtonPress(npc);
            }
            else if (quest2.IsActive)
            {
                quest2.HandleButtonPress(npc);
            }
        }
    }

    public void StartGatheringFeedbackQuest()
    {
        quest1.StartGatheringFeedbackQuest("1001");
    }

    public void StartMaruCaseQuest()
    {
        quest2.StartMaruCaseQuest("1002");
    }
}
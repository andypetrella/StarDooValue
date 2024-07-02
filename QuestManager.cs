using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Quests;
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
        DialogueManager dialogueManager = new DialogueManager(helper, monitor, this);
        this.quest1 = new Quest1(helper, monitor, this, dialogueManager);
        this.quest2 = new Quest2(helper, monitor, this, dialogueManager);
    }

    public void OnDayStarted(object? sender, DayStartedEventArgs e)
    {
        quest1.CheckMail();
        quest2.CheckMail();
    }

    public void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
    {
        if (!Context.IsWorldReady || Game1.activeClickableMenu != null)
            return;

        NPC npc = Game1.currentLocation.isCharacterAtTile(e.Cursor.Tile);
        if (e.Button.IsActionButton() && npc != null)
        {
            if (quest1.IsActive())
            {
                quest1.HandleButtonPress(npc);
            }
            else if (quest2.IsActive)
            {
                quest2.HandleButtonPress(npc);
            }
        }
    }

    public bool IsActive(string questId)
    {
        for (int num = Game1.player.questLog.Count - 1; num >= 0; num--)
        {
            Quest quest = Game1.player.questLog[num];
            if (quest.id.Value == questId)
            {
                return !quest.completed.Get();
            }
        }
        return false;
    }

}
using StardewModdingAPI;
using StardewValley;
using StardewValley.Quests;
using System.Collections.Generic;

public class Quest2
{
    private string questId = "1002";
    private readonly IModHelper helper;
    private readonly IMonitor monitor;
    private readonly QuestManager questManager;
    private readonly DialogueManager dialogueManager;
    public bool IsActive { get; private set; }

    public Quest2(IModHelper helper, IMonitor monitor, QuestManager questManager, DialogueManager dialogueManager)
    {
        this.helper = helper;
        this.monitor = monitor;
        this.questManager = questManager;
        this.dialogueManager = dialogueManager;
        this.IsActive = false;
    }

    public void CheckMail()
    {
        if (Game1.player.mailReceived.Contains("DataObservabilityOfficerIntroduction") &&
            !Game1.player.mailReceived.Contains("DataObservabilityOfficerNextQuest"))
        {
            Game1.addMailForTomorrow("DataObservabilityOfficerNextQuest");
            monitor.Log("Added DataObservabilityOfficerNextQuest mail for tomorrow.", LogLevel.Debug);
        }
    }

    public void HandleButtonPress(NPC npc)
    {
        if (npc.Name == "Lewis")
        {
            StartNextQuestDialogue(npc);
        }
        else if (new List<string> { "Clint", "Maru", "Emily", "Leah" }.Contains(npc.Name))
        {
            EngineerAnalystDialogue(npc);
        }
    }

    public void EngineerAnalystDialogue(NPC npc)
    {
        dialogueManager.DrawDialogueFromKey(npc, $"{npc.Name}Feedback");
    }

    public void StartNextQuestDialogue(NPC npc)
    {
        dialogueManager.GroupChat(new (NPC, string)[][]
        {
            new (NPC, string)[] { (npc, "LewisNextQuest") },
            new (NPC, string)[] { (npc, "PlayerMaruCase") },
            new (NPC, string)[] { (npc, "LewisApprove"), (npc, "PlayerStart") }
        });

        Game1.player.eventsSeen.Add("DataObservabilityOfficerNextQuest");
        StartMaruCaseQuest();
    }

    public void StartMaruCaseQuest()
    {
        Game1.player.addQuest(questId);
        monitor.Log("Started quest: Investigate Maru's Case", LogLevel.Info);
    }
}
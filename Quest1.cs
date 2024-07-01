using StardewModdingAPI;
using StardewValley;
using StardewValley.Quests;
using System.Collections.Generic;

public class Quest1
{
    private string questId = "1001";
    private readonly IModHelper helper;
    private readonly IMonitor monitor;
    private readonly QuestManager questManager;
    private readonly DialogueManager dialogueManager;
    private bool _IsActive;

    public Quest1(IModHelper helper, IMonitor monitor, QuestManager questManager, DialogueManager dialogueManager)
    {
        this.helper = helper;
        this.monitor = monitor;
        this.questManager = questManager;
        this.dialogueManager = dialogueManager;
        this._IsActive = false;
    }

    public void CheckMail()
    {
        if (!Game1.player.mailReceived.Contains("DataObservabilityOfficerInvitation"))
        {
            Game1.addMailForTomorrow("DataObservabilityOfficerInvitation");
            monitor.Log("Added DataObservabilityOfficerInvitation mail for tomorrow.", LogLevel.Debug);
        }
    }

    public bool CanBeStartedWithLewis()
    {
        return Game1.player.mailReceived.Contains("DataObservabilityOfficerInvitation") && !Game1.player.eventsSeen.Contains("DataObservabilityOfficerIntroduction");
    }

    public bool IsActive() 
    {
        return _IsActive || CanBeStartedWithLewis();
    }

    public void HandleButtonPress(NPC npc)
    {

        if (CanBeStartedWithLewis() && npc.Name == "Lewis")
        {
            StartInitialDialogue(npc);
        }
        else if (new List<string> { "Robin", "Pierre", "Marnie", "Gus" }.Contains(npc.Name))
        {
            GatherFeedback(npc);
        }
    }

    public void StartInitialDialogue(NPC npc)
    {
        dialogueManager.SequenceDialogueFromKeys(npc, new string[] { "DataObservabilityOfficerIntroduction1", "DataObservabilityOfficerHelpPrompt", "DataObservabilityOfficerIntroduction2" });

        Game1.afterDialogues += () =>
        {
            StartGatheringFeedbackQuest();
        };
        _IsActive = true;

        Game1.player.eventsSeen.Add("DataObservabilityOfficerIntroduction");
    }

    public void StartGatheringFeedbackQuest()
    {
        Game1.player.addQuest(questId);
        monitor.Log("Started quest: Gathering Feedback from Villagers", LogLevel.Info);
    }

    
    public void GatherFeedback(NPC npc)
    {
        dialogueManager.DrawDialogueFromKey(npc, $"{npc.Name}Feedback");
    }

}
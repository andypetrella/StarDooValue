using StardewModdingAPI;
using StardewValley;
using StardewValley.Quests;
using System.Collections.Generic;

public class Quest1
{
    private readonly IModHelper helper;
    private readonly IMonitor monitor;
    private readonly QuestManager questManager;
    private readonly DialogueManager dialogueManager;
    public bool IsActive { get; private set; }

    public Quest1(IModHelper helper, IMonitor monitor, QuestManager questManager)
    {
        this.helper = helper;
        this.monitor = monitor;
        this.questManager = questManager;
        this.dialogueManager = new DialogueManager(helper, monitor, questManager);
        this.IsActive = false;
    }

    public void CheckMail()
    {
        if (!Game1.player.mailReceived.Contains("DataObservabilityOfficerInvitation"))
        {
            Game1.addMailForTomorrow("DataObservabilityOfficerInvitation");
            monitor.Log("Added DataObservabilityOfficerInvitation mail for tomorrow.", LogLevel.Debug);
        }
    }

    public void HandleButtonPress(NPC npc)
    {
        if (npc.Name == "Lewis")
        {
            StartInitialDialogue(npc);
        }
        else if (new List<string> { "Robin", "Pierre", "Morris", "Marnie", "Gus" }.Contains(npc.Name))
        {
            dialogueManager.GatherFeedback(npc);
        }
    }

    public void StartInitialDialogue(NPC npc)
    {
        dialogueManager.SequenceDialogueFromKeys(npc, new string[] { "DataObservabilityOfficerIntroduction1", "DataObservabilityOfficerHelpPrompt", "DataObservabilityOfficerIntroduction2" });

        Game1.afterDialogues += () =>
        {
            questManager.StartGatheringFeedbackQuest();
        };

        Game1.player.eventsSeen.Add("DataObservabilityOfficerIntroduction");
    }

    public void StartGatheringFeedbackQuest(string quest)
    {
        Game1.player.addQuest(quest);
        monitor.Log("Started quest: Gathering Feedback from Villagers", LogLevel.Info);
    }
}
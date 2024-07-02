using StardewModdingAPI;
using StardewValley;
using StardewValley.Quests;
using System.Collections.Generic;
using System.Linq;

public class Quest1
{
    private string questId = "1001";
    private readonly IModHelper helper;
    private readonly IMonitor monitor;
    private readonly QuestManager questManager;
    private readonly DialogueManager dialogueManager;
    private bool _IsActive = false;
    private bool _IsEnded = false;
    private Dictionary<string, bool> feedbackGathered = new Dictionary<string, bool> (){ 
        {"Robin", false}, {"Pierre", false}, {"Marnie", false}, {"Gus", false}
    };

    public Quest1(IModHelper helper, IMonitor monitor, QuestManager questManager, DialogueManager dialogueManager)
    {
        this.helper = helper;
        this.monitor = monitor;
        this.questManager = questManager;
        this.dialogueManager = dialogueManager;
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
        return !_IsEnded && (_IsActive || CanBeStartedWithLewis());
    }

    public void HandleButtonPress(NPC npc)
    {

        if (npc.Name == "Lewis")
        {
            if (CanBeStartedWithLewis())
            {
                StartInitialDialogue(npc);
            }
            else if (feedbackGathered.Values.All(x => x))
            {
                FeedbackGathered(npc);
            }
        }
        else if (feedbackGathered.ContainsKey(npc.Name))
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
        feedbackGathered[npc.Name] = true;
    }

    public void FeedbackGathered(NPC npc) 
    {
        dialogueManager.DrawDialogueFromKey(npc, "StakeholdersFeedbackCollected");
        Game1.player.completeQuest(questId);
        _IsEnded = true;
        _IsActive = false;
    }

}
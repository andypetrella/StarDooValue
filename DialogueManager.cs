using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Linq;
using System.Text.RegularExpressions;

public class DialogueManager
{
    private readonly IModHelper helper;
    private readonly IMonitor monitor;
    private readonly QuestManager questManager;

    public DialogueManager(IModHelper helper, IMonitor monitor, QuestManager questManager)
    {
        this.helper = helper;
        this.monitor = monitor;
        this.questManager = questManager;
    }


    public Dialogue? PushDialogueFromKey(NPC npc, string key) 
    {
        string dialogueText = helper.Translation.Get(key);
        monitor.Log($"Fetched translation: {dialogueText}", LogLevel.Debug);

        if (!string.IsNullOrEmpty(dialogueText))
        {
            Dialogue dialogue = new Dialogue(npc, key, dialogueText);
            npc.CurrentDialogue.Push(dialogue);
            return dialogue;
        }
        return null;
    }
    public void DrawDialogueFromKey(NPC npc, string key) 
    {
        Dialogue? dialogue = PushDialogueFromKey(npc, key);
        if (dialogue != null)
        {
            Game1.drawDialogue(npc);
        }
    }

    public void SequenceDialogueFromKeys(NPC npc, string[] keys)
    {
        if (keys.Length == 0) {
            return;
        }
        DrawDialogueFromKey(npc, keys[0]);
        Game1.afterDialogues += () => 
        {
            SequenceDialogueFromKeys(npc, keys.Skip(1).ToArray());
        };
    }

    public void GroupChat((NPC, string)[][] discussion)
    {
        if (discussion.Length == 0) {
            return;
        }

        (NPC, string)[] layer = discussion[0];
        foreach ((NPC, string) entry in layer)
        {
            DrawDialogueFromKey(entry.Item1, entry.Item2);        
        }

        Game1.afterDialogues += () => 
        {
            GroupChat(discussion.Skip(1).ToArray());
        };
    }
}

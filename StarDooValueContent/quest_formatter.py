import json



# 0	Type (e.g., Location, Basic, LostItem, etc)
# 1	Title
# 2	Quest details/flavor text
# 3	Hint/condition
# 4	Solution/trigger
# 5	Next quest (-1 if none)
# 6	Gold (0 if none)
# 7	Reward description (Only if Gold is not -1. Apparently unused?)
# 8	Cancellable
# 9	(Optional) reaction text

def main():
    with open("./input/quests.json") as f:
        quests = json.load(f)
        formatted_quests = {}
        for quest in quests:
            id = quest["id"]
            type = quest["type"]
            title = quest["title"]
            details = quest["details"]
            hint = quest["hint"]
            trigger = quest["trigger"]
            next_quest = quest.get("next_quest", "-1")
            gold = quest.get("gold", 0)
            reward = quest.get("reward", "None")
            cancellable = quest.get("cancellable", "true")
            reaction = quest.get("reaction", "")
            if id in formatted_quests:
                raise f"Duplicated id: {id}"
            formatted_quests[id] = f"{type}/{title}/{details}/{hint}/{trigger}/{next_quest}/{gold}/{reward}/{cancellable}/{reaction}"
        print(json.dumps(formatted_quests, indent=4))

if __name__ == "__main__":
    main()
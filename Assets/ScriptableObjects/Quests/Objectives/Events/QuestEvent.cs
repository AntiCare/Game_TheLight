using System;

namespace Quests.Objectives.Events
{
    public static class QuestEvent
    {
        public static event Action<Quest,Character> AddQuest;
        
        public static void OnAddQuest(Quest obj, Character giver)
        {
            AddQuest?.Invoke(obj, giver);
        }

        public static void OnAddQuest(Quest obj)
        {
            AddQuest?.Invoke(obj,null);
        }
    }
}
using System;
using UnityEngine;

namespace Quests.Objectives
{
    [CreateAssetMenu(fileName = "Objective Talk", menuName = "ScriptableObjects/Objective/Objective Talk")]
    [Serializable]
    public class ObjectiveTalk : Objective
    {
        [SerializeField] private Character characterToTalkTo;
        [SerializeField] private Quest     belongsToQuest;

        public override void StartObjective(int ? param)
        {
            DialogEvent.TalkingTo += TalkingTo;
            _started              =  true;
            _isCompleted          =  false;
        }

        private void TalkingTo(Character c)
        {
            _isCompleted = c.CharacterId == characterToTalkTo.CharacterId;
        }

        public override Objective ObjectiveUpdate()
        {
            if (!_started)
                StartObjective(0);

            return this;
        }

        public void CompleteObjective(Character character)
        {
            if (!GameManager.Instance.GameData.activeQuests.Contains(
                GameManager.Instance.GameData.activeQuests.Find(q => q.QuestId == belongsToQuest.QuestId))) return;

            if (character.CharacterId != characterToTalkTo.CharacterId) return;

            _isCompleted = true;

            QuestEvent?.Invoke(belongsToQuest);
        }
    }
}
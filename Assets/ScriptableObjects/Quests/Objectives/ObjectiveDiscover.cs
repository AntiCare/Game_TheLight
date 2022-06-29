using System;
using UnityEngine;

namespace Quests.Objectives
{
    [CreateAssetMenu(fileName = "Objective Discover", menuName = "ScriptableObjects/Objective/Objective Discover")]
    [Serializable]
    public class ObjectiveDiscover : Objective
    {
        [SerializeField] private Vector2    destination;
        [SerializeField] private Vector2    offset = new Vector2(5, 5);
        [SerializeField] private Location   locationToDiscover;
        [SerializeField] private int        locationId;
        [SerializeField] private Quest      belongsToQuest;
        [SerializeField] private GameObject _player;
        private                  Vector2    _current;

        public override void StartObjective(int ? param)
        {
            _isCompleted = false;
            _started     = true;
        }

        public override Objective ObjectiveUpdate()
        {
            if (!_started)
                StartObjective(0);

            return this;
        }

        public void CompleteObjective(Location location)
        {
            if (!GameManager.Instance.GameData.activeQuests.Contains(belongsToQuest)) return;
            
            if (location == null) return;

            if (location.locationId != locationId) return;

            _isCompleted = true;
            QuestEvent?.Invoke(belongsToQuest);
        }
    }
}
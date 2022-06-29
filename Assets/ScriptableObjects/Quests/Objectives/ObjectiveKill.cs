using System;
using System.Diagnostics.Tracing;
using Enums;
using UnityEngine;

namespace Quests.Objectives
{
    [CreateAssetMenu(fileName = "Objective Kill", menuName = "ScriptableObjects/Objective/Objective Kill")]
    [Serializable]
    public class ObjectiveKill : Objective
    {
        private                  string    _originalText;
        private                  int       _start;
        private                  string    _typeString;
        [SerializeField] private int       amount;
        [SerializeField] private EnemyType type;
        [SerializeField] private Quest     belongsToQuest;

        public override void StartObjective(int ? param)
        {
            _started      = true;
            _isCompleted  = false;
            _counter      = param ?? 0;
            _originalText = $"{text} {_counter}/{amount}";

            switch (type)
            {
                case EnemyType.ROCKY:
                    _start = EnemyCounter.slimeCounter - _counter;
                    _typeString = $"{type.ToString().ToLower()} (Melee Enemy)";
                    break;
                case EnemyType.ROCKER:
                    _start = EnemyCounter.rockerCounter - _counter;
                    _typeString = $"{type.ToString().ToLower()} (Range Enemy)";
                    break;
                case EnemyType.TERRANIS:
                    _start = EnemyCounter.bossCounter - _counter;
                    _typeString = $"{type.ToString().ToLower()} (Boss)";
                    break;
            }
            UpdateText();
        }

        void UpdateText()
        {
            text = $"kill {_typeString}:  {_counter}/{amount}";
        }

        private void CounterUpdate()
        {
            switch (type)
            {
                case EnemyType.ROCKY:
                    _counter = EnemyCounter.slimeCounter - _start;
                    break;
                case EnemyType.ROCKER:
                    _counter = EnemyCounter.rockerCounter - _start;
                    break;
                case EnemyType.TERRANIS:
                    _counter = EnemyCounter.bossCounter - _start;
                    break;
            }
        }

        public override Objective ObjectiveUpdate()
        {
            if (!_started)
                StartObjective(null);

            CounterUpdate();
            UpdateText();

            _isCompleted = _counter >= amount;

            QuestEvent?.Invoke(belongsToQuest);

            if (_isCompleted)
            {
                QuestEvent.RemoveListener(QuestManager.Instance.QuestsUpdate);
            }

            return this;
        }

        public void UpdateKillQuest(EnemyType enemyType)
        {
            if (enemyType != type) return;

            if (!GameManager.Instance.GameData.activeQuests.Contains(belongsToQuest)) return;

            ObjectiveUpdate();
        }
    }
}
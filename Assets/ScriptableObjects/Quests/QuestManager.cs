using System.Collections.Generic;
using JetBrains.Annotations;
using Quests.Objectives;
using Quests.Objectives.Events;
using UnityEngine;

namespace Quests
{
    public class QuestManager : Singleton<QuestManager>
    {
        private List<QuestData> _questData;
        private List<Quest>     _quest;
        private HeadsUpDisplay  _hud;
        private List<Quest>     currentQuests = new List<Quest>();

        private NotificationManager _nm;

        void Init()
        {
            _hud = UIManager.Instance.HeadsUpDisplay;
            _nm  = UIManager.Instance.NotificationManager;

            QuestEvent.AddQuest += AddQuest;
        }

        public QuestManager Load(GameData gameData)
        {
            Init();
            _questData = new List<QuestData>(gameData.quests);
            _quest     = new List<Quest>(gameData.activeQuests);

            foreach (var quest in _quest)
            {
                StartQuest(quest, _questData.Find(q => q.id == quest.QuestId));
            }

            return this;
        }


        void StartQuest(Quest quest, QuestData questData)
        {
            quest.StartQuest(questData.objectiveIndex, questData.enemiesSlain);

            if (questData.active)
            {
                quest.SetIsActive(true);
            }

            var currentObjective = quest.GetCurrentObjective();

            currentObjective.QuestEvent.AddListener(QuestsUpdate);

            if (currentObjective.GetType() == typeof(ObjectiveDiscover))
            {
                var obj = (ObjectiveDiscover) currentObjective;
                Debug.Log($"subscribe to discover event : {currentObjective} {Time.time}");
                DiscoverEvent.OnDiscover -= obj.CompleteObjective;
                DiscoverEvent.OnDiscover += obj.CompleteObjective;
                return;
            }

            if (currentObjective.GetType() == typeof(ObjectiveTalk))
            {
                var obj = (ObjectiveTalk) currentObjective;
                Debug.Log($"subscribe to talk event: {currentObjective} {Time.time}");

                DialogEvent.TalkingTo -= obj.CompleteObjective;
                DialogEvent.TalkingTo += obj.CompleteObjective;
                return;
            }

            if (currentObjective.GetType() == typeof(ObjectiveKill))
            {
                var obj = (ObjectiveKill) currentObjective;
                Debug.Log($"subscribe to talk event: {currentObjective} {Time.time}");
                KillEnemyEvent.OnKill -= obj.UpdateKillQuest;
                KillEnemyEvent.OnKill += obj.UpdateKillQuest;
                return;
            }

            if (currentObjective.GetType() == typeof(ObjectiveInteract))
            {
                var obj = (ObjectiveInteract) currentObjective;
                Debug.Log($"subscribe to talk event: {currentObjective} {Time.time}");
                EntityEvent.OnInteraction -= obj.CompleteObjective;
                EntityEvent.OnInteraction += obj.CompleteObjective;
            }
        }

        // Add quests //
        public void AddQuest(Quest quest, Character giver)
        {
            quest.StartQuest(0, 0);

            var q = new QuestData
            {
                id             = quest.QuestId,
                name           = quest.QuestName,
                objectiveIndex = quest.IndexObjective,
                enemiesSlain   = 0,
                active         = quest.IsActive,
            };

            if
            (
                !_questData.Contains(q)                                     &&
                !GameManager.Instance.GameData.activeQuests.Contains(quest) &&
                !GameManager.Instance.GameData.quests.Contains(q)           &&
                !GameManager.Instance.GameData.completedQuests.Contains(quest)
            )
            {
                _questData.Add(q);
            }

            if
            (
                GameManager.Instance.GameData.activeQuests.Contains(quest) ||
                GameManager.Instance.GameData.completedQuests.Contains(quest)
            )
                return;

            GameManager.Instance.GameData.quests.Add(q);
            GameManager.Instance.GameData.activeQuests.Add(quest);

            QuestNotification(quest, null); // notification when a quest gets added
            StartQuest(quest, q);
            ActivateQuest(quest);
            DisplayDetail(quest);
           
            _hud.BindDataToQuest(quest); // for the frontend
        }

        public void QuestsUpdate(Quest quest)
        {
            Objective objective = quest.GetCurrentObjective();

            if (objective.GetType() == typeof(ObjectiveKill))
            {
                var obj = (ObjectiveKill) objective;
                Debug.Log($"subscribe to talk event: {objective} {Time.time}");
                KillEnemyEvent.OnKill -= obj.UpdateKillQuest;
            }

            objective.QuestEvent.RemoveListener(QuestsUpdate);

            QuestNotification(quest, objective);

            quest.Update();

            if
            (
                GameManager
                   .Instance
                   .GameData
                   .quests
                   .Remove
                    (
                        GameManager.Instance.GameData.quests.Find(q => q.id == quest.QuestId)
                    )
                &&
                quest.GetCurrentObjective() != null
            )
            {
                var obj = quest.GetCurrentObjective();

                var newQuest = new QuestData
                {
                    id             = quest.QuestId,
                    name           = quest.QuestName,
                    objectiveIndex = quest.IndexObjective,
                    enemiesSlain   = obj.Counter,
                    active         = quest.IsActive,
                };

                GameManager.Instance.GameData.quests.Add(newQuest);
            }

            if (quest.IsCompleted)
            {
                CompleteQuest(quest);
                return;
            }

            if (quest.GetCurrentObjective() != null)
            {
                var currentObjective = quest.GetCurrentObjective();

                currentObjective.ObjectiveUpdate();

                DisplayObjective(quest);
                DisplayDetail(quest);

                if (currentObjective.GetType() == typeof(ObjectiveDiscover))
                {
                    var obj = (ObjectiveDiscover) currentObjective;
                    obj.QuestEvent.AddListener(QuestsUpdate);

                    Debug.Log($"subscribe to discover event : {currentObjective} {Time.time}");
                    DiscoverEvent.OnDiscover += obj.CompleteObjective;

                    return;
                }

                if (currentObjective.GetType() == typeof(ObjectiveTalk))
                {
                    var obj = (ObjectiveTalk) currentObjective;
                    Debug.Log($"subscribe to talk event: {currentObjective} {Time.time}");
                    obj.QuestEvent.AddListener(QuestsUpdate);

                    DialogEvent.TalkingTo += obj.CompleteObjective;

                    return;
                }

                if (currentObjective.GetType() == typeof(ObjectiveKill))
                {
                    var obj = (ObjectiveKill) currentObjective;
                    obj.QuestEvent.AddListener(QuestsUpdate);

                    Debug.Log($"subscribe to talk event: {currentObjective} {Time.time}");
                    KillEnemyEvent.OnKill += obj.UpdateKillQuest;
                    return;
                }

                if (currentObjective.GetType() == typeof(ObjectiveInteract))
                {
                    var obj = (ObjectiveInteract) currentObjective;
                    Debug.Log($"subscribe to talk event: {currentObjective} {Time.time}");
                    EntityEvent.OnInteraction += obj.CompleteObjective;
                }
            }
        }

        // Display logic methods //
        private void CompleteQuest(Quest quest)
        {
            if (GameManager.Instance.GameData.activeQuests.Contains(quest))
            {
                GameManager.Instance.GameData.activeQuests.Remove(quest);
            }

            if (!GameManager.Instance.GameData.completedQuests.Contains(quest))
            {
                GameManager.Instance.GameData.completedQuests.Add(quest);
            }

            GameManager.Instance.GameData.quests.Remove
            (
                GameManager.Instance.GameData.quests.Find(q => q.id == quest.QuestId)
            );

            ActivateQuest(quest);

            QuestNotification(quest, null);

            HandleActiveButton(quest, false);

            GiveReward(quest.Reward);

            _hud.QuestCompleted(quest);
        }

        private void GiveReward(int coins)
        {
            Player player = GameManager.Instance.Player;
            player.PlayerWallet += coins;
            Loot l = new Loot();
            l.Item = GameManager.Instance.GetItemById(0);
            l.SetReward(true);
            player.AddToInventory(l);
        }

        public bool ActivateQuest(Quest quest)
        {
            if (quest.IsCompleted)
            {
                if (currentQuests.Contains(quest))
                {
                    currentQuests.Remove(quest);
                    _hud.RemoveFromObjectiveScrollView(quest);
                }

                quest.SetIsActive(false);
                HandleActiveButton(quest, false);
                return false;
            }

            if (quest.IsActive)
            {
                quest.SetIsActive(false);
                HandleActiveButton(quest, false);
                SelectQuest(null);

                if (currentQuests.Contains(quest))
                {
                    currentQuests.Remove(quest);
                    _hud.RemoveFromObjectiveScrollView(quest);
                }

                return false;
            }

            quest.SetIsActive(true);

            if (!currentQuests.Contains(quest))
            {
                currentQuests.Add(quest);
                HandleActiveButton(quest, true);
            }

            SelectQuest(quest);

            return true;
        }

        public void SelectQuest(Quest quest)
        {
            if (quest == null)
            {
                DisplayDetail(null);
                return;
            }

            // if (quest.IsCompleted) return;

            DisplayDetail(quest);
        }

        // Interaction with HeadsUpDisplay.cs //
        private void DisplayDetail(Quest quest)
        {
            if (quest == null)
            {
                _hud.DisplayDetailPage(null);
                return;
            }

            _hud.DisplayDetailPage(quest);
        }

        private void HandleActiveButton(Quest qs, bool active)
        {
            _hud.BindObjectiveToScrollView(qs);
        }

        private void DisplayObjective(Quest questToUpdate)
        {
            _hud.BindObjectiveToScrollView(questToUpdate);
        }

        private void QuestNotification([CanBeNull] Quest quest, [CanBeNull] Objective objective)
        {
            _nm.ShowQuestNotification(quest, objective);
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Quests;
using Quests.Objectives;
using UnityEngine;

[CreateAssetMenu(fileName = "Quest", menuName = "ScriptableObjects/Quest")]
[Serializable]
public class Quest : ScriptableObject
{
    [SerializeField] private int             questId;
    [SerializeField] private string          questName;
    [SerializeField] private string          questDescription;
    [SerializeField] private int             reward;        // amount of coins
    [SerializeField] private bool            isMainQuest;
    [SerializeField] private List<Objective> objectives;
    [SerializeField] private Quest           nextQuest;
    [SerializeField] private Character       _giver;

    [SerializeField] private bool _isActive       = false;
    private                  bool _isCompleted    = false;
    private                  int  _indexObjective = 0;

    public List<Objective> Objectives
    {
        get => objectives;
        set => objectives = value;
    }

    public int QuestId
    {
        get => questId;
        set => questId = value;
    }

    public int Reward
    {
        get => reward;
        set => reward = value;
    }

    public string QuestName
    {
        get => questName;
        set => questName = value;
    }

    public string QuestDescription
    {
        get => questDescription;
        set => questDescription = value;
    }

    public bool IsActive
    {
        get => _isActive;
        set => _isActive = value;
    }

    public bool IsCompleted
    {
        get => _isCompleted;
        set => _isCompleted = value;
    }

    public bool IsMainQuest
    {
        get => isMainQuest;
        set => isMainQuest = value;
    }

    public int IndexObjective
    {
        get => _indexObjective;
        set => _indexObjective = value;
    }

    public Character Giver
    {
        set => _giver = value;
        get => _giver;
    }

    public void StartQuest(int ? objectiveIndex, int ? enemiesSlain)
    {
        foreach (var objective in objectives)
        {
            objective.StartObjective(enemiesSlain);
        }

        _isActive       = false;
        _isCompleted    = false;
        _indexObjective = objectiveIndex ?? 0;
    }

    public Objective GetCurrentObjective()
    {
        // if (_isCompleted)
        //     return null;

        return objectives[_indexObjective];
    }

    private Objective NextObjective()
    {
        _indexObjective += 1;
        if (_indexObjective >= objectives.Count)
            _isCompleted = true;

        return GetCurrentObjective();
    }

    public void SetIsActive(bool activity)
    {
        _isActive = activity;
    }

    public Quest Update()
    {
        var o = objectives[_indexObjective];

        Debug.Log($"Completed a objective: {o}");

        if (!o.IsCompleted)
        {
            return this;
        }


        if (_indexObjective + 1 != Objectives.Count)
        {
            Debug.Log($"next obj pls");
            NextObjective();

            return this;
        }

        _isCompleted = true;
        // if (_isCompleted && nextQuest != null)
            // QuestManager.Instance.AddQuest();
        return this;
    }
}
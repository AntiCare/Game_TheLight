using System;
using UnityEngine;

[Serializable]
public abstract class Objective : ScriptableObject
{
    [SerializeField] protected string text;

    public    bool              _isCompleted = false;
    public    bool              _started     = false;
    protected int               _counter;
    public    Events.QuestEvent QuestEvent;

    public bool Started
    {
        get => _started;
        set => _started = value;
    }

    public bool IsCompleted
    {
        get => _isCompleted;
        set => _isCompleted = value;
    }

    public int Counter => _counter;

    public string Text => text;

    public abstract void StartObjective(int ? param);

    public abstract Objective ObjectiveUpdate();
}
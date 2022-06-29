using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class GameData : SaveData
{
    public float   currentHealth = 10f;
    public float   totalHealth   = 10f;
    public float   currentLevel  = 0f;
    public int     LevelNum      = 0;
    public int     wallet        = 0;
    public Vector3 position      = new Vector3(1620f, -2190, 0);

    public int slimeCounter;
    public int rockerCounter;
    public int bossCounter;

    public Inventory          inventory           = new Inventory();
    public List<Item>         items               = new List<Item>();
    public List<DiaryPage>    pages               = new List<DiaryPage>();
    public List<Note>         notes               = new List<Note>();
    public List<Character>    characters          = new List<Character>();
    public List<Quest>        completedQuests     = new List<Quest>();
    public List<Quest>        activeQuests        = new List<Quest>();
    public List<QuestData>    quests              = new List<QuestData>();
    public List<Enemy>        enemies             = new List<Enemy>();
    public List<Interactable> Interactable        = new List<Interactable>();
    public List<Location>     discoveredLocations = new List<Location>();

    public string SaveData()
    {
        Debug.Log(JsonUtility.ToJson(this));
        return JsonUtility.ToJson(this);
    }

    public GameData LoadData(string savedData)
    {
        JsonUtility.FromJsonOverwrite(savedData, this);
        return this;
    }
}

[Serializable]
public struct Enemy
{
    public int health;
    public int id;
}

[Serializable]
public struct Interactable
{
    public int id;
}

[Serializable]
public struct QuestData
{
    public int    id;
    public string name;
    public int    objectiveIndex;
    public int    enemiesSlain;
    public bool   active;
}
using System;
using System.Collections.Generic;
using System.Linq;
using Quests;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class Inventory
{
    public List<Item> _itemIds;

    public List<Note> _noteIds;

    public List<Character> _characterInteractionIds;

    public List<DiaryPage> _pages;

    public List<Quest> _activeQuests;
    public List<Quest> _completeQuests;

    public List<Enemy> enemies;

    public List<Enemy>     Enemies        => enemies;
    public List<Quest>     ActiveQuests   => _activeQuests;
    public List<Quest>     CompleteQuests => _completeQuests;
    public List<Item>      Items          => _itemIds;
    public List<Note>      Notes          => _noteIds;
    public List<Character> Characters     => _characterInteractionIds;
    public List<DiaryPage> Pages          => _pages;

    public Inventory(
        List<Item>      itemIds,
        List<Character> characterInteractionIds,
        List<Note>      noteIds,
        List<DiaryPage> pages,
        List<Quest>     completeQuests,
        List<Quest>     activeQuests
    )
    {
        _itemIds                 = itemIds;
        _characterInteractionIds = characterInteractionIds;
        _noteIds                 = noteIds;
        _pages                   = pages;
        _activeQuests            = activeQuests;
        _completeQuests          = completeQuests;
    }

    public string SaveData()
    {
        return JsonUtility.ToJson(this);
    }

    public void LoadData(string savedData)
    {
        JsonUtility.FromJsonOverwrite(savedData, this);
    }

    public Inventory()
    {
        _activeQuests            = new List<Quest>();
        _completeQuests          = new List<Quest>();
        _itemIds                 = new List<Item>();
        _noteIds                 = new List<Note>();
        _characterInteractionIds = new List<Character>();
        _pages                   = new List<DiaryPage>();
    }

    public bool AddItemToInventory(Loot item)
    {
        if (item.GetItem.QuestItem && _itemIds.Contains(item.GetItem)) return false;
        
        _itemIds.Add(item.GetItem);

        return item.GetItem;
    }

    //must change note to a non Unity object. Note is a ScriptableObject not accepted by the Serializer
    public Note AddNoteId(Note note)
    {
        if (_noteIds.Contains(note))
        {
            return null;
        }

        _noteIds.Add(note);

        return note;
    }

    public Character AddCharacterId(Character character)
    {
        if (_characterInteractionIds.Contains(character))
        {
            return null;
        }

        _characterInteractionIds.Add(character);

        return character;
    }

    public DiaryPage AddPagId(Page page)
    {
        if (_pages.Contains(page.GetPage))
        {
            return null;
        }

        _pages.Add(page.GetPage);

        return page.GetPage;
    }
    //
    // public Quest AddQuest(Quest quest)
    // {
    //     if (_activeQuests.FirstOrDefault(q => q.QuestId == quest.QuestId) != null)
    //     {
    //         return null;
    //     }
    //
    //     quest.StartQuest();
    //     _activeQuests.Add(quest);
    //
    //     return quest;
    // }

    public bool AddToSlainEnemies(Enemy enemy)
    {
        if (enemies.Contains(enemy))
        {
            return false;
        }

        enemies.Add(enemy);

        return true;
    }

    public bool RemoveItemId(Item item)
    {
        return _itemIds.Remove(item);
    }
}
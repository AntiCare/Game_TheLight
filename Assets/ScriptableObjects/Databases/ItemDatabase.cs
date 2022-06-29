using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Daddybase", menuName = "ScriptableObjects/Database", order = 1)]
[Serializable]
public class ItemDatabase : ScriptableObject
{
    [Header("Items")] [SerializeField] 
    private List<Item> items;

    [Header("Notes")] [SerializeField] 
    private List<Note> notes;

    [Header("Diary Pages")] [SerializeField]
    private List<DiaryPage> pages;

    [Header("Characters")] [SerializeField]
    private List<Character> characters;

    [Header("Quests")] [SerializeField] 
    private List<Quest> quests;

    public List<Quest> Quests => quests;
    public List<Item> Items => items;
    public List<Note> Notes => notes;
    public List<DiaryPage> Pages => pages;
    public List<Character> Characters => characters;
}
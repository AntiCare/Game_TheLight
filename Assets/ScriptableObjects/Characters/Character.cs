using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Character", menuName = "ScriptableObjects/Character")]
[Serializable]
public class Character : ScriptableObject
{
    [SerializeField] private int characterId;
    [SerializeField] private string characterName;
    [SerializeField] private Sprite characterSprite;
    [SerializeField] private Sprite characterNoteSprite;
    [SerializeField] private GameObject characterObject;
    [SerializeField] private List<Note> notes;
    [SerializeField] private List<CharacterDialog> dialog;

    public int CharacterId => characterId;

    public string CharacterName => characterName;

    public Sprite CharacterSprite => characterSprite;
    public Sprite CharacterNoteSprite => characterNoteSprite;

    public GameObject CharacterObject => characterObject;

    public List<Note> Notes => notes;

    public List<CharacterDialog> Dialog => dialog;
}

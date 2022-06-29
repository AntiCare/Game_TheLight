using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Note", menuName = "ScriptableObjects/Note")]
[Serializable]
public class Note : ScriptableObject
{
    [SerializeField] private int noteId;
    [SerializeField] private string noteContents;
    [SerializeField] private Sprite notePageImage;
    [SerializeField] private Character characterBelongsTo;
    [SerializeField] private GameObject notePage;

    public int NoteId => noteId;

    public string NoteContents => noteContents;

    public Sprite NotePageImage => notePageImage;

    public Character CharacterBelongsTo => characterBelongsTo;

    public GameObject NotePage => notePage;
}

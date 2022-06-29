using System;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterDialog", menuName = "ScriptableObjects/CharacterDialog")]
[Serializable]
public class CharacterDialog : ScriptableObject
{
    [SerializeField] private int dialogId;
    [SerializeField] private Character characterBelongsTo;
    [SerializeField] private string dialogText;
    
    [SerializeField] private string positiveResponse;
    [SerializeField] private string negativeResponse;
    [SerializeField] private string inquisitiveResponse;

    [SerializeField] private CharacterDialog positiveDialogResponse;
    [SerializeField] private CharacterDialog NegativeDialogResponse;
    [SerializeField] private CharacterDialog inquisitiveDialogResponse;

    [SerializeField] private bool endOfDialog;

    [SerializeField] private Quest quest;

    [SerializeField] private bool hasSpecialInteraction;
    
    public Quest Quest => quest;

    public int DialogId => dialogId;

    public Character CharacterBelongsTo => characterBelongsTo;

    public string DialogText => dialogText;

    public string PositiveResponse => positiveResponse;

    public string NegativeResponse => negativeResponse;

    public string InquisitiveResponse => inquisitiveResponse;

    public CharacterDialog PositiveDialogResponse => positiveDialogResponse;

    public CharacterDialog InquisitiveDialogResponse => inquisitiveDialogResponse;

    public CharacterDialog NegativeDialogResponse1 => NegativeDialogResponse;

    public bool EndOfDialog => endOfDialog;

    public bool HasSpecialInteraction => hasSpecialInteraction;
}
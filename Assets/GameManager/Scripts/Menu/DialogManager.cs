using System;
using Quests.Objectives.Events;
using UnityEngine;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    public Button closeDialog;
    public Button positiveResponse;
    public Button inquisitiveResponse;
    public Text   characterName;

    public Events.CharacterEvent TalkToEvent;

    public int currentDialogNUM => _currentDialog.DialogId;

    // public Button negativeResponse;
    public Image characterSprite;
    public Text  dialogText;

    private CharacterDialog _currentDialog;

    void Start()
    {
        closeDialog.onClick.AddListener(DisableDialogBox);
        positiveResponse.onClick.AddListener(HandlePositiveResponse);
        inquisitiveResponse.onClick.AddListener(HandleInquisitiveResponse);
    }

    public void ShowDialog(CharacterDialog characterDialog)
    {
        positiveResponse.gameObject.SetActive(false);
        inquisitiveResponse.gameObject.SetActive(false);

        _currentDialog = characterDialog;

        // DialogEvent.OnTalkingTo(_currentDialog.CharacterBelongsTo); // This is for the Objective Dialog

        dialogText.text    = _currentDialog.DialogText;
        
        characterName.text = characterDialog.CharacterBelongsTo.CharacterName;

        if (_currentDialog.PositiveResponse != string.Empty)
        {
            positiveResponse.GetComponentInChildren<Text>().text = _currentDialog.PositiveResponse;
            positiveResponse.gameObject.SetActive(true);
        }

        if (_currentDialog.InquisitiveDialogResponse != null)
        {
            inquisitiveResponse.GetComponentInChildren<Text>().text = _currentDialog.InquisitiveResponse;
            inquisitiveResponse.gameObject.SetActive(true);
        }

        if (_currentDialog.Quest != null)
        {
            QuestEvent.OnAddQuest(_currentDialog.Quest, _currentDialog.CharacterBelongsTo);
        }

        if (_currentDialog.HasSpecialInteraction)
        {
            DialogEvent.OnTalkingTo(_currentDialog.CharacterBelongsTo);
        }
        
        characterSprite.sprite = characterDialog.CharacterBelongsTo.CharacterSprite;
    }

    private void DisableDialogBox() => gameObject.SetActive(false);

    public void ToggleDialogBox(bool enable) => gameObject.SetActive(enable);

    void HandlePositiveResponse()
    {
        if (_currentDialog.PositiveResponse == String.Empty)
        {
            return;
        }

        if (_currentDialog.HasSpecialInteraction && GameManager.Instance.Player.CurrentEntity != null)
        {
            // _currentDialog.CharacterBelongsTo.CharacterObject.GetComponent<Entity>().SpecialInteraction(true);
            // DialogEvent.OnTalkingTo(_currentDialog.CharacterBelongsTo);

            try
            {
                GameManager.Instance.Player.CurrentEntity.SpecialInteraction(true);
            }
            catch (Exception e)
            {
                Debug.Log($"No current entity: {e.Message}");
                throw;
            }
        }

        if (_currentDialog.EndOfDialog)
        {
            ToggleDialogBox(false);
            GameManager.Instance.Player.AddCharacterInteraction(_currentDialog.CharacterBelongsTo.CharacterObject
                                                                              .GetComponent<Entity>());
        }

        if (_currentDialog.PositiveDialogResponse != null)
        {
            _currentDialog = _currentDialog.PositiveDialogResponse;

            ShowDialog(_currentDialog);
        }
    }


    void HandleInquisitiveResponse()
    {
        if (_currentDialog.HasSpecialInteraction)
        {
            _currentDialog.CharacterBelongsTo.CharacterObject.GetComponent<Entity>().SpecialInteraction(false);
        }

        if (_currentDialog.EndOfDialog)
        {
            ToggleDialogBox(false);
            GameManager.Instance.Player.AddCharacterInteraction(_currentDialog.CharacterBelongsTo.CharacterObject
                                                                              .GetComponent<Entity>());
        }

        if (_currentDialog.InquisitiveResponse != null)
        {
            _currentDialog = _currentDialog.InquisitiveDialogResponse;
            ShowDialog(_currentDialog);
        }
    }
}
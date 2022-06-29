using System;
using UnityEngine;

public class Village_tilo : Entity
{
    #region Variables and properties

    [SerializeField] private bool       canInteract;
    [SerializeField] private Character  character;
    [SerializeField] private GameObject interactPrefab;
    [SerializeField] private int        indexDialogColorBack;
    [SerializeField] private int        indexDialogWithoutQuest;
    [SerializeField] private Quest      prerequisiteQuest;

    private bool isInteracting = false;
    public  bool CanInteract => canInteract;

    public Character Character => character;

    #endregion

    private void Start()
    {
        hasInteracted = false;

        if (GameManager.Instance.GameData.completedQuests.Contains(prerequisiteQuest))
        {
            GameManager.inventoryLock    = false;
            HeadsUpDisplay.inventoryLock = false;
        }

        foreach (Interactable interactableData in GameManager.Instance.GameData.Interactable)
        {
            if (interactableData.id == interactable.id)
            {
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            canInteract = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        interactPrefab.SetActive(false);

        if (other.CompareTag("Player"))
        {
            isInteracting = false;
            canInteract   = false;
            UIManager.Instance.ToggleDialogBox(false, Character.Dialog[0]);

            if (TryGetComponent(out Merchant merchant))
                merchant.DisplayShop(false);
        }
    }

    public override Entity Interact()
    {
        if (isInteracting)
        {
            return this;
        }

        isInteracting = true;

        if (GameEnding.plantFlower)
        {
            UIManager.Instance.ToggleDialogBox(true, Character.Dialog[indexDialogColorBack]);
            return this;
        }

        if (!GameManager.Instance.GameData.activeQuests.Contains(prerequisiteQuest))
        {
            UIManager.Instance.ToggleDialogBox(true, Character.Dialog[indexDialogWithoutQuest]);
            return this;
        }

        UIManager.Instance.ToggleDialogBox(true, Character.Dialog[0]);

        return this;
    }

    public override Entity StartInteraction(bool enable)
    {
        interactPrefab.SetActive(enable);
        return this;
    }

    public override ScriptableObject GetScriptableObject()
    {
        return Character;
    }

    public override void SpecialInteraction(bool enableInteraction)
    {
        GameManager.Instance.GameData.Interactable.Add(interactable);
        if (GameManager.Instance.GameData.completedQuests.Contains(prerequisiteQuest))
        {
            GameManager.inventoryLock    = false;
            HeadsUpDisplay.inventoryLock = false;
        }

        if (TryGetComponent(out Merchant merchant))
            merchant.DisplayShop(enableInteraction);
    }
}
using System;
using UnityEngine;

public class NPC : Entity
{
    #region Variables and properties

    [SerializeField] private bool       canInteract;
    [SerializeField] private Character  character;
    [SerializeField] private GameObject interactPrefab;
    [SerializeField] private int        indexDialogColorBack;
    [SerializeField] private Sprite     undiscovered;

    private bool      isInteracting = false;
    public  bool      CanInteract => canInteract;
    public  Character Character   => character;

    public Sprite Undiscovered => undiscovered;

    [SerializeField] private Sprite coloredSprite;

    #endregion

    private void Start()
    {
        GameManager.OnColorChange += ChangeSprite;
        hasInteracted             =  false;

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

        if (GameEnding.plantFlower)
        {
            UIManager.Instance.ToggleDialogBox(true, Character.Dialog[indexDialogColorBack]);
        }
        else
        {
            UIManager.Instance.ToggleDialogBox(true, Character.Dialog[0]);
        }

        isInteracting = true;
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
        if (TryGetComponent(out Merchant merchant))
            merchant.DisplayShop(enableInteraction);
    }

    private void ChangeSprite()
    {
        if (coloredSprite == null) return;
        GetComponent<SpriteRenderer>().sprite =  coloredSprite;
        GameManager.OnColorChange             -= ChangeSprite;
    }
}
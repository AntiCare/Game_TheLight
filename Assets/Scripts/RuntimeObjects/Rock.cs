using System;
using UnityEngine;

public class Rock : Entity
{
    [SerializeField] private GameObject interactPrefab;
    [SerializeField] private Character  lulu;
    [SerializeField] private Quest      prerequisite;
    public                   Character  Lulu => lulu;

    private void Start()
    {
        hasInteracted = false;
        foreach (Interactable interactableData in GameManager.Instance.GameData.Interactable)
        {
            if (interactableData.id == interactable.id)
            {
                hasInteracted = true;
                EntityEvent.Interacting(interactable);
            }
        }
    }

    public override Entity Interact()
    {
        if (!hasInteracted && GameManager.Instance.GameData.activeQuests.Contains(prerequisite))
        {
            GetComponent<DropLoot>().GenLoots();
            interactPrefab.SetActive(false);
            hasInteracted = true;
            UIManager.Instance.ToggleDialogBox(true, Lulu.Dialog[0]);
            GameManager.Instance.GameData.Interactable.Add(interactable);
            EntityEvent.Interacting(interactable);
        }

        return this;
    }

    public override Entity StartInteraction(bool enable)
    {
        if (!GameManager.Instance.GameData.activeQuests.Contains(prerequisite)) return this;

        if (!hasInteracted) interactPrefab.SetActive(true);

        return this;
    }

    public override ScriptableObject GetScriptableObject()
    {
        return null;
    }

    public override void SpecialInteraction(bool enable)
    {
        Debug.Log($"not implemented ont Rock.cs");
    }
}
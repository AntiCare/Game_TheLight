using System;
using UnityEngine;

public class InteractableSeed : Entity
{
    [SerializeField] private GameObject interactPrefab;
    [SerializeField] private Character  lulu;
    [SerializeField] private Item  prerequisiteItem;
   
    public Character Lulu => lulu;
    
    public static bool interactedSeed = false;
    
    private void Start()
    {
        hasInteracted = false;
        foreach (Interactable interactableData in GameManager.Instance.GameData.Interactable)
        {
            if (interactableData.id == interactable.id)
            {
                interactPrefab.SetActive(true);
                hasInteracted  = true;
                interactedSeed = true;
                EntityEvent.Interacting(interactable);
            }
        }
    }

    public void InteractPrefab(bool active)
    {
        interactPrefab.SetActive(active);
    }
    
    public  override Entity Interact()
    {
        if (hasInteracted)
        {
            return this;
        }

        if (!GameManager.Instance.Player.Inventory.Items.Contains(prerequisiteItem))
        {
            return this;
        }
        
        interactedSeed = true;
        interactPrefab.SetActive(false);
        GameEnding.GameEnd = true;
        hasInteracted = true;
        GameManager.Instance.GameData.Interactable.Add(interactable);
        EntityEvent.Interacting(interactable);
        return this;
    }

    public override Entity StartInteraction(bool enable)
    {
        if (hasInteracted)
        {
            return this;
        }
        
        interactPrefab.SetActive(true);
        return this;
    }

    public override ScriptableObject GetScriptableObject()
    {
        return null;
    }

    public override void SpecialInteraction(bool enable)
    {
        throw new NotImplementedException();
    }
}
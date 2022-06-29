using System;
using UnityEngine;

public class Hole : Entity
{
    [SerializeField] private GameObject interactPrefab;
    [SerializeField] private Character  lulu;
    [SerializeField] public  GameObject blackFlower;
    public                   Character  Lulu => lulu;
    
    private void Start()
    {
        hasInteracted = false;
        foreach (Interactable interactableData in GameManager.Instance.GameData.Interactable)
        {
            if (interactableData.id == interactable.id)
            {
                blackFlower.SetActive(true);
                EntityEvent.Interacting(interactable);
                // GameEnding.plantFlower = true;
            }
        }
    }

    void Update()
    {
        if (GorillaFSM.getPlantTask)
        {
            gameObject.GetComponent<BoxCollider2D>().enabled = true;
        }
    }

    public override Entity Interact()
    {
        if (!hasInteracted)
        {
            interactPrefab.SetActive(false);
            
            UIManager.Instance.ToggleDialogBox(true, lulu.Dialog[5]);
        }

        return this;
    }

    public override Entity StartInteraction(bool enable)
    {
        if (!hasInteracted) interactPrefab.SetActive(true);

        return this;
    }

    public override ScriptableObject GetScriptableObject()
    {
        return null;
    }

    public override void SpecialInteraction(bool enable)
    {
        blackFlower.SetActive(true);
        GameEnding.plantFlower = true;
        interactPrefab.SetActive(false);
        hasInteracted = true;
        GameManager.Instance.GameData.Interactable.Add(interactable);
        EntityEvent.Interacting(interactable);
    }
}
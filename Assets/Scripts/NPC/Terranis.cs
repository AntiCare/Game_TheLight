using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Terranis : Entity
{
    #region Variables and properties

    [SerializeField] private bool       canInteract;
    [SerializeField] private Character  character;
    [SerializeField] private GameObject interactPrefab;
    public                   bool       CanInteract => canInteract;

    public  Character Character => character;
    
    #endregion

    private bool activeTerranis = true;

    // Start is called before the first frame update
    void Start()
    {
        hasInteracted = false;
        foreach (Interactable interactableData in GameManager.Instance.GameData.Interactable)
        {
            if (interactableData.id == interactable.id)
            {
                hasInteracted             = true;
                GameEnding.TerranisActive = true;
                EntityEvent.Interacting(interactable);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (GameEnding.TerranisActive && activeTerranis)
        {
            gameObject.GetComponent<SpriteRenderer>().enabled    = true;
            gameObject.GetComponent<BoxCollider2D>().enabled     = true;
            gameObject.GetComponent<CapsuleCollider2D>().enabled = true;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
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
            canInteract   = false;
            UIManager.Instance.ToggleDialogBox(false, Character.Dialog[0]);

            if (TryGetComponent(out Merchant merchant))
                merchant.DisplayShop(false);
        }
    }

    public override Entity Interact()
    {
        if (hasInteracted)
        {
            return this;
        }

        UIManager.Instance.ToggleDialogBox(true, Character.Dialog[0]);
        hasInteracted = true;
        return this;
    }

    public override Entity StartInteraction(bool enable)
    {
        if (hasInteracted)
        {
            return this;
        }
        
        interactPrefab.SetActive(enable);
        return this;
    }

    public override ScriptableObject GetScriptableObject()
    {
        return Character;
    }

    public override void SpecialInteraction(bool enableInteraction)
    {
        GameEnding.EarthEndingFinish = true;
        GetComponent<DropLoot>().GenLoots();
        activeTerranis                                   = false;
        gameObject.GetComponent<BoxCollider2D>().enabled = false;
        hasInteracted                                    = true;
        
        GameManager.Instance.GameData.Interactable.Add(interactable);
        EntityEvent.Interacting(interactable);
    }
}
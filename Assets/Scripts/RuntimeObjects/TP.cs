using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TP : Entity

{
    public  Transform  backDoor;
    private Transform  playerTransform;
    public  GameObject interactPrefab;

    // Start is called before the first frame update
    void Start()
    {
        playerTransform = GameManager.Instance.Player.GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            interactPrefab.SetActive(true);
        }

        if (other.gameObject.CompareTag("Player")
            && other.GetType().ToString() == "UnityEngine.CapsuleCollider2D")
        {
            // playerTransform.position = backDoor.position;
            // isDoor                   = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        interactPrefab.SetActive(false);

        if (other.gameObject.CompareTag("Player")
            && other.GetType().ToString() == "UnityEngine.CapsuleCollider2D")
        {
        }
    }

    public override Entity Interact()
    {
        playerTransform.position = backDoor.position;

        return this;
    }

    public override Entity StartInteraction(bool enable)
    {
        interactPrefab.SetActive(true);
        return this;
    }

    public override ScriptableObject GetScriptableObject()
    {
        throw new System.NotImplementedException();
    }

    public override void SpecialInteraction(bool enable)
    {
        throw new System.NotImplementedException();
    }
}
using System;
using UnityEngine;

public class Loot : Entity
{
    [SerializeField] private Item    item;
    private                  bool    pickUp;
    private                  Transform playerPosition;
    public bool reward = false;

    public void SetReward(bool b)
    {
        reward = b;
    }

    public bool GetReward()
    {
        return reward;
    }

    public Item Item
    {
        get => item;
        set => item = value;
    }
    

    public Item             GetItem => item;
    public Events.ItemEvent PlayerItemPickUp;

    // Start is called before the first frame update
    void Start()
    {
        DropLoot();
    }

    // Update is called once per frame
    void Update()
    {
        if (rb2d == null)
            return;

        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            rb2d.gravityScale = 0;
            rb2d.velocity     = Vector3.zero;
        }
    }

    private void FixedUpdate()
    {
        MoveToPlayer();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerPosition = other.transform;
            pickUp         = true;
        }
    }
    
    void MoveToPlayer()
    {
        if (pickUp)
        {
            transform.position = Vector3.MoveTowards(transform.position, playerPosition.position, Time.deltaTime * 40f);
        }
    }

    public override Entity Interact()
    {
        GameManager.Instance.Player.AddToInventory(this);
        Destroy(gameObject);
        return this;
    }

    public override Entity StartInteraction(bool enable)
    {
        
        if (Vector3.Distance(transform.position, playerPosition.position) <= 1f)
        {
            GameManager.Instance.Player.AddToInventory(this);
            Destroy(gameObject);
        }
        
        return this;
    }

    public override ScriptableObject GetScriptableObject()
    {
        return GetItem;
    }

    public override void SpecialInteraction(bool enable)
    {
        throw new System.NotImplementedException();
    }
}
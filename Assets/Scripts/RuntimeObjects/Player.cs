using System;
using Quests;
using UnityEngine;

public class Player : Entity
{
    private GameData _gameData;

    private Vector3 _position;

    [SerializeField] private float _currentPlayerHealth;

    [SerializeField] private float _totalPlayerHealth;

    [SerializeField] private float _currentPlayerLevel;

    [SerializeField] private Character _character;

    [SerializeField] private Camera _miniMapCamera;

    public GameObject interact;

    private int       LevelNum;
    private int       playerWallet = 0;
    private Inventory _inventory;
    private Entity    _currentEntity;
    private bool      canHurt;
    private float     hurtCooldown = 1f;

    public Events.PlayerChange   PlayerChange;
    public Events.ItemEvent      PlayerItemPickUp;
    public Events.CharacterEvent PlayerCharacterInteraction;
    public Events.PageEvent      PlayerPagePickup;
    public Events.PlayerChange   PlayerTakeDamage;
    public Events.PlayerChange   PlayerDied;

    public float     TotalHealth   => _totalPlayerHealth;
    public float     CurrentHealth => _currentPlayerHealth;
    public float     CurrentLevel  => _currentPlayerLevel;
    public Inventory Inventory     => _inventory;
    public Character Character     => _character;
    public Camera    MiniMapCamera => _miniMapCamera;
    public Entity    CurrentEntity => _currentEntity;

    public RuntimeAnimatorController coloredAnimator;

    public int PlayerWallet
    {
        get => playerWallet;
        set => playerWallet = value;
    }

    void Start()
    {
        PlayerChange?.Invoke(this);
        GameManager.OnColorChange += ChangeAnimator;
    }

    private void ChangeAnimator()
    {
        GetComponent<Animator>().runtimeAnimatorController = coloredAnimator;
        GetComponent<PlayerCombat>().TogglePermaLock(true);
        GameManager.OnColorChange -= ChangeAnimator;
    }

    void Update()
    {
        if (!canHurt)
        {
            HurtCooldown();
        }

        if (Input.GetKeyUp(KeyCode.E) && _currentEntity != null)
        {
            _currentEntity.Interact();
            interact.SetActive(false);
        }

        if (Input.GetButtonDown("Num1"))
        {
            UseItem(GameManager.Instance.GetItemById(2));
        }
    }

    void HurtCooldown()
    {
        if (!canHurt && hurtCooldown > 0)
        {
            hurtCooldown -= Time.deltaTime;
        }

        if (hurtCooldown <= 0)
        {
            canHurt      = true;
            hurtCooldown = 1f;
        }
    }

    public void SaveData(GameData gameData)
    {
        gameData.inventory     = _inventory;
        gameData.wallet        = playerWallet;
        gameData.currentHealth = _currentPlayerHealth;
        gameData.totalHealth   = _totalPlayerHealth;
        gameData.LevelNum      = LevelNum;
        gameData.currentLevel  = CurrentLevel;
        gameData.position      = transform.position;
        gameData.slimeCounter  = EnemyCounter.slimeCounter;
        gameData.rockerCounter  = EnemyCounter.rockerCounter;
        gameData.bossCounter  = EnemyCounter.bossCounter;

        SaveLoad.SaveData(gameData, GameManager.Instance.FilePath);
    }

    public void LoadData(GameData playerData)
    {
        _position            = playerData.position;
        _currentPlayerHealth = playerData.currentHealth;
        _totalPlayerHealth   = playerData.totalHealth;
        _currentPlayerLevel  = playerData.currentLevel;
        LevelNum             = playerData.LevelNum;
        playerWallet         = playerData.wallet;
        _inventory           = playerData.inventory;

        _inventory._activeQuests = playerData.activeQuests;
        _inventory._completeQuests = playerData.completedQuests;
        
        _gameData          = playerData;
        
        transform.position = _position;
        PlayerChange?.Invoke(this);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.GetComponent<Entity>())
        {
            other.TryGetComponent(out Entity entity);
            _currentEntity = entity;
            _currentEntity.StartInteraction(true);
        }

        if (other.CompareTag("EnemyDamage"))
        {
            TakeDamage(1f);
        }
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        if (other.collider.CompareTag("EnemyDamage") &&
            canHurt
            || other.collider.CompareTag("GorillaDamage")
            && canHurt)
        {
            TakeDamage(1f);
            canHurt = false;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        other.TryGetComponent(out Entity entity);
        //_currentEntity.StartInteraction(false);
        _currentEntity = null;
    }

    void TakeDamage(float damage)
    {
        if (CurrentHealth <= 0)
        {
            return;
        }

        if ((_currentPlayerHealth - damage) <= 0)
        {
            _currentPlayerHealth = 0;
            PlayerChange?.Invoke(this);
            PlayerTakeDamage?.Invoke(this);
            PlayerDied?.Invoke(this);
            return;
        }

        _currentPlayerHealth -= damage;
        PlayerTakeDamage?.Invoke(this);
        PlayerChange?.Invoke(this);
    }


    public void Upgrade(float xp)
    {
        _currentPlayerLevel += xp;
        if (_currentPlayerLevel >= 1)
        {
            LevelNum            += 1;
            _currentPlayerLevel -= (int) Math.Truncate(_currentPlayerLevel);
        }

        PlayerChange?.Invoke(this);
    }

    public string getLevel()
    {
        return LevelNum.ToString();
    }

    bool AddHitPoints(float hp)
    {
        if (_currentPlayerHealth == _totalPlayerHealth)
        {
            return false;
        }

        if ((_currentPlayerHealth + hp) > _totalPlayerHealth)
        {
            _currentPlayerHealth = _totalPlayerHealth;

            PlayerChange?.Invoke(this);

            return true;
        }

        _currentPlayerHealth += hp;

        PlayerChange?.Invoke(this);

        return true;
    }

    public void ConsumeUpgrade(Item item)
    {
        //Health upgrade Id 10
        if (item.ID == 10)
        {
            _totalPlayerHealth += item.ConsumeAmount;
            _currentPlayerHealth    =  _totalPlayerHealth;
            PlayerChange?.Invoke(this);
        }
    }

    public void AddToInventory(Loot item)
    {
        if (item.GetItem.ID == 0 && !item.GetReward())
        {
            playerWallet++;
        }

        if (_inventory.AddItemToInventory(item))
        {
            PlayerItemPickUp?.Invoke(item);
            PlayerChange?.Invoke(this);
        }
    }

    public void AddToPages(Page page)
    {
        if (_inventory.AddPagId(page) != null)
        {
            PlayerPagePickup?.Invoke(page.GetPage);
        }
    }

    public void AddToQuests(Quest quest)
    {
    }

    public void AddCharacterInteraction(Entity character)
    {
        Character entity = (Character) character.GetScriptableObject();

        if (_inventory.AddCharacterId(entity) != null)
        {
            PlayerCharacterInteraction?.Invoke(entity);
            SaveData(_gameData);
        }
    }

    public bool RemoveFromInventory(Item item)
    {
        return _inventory.RemoveItemId(item);
    }

    public bool UseItem(Item item)
    {
        if (!_inventory.Items.Contains(item)) return _inventory.Items.Contains(item);

        if (!AddHitPoints(item.ConsumeAmount)) return false;

        RemoveFromInventory(item);
        PlayerChange?.Invoke(this);
        return true;
    }


    public override Entity Interact()
    {
        SaveData(_gameData);
        return this;
    }

    public override Entity StartInteraction(bool enable)
    {
        return this;
    }

    public override ScriptableObject GetScriptableObject()
    {
        return Character;
    }

    public override void SpecialInteraction(bool enable)
    {
        throw new NotImplementedException();
    }
}
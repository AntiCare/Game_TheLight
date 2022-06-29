using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Quests;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    public enum GameState
    {
        PREGAME,
        RUNNING,
        PAUSED,
        INVENTORY,
        TUTORIAL,
        MAP,
        GAME_OVER,
    }


    [Serializable]
    public struct Level
    {
        public string     levelName;
        public GameObject LevelManager;
    }

    #region GAME MANAGER VARIABLES

    [SerializeField] private ItemDatabase ItemDatabase;
    [SerializeField] private GameObject   PlayerPrefab;
    [SerializeField] private List<Level>  levels;

    public        EventGameState                 OnGameStateChanged;
    public        EventDataSavingAndLoading      SavingOnQuit;
    private       List<AsyncOperation>           loadOperations;
    private       List<GameObject>               instancedSystemPrefabs;
    private       GameState                      currentGameState = GameState.PREGAME;
    private       string                         currentLevelName;
    private       string                         filePath;
    private       AudioListener                  audioListener;
    private       Dictionary<string, GameObject> levelDictionary;
    private       LevelManager                   currentLevelManager;
    private       QuestManager                   questManager;
    public static bool                           inventoryLock = true;
    public static Action                         OnColorChange;

    public static void ColorChange()
    {
        OnColorChange?.Invoke();
    }

    public QuestManager QuestManager
    {
        get => questManager;
        set => questManager = value;
    }

    private List<Task>    tasks;
    public  AudioListener AudioListener => audioListener;

    public GameData GameData { get; set; }

    #endregion

    #region GAME INSTANCE VARIABLES THAT MUST SAVED AT ALL COSTS

    public Player Player { get; set; }

    #endregion

    public float[] StartingPosition => new[] {85.0f, -500.0f};

    public GameState CurrentGameState
    {
        get => currentGameState;
        private set => currentGameState = value;
    }

    public string FilePath => filePath ?? string.Empty;

    void Start()
    {
        Player = null;
        DontDestroyOnLoad(gameObject);
        instancedSystemPrefabs = new List<GameObject>();
        loadOperations         = new List<AsyncOperation>();
        tasks                  = new List<Task>();
        OnGameStateChanged.Invoke(GameState.PREGAME, currentGameState);
        audioListener = GetComponent<AudioListener>();

        levelDictionary = new Dictionary<string, GameObject>();
        CreateLevelDictionary();
    }

    void Update()
    {
        if (currentGameState == GameState.PREGAME)
        {
            return;
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            currentLevelManager.SpawnEnemies();
            UIManager.Instance.LoadingScreen();
        }

        if (Input.GetKeyUp(KeyCode.Escape))
        {
            TogglePause();
        }

        if (Input.GetButtonDown("Inventory") && !inventoryLock)
        {
            ToggleInventory();
        }

        if (Input.GetButtonDown("Map") && !inventoryLock)
        {
            ToggleMap();
        }
    }

    void CreateLevelDictionary()
    {
        foreach (Level level in levels)
        {
            levelDictionary.Add(level.levelName, level.LevelManager);
        }
    }

    public void ZoomInMiniMap()
    {
        float minZoom = 300f;

        if (Player.MiniMapCamera.orthographicSize > minZoom)
        {
            Player.MiniMapCamera.orthographicSize -= 50f;
        }
    }

    public void ZoomOutMiniMap()
    {
        float maxZoom = 700f;

        if (Player.MiniMapCamera.orthographicSize < maxZoom)
        {
            Player.MiniMapCamera.orthographicSize += 50f;
        }
    }


    Player FindPlayer()
    {
        Player = currentLevelManager.Player;

        Player.PlayerChange.AddListener(HandlePlayerChangeForUI);

        HandlePlayerChangeForUI(Player);

        Player.PlayerItemPickUp.AddListener(HandlePlayerChangeForInventory);

        Player.PlayerCharacterInteraction.AddListener(HandlePlayerCharacterInteractionForInventory);

        Player.PlayerPagePickup.AddListener(HandlePlayerPagePickUp);

        Player.PlayerChange.AddListener(HandlePlayerPotionHotkey);
        HandlePlayerPotionHotkey(Player);

        Player.PlayerDied.AddListener(HandleGameOver);


        InitializePlayerInventory(Player.Inventory);

        return Player;
    }

    void OnLoadOperationComplete(AsyncOperation ao)
    {
        if (loadOperations.Contains(ao))
        {
            loadOperations.Remove(ao);

            if (loadOperations.Count == 0)
            {
                UpdateState(GameState.RUNNING);
            }

            FindPlayer();
        }
    }

    void OnUnloadOperationComplete(AsyncOperation ao)
    {
        // Clean up level is necessary, go back to main menu, save progress maybe, save game perhaps
    }

    void UpdateState(GameState state)
    {
        GameState previousGameState = currentGameState;
        currentGameState = state;

        switch (CurrentGameState)
        {
            case GameState.PREGAME:
                Time.timeScale = 1.0f;
                break;

            case GameState.RUNNING:
                Time.timeScale      = 1.0f;
                AudioListener.pause = false;
                break;

            case GameState.PAUSED:
                Time.timeScale      = 0.0f;
                AudioListener.pause = true;
                break;

            case GameState.INVENTORY:
                Time.timeScale = 0.0f;
                break;

            case GameState.MAP:
                Time.timeScale = 0.0f;
                break;

            case GameState.TUTORIAL:
                Time.timeScale = 0.0f;
                break;

            case GameState.GAME_OVER:
                Time.timeScale = 1.0f;
                break;

            default:
                break;
        }

        OnGameStateChanged.Invoke(currentGameState, previousGameState);
    }

    void LoadLevel(string levelName)
    {
        if (currentLevelName == levelName)
        {
            return;
        }

        AsyncOperation ao = SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Additive);

        LoadLevelManager(levelName);

        //TODO test this by changing levels and loading a new game
        // LoadQuestManager();


        if (ao == null)
        {
            Debug.LogError("[GameManager] Unable to load level " + levelName);
            return;
        }

        ao.completed += OnLoadOperationComplete;
        loadOperations.Add(ao);

        currentLevelName = levelName;
    }

    void LoadQuestManager()
    {
        var go = new GameObject("QuestManager");
        questManager = go
                      .AddComponent<QuestManager>()
                      .GetComponent<QuestManager>(); // makes a new gameobject with the script QuestManager

        instancedSystemPrefabs.Add(go);
    }

    void LoadLevelManager(string levelName)
    {
        if (levelDictionary.ContainsKey(levelName))
        {
            var lm = Instantiate(levelDictionary[levelName]);

            currentLevelManager = lm.GetComponent<LevelManager>();

            instancedSystemPrefabs.Add(lm);
        }
    }

    public Item      GetItemById(int      id) => ItemDatabase.Items.Find(item => item.ID                         == id);
    public Character GetCharacterById(int id) => ItemDatabase.Characters.Find(Character => Character.CharacterId == id);
    public DiaryPage GetPagesById(int     id) => ItemDatabase.Pages.Find(page => page.DiaryPageId                == id);
    public Note      GetNoteById(int      id) => ItemDatabase.Notes.Find(note => note.NoteId                     == id);
    public Quest     GetQuestById(int     id) => ItemDatabase.Quests.Find(quest => quest.QuestId                 == id);

    void UnloadLevel(string levelName)
    {
        AsyncOperation ao = SceneManager.UnloadSceneAsync(levelName);
        ao.completed += OnUnloadOperationComplete;
        currentLevelManager.DestroyInstantiatedObjects();
        currentLevelName    = null;
        currentLevelManager = null;
        DestroyInstantiatedObjects();
    }

    public void SetPath(string path) => filePath = path;

    public void TogglePause() =>
        UpdateState(currentGameState == GameState.RUNNING ? GameState.PAUSED : GameState.RUNNING);

    public void ToggleInventory() =>
        UpdateState(currentGameState == GameState.RUNNING ? GameState.INVENTORY : GameState.RUNNING);

    public void ToggleMap() =>
        UpdateState(currentGameState == GameState.RUNNING ? GameState.MAP : GameState.RUNNING);

    public void ToggleTutorial() =>
        UpdateState(currentGameState == GameState.RUNNING ? GameState.TUTORIAL : GameState.RUNNING);

    void HandleGameOver(Player player)
    {
        StartCoroutine(WaitForIt());
    }

    IEnumerator WaitForIt()
    {
        yield return new WaitForSeconds(2f);
        UpdateState(currentGameState == GameState.RUNNING ? GameState.GAME_OVER : GameState.RUNNING);
    }

    public void MainMenu()
    {
        UIManager.Instance.SetDummyCameraActive(true);
        UnloadLevel(currentLevelName);
        currentLevelName = null;
        UpdateState(GameState.PREGAME);
    }

    public void StartGame()
    {
        LoadLevel("EarthLevel");
    }

    public void RestartGameFromLastSave()
    {
        UnloadLevel(currentLevelName);
        currentLevelName = null;
        UpdateState(GameState.PREGAME);
        UIManager.Instance.SetDummyCameraActive(true);
        StartGame();
    }

    public void Settings()
    {
        LoadLevel("Settings");
    }

    public void BackToBoot()
    {
        UIManager.Instance.SetDummyCameraActive(true);
        UnloadLevel(currentLevelName);
        currentLevelName = null;
        UpdateState(GameState.PREGAME);
    }

    public void QuitGame()
    {
        SavingOnQuit?.Invoke();

        Debug.Log("[GameManager] Quit Game.");

        Application.Quit();
    }

    public void SavePlayer()
    {
        Player.SaveData(GameData);
        UpdateState(GameState.RUNNING);
        UIManager.Instance.ShowSaveGameNotification();
    }

    public void LoadNewPlayer()
    {
        filePath = string.Empty;
        StartGame();
    }

    void HandlePlayerChangeForUI(Player player)
    {
        UIManager.Instance.BindPlayerToUIElements(player);
    }

    void HandlePlayerChangeForInventory(Loot item)
    {
        UIManager.Instance.BindPlayerToInventoryElements(item.GetItem);
        UIManager.Instance.ShowNotification(item);
    }

    void HandlePlayerPagePickUp(DiaryPage page) => UIManager.Instance.BindPlayerPagesToInventory(page);

    void HandlePlayerPotionHotkey(Player player) =>
        UIManager.Instance.BindPlayerInventoryToUIElements(player.Inventory);

    void HandlePlayerCharacterInteractionForInventory(Character character)
    {
        UIManager.Instance.BindCharacterToNotes(character);
    }


    void InitializePlayerInventory(Inventory inventory)
    {
        UIManager.Instance.InitializePlayerInventory(inventory);
    }

    void DestroyInstantiatedObjects()
    {
        foreach (GameObject prefab in instancedSystemPrefabs)
        {
            Destroy(prefab);
        }
    }

    [Serializable]
    public class EventGameState : UnityEvent<GameState, GameState>
    {
    }

    [Serializable]
    public class EventDataSavingAndLoading : UnityEvent
    {
    }
}
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    #region Properites and variables

    [SerializeField] private StartMenu           _startMenu;
    [SerializeField] private PauseMenu           _pauseMenu;
    [SerializeField] private Camera              _dummyCamera;
    [SerializeField] private DialogManager       dialogManager;
    public                   DialogManager       DialogManager => dialogManager;
    [SerializeField] private HeadsUpDisplay      _headsUpDisplay;
    public                   HeadsUpDisplay      HeadsUpDisplay => _headsUpDisplay; // gets used in the QuestManager.cs
    [SerializeField] private EnemyManager        _enemyManager;
    [SerializeField] private NotificationManager _notificationManager;
    public                   NotificationManager NotificationManager => _notificationManager; // gets used in the QuestManager.cs
    [SerializeField] private HotKeyManager       _hotKeyManager;
    [SerializeField] private GameObject          enemyCounter;
    [SerializeField] private GameOver            gameOver;
    [SerializeField] private ItemShop            itemShop;
    [SerializeField] private LoadingScreen       loadingScreen;

    private AudioSource audioSource;
    public  AudioClip   earthLevel;
    public  AudioClip   MainMenuThemeClip;

    public AudioSource AudioSource => audioSource;

    #endregion

    private void Start()
    {
        GameManager.Instance.OnGameStateChanged.AddListener(HandleGameStateChanged);
        audioSource = GetComponent<AudioSource>();
    }

    private void HandleGameStateChanged(GameManager.GameState currentState, GameManager.GameState previousState)
    {
        if (previousState == GameManager.GameState.PREGAME)
            PlayEarthLevelClip(true);


        if (previousState != GameManager.GameState.PREGAME)
            loadingScreen.DisableLoadingScreen();

        switch (currentState)
        {
            case GameManager.GameState.RUNNING:
                
                if (previousState == GameManager.GameState.PREGAME )
                {
                    loadingScreen.ToggleLoadingScreen(true);
                }

                // enemyCounter.SetActive(true);
                _pauseMenu.gameObject.SetActive(false);
                ToggleInventoryWindow(false, null);
                ToggleTutorial(false);
                ToggleHUD(true);
                break;

            case GameManager.GameState.PAUSED:
                _pauseMenu.gameObject.SetActive(true);
                ToggleInventoryWindow(false, null);
                ToggleTutorial(false);
                ToggleDialogBox(false, null);
                ToggleHUD(false);
                ToggleItemShop(false, null);

                break;

            case GameManager.GameState.INVENTORY:
                ToggleDialogBox(false, null);
                ToggleInventoryWindow(true, null);
                ToggleItemShop(false, null);

                break;

            case GameManager.GameState.TUTORIAL:
                ToggleDialogBox(false, null);
                ToggleTutorial(true);
                ToggleItemShop(false, null);

                break;

            case GameManager.GameState.PREGAME:
                if (previousState == GameManager.GameState.GAME_OVER)
                    gameOver.FadeIn();
                
                loadingScreen.DisableLoadingScreen();
                // enemyCounter.SetActive(false);
            
                MainMenuMusic(true);
                _pauseMenu.gameObject.SetActive(false);
                ToggleHUD(false);
                ToggleItemShop(false, null);
                break;

            case GameManager.GameState.MAP:
                ToggleDialogBox(false, null);
                ToggleInventoryWindow(true, true);
                break;

            case GameManager.GameState.GAME_OVER:
                gameOver.FadeOut();
                ToggleItemShop(false, null);
                break;

            default:
                _pauseMenu.gameObject.SetActive(false);
                ToggleHUD(false);
                break;
        }
    }

    public void ToggleDialogBox(bool enable, [CanBeNull] CharacterDialog characterDialog)
    {
        if (characterDialog != null)
        {
            dialogManager.ShowDialog(characterDialog);
        }

        dialogManager.ToggleDialogBox(enable);
    }

    void MainMenuMusic(bool play)
    {
        audioSource.clip = MainMenuThemeClip;
        if (!play)
        {
            audioSource.Stop();
            return;
        }

        audioSource.Play();
    }

    void PlayEarthLevelClip(bool play)
    {
        audioSource.clip = earthLevel;
        if (!play)
        {
            audioSource.Stop();
            return;
        }

        audioSource.Play();
    }

    public void SetDummyCameraActive(bool active) => _dummyCamera.gameObject.SetActive(active);

    void ToggleHUD(bool active) => _headsUpDisplay.gameObject.SetActive(active);

    public void BindPlayerToUIElements(Player player) => _headsUpDisplay.BindHealthDataFromPlayer(player);

    public void InitializePlayerInventory(Inventory inventory) => _headsUpDisplay.InitializeInventoryData(inventory);

    public void BindPlayerToInventoryElements(Item item) => _headsUpDisplay.BindDataInventory(item);

    public void BindPlayerPagesToInventory(DiaryPage page) => _headsUpDisplay.BindDataToDiaryPages(page);

    public void BindPlayerInventoryToUIElements(Inventory inventory) =>
        _headsUpDisplay.BindInventoryToElements(inventory);

    public void BindCharacterToNotes(Character character)
    {
        _headsUpDisplay.BindDataToNotes(character);
        _notificationManager.ShowCharacterInteractionNotification(character);
    }

    public void BindQuestToPlayer(Quest quest)
    {
        _headsUpDisplay.BindDataToQuest(quest);
        _notificationManager.ShowQuestNotification(quest, null);
    }

    public void LoadingScreen()
    {
        loadingScreen.FadeIn();
        GameEnding.GameIntroActive = true;
    }

    public void ToggleInventoryWindow(bool active, bool ? map) => _headsUpDisplay.ToggleInventory(active, map);

    public void ToggleTutorial(bool active) => _headsUpDisplay.ToggleTutorial(active);

    public void ShowNotification(Entity notify) => _notificationManager.ShowItemPickUpNotification(notify);

    public void ShowSaveGameNotification() => _notificationManager.ShowSaveGameNotification();

    public void ToggleItemShop(bool active, [CanBeNull] List<Item> items) => itemShop.ToggleShop(active, items);
}
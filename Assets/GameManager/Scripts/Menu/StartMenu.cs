using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartMenu : MonoBehaviour
{
    [SerializeField] private Animation     _mainMenuAnimator;
    [SerializeField] private AnimationClip _fadeOutAnimation;
    [SerializeField] private AnimationClip _fadeInAnimation;
    [SerializeField] private GameObject    _buttons;

    public Button     NewGameButton;
    public Button     PlayButton;
    public Button     OptionsButton;
    public Button     QuitButton;
    public GameObject background;


    private void Awake()
    {
        NewGameButton.onClick.AddListener(HandleNewGameClick);
        OptionsButton.onClick.AddListener(HandleSettingClick);
        QuitButton.onClick.AddListener(HandleQuitClick);
        PlayButton.onClick.AddListener(HandlePlayClick);
    }

    public Events.EventFadeComplete OnFadeComplete; //Fade event no longer needed?
    public Events.EventFadeComplete OnFadeBegin;    //Fade event no longer needed?

    private void Start()
    {
        GameManager.Instance.OnGameStateChanged.AddListener(HandleGameStateChanged);
    }

    public void OnFadeInBegin()
    {
        _buttons.SetActive(false);
        OnFadeBegin.Invoke(true);
    }

    public void OnFadeOutComplete()
    {
        OnFadeComplete.Invoke(true);
    }

    public void OnFadeInComplete()
    {
        _buttons.SetActive(true);

        OnFadeComplete.Invoke(false);
    }

    void DisableBackGround()
    {
        background.SetActive(false);
    }

    void EnableBackGround()
    {
        background.SetActive(true);
    }

    void HandleGameStateChanged(GameManager.GameState currentState, GameManager.GameState previousState)
    {
        if (_mainMenuAnimator == null)
        {
            return;
        }

        if (previousState == GameManager.GameState.PREGAME && currentState != GameManager.GameState.PREGAME)
        {
            UIManager.Instance.SetDummyCameraActive(false);
            _mainMenuAnimator.clip = _fadeOutAnimation;
            _mainMenuAnimator.Play();
        }

        if (previousState != GameManager.GameState.PREGAME && currentState == GameManager.GameState.PREGAME)
        {
            _mainMenuAnimator.Stop();
            _mainMenuAnimator.clip = _fadeInAnimation;
            _mainMenuAnimator.Play();
        }
    }

    void HandleSettingClick()
    {
        DisableBackGround();
        GameManager.Instance.Settings();
    }

    void HandleQuitClick()
    {
        _buttons.SetActive(false);
        GameManager.Instance.QuitGame();
    }

    void HandlePlayClick()
    {
        // DisableBackGround();
        // _buttons.SetActive(false);
        // GameManager.Instance.StartGame();
    }

    void HandleNewGameClick()
    {
        DisableBackGround();
        _buttons.SetActive(false);
        EnemyCounter.slimeCounter  = 0;
        EnemyCounter.rockerCounter = 0;
        EnemyCounter.bossCounter   = 0;

        GameEnding.GameIntroCGActive = true;
        GameEnding.newGame= true;
        GameEnding.GameEmdingCGActive = true;
        GameEnding.CGActive = true;
        GameEnding.flowerActive = true;
        GameEnding.EarthBadEndingCGActive = true;
        GameManager.Instance.LoadNewPlayer();
    }
}
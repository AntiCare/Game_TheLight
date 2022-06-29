using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public Button ResumeButton;
    public Button QuitButton;
    public Button RestartButton;
    public Button SaveButton;
    
    private void Awake()
    {
        ResumeButton.onClick.AddListener(HandleResumeClick);
        SaveButton.onClick.AddListener(HandleSaveGameClick);
        QuitButton.onClick.AddListener(HandleQuitClick);
        RestartButton.onClick.AddListener(HandleRestartClick);
    }

    void HandleResumeClick()
    {
        GameManager.Instance.TogglePause();
    }

    void HandleQuitClick()
    {
        GameManager.Instance.QuitGame();
    }

    void HandleRestartClick()
    {
        GameManager.Instance.MainMenu();
    }
    
    void HandleSaveGameClick()
    {
        GameManager.Instance.SavePlayer();
    }
}

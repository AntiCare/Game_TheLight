using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingMenu : MonoBehaviour
{
    public Button BackButton;
    private void Awake()
    {
        BackButton.onClick.AddListener(HandleBackClick);
    }
    public void ChangeVideoSetting()
    {
        SceneManager.LoadScene("SettingVideo");
       
    }

    void HandleBackClick()
    {
        GameManager.Instance.BackToBoot();
    }
   
    
}

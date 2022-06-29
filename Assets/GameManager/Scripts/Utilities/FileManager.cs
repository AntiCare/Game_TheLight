using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class FileManager : MonoBehaviour
{
    [SerializeField] private GameObject saveGameFilePrefab;
    [SerializeField] private Transform  saveGameContent;
    [SerializeField] private GameObject _buttons;

    private List<GameObject> instantiatedItems;

    private void Awake()
    {
        instantiatedItems = new List<GameObject>();
    }

    private void Start()
    {
        GameManager.Instance.OnGameStateChanged.AddListener(HandleStateChange);
        InitializeSaveGames();
    }

    private void OnEnable()
    {
        InitializeSaveGames();
    }

    void HandleStateChange(GameManager.GameState currentState, GameManager.GameState previousState)
    {
        if (previousState == GameManager.GameState.PREGAME && currentState != GameManager.GameState.PREGAME)
        {
            ToggleSaveGameView(false);
        }

        if (previousState != GameManager.GameState.PREGAME && currentState == GameManager.GameState.PREGAME)
        {
        }
    }

    public void ToggleSaveGameView(bool active)
    {
        gameObject.SetActive(active);
    }

    private void InitializeSaveGames()
    {
        int count      = SaveLoad.GetSaveGamesFromDirectory().Count();
        int childCount = 1;

        if (count > 0)
        {
            DestroyInstantiatedItems();
        }

        if (saveGameContent.childCount < 1)
        {
            var item_go = Instantiate(saveGameFilePrefab, saveGameContent, true);
            item_go.GetComponentInChildren<Text>().text = "No Saved Games";
            item_go.transform.localScale                = Vector2.one;

            instantiatedItems.Add(item_go);

            return;
        }

        foreach (KeyValuePair<string, DateTime> pair in SaveLoad.GetSaveGamesFromDirectory()
                                                                .OrderByDescending(pair => pair.Value))
        {
            var item_go = Instantiate(saveGameFilePrefab, saveGameContent, true);

            Button use    = item_go.transform.GetChild(0).GetComponent<Button>();
            Button remove = item_go.transform.GetChild(1).GetComponent<Button>();

            use.onClick.AddListener(() => { HandleSaveGameButton(pair.Key); });
            remove.onClick.AddListener(() => { HandleSRemoveButton(pair.Key, item_go); });

            use.GetComponentInChildren<Text>().text = $"{childCount} Saved Game: {pair.Value} ";

            item_go.transform.localScale = Vector2.one;

            instantiatedItems.Add(item_go);
            childCount++;
        }
    }


    void DestroyInstantiatedItems()
    {
        if (instantiatedItems.Count < 1)
        {
            return;
        }

        foreach (GameObject item in instantiatedItems)
        {
            Destroy(item);
        }
    }

    void HandleSaveGameButton(string filePath)
    {
        GameManager.Instance.SetPath(filePath);
        GameManager.Instance.StartGame();
        _buttons.SetActive(false);
        ToggleSaveGameView(false);
    }

    void HandleSRemoveButton(string filePath, GameObject obj)
    {
        SaveLoad.DeleteFile(filePath);
        Destroy(obj);
    }

    public void HandleCloseButton()
    {
        ToggleSaveGameView(false);
    }
}
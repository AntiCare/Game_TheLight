using System;
using System.Collections.Generic;
using System.Linq;
using Enums;
using JetBrains.Annotations;
using Quests;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class HeadsUpDisplay : MonoBehaviour
{
    #region HealthUI

    [SerializeField] private GameObject _healthBar;
    [SerializeField] private GameObject _levelBar;
    [SerializeField] private Transform  _heartsContent;
    [SerializeField] private Transform  _heartsOutlineContent;
    [SerializeField] private GameObject _heartImagePrefab;
    [SerializeField] private GameObject _heartOutlineImagePrefab;

    #endregion

    #region Notebook Elements: inventory, tutorial, notes, diary, map, questbook

    [SerializeField] private Text       _level;
    [SerializeField] private GameObject _visualComponents;
    [SerializeField] private GameObject _inventory;
    [SerializeField] private GameObject _tutorial;
    [SerializeField] private GameObject _notes;
    [SerializeField] private GameObject _fathersDiary;
    [SerializeField] private GameObject _map;
    [SerializeField] private GameObject _questBook;

    #endregion

    #region Invetory Scrollview

    [SerializeField] private Transform  _content;
    [SerializeField] private GameObject _overlay;
    [SerializeField] private GameObject inventoryItemPrefab;

    #endregion

    #region CharacterImages

    [SerializeField] private GameObject characterImagePrefab;
    [SerializeField] private Transform  _characterImagesContent;

    #endregion

    #region CharacterNotes

    [SerializeField] private Transform  _characterNotesContent;
    [SerializeField] private GameObject characterNotesPrefab;

    #endregion

    #region Diary

    [SerializeField] private GameObject _pageImagePrefab;
    [SerializeField] private Transform  _DiaryPagesImagesContent;

    #endregion

    [SerializeField] private Transform  _DiaryPagesContent;
    [SerializeField] private GameObject _DiaryPagePrefab;

    [SerializeField] private GameObject hotKey1;
    [SerializeField] private GameObject numKey1;

    [SerializeField] private GameObject hotKey2;
    [SerializeField] private GameObject numKey2;

    [SerializeField] private GameObject hotKey3;
    [SerializeField] private GameObject numKey3;

    #region Quest manager and Objectives

    // For QuestBook //
    private                  QuestManager _questManager;
    [SerializeField] private Transform    QuestContent;
    [SerializeField] private GameObject   QuestPrefab;
    [SerializeField] private Transform    QuestDetailContent;
    [SerializeField] private GameObject   QuestDescriptionPrefab;

    [SerializeField] private Transform QuestCompleteContent;

    // For Objectives //
    [SerializeField] private Transform  objectivesContent;
    [SerializeField] private GameObject objectivePrefab;

    #endregion

    // For Objectives //

    private List<GameObject> instantiatedNoteItems;

    public AudioSource audioSource;
    public AudioClip   OpenInventoryAudioClip;
    public AudioClip   selectAudioClip;
    public AudioClip   turnPageAudioClip;

    public static bool       inventoryLock = true;
    public        GameObject inventoryButton;
    public        Button     toInventory;
    public        Button     toNotes;
    public        Button     toFathersDiary;
    public        Button     toMap;
    public        Button     toQuest;

    public Events.ItemEvent UseItem;

    private void Start()
    {
        instantiatedNoteItems = new List<GameObject>();

        toInventory.onClick.AddListener(() => { HandleTo(NotebookNavigation.INVENTORY); });
        toNotes.onClick.AddListener(() => { HandleTo(NotebookNavigation.NOTES); });
        toFathersDiary.onClick.AddListener(() => { HandleTo(NotebookNavigation.FATHERS_DIARY); });
        toMap.onClick.AddListener(() => { HandleTo(NotebookNavigation.MAP); });
        toQuest.onClick.AddListener(() => { HandleTo(NotebookNavigation.QUESTS); });

        ToggleInventory(false, null);
        ToggleTutorial(false);
        _visualComponents.SetActive(gameObject.activeSelf);
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        inventoryButton.SetActive(!inventoryLock);
    }

    private void InitializeQuestManager()
    {
        _questManager = QuestManager.Instance;
    }

    public void BindHealthDataFromPlayer(Player player)
    {
        for (int i = 0; i < _heartsContent.childCount; i++)
        {
            Destroy(_heartsContent.GetChild(i).gameObject);
        }

        for (int i = 0; i < _heartsOutlineContent.childCount; i++)
        {
            Destroy(_heartsOutlineContent.GetChild(i).gameObject);
        }

        int hearts = (int) player.CurrentHealth / 2;

        for (int i = 0; i < hearts; i++)
        {
            GameObject heart = Instantiate(_heartImagePrefab, _heartsContent, true);
            heart.transform.localScale = Vector2.one;
        }

        for (int i = 0; i < player.TotalHealth / 2; i++)
        {
            GameObject heart = Instantiate(_heartOutlineImagePrefab, _heartsOutlineContent, true);
            heart.transform.localScale = Vector2.one;
        }

        if ((int) player.CurrentHealth % 2 == 1)
        {
            GameObject heart = Instantiate(_heartImagePrefab, _heartsContent, true);
            heart.transform.localScale             = Vector2.one;
            heart.GetComponent<Image>().fillAmount = (player.CurrentHealth % 2) / 2;
        }

        _levelBar.GetComponent<Image>().fillAmount = player.CurrentLevel;
        _level.GetComponent<Text>().text           = player.getLevel();
    }

    public void InitializeInventoryData(Inventory inventory)
    {
        BindInventory(inventory.Items);
        BindCharacters(inventory.Characters);
        BindDiaryPages(inventory.Pages);
        InitQuests(inventory.ActiveQuests, GameManager.Instance.GameData.completedQuests);
    }

    public void BindInventory(List<Item> items)
    {
        for (int i = 0; i < _content.childCount; i++)
        {
            Destroy(_content.GetChild(i).gameObject);
        }

        Dictionary<int, int> pairs = GetCountForItems(items);

        foreach (KeyValuePair<int, int> pair in pairs)
        {
            Item item = GameManager.Instance.GetItemById(pair.Key);

            if (!item.Upgrade)
            {
                var item_go = Instantiate(inventoryItemPrefab, _content, true);

                Loot   lootItem  = item_go.transform.GetChild(0).GetComponent<Loot>();
                Button use       = item_go.transform.GetChild(1).GetComponent<Button>();
                Button remove    = item_go.transform.GetChild(2).GetComponent<Button>();
                Text   itemDesc  = item_go.transform.GetChild(3).GetComponent<Text>();
                Text   itemCount = item_go.transform.GetChild(4).GetComponent<Text>();


                lootItem.Item = item;

                itemCount.text = pair.Value.ToString();

                if (item.ID == 0)
                {
                    itemCount.text = GameManager.Instance.Player.PlayerWallet.ToString();
                }

                itemDesc.text = item.ItemName;


                if (item.QuestItem)
                {
                    item_go.transform.GetChild(1).gameObject.SetActive(false);
                    item_go.transform.GetChild(2).gameObject.SetActive(false);
                }

                if (!item.CanUse)
                {
                    item_go.transform.GetChild(1).gameObject.SetActive(false);
                }

                if (!item.CanRemove)
                {
                    item_go.transform.GetChild(2).gameObject.SetActive(false);
                }


                use.onClick
                   .AddListener(() => { HandleUseButton(item, item_go); });

                remove.onClick
                      .AddListener(() => { HandleRemoveButton(item, item_go); });

                Image itemSprite = item_go.GetComponentInChildren<Image>();

                itemSprite.sprite = item.ItemSprite;


                item_go.transform.localScale = Vector2.one;
            }
        }
    }

    Dictionary<int, int> GetCountForItems(List<Item> items)
    {
        Dictionary<int, int> counts = new SerializedDictionary<int, int>();

        foreach (Item item in items)
        {
            int count = 0;

            foreach (Item itemCount in items)
            {
                if (counts.ContainsKey(item.ID) && item.ID == itemCount.ID)
                {
                    count++;
                    counts[item.ID] = count;
                }

                if (item.ID == itemCount.ID && !counts.ContainsKey(item.ID))
                {
                    count++;
                    counts.Add(item.ID, count);
                }
            }
        }

        return counts;
    }

    void BindCharacters(List<Character> characters)
    {
        for (int i = 0; i < _characterImagesContent.childCount; i++)
        {
            Destroy(_characterImagesContent.GetChild(i).gameObject);
        }

        foreach (var character in characters)
        {
            var item_go = Instantiate(characterImagePrefab, _characterImagesContent, true);

            Button characterButton = item_go.GetComponent<Button>();

            characterButton.onClick.AddListener((() => { HandleCharacterButton(character); }));

            characterButton.GetComponent<Image>().sprite = character.CharacterNoteSprite;

            item_go.transform.localScale = Vector2.one;
        }
    }

    void BindDiaryPages(List<DiaryPage> pages)
    {
        for (int i = 0; i < _DiaryPagesImagesContent.childCount; i++)
        {
            Destroy(_DiaryPagesImagesContent.GetChild(i).gameObject);
        }

        foreach (var page in pages)
        {
            var item_go = Instantiate(_pageImagePrefab, _DiaryPagesImagesContent, true);

            Button pageButton = item_go.GetComponent<Button>();

            pageButton.onClick.AddListener((() => { HandleDiaryButton(page); }));

            pageButton.GetComponent<Image>().sprite = page.DiaryPageImage;

            item_go.transform.localScale = Vector2.one;
        }
    }

    public void BindDataInventory(Item item)
    {
        if (item.Upgrade)
        {
            return;
        }

        var item_go = Instantiate(inventoryItemPrefab, _content, true);

        Button use       = item_go.transform.GetChild(1).GetComponent<Button>();
        Button remove    = item_go.transform.GetChild(2).GetComponent<Button>();
        Text   itemCount = item_go.transform.GetChild(4).GetComponent<Text>();

        if (item.ID == 0)
        {
            itemCount.text = GameManager.Instance.Player.PlayerWallet.ToString();
        }

        if (item.QuestItem || !item.CanUse && !item.CanRemove)
        {
            item_go.transform.GetChild(1).gameObject.SetActive(false);
            item_go.transform.GetChild(2).gameObject.SetActive(false);
        }

        use.onClick
           .AddListener(() => { HandleUseButton(item, item_go); });

        remove.onClick
              .AddListener(() => { HandleRemoveButton(item, item_go); });

        item_go.GetComponentInChildren<Image>().sprite = item.ItemSprite;

        item_go.transform.localScale = Vector2.one;
    }

    public void BindDataToDiaryPages(DiaryPage diaryPage)
    {
        var item_go = Instantiate(_pageImagePrefab, _DiaryPagesImagesContent, true);

        Button pageButton = item_go.GetComponent<Button>();

        pageButton.onClick.AddListener((() => { HandleDiaryButton(diaryPage); }));

        pageButton.GetComponent<Image>().sprite = diaryPage.DiaryPageImage;

        item_go.transform.localScale = Vector2.one;
    }

    public void ToggleInventory(bool active, bool ? map)
    {
        _inventory.SetActive(active);
        _overlay.SetActive(active);

        toInventory.image.color    = map ?? false ? Color.gray : Color.white;
        toNotes.image.color        = Color.grey;
        toFathersDiary.image.color = Color.gray;
        toMap.image.color          = map ?? false ? Color.white : Color.gray;
        toQuest.image.color        = Color.gray;

        _visualComponents.SetActive(!active);
        _fathersDiary.SetActive(false);
        _notes.SetActive(false);
        _questBook.SetActive(false);
        _map.SetActive(map ?? false);

        PlayOpenInventorySound(active);
    }


    public void BindDataToNotes(Character character)
    {
        var item_go = Instantiate(characterImagePrefab, _characterImagesContent, true);

        Button characterButton = item_go.GetComponent<Button>();

        characterButton.onClick.AddListener((() => { HandleCharacterButton(character); }));

        characterButton.GetComponent<Image>().sprite = character.CharacterNoteSprite;

        item_go.transform.localScale = Vector2.one;
    }

    #region Quest Methods

    private void InitQuests(List<Quest> quests, List<Quest> completedQuests)
    {
        DisplayDetailPage(null);

        for (int i = 0; i < QuestContent.childCount; i++)
        {
            Destroy(QuestContent.GetChild(i).gameObject);
        }

        for (int i = 0; i < QuestDetailContent.childCount; i++)
        {
            Destroy(QuestDetailContent.GetChild(i).gameObject);
        }

        for (int i = 0; i < QuestCompleteContent.childCount; i++)
        {
            Destroy(QuestCompleteContent.GetChild(i).gameObject);
        }

        for (int i = 0; i < objectivesContent.childCount; i++)
        {
            Destroy(objectivesContent.GetChild(i).gameObject);
        }

        // DisplayObjective(null);

        foreach (var q in quests)
        {
            BindDataToQuest(q);

            if (q.IsActive)
            {
                BindObjectiveToScrollView(q);
            }
        }

        foreach (var completedQuest in completedQuests)
        {
            MoveQuestToFinished(completedQuest);
        }
    }

    public GameObject BindDataToQuest(Quest quest)
    {
        var item_go = Instantiate(QuestPrefab, QuestContent, true);
        item_go.GetComponent<QuestContainer>().Quest = quest;

        Button questButton  = item_go.transform.GetChild(0).GetComponent<Button>();
        Button activeButton = item_go.transform.GetChild(1).GetComponent<Button>();

        questButton.onClick.AddListener(() => { _questManager.SelectQuest(quest); });
        
        activeButton.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("checkmark");
        activeButton.transform.GetChild(0).GetComponent<Image>().color  = quest.IsActive ? Color.black : Color.clear;
        
        activeButton.onClick.AddListener(() => { QuestCheckmark(activeButton, _questManager.ActivateQuest(quest)); });

        TMP_Text questText = item_go.transform.GetChild(3).GetComponent<TMP_Text>();
        questText.text = quest.QuestName;

        item_go.transform.localScale = Vector2.one;

        if (quest.IsActive)
        {
            QuestCheckmark(activeButton, true);
        }

        return item_go;
    }

    public void MoveQuestToFinished(Quest finishedQuest)
    {
        var item_go = Instantiate(QuestPrefab, QuestCompleteContent, true);

        TMP_Text questText = item_go.transform.GetChild(3).GetComponent<TMP_Text>();
        questText.text = finishedQuest.QuestName;

        questText.fontStyle = FontStyles.Strikethrough;

        item_go.GetComponent<QuestContainer>().Quest = finishedQuest;

        Button questButton  = item_go.transform.GetChild(0).GetComponent<Button>();
        Button activeButton = item_go.transform.GetChild(1).GetComponent<Button>();

        questButton.onClick.AddListener(() => { _questManager.SelectQuest(finishedQuest); });

        activeButton.gameObject.SetActive(false);

        item_go.transform.localScale = Vector2.one;
    }

    public void QuestCompleted(Quest quest)
    {
        RemoveFromObjectiveScrollView(quest);
        RemoveFromActiveQuestScrollView(quest);
        MoveQuestToFinished(quest);
    }

    // Quest Checkmark //
    public void QuestCheckmark(Button activeButton, bool activate)
    {
        var buttonImage = activeButton.transform.GetChild(0).GetComponent<Image>();
        
        if (!activate)
        {
            buttonImage.sprite = Resources.Load<Sprite>("checkmark");
            buttonImage.color  = Color.clear;
            return;
        }
        
        buttonImage.sprite = Resources.Load<Sprite>("checkmark");
        buttonImage.color  = Color.black;
    }


    // Quest Detail Page //
    public void DisplayDetailPage([CanBeNull] Quest quest)
    {
        DestroyOldQuestDetails();

        if (quest == null)
        {
            return;
        }

        GameObject page = Instantiate(QuestDescriptionPrefab, QuestDetailContent, true);

        var QuestGo = page.transform.GetChild(0).gameObject;
        QuestDetail(QuestGo, quest);

        var Objective = page.transform.GetChild(1).gameObject;
        ObjectiveDetail(Objective, quest.GetCurrentObjective());

        var Reward = page.transform.GetChild(2).gameObject;
        RewardDetail(Reward, quest.Reward);

        var Divider = page.transform.GetChild(3).gameObject;
        Divider.SetActive(true);

        page.transform.localScale = Vector2.one;
    }

    void RewardDetail(GameObject go, [CanBeNull] int r)
    {
        var rewardTextGameObject = go.transform.GetChild(0).gameObject;
        rewardTextGameObject.GetComponent<Text>().text = "Rewards:";

        var reward = go.transform.GetChild(1).gameObject;

        var rewardAmount = reward.transform.GetChild(0).gameObject;
        rewardAmount.GetComponent<Text>().text = r.ToString();

        var rewardSprite = reward.transform.GetChild(1).gameObject;
    } //To display reward in details page


    public void BindObjectiveToScrollView(Quest activeQuest)
    {
        RemoveFromObjectiveScrollView(activeQuest);
        
        var item_go = Instantiate(objectivePrefab, objectivesContent, true);

        item_go.transform.GetChild(0).GetComponent<TMP_Text>().text = activeQuest.QuestName;

        item_go.transform.GetChild(1).GetComponent<TMP_Text>().text = activeQuest.GetCurrentObjective().Text;

        item_go.GetComponent<QuestContainer>().Quest = activeQuest;

        item_go.transform.localScale = Vector2.one;
    }

    public void RemoveFromObjectiveScrollView(Quest questToRemove)
    {
        for (int i = 0; i < objectivesContent.childCount; i++)
        {
            if (objectivesContent.GetChild(i).GetComponent<QuestContainer>().Quest.QuestId == questToRemove.QuestId)
            {
                Destroy(objectivesContent.GetChild(i).gameObject);
            }
        }
    }

    public void RemoveFromActiveQuestScrollView(Quest questToRemove)
    {
        for (int i = 0; i < QuestContent.childCount; i++)
        {
            if (QuestContent.GetChild(i).GetComponent<QuestContainer>().Quest.QuestId == questToRemove.QuestId)
            {
                Destroy(QuestContent.GetChild(i).gameObject);
            }
        }
    }

    void ObjectiveDetail(GameObject go, Objective o)
    {
        var curObj = go.transform.GetChild(0).gameObject;
        curObj.GetComponent<Text>().text = "Current objective";

        var obj = go.transform.GetChild(1).gameObject;
        obj.GetComponent<Text>().text = $"• {o.Text}"; // • or ○
    }                                                  //To Display objective in details page

    void QuestDetail(GameObject go, Quest quest)
    {
        var title = go.transform.GetChild(0).gameObject;
        title.GetComponent<Text>().text = quest.QuestName;

        var owner = go.transform.GetChild(1).gameObject;
        owner.GetComponent<Text>().text = $"From: {quest.Giver.CharacterName}";

        var description = go.transform.GetChild(2).gameObject;
        description.GetComponent<Text>().text = quest.QuestDescription;
    } //To Display quest in details page

    void DestroyOldQuestDetails()
    {
        for (int i = QuestDetailContent.transform.childCount - 1; i >= 0; i--)
        {
            var go = QuestDetailContent.GetChild(i);
            Destroy(go.gameObject);
        }
    }

    #endregion

    public void BindInventoryToElements(Inventory inventory)
    {
        int potionCount = 0;

        foreach (var item in inventory.Items)
        {
            if (item.ID == 2)
            {
                potionCount++;
            }
        }

        if (!hotKey1.activeSelf && potionCount > 0)
        {
            hotKey1.SetActive(true);
            numKey1.SetActive(true);
        }

        hotKey1.GetComponentInChildren<Text>().text = potionCount.ToString();

        BindInventory(inventory.Items);
    }


    void DestroyInstantiatedNoteItems()
    {
        _characterNotesContent.DetachChildren();
        _DiaryPagesContent.DetachChildren();
        foreach (var obj in instantiatedNoteItems)
        {
            Destroy(obj);
        }
    }

    void PlayOpenInventorySound(bool play)
    {
        if (play)
        {
            audioSource.clip = OpenInventoryAudioClip;
            audioSource.Play();
        }
    }

    void PlayTurnPageSound(bool play)
    {
        if (play)
        {
            audioSource.clip = turnPageAudioClip;
            audioSource.Play();
        }
    }

    void PlaySelectSound(bool play)
    {
        if (play)
        {
            audioSource.clip = selectAudioClip;
            audioSource.Play();
        }
    }

    public void ToggleTutorial(bool active)
    {
        _tutorial.SetActive(active);
        _visualComponents.SetActive(!active);
        _inventory.SetActive(false);
        PlaySelectSound(active);
    }

    void HandleItemHover(Item item)
    {
        Debug.Log($"Item: {item.ItemName}");
    }

    void HandleDiaryButton(DiaryPage diaryPage)
    {
        DestroyInstantiatedNoteItems();

        var item_go = Instantiate(_DiaryPagePrefab, _DiaryPagesContent, true);

        item_go.GetComponent<Text>().text = diaryPage.DiaryContents;

        item_go.transform.localScale = Vector2.one;

        instantiatedNoteItems.Add(item_go);

        PlaySelectSound(true);
    }

    void HandleUseButton(Item item, GameObject go)
    {
        if (item.CanUse && !item.QuestItem)
        {
            if (GameManager.Instance.Player.UseItem(item))
            {
                Destroy(go);
                UseItem?.Invoke(item.ItemObject.GetComponent<Loot>());
            }
        }

        PlaySelectSound(true);
    }

    void HandleRemoveButton(Item item, GameObject go)
    {
        if (item.CanRemove)
        {
            if (GameManager.Instance.Player.RemoveFromInventory(item))
            {
                Destroy(go);
            }
        }

        Destroy(go);
        PlaySelectSound(true);
    }

    void HandleCharacterButton(Character character)
    {
        DestroyInstantiatedNoteItems();

        foreach (var note in character.Notes)
        {
            var item_go = Instantiate(characterNotesPrefab, _characterNotesContent, true);

            item_go.GetComponent<Text>().text = note.NoteContents;

            item_go.transform.localScale = Vector2.one;

            instantiatedNoteItems.Add(item_go);
        }
    }

    void HandleTo(NotebookNavigation to)
    {
        _notes.SetActive(false);
        toNotes.image.color = Color.gray;

        toInventory.image.color = Color.gray;

        _fathersDiary.SetActive(false);
        toFathersDiary.image.color = Color.gray;

        _map.SetActive(false);
        toMap.image.color = Color.gray;

        _questBook.SetActive(false);
        toQuest.image.color = Color.gray;

        switch (to)
        {
            case NotebookNavigation.MAP:
                _map.SetActive(true);
                toMap.image.color = Color.white;
                break;

            case NotebookNavigation.NOTES:
                _notes.SetActive(true);
                toNotes.image.color = Color.white;
                break;

            case NotebookNavigation.QUESTS:
                if (_questManager == null) // TODO maybe add it somewhere else
                    InitializeQuestManager();
                _questBook.SetActive(true);
                toQuest.image.color = Color.white;
                // _questManager.DisplayDefault();
                break;

            case NotebookNavigation.INVENTORY:
                _inventory.SetActive(true);
                toInventory.image.color = Color.white;
                break;

            case NotebookNavigation.FATHERS_DIARY:
                _fathersDiary.SetActive(true);
                toFathersDiary.image.color = Color.white;
                break;
        }

        PlayTurnPageSound(true);
    }


    public void EnableHotkey1(bool active)
    {
        hotKey1.gameObject.SetActive(active);
    }

    public void EnableHotkey2(bool active)
    {
        hotKey1.gameObject.SetActive(active);
    }

    public void EnableHotkey3(bool active)
    {
        hotKey1.gameObject.SetActive(active);
    }
}
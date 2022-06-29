using System.Collections;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public class NotificationManager : MonoBehaviour
{
    public GameObject notificationPrefab;
    public Transform  notificationNotificationScrollViewContent;
    public Sprite     closeSprite;
    public AudioClip  notificationPopUpSoundCLip;
    public AudioClip  CoinClip;

    public AudioSource _audioSource;

    private void Start()
    {
        GameManager.Instance.OnGameStateChanged.AddListener(HandleGameStateChanged);
        _audioSource = GetComponent<AudioSource>();
    }


    private void HandleGameStateChanged(GameManager.GameState currentState, GameManager.GameState previousState)
    {
        if (previousState == GameManager.GameState.RUNNING)
        {
            ClearNotifications();
        }
    }

    public void ShowItemPickUpNotification(Entity notify)
    {
        if (notify.GetScriptableObject() == null)
            return;


        if (!gameObject.activeSelf)
            ToggleNotificationView(true);


        Item item = (Item) notify.GetScriptableObject();

        GameObject notification = Instantiate(notificationPrefab, notificationNotificationScrollViewContent, true);

        Button button = notification.GetComponentInChildren<Button>();


        if (item.QuestItem)
        {
            button.onClick.AddListener(() => { HandleNotificationOnCLick(notify); });
        }
        else
        {
            button.onClick.AddListener(() => { HandleSaveGameNotification(notification); });
            button.GetComponent<Image>().sprite = closeSprite;
        }

        Text text = notification.GetComponentInChildren<Text>();

        Image image = notification.GetComponentInChildren<Image>();

        text.text = item.ItemName;

        image.sprite = item.ItemSprite;

        notification.transform.localScale = Vector2.one;

        if (item.ID != 0)
            PlayPopUpNotificationSound();
        else
            PlayCoinClip();


        StartCoroutine(DeleteNotification(notification));
    }

    public void ShowCharacterInteractionNotification(Character character)
    {
        if (!gameObject.activeSelf)
            ToggleNotificationView(true);


        GameObject notification = Instantiate(notificationPrefab, notificationNotificationScrollViewContent, true);

        Button button = notification.GetComponentInChildren<Button>();

        button.onClick.AddListener(() => { HandleCharacterNotificationOnCLick(notification); });

        Text text = notification.GetComponentInChildren<Text>();

        Image image = notification.GetComponentInChildren<Image>();

        text.text = character.CharacterName;

        image.sprite = character.CharacterNoteSprite;

        notification.transform.localScale = Vector2.one;

        PlayPopUpNotificationSound();

        StartCoroutine(DeleteNotification(notification));
    }

    public void ShowQuestNotification(Quest quest, [CanBeNull] Objective objective)
    {
        if (!gameObject.activeSelf)
            ToggleNotificationView(true);

        GameObject notification = Instantiate(notificationPrefab, notificationNotificationScrollViewContent, true);

        Button button = notification.GetComponentInChildren<Button>();

        button.onClick.AddListener(() => { HandleSaveGameNotification(notification); });

        button.GetComponent<Image>().sprite = closeSprite;

        Text text = notification.GetComponentInChildren<Text>();

        text.text = quest.IsCompleted ? $"Completed {quest.QuestName}" : quest.QuestName;

        if (objective != null)
        {
            text.text = $"Completed: {objective.Text}";
        }

        notification.transform.localScale = Vector2.one;

        PlayPopUpNotificationSound();

        StartCoroutine(DeleteNotification(notification));
    }

    public void ShowSaveGameNotification()
    {
        if (!gameObject.activeSelf)
            ToggleNotificationView(true);

        GameObject notification = Instantiate(notificationPrefab, notificationNotificationScrollViewContent, true);

        Button button = notification.GetComponentInChildren<Button>();

        button.onClick.AddListener(() => { HandleSaveGameNotification(notification); });

        button.GetComponent<Image>().sprite = closeSprite;

        Text text = notification.GetComponentInChildren<Text>();

        text.text = "Game Saved!";

        notification.transform.localScale = Vector2.one;

        PlayPopUpNotificationSound();

        StartCoroutine(DeleteNotification(notification));
    }

    void PlayPopUpNotificationSound()
    {
        _audioSource.clip = notificationPopUpSoundCLip;
        _audioSource.Play();
    }

    void PlayCoinClip()
    {
        _audioSource.clip = CoinClip;

        if (_audioSource.isPlaying)
            return;

        _audioSource.Play();
    }

    private IEnumerator DeleteNotification(GameObject notification)
    {
        if (!gameObject.activeSelf)
            yield return new WaitUntil(() => gameObject.activeSelf);

        yield return new WaitForSeconds(5f);

        Destroy(notification.gameObject);

        HideNotificationsWhenEmpty();
    }

    private void HideNotificationsWhenEmpty()
    {
        if (notificationNotificationScrollViewContent.childCount == 0)
        {
            ToggleNotificationView(false);
        }
    }

    void ClearNotifications()
    {
        for (int i = 0; i < notificationNotificationScrollViewContent.childCount; i++)
        {
            Destroy(notificationNotificationScrollViewContent.GetChild(i).gameObject);
        }

        HideNotificationsWhenEmpty();
    }

    void HandleNotificationOnCLick(Entity notify)
    {
        GameManager.Instance.ToggleInventory();
    }

    void HandleCharacterNotificationOnCLick(GameObject notify)
    {
        GameManager.Instance.ToggleInventory();
        Destroy(notify);
    }

    void HandleSaveGameNotification(GameObject notify)
    {
        Destroy(notify);
    }

    private void ToggleNotificationView(bool active) => gameObject.SetActive(active);
}
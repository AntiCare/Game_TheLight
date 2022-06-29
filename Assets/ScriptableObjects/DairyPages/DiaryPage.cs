using System;
using UnityEngine;

[CreateAssetMenu(fileName = "DiaryPage", menuName = "ScriptableObjects/DiaryPage")]
[Serializable]
public class DiaryPage : ScriptableObject
{
    [SerializeField] private int diaryPageId;
    [SerializeField] private string diaryPageName;
    [SerializeField] private string diaryContents;
    [SerializeField] private Sprite diaryPageImage;
    [SerializeField] private GameObject diaryPage;

    public int DiaryPageId => diaryPageId;

    public string DiaryPageName => diaryPageName;

    public string DiaryContents => diaryContents;

    public Sprite DiaryPageImage => diaryPageImage;

    public GameObject DiaryPageObeGameObject => diaryPage;
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DropLoot : MonoBehaviour
{
    [SerializeField] private List<Item> commonLoot;
    [SerializeField] private List<Item> rareLoot;
    [SerializeField] private List<Item> legendaryLoot;
    [SerializeField] private List<Item> questLoot;
    
    [SerializeField] private List<DiaryPage> diaryPages;

    private List<List<Item>> _lootsList;

    private void Start()
    {
        _lootsList = new List<List<Item>>();

        _lootsList.Add(commonLoot);
        _lootsList.Add(rareLoot);
        _lootsList.Add(legendaryLoot);
        _lootsList.Add(questLoot);
    }
    
    public void GenLoots()
    {
        if (_lootsList == null)
        {
            return;
        }
        foreach (var lootList in _lootsList)
        {
            if (lootList.Count > 0)
            {
                Vector3 pos = transform.position;

                Instantiate(lootList[UnityEngine.Random.Range(0, lootList.Count)].ItemObject, pos, Quaternion.identity);
            }
        }

        if (diaryPages.Count > 0)
        {
            foreach (var page in diaryPages)
            {
                Vector3 pos = transform.position;

                Instantiate(page.DiaryPageObeGameObject, pos, Quaternion.identity);
            }
        }
    }
}
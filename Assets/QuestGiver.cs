using System.Collections;
using System.Collections.Generic;
using Quests.Objectives.Events;
using UnityEngine;

public class QuestGiver : MonoBehaviour
{
    public  Quest  questToGive;
    private Player player;

    // Start is called before the first frame update
    void Start()
    {
        player = GameManager.Instance.Player;
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void GiveQuest()
    {
        QuestEvent.OnAddQuest(questToGive, questToGive.Giver);
    }
}
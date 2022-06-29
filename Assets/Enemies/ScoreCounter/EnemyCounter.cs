using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyCounter : Singleton<EnemyCounter>
{
    public static int slimeCounter, rockerCounter, bossCounter = 0;

    public static event Action EnemySlain;

    public Text score;

    // Start is called before the first frame update
    void Start()
    {
        score = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        score.text = "Slime: " + slimeCounter + "\r\nRocker: " + rockerCounter + "\r\nBoss: " + bossCounter;
    }

    public static void InitData(int slime, int rocker, int boss)
    {
        slimeCounter  = slime;
        rockerCounter = rocker;
        bossCounter   = boss;
    }

    public void OnEnemySlain()
    {
        EnemySlain?.Invoke();
    }
}
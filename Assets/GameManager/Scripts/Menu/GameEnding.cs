using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameEnding : MonoBehaviour
{
    private AnimatorStateInfo animationInfo;

    //EarthLevel
    public static bool EarthEndingFinish = false;

    //bad ending
    public static bool gorillaDead = false;

    public GameObject EarthBadEndingImage;
    public Animator   EarthBadEnding;
    public static bool      EarthBadEndingCGActive = true;
    //Good ending
    //plant flower
    public static bool       plantFlower = false;
    public        GameObject EarthGoodEndingImage;
    public        Animator   EarthGoodEnding;
    public static bool       flowerActive = true;

    //TerranisCG
    public static bool TerranisActive = false;

    public  GameObject EarthGoodEndingTerranis;
    public  Animator   EarthGoodEndingTerranisCG;
    public static bool       CGActive = true;
    
    //GameEnding
    public  static bool GameEnd = false;
    public  GameObject EarthGameEnding;
    public  Animator   EarthGameEndingCG;
    public static bool       GameEmdingCGActive = true;
    
    //GameIntro
    public  static bool GameIntroActive = false;
    public  static bool newGame = false;
    public  GameObject  GameIntro;
    public  Animator    GameIntroCG;
    public static bool       GameIntroCGActive = true;
    public void HandleGameStateChanged(GameManager.GameState currentState, GameManager.GameState previousState)
    {
        plantFlower = false;
        if (previousState == GameManager.GameState.PREGAME && currentState == GameManager.GameState.RUNNING)
        {
            var quests       = GameManager.Instance.GameData.completedQuests;
            var activeQuests = GameManager.Instance.GameData.activeQuests;


            var quest = activeQuests.Find(q => q.QuestId == 10);

            if (quest != null && quest.IndexObjective == 2)
            {
                ChangeColor.color = true;
                EarthGoodEndingImage.SetActive(false);
                flowerActive = false;
                plantFlower  = true;
            }

            if (quests.Find(q => q.QuestId == 10)) //terranis quest id
            {
                ChangeColor.color = true;
                EarthGoodEndingImage.SetActive(false);
                flowerActive      = false;
                plantFlower       = true;
                EarthEndingFinish = true;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        //EarthLevel-bad ending
        if (gorillaDead && EarthBadEndingCGActive)
        {
            EarthBadEndingImage.SetActive(true);
            EarthBadEnding.Play("EarthBadending");
            animationInfo = EarthBadEnding.GetCurrentAnimatorStateInfo(0);

            if (animationInfo.normalizedTime >= .99f)
            {
                EarthEndingFinish = true;
                EarthBadEndingImage.SetActive(false);
                EarthBadEndingCGActive = false;
            }
        }

        //EarthLevel-good ending - plant flower
        if (plantFlower && flowerActive)
        {
            EarthGoodEndingImage.SetActive(true);
            EarthGoodEnding.Play("EGED2");
            animationInfo = EarthGoodEnding.GetCurrentAnimatorStateInfo(0);

            if (animationInfo.normalizedTime >= .99f)
            {
                ChangeColor.color = true;
                EarthGoodEndingImage.SetActive(false);
                flowerActive = false;
            }
        }

        //EarthLevel-ending - TerranisCG
        if (GorillaFSM.startTerranisCG && CGActive)
        {
            EarthGoodEndingTerranis.SetActive(true);
            EarthGoodEndingTerranisCG.Play("Terranis2");
            animationInfo = EarthGoodEndingTerranisCG.GetCurrentAnimatorStateInfo(0);

            if (animationInfo.normalizedTime >= .99f)
            {
                TerranisActive = true;
                EarthGoodEndingTerranis.SetActive(false);
                CGActive = false;
            }
        }
        
        //EarthLevel-GameEnding
        if (GameEnd && GameEmdingCGActive)
        {
            EarthGameEnding.SetActive(true);
            EarthGameEndingCG.Play("GameEnding2");
            animationInfo = EarthGameEndingCG.GetCurrentAnimatorStateInfo(0);

            if (animationInfo.normalizedTime >= .99f)
            {
                GameManager.Instance.MainMenu();
                EarthGameEnding.SetActive(false);
                GameEmdingCGActive = false;
                
            }
        }
        
        //GameIntro
        if (newGame && GameIntroCGActive && GameIntroActive)
        {
            //newGame = false;
            GameIntro.SetActive(true);
            GameIntroCG.Play("IntroCG2");
            animationInfo = GameIntroCG.GetCurrentAnimatorStateInfo(0);

            if (animationInfo.normalizedTime >= .99f)
            {
                GameIntro.SetActive(false);
                GameIntroCGActive = false;
            }
        }
    }
}
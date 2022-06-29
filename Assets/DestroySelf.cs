using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroySelf : MonoBehaviour
{
   void Start()
    {
        GameManager.Instance.Player.PlayerDied.AddListener(HandlePlayerFaint);
        GameManager.Instance.Player.PlayerTakeDamage.AddListener(PlayerHurt);
    }

    private void HandlePlayerFaint(Player player)
    {
        Destroy(gameObject);
    }

    void PlayerHurt(Player player)
    {
        gameObject.SetActive(false);
    }
}
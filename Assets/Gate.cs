using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour
{
    [SerializeField] private Quest prerequisite;

    private void OnTriggerStay2D(Collider2D other)
    {
        if (GameManager.Instance.GameData.completedQuests.Contains(prerequisite))
        {
            GetComponent<Animator>().SetTrigger("Open");
        }
    }

    public void DisableGameObject()
    {
        gameObject.SetActive(false);
    }
}
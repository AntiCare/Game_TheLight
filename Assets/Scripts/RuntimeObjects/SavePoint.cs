using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePoint : MonoBehaviour
{
    private float saveCoolDown = 10f;
    private bool canSave = true;
    private void Update()
    {
        if (!canSave && saveCoolDown > 0)
        {
            saveCoolDown -= Time.deltaTime;
        }

        if (saveCoolDown <= 0)
        {
            canSave = true;
            saveCoolDown = 10f;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player") && canSave)
        {
            other.TryGetComponent(out Entity entity);
            entity.Interact();
            UIManager.Instance.ShowSaveGameNotification();
            canSave = false;
        }
    }
}
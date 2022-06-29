using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : Entity
{
    private float arrowSpeed     = 100f;
    private float disappearTimer = 3.5f;
    private bool  canDisappear;

    private void Update()
    {
        if (!gameObject.GetComponent<Renderer>().isVisible || canDisappear)
        {
            gameObject.SetActive(false);
            canDisappear = false;
        }

        if (!canDisappear)
        {
            DisappearCooldown();
        }
    }

    void DisappearCooldown()
    {
        if (!canDisappear && disappearTimer > 0)
        {
            disappearTimer -= Time.deltaTime;
        }

        if (disappearTimer <= 0)
        {
            canDisappear   = true;
            disappearTimer = 3.5f;
        }
    }

    public void Fire()
    {
        Rigidbody2D rigid = gameObject.GetComponent<Rigidbody2D>();
        rigid.velocity = transform.right * arrowSpeed;
    }

    private void OnCollisionEnter(Collision other)
    {
    }

    public override Entity Interact()
    {
        return this;
    }

    public override Entity StartInteraction(bool enable)
    {
        throw new NotImplementedException();
    }

    public override ScriptableObject GetScriptableObject()
    {
        throw new NotImplementedException();
    }

    public override void SpecialInteraction(bool enable)
    {
        throw new NotImplementedException();
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Page : Entity
{
    [SerializeField] private DiaryPage page;

    public DiaryPage GetPage => page;

    private void Start()
    {
        GetComponent<BoxCollider2D>().enabled = false;
        DropLoot();
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            rb2d.gravityScale = 0;
            rb2d.velocity     = Vector3.zero;
        }
    }

    public override Entity Interact()
    {
        GameManager.Instance.Player.AddToPages(this);
        Destroy(gameObject);
        return this;
    }

    public override Entity StartInteraction(bool enable)
    {
        GameManager.Instance.Player.AddToPages(this);
        Destroy(gameObject);
        return this;
    }

    public override ScriptableObject GetScriptableObject()
    {
        return GetPage;
    }

    public override void SpecialInteraction(bool enable)
    {
        throw new NotImplementedException();
    }
}
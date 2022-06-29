using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeColor : MonoBehaviour
{

    public static bool color = false;
    
    public SpriteRenderer spriteRenderer;
    public Sprite colorSprite;
    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (color)
        {
            spriteRenderer.sprite = colorSprite; 
            GameManager.ColorChange();
        }

    }
}

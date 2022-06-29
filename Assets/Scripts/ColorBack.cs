using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorBack : MonoBehaviour
{
    [SerializeField] private Sprite colorSprite;
    // Start is called before the first frame update
    void Start()
    {
        GameManager.OnColorChange += ColorChange;
    }

    void ColorChange()
    {
        GetComponent<Image>().sprite =  colorSprite;
        GameManager.OnColorChange    -= ColorChange;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SeedActive : MonoBehaviour
{
    public GameObject GameObject;

    public GameObject Fog;

    public GameObject Flower;

    public GameObject GlobalLight;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if ( GameEnding.EarthEndingFinish)
        {
            GameObject.SetActive(true);
        }

        if (zoom.fog)
        {
            Fog.SetActive(true);
        }
        else
        {
            Fog.SetActive(false);
        }

        if (ChangeColor.color)
        {
            Flower.SetActive(true);
            GlobalLight.GetComponent<UnityEngine.Rendering.Universal.Light2D>().intensity = 0.2f;
        }
    }
}

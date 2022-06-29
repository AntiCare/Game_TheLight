using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ground1 : MonoBehaviour
{

    public GameObject ground;
    // Start is called before the first frame update
    void Start()
    {
        
        
    }

    // Update is called once per frame
    void Update()
    {
        if (ChangeColor.color)
        {
            ground.SetActive(false);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using AkilliMum.SRP.D2WeatherEffects.URP;
using UnityEngine;

public class zoom : MonoBehaviour
{

    public Camera mainCamera;

    public float minFov = 50;

    public float maxFov = 100;

    public float sensitivity = 10f;

    public static bool fog =false;
    
    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        float fov = mainCamera.fieldOfView;
        fov += Input.GetAxis("Mouse ScrollWheel") * sensitivity;
        fov = Mathf.Clamp(fov, minFov, maxFov);
        mainCamera.fieldOfView = fov;
        
        if (fov >= maxFov-10)
        {
            fog = true;
        }
        else
        {
            fog = false;
        }
    }
}

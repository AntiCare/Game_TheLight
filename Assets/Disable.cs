using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Disable : MonoBehaviour
{
    
    
    public void DisableThisGameObject()
    {
        Debug.Log($"disabled"); 
        this.gameObject.SetActive(false);
    }
}

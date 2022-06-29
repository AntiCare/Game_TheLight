using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using JetBrains.Annotations;
using UnityEngine;

public class HotKeyManager : MonoBehaviour
{
    [SerializeField] private GameObject hotKey1;
    [SerializeField] private GameObject hotKey2;
    [SerializeField] private GameObject hotKey3;
    
    void Start()
    {
        
    }
    
    public void EnableHotkey1(bool active)
    {
        hotKey1.gameObject.SetActive(active);
    }

    public void EnableHotkey2(bool active)
    {
        hotKey2.gameObject.SetActive(active);
    }

    public void EnableHotkey3(bool active)
    {
        hotKey3.gameObject.SetActive(active);
    }
}
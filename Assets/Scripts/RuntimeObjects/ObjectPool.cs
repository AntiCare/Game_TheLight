using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool objectPoolInstance;
    public List<GameObject> pooledObjects;
    public GameObject objectToPool;
    [SerializeField] private int poolAmount = 10;

    private void Awake()
    {
        objectPoolInstance = this;
    }

    private void Start()
    {
        pooledObjects = new List<GameObject>();

        GameObject temp;

        for (int i = 0; i < poolAmount; i++)
        {
            temp = Instantiate(objectToPool);
            temp.SetActive(false);
            pooledObjects.Add(temp);
        }
    }
    
    public GameObject GetPooledObject()
    {
        for (int i = 0; i < poolAmount; i++)
        {
            if (!pooledObjects[i].activeInHierarchy)
            {
                return pooledObjects[i];
            }
        }

        return null;
    }

    private void OnDestroy()
    {
        foreach (GameObject pooledObject in pooledObjects)
        {
            Destroy(pooledObject);
        }
    }
}

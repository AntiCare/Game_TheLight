using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapLocation : MonoBehaviour
{
    [SerializeField] private Location location;

    public  Location                  Location => location;
    private CircleCollider2D          _collider2D;
    public  Events.LocationDiscovered OnDiscovered;

    void Start()
    {
        _collider2D = GetComponent<CircleCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            OnDiscovered?.Invoke(location);
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
}
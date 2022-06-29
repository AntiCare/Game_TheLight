using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Location", menuName = "ScriptableObjects/Location")]
[Serializable]
public class Location : ScriptableObject
{
    [SerializeField] public int         locationId;
    [SerializeField] public string      locationName;
    [SerializeField] public Vector3     locationPosition;
    [SerializeField] public MapLocation mapLocation;

    public int LocationId => locationId;

    public string LocationName => locationName;

    public Vector3 LocationPosition => locationPosition;

    public MapLocation MapLocation => mapLocation;
}
using Quests.Objectives.Events;
using UnityEngine;

public class DiscoverManager : MonoBehaviour
{
    public Events.LocationDiscovered LocationReached;

    void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            MapLocation location = transform.GetChild(i).GetComponent<MapLocation>();

            location.OnDiscovered.AddListener(LocationDiscovered);
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    void LocationDiscovered(Location location)
    {
        if (!GameManager.Instance.GameData.discoveredLocations.Contains(location))
        {
            GameManager.Instance.GameData.discoveredLocations.Add(location);
        }
        
        LocationReached?.Invoke(location);
        DiscoverEvent.Discover(location);
    }
}
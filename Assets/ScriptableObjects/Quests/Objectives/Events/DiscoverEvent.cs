using System;

namespace Quests.Objectives.Events
{
    public static class DiscoverEvent
    {
        public static event Action<Location> OnDiscover;
        
        public static void Discover(Location location)
        {
            OnDiscover?.Invoke(location);
        }
    }
}
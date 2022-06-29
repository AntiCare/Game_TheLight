using System;
using UnityEngine;
using UnityEngine.Events;

public class Events
{
    [Serializable] public class EventFadeComplete : UnityEvent<bool> { }
    [Serializable] public class UIStateChange : UnityEvent<bool> { }
    [Serializable] public class PlayerChange : UnityEvent<Player> { }
    [Serializable] public class ItemEvent : UnityEvent<Loot> { }
    [Serializable] public class CharacterEvent : UnityEvent<Character> { }
    [Serializable] public class PageEvent : UnityEvent<DiaryPage> { }

    [Serializable] public class QuestEvent : UnityEvent<Quest> { }
    [Serializable] public class ObjectiveEvent : UnityEvent<Objective> { }
    [Serializable] public class LocationDiscovered : UnityEvent<Location> { }
}

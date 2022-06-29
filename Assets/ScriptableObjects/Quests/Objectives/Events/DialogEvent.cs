using System;
using UnityEngine;

[Serializable]
public static class DialogEvent
{
    public static event Action<Character> TalkingTo;

    public static void OnTalkingTo(Character obj)
    {
        TalkingTo?.Invoke(obj);
    }
}
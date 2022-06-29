using System;

[Serializable]
public static class EntityEvent
{
    public static event Action<Interactable> OnInteraction;

    public static void Interacting(Interactable interactable)
    {
        OnInteraction?.Invoke(interactable);
    }
}
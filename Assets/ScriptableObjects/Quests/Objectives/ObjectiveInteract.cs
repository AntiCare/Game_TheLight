using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Objective Interact", menuName = "ScriptableObjects/Objective/Objective Interact")]
[Serializable]
public class ObjectiveInteract : Objective
{
    [SerializeField] private int interactableId;
    [SerializeField] private Quest     belongsToQuest;

    public override void StartObjective(int? param)
    {
       EntityEvent.OnInteraction += OnInteraction;
        _started              =  true;
        _isCompleted          =  false;
    }

    private void OnInteraction(Interactable interactable)
    {
        _isCompleted = interactable.id == interactableId;
    }

    public override Objective ObjectiveUpdate()
    {
        if (!_started)
            StartObjective(0);

        return this;
    }

    public void CompleteObjective(Interactable interactable)
    {
        if (!GameManager.Instance.GameData.activeQuests.Contains(belongsToQuest)) return;

        if (interactable.id != interactableId) return;

        _isCompleted = true;
          
        QuestEvent?.Invoke(belongsToQuest);
    }
}

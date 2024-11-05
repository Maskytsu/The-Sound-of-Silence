using System;
using UnityEngine;

public class GlassOfWater : Interactable
{
    public event Action OnInteract;

    [SerializeField] private QuestScriptable _drinkQuest;

    public override void Interact()
    {
        OnInteract?.Invoke();
        //could make animation in which water disapears from glass instead of whole glass
        QuestManager.Instance.EndQuest(_drinkQuest);
        Destroy(transform.parent.gameObject);
    }
}

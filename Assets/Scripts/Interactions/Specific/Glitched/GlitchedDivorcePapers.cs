using UnityEngine;

public class GlitchedDivorcePapers : GlitchedNote
{
    PlayerEquipment Equipment => PlayerObjects.Instance.PlayerEquipment;

    protected override void SetGameStateValue()
    {
        GameState.Instance.ReadDivorcePapers = true;

        if (Equipment.SpawnedItemInHand.ItemType == ItemType.PHONE)
        {
            Equipment.ChangeItem(ItemType.NONE);
        }
    }
}
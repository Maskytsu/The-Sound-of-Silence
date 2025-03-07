using FMODUnity;
using NaughtyAttributes;
using UnityEngine;

public class PickableItem : Interactable
{
    public InteractionHitbox InteractionHitbox => _interactionHitbox;
    [Space]
    [SerializeField] private bool SpawnItemTutorial = true;
    [ShowIf(nameof(SpawnItemTutorial)), SerializeField] private ItemTutorial _itemTutorialPrefab;
    public ItemType ItemType;

    private ItemTutorial _itemTutorial;

    private ItemManager ItemManager => ItemManager.Instance;

    protected override void Interact()
    {
        ItemManager.ItemsPerType[ItemType].PlayerHasIt = true;

        RuntimeManager.PlayOneShot(ItemManager.ItemsPerType[ItemType].PickingUpSound.Value);

        if (SpawnItemTutorial)
        {
            _itemTutorial = Instantiate(_itemTutorialPrefab);
            _itemTutorial.ItemType = ItemType;
        }

        Destroy(gameObject);
    }
}
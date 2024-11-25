using UnityEngine;

public class PickableItem : Interactable
{
    public InteractionHitbox InteractionHitbox => _interactionHitbox;
    [Space]
    [SerializeField] private ItemTutorial _itemTutorialPrefab;
    public ItemType ItemType;

    private ItemTutorial _itemTutorial;

    private ItemManager ItemManager => ItemManager.Instance;

    protected override void Interact()
    {
        ItemManager.ItemsPerType[ItemType].PlayerHasIt = true;

        _itemTutorial = Instantiate(_itemTutorialPrefab);
        _itemTutorial.ItemType = ItemType;

        Destroy(gameObject);
    }
}
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    public abstract ItemType ItemType { get; }
    public abstract void UseItem();
}
using NaughtyAttributes;
using System;
using UnityEngine;

public class TriggerChild : MonoBehaviour
{
    public event Action OnObjectTriggerEnter;
    public event Action OnObjectTriggerExit;

    [HideInInspector] public int Layer;
    public bool IsObjectInsideThisTrigger { get; private set; }

    private void Awake()
    {
        IsObjectInsideThisTrigger = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == Layer)
        {
            //this order matters
            OnObjectTriggerEnter?.Invoke();
            IsObjectInsideThisTrigger = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == Layer)
        {
            //this order matters
            IsObjectInsideThisTrigger = false;
            OnObjectTriggerExit?.Invoke();
        }
    }
}

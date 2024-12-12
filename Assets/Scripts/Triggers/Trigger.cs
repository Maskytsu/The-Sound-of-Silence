using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class Trigger : MonoBehaviour
{
    public event Action OnObjectTriggerEnter;
    public event Action OnObjectTriggerExit;

    [Layer, SerializeField] private int _layer;
    [SerializeField] private List<TriggerChild> _triggerChildren = new();
    [Space]
    [SerializeField] private Color _gizmoColor;
    [SerializeField] private bool _drawWireCube = true;

    private bool _isObjectInsideThisTrigger = false;

    private void Start()
    {
        foreach (TriggerChild child in _triggerChildren)
        {
            child.Layer = _layer;
            child.OnObjectTriggerEnter += TryInvokeEnter;
            child.OnObjectTriggerExit += TryInvokeExit;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == _layer)
        {
            //this order matters
            TryInvokeEnter();
            _isObjectInsideThisTrigger = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == _layer)
        {
            //this order matters
            _isObjectInsideThisTrigger = false;
            TryInvokeExit();
        }
    }

    private void TryInvokeEnter()
    {
        if (CheckIfObjectInsideTriggerFamily()) return;

        OnObjectTriggerEnter?.Invoke();
    }

    private void TryInvokeExit()
    {
        if (CheckIfObjectInsideTriggerFamily()) return;

        OnObjectTriggerExit?.Invoke();
    }

    private bool CheckIfObjectInsideTriggerFamily()
    {
        if (_isObjectInsideThisTrigger) return true;

        foreach (TriggerChild child in _triggerChildren)
        {
            if (child.IsObjectInsideThisTrigger) return true;
        }

        return false;
    }

    private void OnDrawGizmos()
    {
        BoxCollider boxTrigger = GetComponent<BoxCollider>();

        Gizmos.color = _gizmoColor;
        Matrix4x4 oldMatrix = Gizmos.matrix;
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);

        if (_drawWireCube) Gizmos.DrawWireCube(boxTrigger.center, boxTrigger.size);
        else Gizmos.DrawCube(boxTrigger.center, boxTrigger.size);
        Gizmos.matrix = oldMatrix;

        foreach (TriggerChild child in _triggerChildren)
        {
            BoxCollider childBoxTrigger = child.GetComponent<BoxCollider>();

            Gizmos.color = _gizmoColor;
            Matrix4x4 childOldMatrix = Gizmos.matrix;
            Gizmos.matrix = Matrix4x4.TRS(child.transform.position, child.transform.rotation, child.transform.lossyScale);

            if (_drawWireCube) Gizmos.DrawWireCube(childBoxTrigger.center, childBoxTrigger.size);
            else Gizmos.DrawCube(childBoxTrigger.center, childBoxTrigger.size);
            Gizmos.matrix = childOldMatrix;
        }
    }
}

using NaughtyAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Trigger : MonoBehaviour
{
    public event Action OnObjectTriggerEnter;
    public event Action OnObjectTriggerExit;

    [Layer, SerializeField] private int _layer;
    [SerializeField] private TriggerChild _childPrefab;
    [SerializeField] private List<TriggerChild> _triggerChildren = new();
    [Space]
    [SerializeField] private Color _gizmoColor;
    [Space]
    [SerializeField] private UnityEvent OnObjectTriggerEnterUE;
    [SerializeField] private UnityEvent OnObjectTriggerExitUE;

    private bool _isObjectInsideThisTrigger = false;
    private IEnumerable<TriggerChild> TriggerChildren => _triggerChildren.Where(child => child != null);

    private void Start()
    {
        foreach (TriggerChild child in TriggerChildren)
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

        OnObjectTriggerEnterUE?.Invoke();
        OnObjectTriggerEnter?.Invoke();
    }

    private void TryInvokeExit()
    {
        if (CheckIfObjectInsideTriggerFamily()) return;

        OnObjectTriggerExitUE?.Invoke();
        OnObjectTriggerExit?.Invoke();
    }

    private bool CheckIfObjectInsideTriggerFamily()
    {
        if (_isObjectInsideThisTrigger) return true;

        foreach (TriggerChild child in TriggerChildren)
        {
            if (child.IsObjectInsideThisTrigger) return true;
        }

        return false;
    }

    [Button]
    private void AddTriggerChild()
    {
        var child = Instantiate(_childPrefab, transform.position, transform.rotation, transform);
        _triggerChildren.Add(child);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!SceneViewGizmoSettings.DrawTriggers) return;

        BoxCollider boxTrigger = GetComponent<BoxCollider>();

        Gizmos.color = _gizmoColor;
        Matrix4x4 oldMatrix = Gizmos.matrix;
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);

        if (SceneViewGizmoSettings.DrawFullTriggers) Gizmos.DrawCube(boxTrigger.center, boxTrigger.size);
        else Gizmos.DrawWireCube(boxTrigger.center, boxTrigger.size);
        Gizmos.matrix = oldMatrix;

        foreach (TriggerChild child in TriggerChildren)
        {
            BoxCollider childBoxTrigger = child.GetComponent<BoxCollider>();

            Gizmos.color = _gizmoColor;
            Matrix4x4 childOldMatrix = Gizmos.matrix;
            Gizmos.matrix = Matrix4x4.TRS(child.transform.position, child.transform.rotation, child.transform.lossyScale);

            if (SceneViewGizmoSettings.DrawFullTriggers) Gizmos.DrawCube(childBoxTrigger.center, childBoxTrigger.size);
            else Gizmos.DrawWireCube(childBoxTrigger.center, childBoxTrigger.size);
            Gizmos.matrix = childOldMatrix;
        }
    }
#endif
}
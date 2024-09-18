using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Unlockable : MonoBehaviour
{
    [SerializeField] private Canvas _promptLocked;
    [Layer, SerializeField] protected int _interactableLayer;

    private PlayerEquipment _playerEquipment;
    protected bool _locked = true;

    private void Start()
    {
        _playerEquipment = PlayerManager.Instance.PlayerEquipment;
    }

    public void ShowPrompt()
    {
        if (_locked)
        {
            _promptLocked.enabled = true;
        }
    }

    public void HidePrompt()
    {
         _promptLocked.enabled = false;
    }

    public abstract void Unlock();
}

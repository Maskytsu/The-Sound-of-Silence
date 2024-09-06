using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Unlockable : MonoBehaviour
{
    [SerializeField] private GameObject _prompt;
    [SerializeField] protected LayerMask _interactableMask;

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
            _prompt.SetActive(true);
        }
    }

    public void HidePrompt()
    {
         _prompt.SetActive(false);
    }

    public abstract void Unlock();
}

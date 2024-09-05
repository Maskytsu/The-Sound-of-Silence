using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Unlockable : MonoBehaviour
{
    [SerializeField] private GameObject _promptNoKeys;
    [SerializeField] private GameObject _promptHaveKeys;

    private PlayerEquipment _playerEquipment;

    private void Start()
    {
        _playerEquipment = PlayerManager.Instance.PlayerEquipment;
    }

    public void ShowPrompt()
    {
        if (_playerEquipment.HaveKeys)
        {
            _promptHaveKeys.SetActive(true);
        }   
        else
        {
            _promptNoKeys.SetActive(true);
        }
    }

    public void HidePrompt()
    {
         _promptNoKeys.SetActive(false);
         _promptHaveKeys.SetActive(false);
    }

    public abstract void Unlock();
}

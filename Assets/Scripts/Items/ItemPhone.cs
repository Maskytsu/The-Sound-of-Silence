using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPhone : Item
{
    [SerializeField] private Canvas _phoneCanvas;

    private bool _phoneOpened = false;
    private Camera _phoneInteractCamera;
    private InputProvider _inputProvider;
    private GameObject _middlePointer;

    private void Start()
    {
        _inputProvider = InputProvider.Instance;
        _phoneInteractCamera = PlayerManager.Instance.PhoneInteractCamera;
        _phoneCanvas.worldCamera = _phoneInteractCamera;
        _middlePointer = HUD.Instance.MiddlePointer;
    }

    private void Update()
    {
        if (_phoneOpened && 
           (_inputProvider.UICustomMap.RightClick.WasPerformedThisFrame() ||
           _inputProvider.UICustomMap.Cancel.WasPerformedThisFrame()))
        {
            StartCoroutine(ClosePhone());
        }
    }

    public override void UseItem()
    {
        StartCoroutine(OpenPhone());
    }

    private IEnumerator OpenPhone()
    {
        _middlePointer.SetActive(false);
        _phoneInteractCamera.gameObject.SetActive(true);
        transform.localPosition = new Vector3(0f, -0.1f, 0.275f);
        transform.localRotation = Quaternion.Euler(-60f, 0f, 0f);

        yield return new WaitForSeconds(0);
        _phoneOpened = true;
        _inputProvider.TurnOffPlayerMaps();
        _inputProvider.UnlockCursor();
    }

    private IEnumerator ClosePhone()
    {
        _middlePointer.SetActive(true);
        _phoneInteractCamera.gameObject.SetActive(false);
        transform.localPosition = new Vector3(0.35f, -0.25f, 0.5f);
        transform.localRotation = Quaternion.identity;

        yield return new WaitForSeconds(0);
        _phoneOpened = false;
        _inputProvider.TurnOnPlayerMaps();
        _inputProvider.LockCursor();
    }
}

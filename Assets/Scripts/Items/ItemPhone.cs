using System.Collections;
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
        _phoneInteractCamera = CameraManager.Instance.PhoneInteractCamera;
        _phoneCanvas.worldCamera = _phoneInteractCamera;
        _middlePointer = HUD.Instance.MiddlePointer;
    }

    private void Update()
    {
        if (_phoneOpened && 
           (_inputProvider.UIMap.RightClick.WasPerformedThisFrame() ||
           _inputProvider.UIMap.Cancel.WasPerformedThisFrame()))
        {
            ClosePhone();
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

        _inputProvider.SaveMapStates();
        _inputProvider.TurnOffGameplayMaps();
        _inputProvider.UnlockCursor();

        yield return null;
        _phoneOpened = true;
    }

    private void ClosePhone()
    {
        if (_phoneOpened) 
        {
            _middlePointer.SetActive(true);
            _phoneInteractCamera.gameObject.SetActive(false);
            transform.localPosition = new Vector3(0.35f, -0.25f, 0.5f);
            transform.localRotation = Quaternion.identity;

            _phoneOpened = false;
            _inputProvider.LoadMapStatesAndApplyThem();
            _inputProvider.LockCursor();
        }
    }
}
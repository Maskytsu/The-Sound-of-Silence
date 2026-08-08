using System.Collections;
using UnityEngine;

public class ItemPhone : Item
{
    public override ItemType ItemType => ItemType.PHONE;

    [SerializeField] private Canvas _phoneCanvas;
    [SerializeField] private PhoneScreen _phoneScreen;
    [Header("Opened")]
    [SerializeField] private Vector3 _openedPosition = new Vector3(0f, -0.1f, 0.275f);
    [SerializeField] private Vector3 _openedRotation = new Vector3(-60f, 0f, 0f);
    [Header("In Hand")]
    [SerializeField] private Vector3 _inHandPosition = new Vector3(0.35f, -0.25f, 0.5f);
    [SerializeField] private Vector3 _inHandRotation = new Vector3(0f, 0f, 0f);
    [Header("Flashlight")]
    [SerializeField] private GameObject _flashlight;
    [SerializeField] private Vector3 _flashlightPosition = new Vector3(0.35f, -0.25f, 0.5f);
    [SerializeField] private Vector3 _flashlightRotation = new Vector3(-60f, 0f, 0f);

    private bool _phoneOpened = false;
    private bool _isFlashlightOn = false;

    private Camera _phoneInteractCamera;
    private InputProvider _inputProvider;
    private GameObject _middlePointer;

    private void Start()
    {
        _phoneScreen.OnFlashlightToggled += FlashlightToggled;
        _flashlight.SetActive(_isFlashlightOn);

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

    private void OnDestroy()
    {
        _phoneScreen.OnFlashlightToggled -= FlashlightToggled;
    }

    public override void UseItem()
    {
        StartCoroutine(OpenPhone());
    }

    private IEnumerator OpenPhone()
    {
        _middlePointer.SetActive(false);
        _phoneInteractCamera.gameObject.SetActive(true);
        transform.localPosition = _openedPosition;
        transform.localRotation = Quaternion.Euler(_openedRotation);

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

            var postion = _isFlashlightOn ? _flashlightPosition : _inHandPosition;
            var rotation = _isFlashlightOn ? _flashlightRotation : _inHandRotation;
            transform.localPosition = postion;
            transform.localRotation = Quaternion.Euler(rotation);

            _phoneOpened = false;
            _inputProvider.LoadMapStatesAndApplyThem();
            _inputProvider.LockCursor();
        }
    }

    private void FlashlightToggled()
    {
        _isFlashlightOn = !_isFlashlightOn;
        _flashlight.SetActive(_isFlashlightOn);
    }
}
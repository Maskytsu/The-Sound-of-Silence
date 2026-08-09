using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class CustomCursorManager : MonoBehaviour
{
    [SerializeField] private InputProvider _inputProvider;
    [SerializeField] private RectTransform _cursorPosition;
    [SerializeField] private RectTransform _cursorCanvas;

    private bool _isVisible;
    private Vector2 _mousePosition;

    private void Start()
    {
        Cursor.visible = false;
        _inputProvider.UIMap.Point.performed += UpdateMousePosition;
        _inputProvider.UIMap.Point.performed += SyncCustomPointerPosition;
        _inputProvider.UIMap.LeftClick.performed += BumpCursor;
    }

    private void OnDestroy()
    {
        _inputProvider.UIMap.Point.performed -= UpdateMousePosition;
        _inputProvider.UIMap.Point.performed -= SyncCustomPointerPosition;
        _inputProvider.UIMap.LeftClick.performed -= BumpCursor;
    }

    public void SetVisibility(bool visible)
    {
        _isVisible = visible;
        if (_cursorPosition.gameObject.activeSelf != _isVisible) {
            _cursorPosition.gameObject.SetActive(_isVisible);
            SyncCustomPointerPosition(new());
        }
    }

    private void UpdateMousePosition(InputAction.CallbackContext ctx)
    {
        _mousePosition = ctx.ReadValue<Vector2>();
    }

    private void SyncCustomPointerPosition(InputAction.CallbackContext ctx)
    {
        if (!_isVisible) return;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(_cursorCanvas, _mousePosition, null, out var localPoint))
        {
            _cursorPosition.anchoredPosition = localPoint;
        }
    }

    private void BumpCursor(InputAction.CallbackContext ctx)
    {
        if (!_isVisible) return;

        _cursorPosition.DOPunchScale(Vector2.one * 0.5f, 0.2f, 0).SetUpdate(true);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    public Unlockable PointedUnlockable { get; private set; }

    [SerializeField] private float _interactionRange = 2f;
    [SerializeField] private LayerMask _interactableMask;    
    [SerializeField] private LayerMask _unlockableMask;

    private PlayerInputActions _playerInputActions;

    private Transform _mainCamera;
    private Interactable _pointedInteractable;

    private void Start()
    {
        _playerInputActions = PlayerManager.Instance.PlayerInputProvider.PlayerInputActions;
        _mainCamera = PlayerManager.Instance.MainCamera.transform;
    }

    private void Update()
    {
        PointInteractableObject();
        PointUnlockableObject();
        ManageInteractionInput();
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawRay(_mainCamera.position, _mainCamera.forward * _interactionRange);
        }
    }

    private void PointUnlockableObject()
    {
        RaycastHit hitInfo;
        if (Physics.Raycast(_mainCamera.position, _mainCamera.forward, out hitInfo, _interactionRange))
        {
            if (1 << hitInfo.transform.gameObject.layer == _unlockableMask)
            {
                if (PointedUnlockable == null)
                {
                    PointedUnlockable = hitInfo.transform.gameObject.GetComponent<Unlockable>();
                    PointedUnlockable.ShowPrompt();
                }
            }
            else
            {
                if (PointedUnlockable != null)
                {
                    PointedUnlockable.HidePrompt();
                    PointedUnlockable = null;
                }
            }
        }
        else
        {
            if (PointedUnlockable != null)
            {
                PointedUnlockable.HidePrompt();
                PointedUnlockable = null;
            }
        }
    }
    
    private void PointInteractableObject()
    {
        RaycastHit hitInfo;
        if (Physics.Raycast(_mainCamera.position, _mainCamera.forward, out hitInfo, _interactionRange))
        {
            if (1 << hitInfo.transform.gameObject.layer == _interactableMask)
            {
                if (_pointedInteractable == null)
                {
                    _pointedInteractable = hitInfo.transform.gameObject.GetComponent<Interactable>();
                    _pointedInteractable.ShowPrompt();
                }
            }
            else
            {
                if (_pointedInteractable != null)
                {
                    _pointedInteractable.HidePrompt();
                    _pointedInteractable = null;
                }
            }
        }
        else
        {
            if (_pointedInteractable != null)
            {
                _pointedInteractable.HidePrompt();
                _pointedInteractable = null;
            }
        }
    }

    private void ManageInteractionInput()
    {
        if (_playerInputActions.PlayerMap.Interact.WasPerformedThisFrame() && _pointedInteractable != null)
        {
            _pointedInteractable.Interact();
        }
    }
}

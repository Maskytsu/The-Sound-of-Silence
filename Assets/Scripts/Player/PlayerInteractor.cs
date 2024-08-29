using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    [SerializeField] private float _interactionRange = 2f;
    [SerializeField] private LayerMask _interactableMask;

    private Transform _mainCameraTransform;
    private PlayerInputActions _playerInputActions;
    private GameObject _pointedInteractable = null;

    private void Awake()
    {
        _mainCameraTransform = PlayerManager.Instance.MainCamera.transform;
        _playerInputActions = new PlayerInputActions();
    }

    private void Update()
    {
        PointInteractableObject();
        ManageInteractionInput();
    }

    private void PointInteractableObject()
    {
        RaycastHit hitInfo;
        if (Physics.Raycast(_mainCameraTransform.position, _mainCameraTransform.forward, out hitInfo, _interactionRange))
        {
            if (1 << hitInfo.transform.gameObject.layer == _interactableMask)
            {
                if (_pointedInteractable == null)
                {
                    _pointedInteractable = hitInfo.transform.gameObject;
                    _pointedInteractable.GetComponent<Interactable>().ShowPrompt();
                }
            }
            else
            {
                if (_pointedInteractable != null)
                {
                    _pointedInteractable.GetComponent<Interactable>().HidePrompt();
                    _pointedInteractable = null;
                }
            }
        }
        else
        {
            if (_pointedInteractable != null)
            {
                _pointedInteractable.GetComponent<Interactable>().HidePrompt();
                _pointedInteractable = null;
            }
        }
    }

    private void ManageInteractionInput()
    {
        if (_playerInputActions.PlayerMap.Interact.WasPerformedThisFrame() && _pointedInteractable != null)
        {
            _pointedInteractable.GetComponent<Interactable>().Interact();
        }
    }

    private void OnEnable()
    {
        _playerInputActions.PlayerMap.Enable();
    }

    private void OnDisable()
    {
        _playerInputActions.PlayerMap.Disable();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawRay(_mainCameraTransform.position, _mainCameraTransform.forward * _interactionRange);
    }
}

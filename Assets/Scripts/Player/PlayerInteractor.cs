using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    [SerializeField] private float _interactionRange = 2f;
    [SerializeField] private LayerMask _interactableMask;

    private PlayerInputActions _playerInputActions;

    private Transform _mainCamera;
    private GameObject _pointedInteractable = null;

    private void Awake()
    {
        _playerInputActions = new PlayerInputActions();
    }

    private void Start()
    {
        _mainCamera = PlayerManager.Instance.MainCamera.transform;
    }

    private void Update()
    {
        PointInteractableObject();
        ManageInteractionInput();
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
        Gizmos.DrawRay(_mainCamera.position, _mainCamera.forward * _interactionRange);
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
}

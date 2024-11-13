using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    public InteractionHitbox PointedInteractable { get; private set; }
    public InteractionHitbox PointedUnlockable { get; private set; }

    [SerializeField] private Transform _playerCamera;
    [Space]
    [SerializeField] private float _interactionRange = 2f;
    [Layer, SerializeField] private int _interactableLayer;    
    [Layer, SerializeField] private int _unlockableLayer;

    private PlayerInputActions.PlayerCameraMapActions PlayerCameraMap => InputProvider.Instance.PlayerCameraMap;

    private void Update()
    {
        ManagePointingObjects();
        ManageInteractionInput();
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(_playerCamera.position, _playerCamera.forward * _interactionRange);
        }
    }
    
    private void ManagePointingObjects()
    {
        if (Physics.Raycast(_playerCamera.position, _playerCamera.forward, out RaycastHit hit, _interactionRange))
        {
            InteractionHitbox hitbox = hit.transform.GetComponent<InteractionHitbox>();

            if (hit.transform.gameObject.layer == _interactableLayer)
            {
                if (PointedInteractable != hitbox)
                {
                    PointedInteractable = hitbox;
                    PointedInteractable.OnPointed?.Invoke();
                }
            }
            else if (hit.transform.gameObject.layer == _unlockableLayer)
            {
                if (PointedUnlockable != hitbox)
                {
                    PointedUnlockable = hitbox;
                    PointedUnlockable.OnPointed?.Invoke();
                }
            }
            else
            {
                UnpointPointedObject();
            }
        }
        else
        {
            UnpointPointedObject();
        }
    }

    private void UnpointPointedObject()
    {
        if (PointedInteractable != null)
        {
            PointedInteractable.OnUnpointed?.Invoke();
            PointedInteractable = null;
        }

        if (PointedUnlockable != null)
        {
            PointedUnlockable.OnUnpointed?.Invoke();
            PointedUnlockable = null;
        }
    }

    private void ManageInteractionInput()
    {
        if (PlayerCameraMap.Interact.WasPerformedThisFrame() && PointedInteractable != null)
        {
            PointedInteractable.OnInteract?.Invoke();
        }
    }
}

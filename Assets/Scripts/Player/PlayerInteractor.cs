using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    public Unlockable PointedUnlockable { get; private set; }
    public Interactable PointedInteractable { get; private set; }

    [SerializeField] private float _interactionRange = 2f;
    [Layer, SerializeField] private int _interactableLayer;    
    [Layer, SerializeField] private int _unlockableLayer;

    private Transform _mainCamera;
    private PlayerInputActions.PlayerMainMapActions PlayerMainMap => InputProvider.Instance.PlayerMainMap;

    private void Start()
    {
        _mainCamera = PlayerManager.Instance.VirtualMainCamera.transform;
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
            if (hitInfo.transform.gameObject.layer == _unlockableLayer)
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
            if (hitInfo.transform.gameObject.layer == _interactableLayer)
            {
                if (PointedInteractable == null)
                {
                    PointedInteractable = hitInfo.transform.gameObject.GetComponent<Interactable>();
                    PointedInteractable.ShowPrompt();
                }
            }
            else
            {
                if (PointedInteractable != null)
                {
                    PointedInteractable.HidePrompt();
                    PointedInteractable = null;
                }
            }
        }
        else
        {
            if (PointedInteractable != null)
            {
                PointedInteractable.HidePrompt();
                PointedInteractable = null;
            }
        }
    }

    private void ManageInteractionInput()
    {
        if (PlayerMainMap.Interact.WasPerformedThisFrame() && PointedInteractable != null)
        {
            PointedInteractable.Interact();
        }
    }
}

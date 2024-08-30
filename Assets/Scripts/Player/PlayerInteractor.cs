using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    [SerializeField] private float interactionRange = 2f;
    [SerializeField] private LayerMask interactableMask;

    private Transform mainCamera;
    private PlayerInputActions playerInputActions;
    private GameObject pointedInteractable = null;

    private void Awake()
    {
        mainCamera = PlayerManager.Instance.MainCamera.transform;
        playerInputActions = new PlayerInputActions();
    }

    private void Update()
    {
        PointInteractableObject();
        ManageInteractionInput();
    }

    private void PointInteractableObject()
    {
        RaycastHit hitInfo;
        if (Physics.Raycast(mainCamera.position, mainCamera.forward, out hitInfo, interactionRange))
        {
            if (1 << hitInfo.transform.gameObject.layer == interactableMask)
            {
                if (pointedInteractable == null)
                {
                    pointedInteractable = hitInfo.transform.gameObject;
                    pointedInteractable.GetComponent<Interactable>().ShowPrompt();
                }
            }
            else
            {
                if (pointedInteractable != null)
                {
                    pointedInteractable.GetComponent<Interactable>().HidePrompt();
                    pointedInteractable = null;
                }
            }
        }
        else
        {
            if (pointedInteractable != null)
            {
                pointedInteractable.GetComponent<Interactable>().HidePrompt();
                pointedInteractable = null;
            }
        }
    }

    private void ManageInteractionInput()
    {
        if (playerInputActions.PlayerMap.Interact.WasPerformedThisFrame() && pointedInteractable != null)
        {
            pointedInteractable.GetComponent<Interactable>().Interact();
        }
    }

    private void OnEnable()
    {
        playerInputActions.PlayerMap.Enable();
    }

    private void OnDisable()
    {
        playerInputActions.PlayerMap.Disable();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawRay(mainCamera.position, mainCamera.forward * interactionRange);
    }
}

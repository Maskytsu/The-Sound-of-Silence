using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    [SerializeField] private Transform mainCamera;
    [SerializeField] private float interactionRange = 2f;
    [SerializeField] private LayerMask interactableMask;

    [Header("------Public Parameters------")]
    public GameObject pointedInteractable = null;

    private void Update()
    {
        RaycastHit hitInfo;
        if(Physics.Raycast(mainCamera.position, mainCamera.forward, out hitInfo, interactionRange))
        {
            if(1 << hitInfo.transform.gameObject.layer == interactableMask)
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
            if(pointedInteractable != null)
            {
                pointedInteractable.GetComponent<Interactable>().HidePrompt();
                pointedInteractable = null;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(mainCamera.position, mainCamera.forward * interactionRange);
    }
}

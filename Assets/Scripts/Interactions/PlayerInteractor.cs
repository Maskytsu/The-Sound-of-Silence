using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    [SerializeField] private Transform mainCameraPosition;
    [SerializeField] private float interactionRange = 2f;
    [SerializeField] private LayerMask intractableMask;

    [Header("------Public Parameters------")]
    public GameObject pointedInteractable = null;

    private void Update()
    {
        RaycastHit hitInfo;
        if(Physics.Raycast(mainCameraPosition.position, mainCameraPosition.forward, out hitInfo, interactionRange, intractableMask))
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(mainCameraPosition.position, mainCameraPosition.forward * interactionRange);
    }
}

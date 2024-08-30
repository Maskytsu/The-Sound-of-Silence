using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionPrompt : MonoBehaviour
{
    [SerializeField] Transform mainCameraPosition;
    void Update()
    {
        transform.forward = mainCameraPosition.forward;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivatePixelatedView : MonoBehaviour
{
    [SerializeField] private GameObject pixelatedView;
    [SerializeField] bool activatePixelatedView = true;
    private void Awake()
    {
        if (activatePixelatedView) pixelatedView.SetActive(true);
    }
}

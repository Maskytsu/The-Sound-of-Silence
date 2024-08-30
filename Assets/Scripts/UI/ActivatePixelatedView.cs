using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivatePixelatedView : MonoBehaviour
{
    [SerializeField] private GameObject pixelatedView;
    [SerializeField] private bool activatePixelatedView = true;
    private void Awake()
    {
        if (activatePixelatedView) pixelatedView.SetActive(true);
    }
}

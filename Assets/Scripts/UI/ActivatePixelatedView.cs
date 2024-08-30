using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivatePixelatedView : MonoBehaviour
{
    [SerializeField] private GameObject _pixelatedView;
    [SerializeField] private bool _activatePixelatedView = true;
    private void Awake()
    {
        if (_activatePixelatedView) _pixelatedView.SetActive(true);
    }
}

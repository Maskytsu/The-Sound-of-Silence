using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivatePixelatedView : MonoBehaviour
{
    [SerializeField] private GameObject pixelatedView;
    private void Awake()
    {
        pixelatedView.SetActive(true);
    }
}

using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Window : Interactable
{
    [Space]
    [SerializeField] private Transform _windowPartLeft;
    [SerializeField] private Transform _windowPartRight;
    [SerializeField] private float _leftPartRotationY = 72.7f;
    [SerializeField] private float _rightPartRotationY = -57.52f;

    protected override void Interact()
    {
        CloseWindow();
    }

    public void OpenWindow()
    {
        _windowPartLeft.DOLocalRotate(new Vector3(0, _leftPartRotationY, 0), 1.5f);
        _windowPartRight.DOLocalRotate(new Vector3(0, _rightPartRotationY, 0), 1.5f);
        InteractionHitbox.gameObject.SetActive(true);
    }

    private void CloseWindow()
    {
        HidePrompt();
        InteractionHitbox.gameObject.SetActive(false);

        _windowPartLeft.DOLocalRotate(Vector3.zero, 1.5f);
        _windowPartRight.DOLocalRotate(Vector3.zero, 1.5f);
    }
}

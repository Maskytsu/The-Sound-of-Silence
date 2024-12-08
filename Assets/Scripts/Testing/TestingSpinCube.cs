using DG.Tweening;
using UnityEngine;

public class TestingSpinCube : MonoBehaviour
{
    [SerializeField] private Transform _transform;

    private void Start()
    {
        _transform.DOLocalRotate(new Vector3(0, 360, 0), 1f, RotateMode.FastBeyond360).
            SetRelative(true).SetEase(Ease.Linear).SetLoops(-1);
    }
}
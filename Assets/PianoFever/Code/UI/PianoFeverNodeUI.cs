using DG.Tweening;
using UnityEngine;

public class PianoFeverNodeUI : MonoBehaviour
{
    public void Setup(float fallingDuration, Transform nodeTargetPosition)
    {
        var tween = transform.DOMoveY(nodeTargetPosition.position.y, fallingDuration).SetEase(Ease.Linear).SetUpdate(true);
        tween.onComplete += () => Destroy(gameObject);
    }
}
using UnityEngine;

public class EnableComponentOnAwake : MonoBehaviour
{
    [SerializeField] private Behaviour _component;
    private void Awake()
    {
        _component.enabled = true;
    }
}
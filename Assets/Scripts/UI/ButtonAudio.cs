using FMODUnity;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonAudio : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] private EventReference _hover;
    [SerializeField] private EventReference _click;
    private Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();

        if (!_click.IsNull)
        {
            _button.onClick.AddListener(() => RuntimeManager.PlayOneShot(_click));
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!_hover.IsNull) RuntimeManager.PlayOneShot(_hover);
    }
}

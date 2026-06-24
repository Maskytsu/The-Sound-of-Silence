using FMODUnity;
using UnityEngine;
using UnityEngine.Events;

public class ItemFlashlight : Item
{
    public override ItemType ItemType => ItemType.FLASHLIGHT;

    public GameObject LightCone;
    [SerializeField] private UnityEvent OnFlashlightOn = new();
    [SerializeField] private UnityEvent OnFlashlightOff = new();

    public override void UseItem()
    {
        //turn on or off flashlight
        if (!LightCone.activeSelf)
        {
            RuntimeManager.PlayOneShot(FmodEvents.Instance.FlashlightClickOn);
            OnFlashlightOn?.Invoke();
            LightCone.SetActive(true);
        }
        else
        {
            RuntimeManager.PlayOneShot(FmodEvents.Instance.FlashlightClickOff);
            OnFlashlightOff?.Invoke();
            LightCone.SetActive(false);
        }
    }
}
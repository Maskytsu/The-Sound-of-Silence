using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class FmodBuses : MonoBehaviour
{
    public static Bus Master => RuntimeManager.GetBus("bus:/");
    public static Bus Hearing => RuntimeManager.GetBus("bus:/Hearing");
    public static Bus HearingSFX => RuntimeManager.GetBus("bus:/Hearing/SFX");
    public static Bus HearingMusic => RuntimeManager.GetBus("bus:/Hearing/Music");
    public static Bus Silence => RuntimeManager.GetBus("bus:/Silence");
    public static Bus SilentSFX => RuntimeManager.GetBus("bus:/Silent/SFX");
    public static Bus UI => RuntimeManager.GetBus("bus:/UI");
}
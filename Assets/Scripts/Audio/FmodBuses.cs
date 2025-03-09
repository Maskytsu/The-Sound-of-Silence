using FMOD.Studio;
using FMODUnity;

public class FmodBuses
{
    public static Bus Master => RuntimeManager.GetBus("bus:/");
    public static Bus Ambient => RuntimeManager.GetBus("bus:/Ambience");
    public static Bus Music => RuntimeManager.GetBus("bus:/Music");
    public static Bus SFX => RuntimeManager.GetBus("bus:/SFX");
    public static Bus UI => RuntimeManager.GetBus("bus:/UI");
}
using FMOD.Studio;
using FMODUnity;

public class FmodBuses
{
    public static Bus Master => RuntimeManager.GetBus("bus:/");
}
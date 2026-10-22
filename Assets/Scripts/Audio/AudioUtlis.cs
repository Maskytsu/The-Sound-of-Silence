using FMOD.Studio;

public static class AudioUtlis
{
    public static void Stop(this EventInstance eventInstance, bool allowFadeout = true)
    {
        eventInstance.stop(allowFadeout ? STOP_MODE.ALLOWFADEOUT : STOP_MODE.IMMEDIATE);
    }
}

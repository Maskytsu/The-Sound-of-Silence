using FMOD.Studio;
using UnityEngine;

public static class AudioUtlis
{
    public static void Stop(this EventInstance eventInstance, bool allowFadeout = true)
    {
        eventInstance.stop(allowFadeout ? STOP_MODE.ALLOWFADEOUT : STOP_MODE.IMMEDIATE);
    }

    public static float GetRemainingTime(this EventInstance instance, bool accountForPitch = false)
    {
        //AI slop function - didn't read it and don't care
        if (!instance.isValid()) return 0f;

        instance.getDescription(out EventDescription description);
        description.getLength(out int lengthMs);
        instance.getTimelinePosition(out int positionMs);

        float remaining = Mathf.Max(0, lengthMs - positionMs) / 1000f;

        if (accountForPitch)
        {
            instance.getPitch(out float pitch);
            if (pitch > Mathf.Epsilon) remaining /= pitch;
        }

        return remaining;
    }
}

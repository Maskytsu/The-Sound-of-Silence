using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FmodBuses : MonoBehaviour
{
    public static Bus Master => RuntimeManager.GetBus("bus:/");
    public static Bus Music => RuntimeManager.GetBus("bus:/Music");
    public static Bus SFX => RuntimeManager.GetBus("bus:/SFX");

}

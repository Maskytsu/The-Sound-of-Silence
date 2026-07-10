using System;
using UnityEngine;

public abstract class Checkpoint : MonoBehaviour
{
    public event Action<Checkpoint> OnCheckpointReached;

    [Header("Checkpoint")]
    [SerializeField] protected PlayerTargetTransform _checkpointPosition;

    public void InvokeCheckpointReached()
    {
        Debug.Log("Checkpoint reached: " + gameObject.name);
        OnCheckpointReached?.Invoke(this);
    }

    public abstract void ResetToThisCheckpoint();
}
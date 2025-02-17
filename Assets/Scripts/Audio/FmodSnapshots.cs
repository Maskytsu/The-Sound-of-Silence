using FMODUnity;
using UnityEngine;

public class FmodSnapshots : MonoBehaviour
{
    public static FmodSnapshots Instance { get; private set; }

    [field: SerializeField] public EventReference Silence { get; private set; }

    private void Awake()
    {
        CreateInstance();
    }

    private void CreateInstance()
    {
        if (Instance != null)
        {
            Debug.LogError("Found more than one FmodSnapshots in the scene.");
        }
        Instance = this;
    }
}

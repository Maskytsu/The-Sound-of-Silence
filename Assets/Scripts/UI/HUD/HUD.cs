using UnityEngine;

public class HUD : MonoBehaviour
{
    public static HUD Instance { get; private set; }

    public QuestDisplay QuestDisplay;
    public GameObject MiddlePointer;

    private void Awake()
    {
        CreateInstance();
    }

    private void CreateInstance()
    {
        if (Instance != null)
        {
            Debug.LogError("Found more than one HUD in the scene.");
        }
        Instance = this;
    }
}
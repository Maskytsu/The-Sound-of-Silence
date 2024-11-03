using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public PhoneSetupScriptable CurrentPhoneSetup { get; private set;}

    [SerializeField] private SceneSetup _sceneSetup;

    private void Awake()
    {
        CreateInstance();
        CurrentPhoneSetup = _sceneSetup.StartingPhoneSetup;
    }

    private void OnDestroy()
    {
        if (CurrentPhoneSetup != null) CurrentPhoneSetup.ClearSubscribersFromContacts();
    }

    public void ChangePhoneSetup(PhoneSetupScriptable phoneSetup)
    {
        CurrentPhoneSetup.ClearSubscribersFromContacts();
        CurrentPhoneSetup = phoneSetup;
    }

    private void CreateInstance()
    {
        if (Instance != null)
        {
            Debug.LogError("Found more than one GameManager in the scene.");
        }
        Instance = this;
    }
}

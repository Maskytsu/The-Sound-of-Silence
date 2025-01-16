using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    [SerializeField] private SceneSetup _sceneSetup;
    [SerializeField] private GameState _gameState;
    [SerializeField] private Settings _settings;

    private void Awake()
    {
        CreateInstance();

        if (_sceneSetup.SaveSceneOnAwake)
        {
            SaveCurrentScene();
        }

        LoadGameState();
        LoadSettings();
    }

    public void SaveCurrentScene()
    {
        PlayerPrefs.SetString("SavedScene", SceneManager.GetActiveScene().name);
    }

    public void SaveGameState()
    {
        PlayerPrefs.SetInt("CheckedMechanic", _gameState.MechanicChecked ? 1 : 0);
        PlayerPrefs.SetInt("MessageSentToMechanic", _gameState.MechanicMessaged ? 1 : 0);
        PlayerPrefs.SetInt("MessageSentToClaire", _gameState.ClaireMessaged ? 1 : 0);
        PlayerPrefs.SetInt("CalledToClaire", _gameState.ClaireCalled ? 1 : 0);
        PlayerPrefs.SetInt("CheckedPolice", _gameState.PoliceChecked ? 1 : 0);
        PlayerPrefs.SetInt("CalledToPolice", _gameState.PoliceCalled ? 1 : 0);
        PlayerPrefs.SetInt("TookPills", _gameState.TookPills ? 1 : 0);
        PlayerPrefs.SetInt("ReadNewspaper", _gameState.ReadNewspaper ? 1 : 0);
        PlayerPrefs.SetInt("TookKeys", _gameState.TookKeys ? 1 : 0);
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetFloat("Volume", _settings.Volume);
        PlayerPrefs.SetFloat("Brightness", _settings.Brightness);
    }

    public void ClearSave()
    {
        //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        //!REMEMBER TO ADD HERE WHEN ADDING SOMETHING TO GAME STATE!
        //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

        PlayerPrefs.SetString("SavedScene", "");

        PlayerPrefs.SetInt("CheckedMechanic", 0);
        PlayerPrefs.SetInt("MessageSentToMechanic", 0);
        PlayerPrefs.SetInt("MessageSentToClaire", 0);
        PlayerPrefs.SetInt("CalledToClaire", 0);
        PlayerPrefs.SetInt("CheckedPolice", 0);
        PlayerPrefs.SetInt("CalledToPolice", 0);
        PlayerPrefs.SetInt("TookPills", 0);
        PlayerPrefs.SetInt("ReadNewspaper", 0);
        PlayerPrefs.SetInt("TookKeys", 0);
    }

    public void LoadSavedScene()
    {
        SceneManager.LoadScene(PlayerPrefs.GetString("SavedScene"));
    }

    public void LoadGameState()
    {
        _gameState.MechanicChecked = PlayerPrefs.GetInt("CheckedMechanic") == 1;
        _gameState.MechanicMessaged = PlayerPrefs.GetInt("MessageSentToMechanic") == 1;
        _gameState.ClaireMessaged = PlayerPrefs.GetInt("MessageSentToClaire") == 1;
        _gameState.ClaireCalled = PlayerPrefs.GetInt("CalledToClaire") == 1;
        _gameState.PoliceChecked = PlayerPrefs.GetInt("CheckedPolice") == 1;
        _gameState.PoliceCalled = PlayerPrefs.GetInt("CalledToPolice") == 1;
        _gameState.TookPills = PlayerPrefs.GetInt("TookPills") == 1;
        _gameState.ReadNewspaper = PlayerPrefs.GetInt("ReadNewspaper") == 1;
        _gameState.TookKeys = PlayerPrefs.GetInt("TookKeys") == 1;
    }

    public void LoadSettings()
    {
        _settings.Volume = PlayerPrefs.GetFloat("Volume", _settings.Volume);
        _settings.Brightness = PlayerPrefs.GetFloat("Brightness", _settings.Brightness);
    }

    private void CreateInstance()
    {
        if (Instance != null)
        {
            Debug.LogError("Found more than one SaveManager in the scene.");
        }
        Instance = this;
    }
}
using UnityEngine;

public class HumpScareNoteHandler : MonoBehaviour
{
    [SerializeField] private Door _sharonRoomDoor;
    [SerializeField] private GameObject _doorHinge;
    [SerializeField] private GameObject _brokenDoor;
    [SerializeField] private Note _humpScareNote1;
    [SerializeField] private Note _humpScareNote2;
    [SerializeField] private Scene7ResetHandler _sceneResetHandler;
    [Space]
    [SerializeField] private Vector2 _activationPlayerRotationBounds = new Vector2(-45f, 45f);
    [SerializeField] private float _activationCameraMaxXRotation = 50f;

    private bool _doorWasOpened;

    private bool _note1WasRead;
    private bool _note2WasRead;

    private bool _gotHumpScared1;
    private bool _gotHumpScared2;

    private void Start()
    {
        _brokenDoor.SetActive(false);
        if (_sceneResetHandler.SceneWasReseted)
        {
            _humpScareNote1.gameObject.SetActive(false);
            return;
        }

        _sharonRoomDoor.OnInteract += () => _doorWasOpened = true;
        _humpScareNote1.OnFirstReadingEnd += () => _note1WasRead = true;
        _humpScareNote2.OnFirstReadingEnd += () => _note2WasRead = true;
    }

    private void Update()
    {
        if (_doorWasOpened) return;

        if (!_gotHumpScared1 && _note1WasRead && IsTurnedBack())
        {
            _gotHumpScared1 = true;
            _sharonRoomDoor.gameObject.SetActive(false);
            _doorHinge.SetActive(false);
            _brokenDoor.SetActive(true);
        }

        if (!_gotHumpScared2 && _note2WasRead && IsTurnedBack())
        {
            _gotHumpScared2 = true;
            _brokenDoor.SetActive(false);
        }
    }

    private bool IsTurnedBack()
    {
        var playerRotation = PlayerObjects.Instance.Player.transform.rotation.eulerAngles.y;
        var playerRotationInBounds = playerRotation > 360f + _activationPlayerRotationBounds.x || playerRotation < _activationPlayerRotationBounds.y;
        if (!playerRotationInBounds) return false;

        var cameraRotation = PlayerObjects.Instance.PlayerVirtualCamera.transform.rotation.eulerAngles.x;
        var cameraRotationInBounds = cameraRotation < _activationCameraMaxXRotation;
        if (!cameraRotationInBounds) return false;

        return true;
    }
}

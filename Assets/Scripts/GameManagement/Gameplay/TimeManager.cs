using UnityEngine;
using UnityEngine.InputSystem;

public class TimeManager : SingletonMonobehaviour<TimeManager>
{
    private const float BASE_TIME_SCALE = 1.0f;
    private const float ULTRA_TIME_SCALE = 50.0f;

    private bool _isDebugUltraTimeScaleOn = false;

    private float BaseTimeScale => _isDebugUltraTimeScaleOn ? ULTRA_TIME_SCALE : BASE_TIME_SCALE;

    private void Start()
    {
        InputProvider.Instance.DebugMap.ToggleUltraTimeScale.performed += ToggleUltraTimeScale;
    }

    private void OnDestroy()
    {
        InputProvider.Instance.DebugMap.ToggleUltraTimeScale.performed -= ToggleUltraTimeScale;
    }

    public void SetTimeScale(float timeScale)
    {
        Time.timeScale = timeScale;
    }

    public void PauseTimeScale()
    {
        Time.timeScale = 0.0f;
    }

    public void ResetTimeScale()
    {
        Time.timeScale = BaseTimeScale;
    }

    private void ToggleUltraTimeScale(InputAction.CallbackContext context)
    {
        _isDebugUltraTimeScaleOn = !_isDebugUltraTimeScaleOn;
        Debug.LogWarning("Debug ultra time scale state: " + _isDebugUltraTimeScaleOn);

        if (Time.timeScale == BASE_TIME_SCALE || Time.timeScale == ULTRA_TIME_SCALE)
        {
            Time.timeScale = BaseTimeScale;
        }
    }
}
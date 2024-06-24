using UnityEngine;

public class SkateCamSetup : MonoBehaviour
{
    [SerializeField] private Camera _camera;

    private void Awake()
    {
        Vector2 baseResolution = GameSettingsManager.GameSettings._baseScreenResolution;

        float targetAspect = baseResolution.x / baseResolution.y;
        float windowAspect = (float)Screen.width / (float)Screen.height;
        float scaleHeight = windowAspect / targetAspect;

        float totalScale = 0.15f;

        // Ensure the camera adjusts to fit height and expands width
        if (scaleHeight >= 1.0f)
        {
            _camera.orthographicSize = 360.0f * totalScale; // Half of 720 to fit the height
        }
        else
        {
            _camera.orthographicSize = (360.0f / scaleHeight) * totalScale;
        }
    }

}

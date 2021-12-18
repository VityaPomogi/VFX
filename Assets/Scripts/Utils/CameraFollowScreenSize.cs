using UnityEngine;

[RequireComponent( typeof( Camera ) )]
public class CameraFollowScreenSize : MonoBehaviour
{
    private Vector2 targetAspectRatio = new Vector2( 16.0f, 9.0f );

    private Camera thisCamera = null;
    private float currentCameraSize = 0.0f;
    private float targetAspectRatioInFloat = 0.0f;

    void Awake()
    {
        thisCamera = this.GetComponent<Camera>();
        currentCameraSize = thisCamera.orthographicSize;
        targetAspectRatioInFloat = targetAspectRatio.x / targetAspectRatio.y;

        UpdateSettings();
    }

    public void UpdateSettings()
    {
        float _currentAspectRatioInFloat = ( float )Screen.width / ( float )Screen.height;
        if (_currentAspectRatioInFloat < targetAspectRatioInFloat)
        {
            thisCamera.orthographicSize = ( targetAspectRatioInFloat / _currentAspectRatioInFloat ) * currentCameraSize;
        }
    }
}

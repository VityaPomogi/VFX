using UnityEngine;
using UnityEngine.UI;

[RequireComponent( typeof( CanvasScaler ) )]
public class CanvasScalerAdjuster : MonoBehaviour
{
    private Vector2 targetAspectRatio = new Vector2( 16.0f, 9.0f );

    void Awake()
    {
        UpdateSettings();
    }

    public void UpdateSettings()
    {
        CanvasScaler _canvasScaler = this.GetComponent<CanvasScaler>();
        if (( float )Screen.width / ( float )Screen.height < targetAspectRatio.x / targetAspectRatio.y)
        {
            _canvasScaler.matchWidthOrHeight = 0.0f;
        }
        else
        {
            _canvasScaler.matchWidthOrHeight = 1.0f;
        }
    }
}

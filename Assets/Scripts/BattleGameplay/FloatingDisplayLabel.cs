using UnityEngine;
using TMPro;

public class FloatingDisplayLabel : MonoBehaviour
{
    [SerializeField] private TextMeshPro displayLabel;

    public void ShowLabel( string displayText, float floatingHeight, float floatingDuration, float fadeOutDuration )
    {
        displayLabel.text = displayText;

        GameObject _displayLabelObject = displayLabel.gameObject;
        float _fadeOutDelay = floatingDuration - fadeOutDuration;
        LeanTween.moveLocalY( _displayLabelObject, floatingHeight, floatingDuration ).setEase( LeanTweenType.easeOutCirc );
        LeanTween.value( _displayLabelObject, 1.0f, 0.0f, fadeOutDuration ).setDelay( _fadeOutDelay ).setOnUpdate( UpdateDisplayLabelAlphaValue ).setOnComplete( DestroyThis );
    }

    private void UpdateDisplayLabelAlphaValue( float alphaValue )
    {
        displayLabel.alpha = alphaValue;
    }

    public void DestroyThis()
    {
        Destroy( this.gameObject );
    }

    public TextMeshPro GetDisplayLabel()
    {
        return displayLabel;
    }
}

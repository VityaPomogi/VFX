using UnityEngine;
using TMPro;

public class FloatingDisplayObject : MonoBehaviour
{
    [SerializeField] private SpriteRenderer[] spriteRendererObjects = new SpriteRenderer[ 0 ];
    [SerializeField] private TextMeshPro[] textMeshProObjects = new TextMeshPro[ 0 ];

    public void Show( float floatingHeight, float floatingDuration, float fadeOutDuration )
    {
        GameObject _displayLabelObject = this.gameObject;
        float _fadeOutDelay = floatingDuration - fadeOutDuration;
        LeanTween.moveLocalY( this.gameObject, floatingHeight, floatingDuration ).setEase( LeanTweenType.easeOutCirc );
        LeanTween.value( this.gameObject, 1.0f, 0.0f, fadeOutDuration ).setDelay( _fadeOutDelay ).setOnUpdate( UpdateDisplayObjectAlphaValue ).setOnComplete( DestroyThis );
    }

    private void UpdateDisplayObjectAlphaValue( float alphaValue )
    {
        if (spriteRendererObjects.Length > 0)
        {
            for (int i = 0; i < spriteRendererObjects.Length; i++)
            {
                SpriteRenderer _spriteRenderer = spriteRendererObjects[ i ];
                Color _spriteRendererColor = _spriteRenderer.color;
                _spriteRendererColor.a = alphaValue;
                _spriteRenderer.color = _spriteRendererColor;
            }
        }
        if (textMeshProObjects.Length > 0)
        {
            for (int i = 0; i < textMeshProObjects.Length; i++)
            {
                TextMeshPro _textMeshPro = textMeshProObjects[ i ];
                Color _textMeshProColor = _textMeshPro.color;
                _textMeshProColor.a = alphaValue;
                _textMeshPro.color = _textMeshProColor;
            }
        }
    }

    public void DestroyThis()
    {
        Destroy( this.gameObject );
    }
}

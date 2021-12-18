using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class MyButton : Button
{
    [Header( "UGUI" )]
    public Text buttonLabel;

    [Header( "TextMesh Pro UGUI" )]
    public TextMeshProUGUI textMeshProButtonLabel;
    public Material textMeshProButtonLabelEnabled;
    public Material textMeshProButtonLabelDisabled;

    protected override void Start()
    {
        base.Start();
        UpdateButtonColor();
    }

    private void UpdateButtonColor()
    {
        bool _isInteractable = base.IsInteractable();

        SetButtonLabelColor( ( _isInteractable == true ) ? colors.normalColor : colors.disabledColor );

        if (textMeshProButtonLabel != null)
        {
            Material _material = ( _isInteractable == true ) ? textMeshProButtonLabelEnabled : textMeshProButtonLabelDisabled;
            if (_material != null)
            {
                textMeshProButtonLabel.fontMaterial.CopyPropertiesFromMaterial( _material );
                textMeshProButtonLabel.fontMaterial.shader = _material.shader;
            }
        }
    }

    public void SetIsInteractable( bool isInteractable )
    {
        base.interactable = isInteractable;
        UpdateButtonColor();
    }

    public override void OnPointerDown( PointerEventData eventData )
    {
        if (base.interactable == true)
        {
            base.OnPointerDown( eventData );
            SetButtonLabelColor( colors.pressedColor );
        }
    }

    public override void OnPointerUp( PointerEventData eventData )
    {
        if (base.interactable == true)
        {
            base.OnPointerUp( eventData );
            SetButtonLabelColor( colors.normalColor );
        }
    }

    private void SetButtonLabelColor( Color targetColor )
    {
        if (buttonLabel != null)
        {
            buttonLabel.color = targetColor;
        }
        if (textMeshProButtonLabel != null)
        {
            textMeshProButtonLabel.color = targetColor;
        }
    }
}

using UnityEngine;
using TMPro;

public class DisplayPowerUp : MonoBehaviour
{
    [SerializeField] private SpriteRenderer iconSpriteRenderer;
    [SerializeField] private TextMeshPro displayLabel;
    [SerializeField] private FloatingDisplayObject floatingDisplayObjectRef;

    public void SetUp( Sprite iconSprite, string displayText, Color32 labelOutlineColor )
    {
        iconSpriteRenderer.sprite = iconSprite;
        displayLabel.text = displayText;
        displayLabel.outlineColor = labelOutlineColor;
    }

    public FloatingDisplayObject GetFloatingDisplayObjectRef()
    {
        return floatingDisplayObjectRef;
    }
}

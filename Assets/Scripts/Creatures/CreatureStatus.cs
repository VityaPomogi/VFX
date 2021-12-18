using UnityEngine;
using TMPro;

public class CreatureStatus : MonoBehaviour
{
    [SerializeField] private SpriteRenderer iconSpriteRenderer;
    [SerializeField] private TextMeshPro counterLabel;
    [SerializeField] private Transform minusContainer;
    [SerializeField] private FloatingDisplayLabel floatingDisplayLabelPrefab;

    private int counterNumber = 0;

    public void SetUp( Sprite iconSprite, int counterNumber, Color32 labelOutlineColor )
    {
        iconSpriteRenderer.sprite = iconSprite;
        counterLabel.outlineColor = labelOutlineColor;
        SetCounterNumber( counterNumber );
    }

    public void SetCounterNumber( int counterNumber )
    {
        this.counterNumber = counterNumber;
        counterLabel.text = counterNumber.ToString();
    }

    public void MinusCounterNumber()
    {
        SetCounterNumber( counterNumber - 1 );

        GameObject _floatingDisplayLabelObject = Instantiate( floatingDisplayLabelPrefab.gameObject );
        _floatingDisplayLabelObject.transform.SetParent( minusContainer, false );

        FloatingDisplayLabel _floatingDisplayLabel = _floatingDisplayLabelObject.GetComponent<FloatingDisplayLabel>();
        _floatingDisplayLabel.GetDisplayLabel().outlineColor = counterLabel.outlineColor;
        _floatingDisplayLabel.ShowLabel( "-1", 0.5f, 2.0f, 1.0f );
    }
}

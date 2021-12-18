using UnityEngine;
using TMPro;

public class CreatureShieldBar : MonoBehaviour
{
    [SerializeField] private float animationTime = 0.3f;
    [SerializeField] private float rightPositionX = 0.0f;
    [SerializeField] private TextMeshPro shieldLabel;

    private float shieldNumber = 0;
    private float lastShieldNumber = 0;

    public void SetToRightSide()
    {
        Vector3 _pos = this.transform.localPosition;
        _pos.x = rightPositionX;
        this.transform.localPosition = _pos;
    }

    public void SetShieldNumber( float shieldNumber, bool hasLabelAnimation = false )
    {
        this.shieldNumber = shieldNumber;

        if (hasLabelAnimation == true)
        {
            LeanTween.value( lastShieldNumber, shieldNumber, 0.2f ).setOnUpdate( UpdateShieldLabel );
        }
        else
        {
            UpdateShieldLabel( shieldNumber );
        }

        bool _isShowing = this.gameObject.activeSelf;
        bool _needToShow = ( shieldNumber > 0 );
        if (_isShowing == false && _needToShow == true)
        {
            this.transform.localScale = Vector3.zero;
            this.gameObject.SetActive( true );
            LeanTween.scale( this.gameObject, Vector3.one, animationTime ).setEase( LeanTweenType.easeOutBack );
        }
        else if (_isShowing == true && _needToShow == false)
        {
            LeanTween.scale( this.gameObject, Vector3.zero, animationTime ).setEase( LeanTweenType.easeInBack ).setOnComplete( HideThisGameObject );
        }
        else if (_needToShow == false)
        {
            this.gameObject.SetActive( false );
        }

        lastShieldNumber = shieldNumber;
    }

    public float MinusShieldNumber( float amount )
    {
        float _remainingAmount = amount - shieldNumber;
        if (_remainingAmount > 0)
        {
            SetShieldNumber( 0, true );
        }
        else
        {
            SetShieldNumber( shieldNumber - amount, true );
        }

        return _remainingAmount;
    }

    private void UpdateShieldLabel( float amount )
    {
        shieldLabel.text = Mathf.CeilToInt( amount ).ToString();
    }

    private void HideThisGameObject()
    {
        this.gameObject.SetActive( false );
    }
}

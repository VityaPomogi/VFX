using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CreatureHealthBar : MonoBehaviour
{
    [SerializeField] private float redBarPercentage = 0.4f;
    [SerializeField] private float yellowBarReducingSpeed = 1.0f;
    [SerializeField] private float classIconPosForShort = 0.0f;
    [SerializeField] private float classIconPosForLong = 0.0f;
    [SerializeField] private float healthBarPosForShort = 0.0f;
    [SerializeField] private float healthBarPosForLong = 0.0f;
    [SerializeField] private SpriteRenderer backgroundSpriteRenderer;
    [SerializeField] private Sprite healthBarShort;
    [SerializeField] private Sprite healthBarLong;
    [SerializeField] private SpriteRenderer greenBar;
    [SerializeField] private SpriteRenderer redBar;
    [SerializeField] private SpriteRenderer yellowBar;
    [SerializeField] private TextMeshPro hitpointLabel;
    [SerializeField] private Transform classIconTransform;
    [SerializeField] private Transform healthBarTransform;

    private float maximumHitpoint = 0;
    private float remainingHitpoint = 0;
    private float lastRemainingHitpoint = 0;

    private GameObject greenBarObject = null;
    private GameObject redBarObject = null;

    public void SetUp( float maximumHitpoint )
    {
        this.maximumHitpoint = maximumHitpoint;
        SetRemainingHitpoint( maximumHitpoint );

        greenBarObject = greenBar.gameObject;
        redBarObject = redBar.gameObject;
    }

    public void IncreaseHitpoint( float amount )
    {
        SetRemainingHitpoint( remainingHitpoint + amount, true );
        UpdateBar( false );
    }

    public void ReduceHitpoint( float amount )
    {
        SetRemainingHitpoint( remainingHitpoint - amount, true );
        UpdateBar( true );
    }

    private void SetRemainingHitpoint( float amount, bool hasLabelAnimation = false )
    {
        remainingHitpoint = Mathf.Clamp( amount, 0, maximumHitpoint );

        if (hasLabelAnimation == true)
        {
            LeanTween.value( lastRemainingHitpoint, remainingHitpoint, 0.2f ).setOnUpdate( UpdateRemainingHitpointLabel );
        }
        else
        {
            UpdateRemainingHitpointLabel( remainingHitpoint );
        }

        float _classIconPosX = 0.0f;
        float _healthBarPosX = 0.0f;
        if (remainingHitpoint >= 1000)
        {
            backgroundSpriteRenderer.sprite = healthBarLong;
            _classIconPosX = classIconPosForLong;
            _healthBarPosX = healthBarPosForLong;
        }
        else
        {
            backgroundSpriteRenderer.sprite = healthBarShort;
            _classIconPosX = classIconPosForShort;
            _healthBarPosX = healthBarPosForShort;
        }

        classIconTransform.localPosition = new Vector3( _classIconPosX, classIconTransform.localPosition.y, classIconTransform.localPosition.z );
        healthBarTransform.localPosition = new Vector3( _healthBarPosX, healthBarTransform.localPosition.y, healthBarTransform.localPosition.z );

        lastRemainingHitpoint = remainingHitpoint;
    }

    private void UpdateRemainingHitpointLabel( float amount )
    {
        hitpointLabel.text = Mathf.CeilToInt( amount ).ToString();
    }

    private void UpdateBar( bool isReduced )
    {
        float _percentage = remainingHitpoint / maximumHitpoint;
        SpriteRenderer _bar = null;
        if (_percentage > redBarPercentage)
        {
            greenBarObject.SetActive( true );
            redBarObject.SetActive( false );
            _bar = greenBar;
        }
        else if (remainingHitpoint > 0)
        {
            greenBarObject.SetActive( false );
            redBarObject.SetActive( true );
            _bar = redBar;
        }
        else
        {
            greenBarObject.SetActive( false );
            redBarObject.SetActive( false );
        }

        if (_bar != null)
        {
            _bar.size = new Vector2( _percentage, 1.0f );
        }

        if (isReduced == true)
        {
            float _duration = ( yellowBar.size.x - _percentage ) / yellowBarReducingSpeed;
            LeanTween.value( yellowBar.size.x, _percentage, _duration ).setOnUpdate( UpdateYellowBarSize );
        }
        else
        {
            UpdateYellowBarSize( _percentage );
        }
    }

    private void UpdateYellowBarSize( float percentage )
    {
        yellowBar.size = new Vector2( percentage, 1.0f );
    }

    public bool IsRedBar()
    {
        return ( redBarObject.activeSelf == true );
    }

    public bool IsEmpty()
    {
        return ( remainingHitpoint <= 0 );
    }
}

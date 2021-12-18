using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FillBarProgress : MonoBehaviour
{
    [SerializeField] private Image _fillBarImage = null;
    [SerializeField, Range(0.0f, 1.0f)] private float _defaultFillAmount = 0.0f;

    private SpaceDenPlayermon _target = null;
    private ItemType _itemType = ItemType.TOTAL_ITEM_TYPE;

    void Start()
    {
        _fillBarImage.fillAmount = _defaultFillAmount;
    }

    void Update()
    {
        if (_target != null)
        {
            switch ( _itemType )
            {
                case ItemType.Feed:

                        _fillBarImage.fillAmount = _target.FeedingPercentage;

                    break;

                case ItemType.Bath:

                        _fillBarImage.fillAmount = _target.CleanlinessPercentage;

                    break;

                case ItemType.Play:

                        _fillBarImage.fillAmount = _target.EnjoymentPercentage;

                    break;
            }

            if (_fillBarImage.fillAmount <= 0.0f || _fillBarImage.fillAmount >= 1.0f)
            {
                Reset();
            }
        }
    }

    public void SetTarget( SpaceDenPlayermon target, ItemType type )
    {
        if (_target == null || _target != target)
        {
            SpaceDenPlayermon.RequestedAction _requestedAction = target.GetCurrentRequestedAction();
            bool _isMatched = false;

            switch ( _requestedAction )
            {
                case SpaceDenPlayermon.RequestedAction.FEED:

                    if (_requestedAction == SpaceDenPlayermon.RequestedAction.FEED)
                    {
                        _isMatched = true;
                    }

                    break;

                case SpaceDenPlayermon.RequestedAction.PLAY:

                    if (_requestedAction == SpaceDenPlayermon.RequestedAction.PLAY)
                    {
                        _isMatched = true;
                    }

                    break;

                case SpaceDenPlayermon.RequestedAction.BATH:

                    if (_requestedAction == SpaceDenPlayermon.RequestedAction.BATH)
                    {
                        _isMatched = true;
                    }

                    break;
            }

            if (_isMatched == true)
            {
                _target = target;
                _itemType = type;
                _target.SetCurrentFillBarProgress( this );
            }
        }
    }

    public void Reset()
    {
        if (_target != null)
        {
            _target.SetCurrentFillBarProgress( null );
            _target = null;
        }

        _fillBarImage.fillAmount = _defaultFillAmount;
    }

    public void OnComplete()
    {
        Reset();

        LeanTween.scale( this.gameObject, new Vector3( 1.2f, 1.2f, 1.0f ), 0.15f ).setEaseOutCirc().setOnComplete( () =>
            {
                LeanTween.scale( this.gameObject, new Vector3( 1.0f, 1.0f, 1.0f ), 0.15f ).setEaseOutCirc();
            }
        );
    }
}

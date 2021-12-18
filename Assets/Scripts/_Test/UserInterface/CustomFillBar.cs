using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class CustomFillBar : MonoBehaviour
{
    private enum FillMethod
    {
        HorizontalLeft,
        HorizontalRight,
        VerticalBottom,
        VerticalTop,
    }

    [SerializeField] private RectTransform _fillContainer = null;
    [SerializeField] private RectTransform _fillbar = null;
    [SerializeField] private FillMethod _fillMethod = FillMethod.HorizontalLeft;

    [SerializeField, Range(0.0f, 1.0f)] private float _fillAmount = 0.0f;

#if UNITY_EDITOR
    private FillMethod _lastFillMethod = FillMethod.HorizontalLeft;

    private void OnValidate()
    {
        if (_fillContainer == null)
        {
            _fillContainer = GetComponent<RectTransform>();
        }

        if (_lastFillMethod != _fillMethod)
        {
            _lastFillMethod = _fillMethod;
            UpdateFillBarAnchor();
        }

        if (_fillbar != null && _fillContainer != null)
        {
            UpdateFillBar();
        }
    }
#endif

    private void Awake()
    {
        if (_fillContainer == null)
        {
            _fillContainer = GetComponent<RectTransform>();
        }

        UpdateFillBarAnchor();
        UpdateFillBar();
    }

    private void UpdateFillBarAnchor()
    {
        switch (_fillMethod)
        {
            case FillMethod.HorizontalLeft:
            default:
                _fillbar.anchorMin = Vector2.zero;
                _fillbar.anchorMax = Vector2.up;
                _fillbar.pivot = Vector2.up * 0.5f;
                break;

            case FillMethod.HorizontalRight:
                _fillbar.anchorMin = Vector2.right;
                _fillbar.anchorMax = Vector2.one;
                _fillbar.pivot = Vector2.up * 0.5f + Vector2.right;
                break;

            case FillMethod.VerticalBottom:
                _fillbar.anchorMin = Vector2.zero;
                _fillbar.anchorMax = Vector2.right;
                _fillbar.pivot = Vector2.right * 0.5f;
                break;

            case FillMethod.VerticalTop:
                _fillbar.anchorMin = Vector2.up;
                _fillbar.anchorMax = Vector2.one;
                _fillbar.pivot = Vector2.up + Vector2.right * 0.5f;
                break;
        }
    }

    private void UpdateFillBar()
    {
        switch (_fillMethod)
        {
            case FillMethod.HorizontalLeft:
            case FillMethod.HorizontalRight:
                _fillbar.sizeDelta = Vector2.right * _fillContainer.rect.width * _fillAmount;
                break;

            case FillMethod.VerticalBottom:
            case FillMethod.VerticalTop:
                _fillbar.sizeDelta = Vector2.up * _fillContainer.rect.height * _fillAmount;
                break;
        }
    }


    public void UpdateFillAmount(float value)
    {
        _fillAmount = Mathf.Clamp01(value);

        UpdateFillBar();
    }
}

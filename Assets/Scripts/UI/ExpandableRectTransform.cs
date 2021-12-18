using UnityEngine;

public class ExpandableRectTransform : MonoBehaviour
{
    [SerializeField] private RectTransform referenceRectTransform;

    private RectTransform thisRectTransform = null;
    private float widthDifference = 0;
    private float lastReferenceWidth = 0;

    void Awake()
    {
        thisRectTransform = this.GetComponent<RectTransform>();
        widthDifference = thisRectTransform.rect.width - referenceRectTransform.sizeDelta.x;
    }

    void Update()
    {
        float _referenceWidth = referenceRectTransform.sizeDelta.x;
        if (_referenceWidth != lastReferenceWidth)
        {
            thisRectTransform.SetSizeWithCurrentAnchors( RectTransform.Axis.Horizontal, _referenceWidth + widthDifference );
            lastReferenceWidth = _referenceWidth;
        }
    }
}

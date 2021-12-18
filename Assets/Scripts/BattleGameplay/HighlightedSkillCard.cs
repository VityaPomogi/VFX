using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HighlightedSkillCard : MonoBehaviour
{
    [SerializeField] private float fadingAnimationDuration = 0.2f;
    [SerializeField] private CanvasGroup canvasGroupRef;
    [SerializeField] private SkillCardDisplayInfo displayInfo;

    private RectTransform thisRectTransform;
    private bool isShowing = false;

    private int scaleTweenId = 0;
    private int valueTweenId = 0;

    void Awake()
    {
        thisRectTransform = this.GetComponent<RectTransform>();
    }

    void Update()
    {
        if (isShowing == false)
        {
            return;
        }

        thisRectTransform.position = new Vector3( Input.mousePosition.x, thisRectTransform.position.y, 0.0f );
    }

    public void Show( SkillCardDisplayInfo targetDisplayInfo )
    {
        StopCoroutine( "RunHiding" );
        LeanTween.cancel( scaleTweenId );
        LeanTween.cancel( valueTweenId );

        isShowing = true;
        displayInfo.SetUp( targetDisplayInfo );

        scaleTweenId = LeanTween.scale( thisRectTransform, Vector3.one, fadingAnimationDuration ).uniqueId;
        valueTweenId = LeanTween.value( 0.0f, 1.0f, fadingAnimationDuration ).setOnUpdate( UpdateCanvasGroupAlphaValue ).uniqueId;
    }

    public void Hide()
    {
        StartCoroutine( "RunHiding" );
    }

    private IEnumerator RunHiding()
    {
        isShowing = false;
        yield return new WaitForSeconds( 0.05f );
        LeanTween.scale( thisRectTransform, Vector3.zero, fadingAnimationDuration );
        LeanTween.value( 1.0f, 0.0f, fadingAnimationDuration ).setOnUpdate( UpdateCanvasGroupAlphaValue );
    }

    private void UpdateCanvasGroupAlphaValue( float alphaValue )
    {
        canvasGroupRef.alpha = alphaValue;
    }
}

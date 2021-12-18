using UnityEngine;

public class PopUpMessageBoxBasic : MonoBehaviour
{
    [SerializeField] private float animationDuration = 0.3f;
    [SerializeField] private GameObject container = null;

    private bool isPlayingAnimation = false;
    private bool isShowing = false;

    public void Show()
    {
        isShowing = true;
        isPlayingAnimation = true;

        container.transform.localScale = Vector3.zero;
        container.SetActive( true );

        if (this.gameObject != container)
        {
            this.gameObject.SetActive( true );
        }

        LeanTween.scale( container, Vector3.one, animationDuration ).setEaseOutBack().setOnComplete( OnShowingComplete );
    }

    private void OnShowingComplete()
    {
        isPlayingAnimation = false;
    }

    public void Hide()
    {
        isPlayingAnimation = true;

        LeanTween.cancel( container );
        LeanTween.scale( container, Vector3.zero, animationDuration ).setEaseInBack().setOnComplete( OnHidingComplete );
    }

    private void OnHidingComplete()
    {
        container.SetActive( false );

        if (this.gameObject != container)
        {
            this.gameObject.SetActive( false );
        }

        isPlayingAnimation = false;
        isShowing = false;
    }

    public bool GetIsPlayingAnimation()
    {
        return isPlayingAnimation;
    }

    public bool GetIsShowing()
    {
        return isShowing;
    }
}

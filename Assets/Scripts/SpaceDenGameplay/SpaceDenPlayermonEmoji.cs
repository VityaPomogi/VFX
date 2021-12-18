using System.Collections;
using UnityEngine;

public class SpaceDenPlayermonEmoji : MonoBehaviour
{
    [SerializeField] private float animationDuration = 0.3f;
    [SerializeField] private float emojiDuration = 3.0f;
    [SerializeField] private GameObject container;
    [SerializeField] private SpriteRenderer happyEmoji;
    [SerializeField] private SpriteRenderer angryEmoji;
    [SerializeField] private SpriteRenderer sleepyEmoji;
    [SerializeField] private SpriteRenderer confusedEmoji;
    [SerializeField] private SpriteRenderer shockedEmoji;
    [SerializeField] private SpriteRenderer sadEmoji;

    private Transform containerTransform = null;
    private Vector3 originalLocalScale = Vector3.one;

    private SpriteRenderer currentEmoji = null;
    private bool isShowingHappy = false;

    void Awake()
    {
        containerTransform = container.transform;
        originalLocalScale = containerTransform.localScale;
    }

    private void ShowEmoji( ref int sortingOrderIndex, SpriteRenderer targetEmoji, bool autoHide )
    {
        StopCoroutine( "RunAnimation" );

        if (currentEmoji != null)
        {
            currentEmoji.gameObject.SetActive( false );
        }

        currentEmoji = targetEmoji;
        currentEmoji.sortingOrder = sortingOrderIndex;
        sortingOrderIndex++;

        currentEmoji.gameObject.SetActive( true );
        container.SetActive( true );
        StartCoroutine( "RunAnimation", autoHide );
    }

    public void ShowHappy( ref int sortingOrderIndex, bool autoHide = true )
    {
        isShowingHappy = true;
        ShowEmoji( ref sortingOrderIndex, happyEmoji, autoHide );
    }

    public void ShowAngry( ref int sortingOrderIndex, bool autoHide = true )
    {
        ShowEmoji( ref sortingOrderIndex, angryEmoji, autoHide );
    }

    public void ShowSleepy( ref int sortingOrderIndex, bool autoHide = true )
    {
        ShowEmoji( ref sortingOrderIndex, sleepyEmoji, autoHide );
    }

    public void ShowConfused( ref int sortingOrderIndex, bool autoHide = true )
    {
        ShowEmoji( ref sortingOrderIndex, confusedEmoji, autoHide );
    }

    public void ShowShocked( ref int sortingOrderIndex, bool autoHide = true )
    {
        ShowEmoji( ref sortingOrderIndex, shockedEmoji, autoHide );
    }

    public void ShowSad( ref int sortingOrderIndex, bool autoHide = true )
    {
        ShowEmoji( ref sortingOrderIndex, sadEmoji, autoHide );
    }

    private IEnumerator RunAnimation( bool autoHide )
    {
        containerTransform.localScale = Vector3.zero;
        LeanTween.scale( container, originalLocalScale, animationDuration ).setEaseOutBack();
        yield return new WaitForSeconds( emojiDuration );

        if (autoHide == true)
        {
            LeanTween.scale( container, Vector3.zero, animationDuration ).setEaseInBack();
            yield return new WaitForSeconds( animationDuration );
            container.SetActive( false );
        }

        isShowingHappy = false;
    }

    public void HideEmoji()
    {
        LeanTween.scale( container, Vector3.zero, animationDuration ).setEaseInBack().setOnComplete( HideContainer );
    }

    private void HideContainer()
    {
        container.SetActive( false );
    }

    public bool GetIsShowingHappy()
    {
        return isShowingHappy;
    }
}

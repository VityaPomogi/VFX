using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SpaceDenCrisisMissionResultPanel : MonoBehaviour
{
    [Header( "Mission Complete" )]
    [SerializeField] private GameObject missionCompletePanelObject;
    [SerializeField] private Text missionCompleteRewardedSgemLabel;
    [SerializeField] private GameObject missionCompleteTitleObject;
    [SerializeField] private GameObject missionCompleteMessageObject;
    [SerializeField] private GameObject[] missionCompleteSgemObjects;
    [SerializeField] private Transform missionCompleteSgemStartPoint;
    [SerializeField] private MyButton missionCompleteCollectButton;

    [Header( "Mission Complete Without Playermon" )]
    [SerializeField] private GameObject missionCompleteTwoPanelObject;
    [SerializeField] private GameObject missionCompleteTwoTitleObject;
    [SerializeField] private GameObject missionCompleteTwoMessageObject;
    [SerializeField] private GameObject[] missionCompleteTwoSgemObjects;
    [SerializeField] private Transform missionCompleteTwoSgemStartPoint;
    [SerializeField] private MyButton missionCompleteTwoBuyButton;
    [SerializeField] private MyButton missionCompleteTwoOkayButton;

    [Header( "Mission Failed" )]
    [SerializeField] private GameObject missionFailedPanelObject;
    [SerializeField] private Image missionFailedTitleImage;
    [SerializeField] private GameObject missionFailedMessageObject;
    [SerializeField] private MyButton missionFailedOkayButton;

    private SpaceDenGameplayManager spaceDenGameplayManagerRef = null;
    private bool isMissionComplete = false;

    // Mission Complete
    private Vector3[] missionCompleteSgemObjectPositions;
    private Vector3[] missionCompleteSgemObjectLocalScales;

    // Mission Complete Without Playermon
    private Vector3[] missionCompleteTwoSgemObjectPositions;
    private Vector3[] missionCompleteTwoSgemObjectLocalScales;

    public void SetUp( SpaceDenGameplayManager spaceDenGameplayManagerRef )
    {
        this.spaceDenGameplayManagerRef = spaceDenGameplayManagerRef;
    }

    public void ShowMissionComplete( int rewardedSgemAmount )
    {
        isMissionComplete = true;

        missionCompleteRewardedSgemLabel.text = rewardedSgemAmount + " SGEM Earned";
        missionCompleteTitleObject.transform.localScale = Vector3.zero;
        missionCompleteMessageObject.transform.localScale = Vector3.zero;
        missionCompleteCollectButton.SetIsInteractable( false );

        int _missionCompleteSgemObjectsLength = missionCompleteSgemObjects.Length;
        missionCompleteSgemObjectPositions = new Vector3[ _missionCompleteSgemObjectsLength ];
        missionCompleteSgemObjectLocalScales = new Vector3[ _missionCompleteSgemObjectsLength ];
        for (int i = 0; i < _missionCompleteSgemObjectsLength; i++)
        {
            GameObject _missionCompleteSgemObject = missionCompleteSgemObjects[ i ];
            Transform _missionCompleteSgemTransform = _missionCompleteSgemObject.transform;
            missionCompleteSgemObjectPositions[ i ] = _missionCompleteSgemTransform.position;
            missionCompleteSgemObjectLocalScales[ i ] = _missionCompleteSgemTransform.localScale;
            _missionCompleteSgemTransform.position = missionCompleteSgemStartPoint.position;
            _missionCompleteSgemTransform.localScale = Vector3.zero;
            _missionCompleteSgemObject.SetActive( false );
        }

        missionCompletePanelObject.SetActive( true );
        missionCompleteTwoPanelObject.SetActive( false );
        missionFailedPanelObject.SetActive( false );

        this.gameObject.SetActive( true );
        StartCoroutine( RunMissionCompleteAnimation() );
    }

    private IEnumerator RunMissionCompleteAnimation()
    {
        LeanTween.scale( missionCompleteTitleObject, Vector3.one, 0.3f ).setEaseOutBack();
        yield return new WaitForSeconds( 0.2f );
        LeanTween.scale( missionCompleteMessageObject, Vector3.one, 0.3f ).setEaseOutBack();
        yield return new WaitForSeconds( 0.3f );

        for (int i = 0; i < missionCompleteSgemObjects.Length; i++)
        {
            GameObject _missionCompleteSgemObject = missionCompleteSgemObjects[ i ];
            _missionCompleteSgemObject.SetActive( true );
            LeanTween.move( _missionCompleteSgemObject, missionCompleteSgemObjectPositions[ i ], 0.3f ).setEaseOutCirc();
            LeanTween.scale( _missionCompleteSgemObject, missionCompleteSgemObjectLocalScales[ i ], 0.3f ).setEaseOutCirc();
        }

        yield return new WaitForSeconds( 0.3f );
        missionCompleteCollectButton.SetIsInteractable( true );
    }

    public void ShowMissionCompleteWithoutPlayermon()
    {
        isMissionComplete = true;

        missionCompleteTwoTitleObject.transform.localScale = Vector3.zero;
        missionCompleteTwoMessageObject.transform.localScale = Vector3.zero;
        missionCompleteTwoBuyButton.SetIsInteractable( false );
        missionCompleteTwoOkayButton.SetIsInteractable( false );

        int _missionCompleteSgemTwoObjectsLength = missionCompleteTwoSgemObjects.Length;
        missionCompleteTwoSgemObjectPositions = new Vector3[ _missionCompleteSgemTwoObjectsLength ];
        missionCompleteTwoSgemObjectLocalScales = new Vector3[ _missionCompleteSgemTwoObjectsLength ];
        for (int i = 0; i < _missionCompleteSgemTwoObjectsLength; i++)
        {
            GameObject _missionCompleteTwoSgemObject = missionCompleteTwoSgemObjects[ i ];
            Transform _missionCompleteTwoSgemTransform = _missionCompleteTwoSgemObject.transform;
            missionCompleteTwoSgemObjectPositions[ i ] = _missionCompleteTwoSgemTransform.position;
            missionCompleteTwoSgemObjectLocalScales[ i ] = _missionCompleteTwoSgemTransform.localScale;
            _missionCompleteTwoSgemTransform.position = missionCompleteTwoSgemStartPoint.position;
            _missionCompleteTwoSgemTransform.localScale = Vector3.zero;
            _missionCompleteTwoSgemObject.SetActive( false );
        }

        missionCompletePanelObject.SetActive( false );
        missionCompleteTwoPanelObject.SetActive( true );
        missionFailedPanelObject.SetActive( false );

        this.gameObject.SetActive( true );
        StartCoroutine( RunMissionCompleteTwoAnimation() );
    }

    private IEnumerator RunMissionCompleteTwoAnimation()
    {
        LeanTween.scale( missionCompleteTwoTitleObject, Vector3.one, 0.3f ).setEaseOutBack();
        yield return new WaitForSeconds( 0.2f );
        LeanTween.scale( missionCompleteTwoMessageObject, Vector3.one, 0.3f ).setEaseOutBack();
        yield return new WaitForSeconds( 0.3f );

        for (int i = 0; i < missionCompleteTwoSgemObjects.Length; i++)
        {
            GameObject _missionCompleteTwoSgemObject = missionCompleteTwoSgemObjects[ i ];
            _missionCompleteTwoSgemObject.SetActive( true );
            LeanTween.move( _missionCompleteTwoSgemObject, missionCompleteTwoSgemObjectPositions[ i ], 0.3f ).setEaseOutCirc();
            LeanTween.scale( _missionCompleteTwoSgemObject, missionCompleteTwoSgemObjectLocalScales[ i ], 0.3f ).setEaseOutCirc();
        }

        yield return new WaitForSeconds( 0.3f );
        missionCompleteTwoBuyButton.SetIsInteractable( true );
        missionCompleteTwoOkayButton.SetIsInteractable( true );
    }

    public void ShowMissionFailed()
    {
        isMissionComplete = false;

        Color _missionFailedTitleImageColor = missionFailedTitleImage.color;
        _missionFailedTitleImageColor.a = 0.0f;
        missionFailedTitleImage.color = _missionFailedTitleImageColor;

        missionFailedMessageObject.transform.localScale = Vector3.zero;
        missionFailedOkayButton.SetIsInteractable( false );

        missionCompletePanelObject.SetActive( false );
        missionCompleteTwoPanelObject.SetActive( false );
        missionFailedPanelObject.SetActive( true );

        this.gameObject.SetActive( true );
        StartCoroutine( RunMissionFailedAnimation() );
    }

    private IEnumerator RunMissionFailedAnimation()
    {
        LeanTween.alpha( missionFailedTitleImage.GetComponent<RectTransform>(), 1.0f, 0.3f );
        yield return new WaitForSeconds( 0.3f );
        LeanTween.scale( missionFailedMessageObject, Vector3.one, 0.3f ).setEaseOutBack();
        yield return new WaitForSeconds( 0.3f );
        missionFailedOkayButton.SetIsInteractable( true );
    }

    public void ClickToClosePanel()
    {
        SoundManager.Instance.PlayPositiveClickingClip();

        this.gameObject.SetActive( false );
        spaceDenGameplayManagerRef.OnMissionResultPanelClosed( isMissionComplete );
    }
}

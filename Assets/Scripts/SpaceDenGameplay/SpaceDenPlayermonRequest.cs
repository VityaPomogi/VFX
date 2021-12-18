using UnityEngine;
using UnityEngine.Rendering;
using TMPro;

public class SpaceDenPlayermonRequest : MonoBehaviour
{
    [Header( "Settings" )]
    [SerializeField] private float animationDuration = 0.3f;
    [SerializeField] private float shakingTargetTimeRate = 0.4f;
    [SerializeField] private float fastShakingTargetTimeRate = 0.75f;

    [Header( "References" )]
    [SerializeField] private GameObject requestContainer;
    [SerializeField] private SpriteRenderer feedingRequest;
    [SerializeField] private SpriteRenderer feedingLabelBanner;
    [SerializeField] private SpriteRenderer playingRequest;
    [SerializeField] private SpriteRenderer playingLabelBanner;
    [SerializeField] private SpriteRenderer bathingRequest;
    [SerializeField] private SpriteRenderer bathingLabelBanner;
    [SerializeField] private TextMeshPro sequenceLabel;
    [SerializeField] private SortingGroup sequenceLabelSortingGroup;

    private SpaceDenGameplayManager spaceDenGameplayManagerRef = null;
    private SpaceDenCrisisMission spaceDenCrisisMissionRef = null;
    private Vector3 originalLocalScale = Vector3.one;
    private double requestStartTime = 0.0f;
    private float requestTimeLimit = 0.0f;
    private float shakingTargetTime = 0.0f;
    private float fastShakingTargetTime = 0.0f;
    private bool isMission = false;
    private bool isShaking = false;
    private bool isShakingFast = false;
    private bool isReadyToUpdate = false;

    private SpaceDenPlayermon.RequestedAction targetRequestedAction = SpaceDenPlayermon.RequestedAction.NONE;

    void Update()
    {
        if (isReadyToUpdate == true)
        {
            if (isMission == true)
            {
                if (spaceDenCrisisMissionRef.GetIsMissionRunning() == true)
                {
                    double _timePassed = Time.realtimeSinceStartupAsDouble - requestStartTime;
                    if (isShaking == false && _timePassed >= shakingTargetTime)
                    {
                        Shake();
                    }
                    else if (isShakingFast == false && _timePassed >= fastShakingTargetTime)
                    {
                        ShakeFast();
                    }
                    else if (_timePassed >= requestTimeLimit)
                    {
                        spaceDenGameplayManagerRef.OnMissionFailed();
                    }
                }
            }
        }
    }

    public void ShowRequest( SpaceDenGameplayManager spaceDenGameplayManagerRef, ref int sortingOrderIndex, SpaceDenPlayermon.RequestedAction targetRequestedAction, int sequenceNumber = 0, float timeLimit = 0.0f )
    {
        this.targetRequestedAction = targetRequestedAction;
        this.spaceDenGameplayManagerRef = spaceDenGameplayManagerRef;
        spaceDenCrisisMissionRef = spaceDenGameplayManagerRef.GetCrisisMissionRef();

        bool _hasSequenceNumber = ( sequenceNumber > 0 );

        SpriteRenderer _targetRequest = null;
        SpriteRenderer _targetLabelBanner = null;

        switch ( targetRequestedAction )
        {
            case SpaceDenPlayermon.RequestedAction.FEED:

                _targetRequest = feedingRequest;
                _targetLabelBanner = feedingLabelBanner;

                break;

            case SpaceDenPlayermon.RequestedAction.PLAY:

                _targetRequest = playingRequest;
                _targetLabelBanner = playingLabelBanner;

                break;

            case SpaceDenPlayermon.RequestedAction.BATH:

                _targetRequest = bathingRequest;
                _targetLabelBanner = bathingLabelBanner;

                break;
        }

        _targetLabelBanner.sortingOrder = sortingOrderIndex;
        sortingOrderIndex++;
        _targetLabelBanner.gameObject.SetActive( _hasSequenceNumber );

        _targetRequest.sortingOrder = sortingOrderIndex;
        sortingOrderIndex++;
        _targetRequest.gameObject.SetActive( true );

        if (_hasSequenceNumber == true)
        {
            sequenceLabelSortingGroup.sortingOrder = sortingOrderIndex;
            sortingOrderIndex++;

            sequenceLabel.text = sequenceNumber.ToString();
            sequenceLabel.gameObject.SetActive( true );
        }

        originalLocalScale = requestContainer.transform.localScale;
        requestContainer.transform.localScale = Vector3.zero;
        requestContainer.SetActive( true );
        LeanTween.scale( requestContainer, originalLocalScale, animationDuration ).setEaseOutBack().setOnComplete( Idle );

        requestStartTime = Time.realtimeSinceStartupAsDouble;
        requestTimeLimit = timeLimit;
        shakingTargetTime = timeLimit * shakingTargetTimeRate;
        fastShakingTargetTime = timeLimit * fastShakingTargetTimeRate;

        isMission = _hasSequenceNumber;
        isReadyToUpdate = true;
    }

    public void Idle()
    {
        requestContainer.transform.localScale = originalLocalScale;
        LeanTween.scale( requestContainer, originalLocalScale * 0.95f, 0.5f ).setLoopPingPong();
    }

    public void Shake()
    {
        isShaking = true;
        requestContainer.transform.localScale = originalLocalScale;
        LeanTween.scale( requestContainer, originalLocalScale * 0.8f, 0.3f ).setLoopPingPong();
    }

    public void ShakeFast()
    {
        isShakingFast = true;
        LeanTween.cancel( requestContainer );
        requestContainer.transform.localScale = originalLocalScale;
        LeanTween.scale( requestContainer, originalLocalScale * 0.7f, 0.15f ).setLoopPingPong();
    }

    public double GetRequestStartTime()
    {
        return requestStartTime;
    }

    public SpaceDenPlayermon.RequestedAction GetTargetRequestedAction()
    {
        return targetRequestedAction;
    }
}

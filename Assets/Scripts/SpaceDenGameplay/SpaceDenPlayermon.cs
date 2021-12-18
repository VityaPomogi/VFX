using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using ServerApiResponse;

public class SpaceDenPlayermon : MonoBehaviour
{
    [Header( "Settings" )]
    [SerializeField] private float delayToStopIncreasingProgress = 0.2f;
    [SerializeField] private float requestBubbleDistance = 1.0f;
    [SerializeField] private bool shouldActionProgressDecrease = false;

    [Header( "References" )]
    [SerializeField] private PlayermonAnimations _animation = null;
    [SerializeField] private SpaceDenPlayermonEffectHandler spaceDenPlayermonEffectHandlerRef;
    [SerializeField] private SpaceDenPlayermonRequest spaceDenPlayermonRequestPrefab;
    [SerializeField] private Transform requestContainer = null;
    [SerializeField] private SpaceDenPlayermonEmoji spaceDenPlayermonEmojiRef;

    [Header( "Movement" )]
    [SerializeField] private Transform _playermonTransform = null;
    [SerializeField] private Transform _playermonOrientation = null;
    [SerializeField] private SortingGroup _sortingGroup = null;
    [SerializeField] private float _minXPos = 0.0f;
    [SerializeField] private float _minYPos = 0.0f;
    [SerializeField] private float _maxXPos = 0.0f;
    [SerializeField] private float _maxYPos = 0.0f;
    [SerializeField] private float _moveSpeed = 1.0f;
    [SerializeField] private float _moveSpeedForCrisis = 1.0f;
    [SerializeField] private float _minDistanceToTargetPos = 0.05f;
    [SerializeField] private float _idleDuration = 1.0f;
    [SerializeField] private float _idleDurationRandom = 1.0f;
    [SerializeField] private float _rejectingCooldown = 3.0f;
    [SerializeField] private float _maxSortingLayers = 1000.0f;

    [Header( "Sound Effects" )]
    [SerializeField] private AudioClip[] _feedingSFX = null;
    [SerializeField] private AudioClip[] _bathingSFX = null;
    [SerializeField] private AudioClip[] _playingSFX = null;
    [SerializeField] private AudioClip _successSFX = null;

    [Header( "Visual Effects" )]
    [SerializeField] private GameObject eatingParticle;
    [SerializeField] private GameObject playingParticle;
    [SerializeField] private GameObject bathingParticle;
    [SerializeField] private GameObject loveParticle;

    private SpaceDenGameplayManager spaceDenGameplayManagerRef = null;
    private SpaceDenCrisisMission spaceDenCrisisMissionRef = null;
    private Animator animatorRef = null;
    private int playermonIndex = 0;
    private float currentIdleDuration = 0;

    private Vector2 _startPosition = Vector2.zero;
    private Vector3 _startRotation = Vector3.zero;
    private bool _isGettingReadyForCrisis = false;
    private bool _readyForCrisis = false;

    private Vector2 _targetPos = Vector2.zero;
    private float _idleTimer = 0.0f;
    private bool _isIdle = false;
    private float _totalYDistance = 0.0f;
    private bool _isRejecting = false;
    private float _lastRejectingTime = 0.0f;
    private RequestedAction _lastRejectingAction = RequestedAction.NONE;
    private bool _needToRandomizeTargetPos = true;
    private bool _isWalkingTriggered = false;
    private int speechBubbleSortingOrderIndex = 0;

    #region Feed
    private float _feedingTargetValue = 100.0f;
    private float _feedingValuePerSecond = 25.0f;
    private int _feedingLovePoints = 0;
    private float _feedingCurrentValue = 0.0f;
    private bool _isFeeding = false;
    private bool _isFeedingDone = true;
    //private bool _smelling = false;
    #endregion

    #region Bath
    private float _maxCleanliness = 100.0f;
    private float _cleanlinessPerSecond = 25.0f;
    private int _cleanlinessLovePoints = 0;
    private float _cleanliness = 0.0f;
    private bool _cleaning = false;
    private bool _cleaned = true;
    #endregion

    #region Play
    private float _maxEnjoyment = 100.0f;
    private float _enjoymentPerSecond = 25.0f;
    private int _enjoymentLovePoints = 0;
    private float _enjoyment = 0.0f;
    private bool _playing = false;
    private bool _happy = true;
    #endregion

    private GameObject spaceDenPlayermonRequestPrefabObject = null;
    private SpaceDenPlayermonRequest currentRequest = null;
    private RequestedAction currentRequestedAction = RequestedAction.NONE;
    private List<SpaceDenPlayermonRequest> requestList = new List<SpaceDenPlayermonRequest>();
    //private float currentRequestTimeLimit = 0;
    //private double currentRequestStartTime = 0;

    private float _feedingAudioStartTime = 0.0f;
    private AudioClip _feedingCurrentClip = null;
    private float _bathingAudioStartTime = 0.0f;
    private AudioClip _bathingCurrentClip = null;
    private float _playingAudioStartTime = 0.0f;
    private AudioClip _playingCurrentClip = null;

    private FillBarProgress currentFillBarProgress = null;
    public float FeedingPercentage => _feedingCurrentValue / _feedingTargetValue;
    public float CleanlinessPercentage => _cleanliness / _maxCleanliness;
    public float EnjoymentPercentage => _enjoyment / _maxEnjoyment;

    public enum RequestedAction
    {
        NONE = 0,
        FEED = 1,
        PLAY = 2,
        BATH = 3
    }

    public void SetUp( SpaceDenGameplayManager spaceDenGameplayManagerRef, int playermonIndex, GetSpaceDenProgressResponse_Data_SpaceDenAction[] spaceDenActions )
    {
        this.spaceDenGameplayManagerRef = spaceDenGameplayManagerRef;
        this.playermonIndex = playermonIndex;

        spaceDenCrisisMissionRef = spaceDenGameplayManagerRef.GetCrisisMissionRef();
        animatorRef = _animation.GetAnimator();

        for (int i = 0; i < spaceDenActions.Length; i++)
        {
            GetSpaceDenProgressResponse_Data_SpaceDenAction _action = spaceDenActions[ i ];

            switch (_action.action_type)
            {
                case 1:

                    _feedingTargetValue = _action.target_value;
                    _feedingValuePerSecond = _action.value_increase_per_second;
                    _feedingLovePoints = _action.love_points_earned;

                    break;

                case 2:

                    _maxCleanliness = _action.target_value;
                    _cleanlinessPerSecond = _action.value_increase_per_second;
                    _cleanlinessLovePoints = _action.love_points_earned;

                    break;

                case 3:

                    _maxEnjoyment = _action.target_value;
                    _enjoymentPerSecond = _action.value_increase_per_second;
                    _enjoymentLovePoints = _action.love_points_earned;

                    break;
            }
        }

        spaceDenPlayermonRequestPrefabObject = spaceDenPlayermonRequestPrefab.gameObject;
    }

    private void Start()
    {
        _startPosition = _playermonTransform.position;
        _startRotation = _playermonOrientation.localScale;

        SetToIdle();

        _totalYDistance = Mathf.Abs( _maxYPos - _minYPos );
        UpdateSortingOrder();
    }

    private void Update()
    {
        bool _needToUpdateSortingOrder = false;

        if (!_readyForCrisis)
        {
            if (currentRequestedAction != RequestedAction.NONE || _isRejecting || _playing || _cleaning || _isFeeding)
            {
                if (!_isIdle)
                {
                    SetToIdle();
                }

                _idleTimer = -0.000001f;
            }

            if (_isIdle)
            {
                _idleTimer += Time.deltaTime;
                if (_isWalkingTriggered == true || _idleTimer >= currentIdleDuration)
                {
                    _isWalkingTriggered = false;

                    if (_needToRandomizeTargetPos == true)
                    {
                        do
                        {
                            _targetPos.x = Random.Range( _minXPos, _maxXPos );
                            _targetPos.y = Random.Range( _minYPos, _maxYPos );
                        }
                        while (Vector2.Distance( _playermonTransform.position, _targetPos ) <= _minDistanceToTargetPos);

                        if (_playermonTransform.position.x > _targetPos.x)
                        {
                            _playermonOrientation.localScale = new Vector3( 1.0f, 1.0f, 1.0f );
                        }
                        else
                        {
                            _playermonOrientation.localScale = new Vector3( -1.0f, 1.0f, 1.0f );
                        }
                    }

                    _isIdle = false;
                    _animation.PlayWalkingAnimation();
                }
            }
            else
            {
                _playermonTransform.position = Vector2.MoveTowards( _playermonTransform.position, _targetPos, _moveSpeed * Time.deltaTime );

                if (currentRequestedAction != RequestedAction.NONE || _isRejecting == true
                    || Vector2.Distance( _playermonTransform.position, _targetPos ) <= _minDistanceToTargetPos)
                {
                    _idleTimer = 0.0f;
                    _playermonTransform.position = _targetPos;
                    SetToIdle();

                    if (_isRejecting == false)
                    {
                        _animation.PlayIdleAnimation();
                    }
                }

                _needToUpdateSortingOrder = true;
            }
        }
        else if (_isGettingReadyForCrisis == true)
        {
            bool _isCloseToTargetPos = false;
            if (Vector2.Distance( _playermonTransform.position, _startPosition ) > _minDistanceToTargetPos)
            {
                _playermonTransform.position = Vector2.MoveTowards( _playermonTransform.position, _startPosition, _moveSpeedForCrisis * Time.deltaTime );

                if (Vector2.Distance( _playermonTransform.position, _startPosition ) <= _minDistanceToTargetPos)
                {
                    _isCloseToTargetPos = true;
                }
            }
            else
            {
                _isCloseToTargetPos = true;
            }

            if (_isCloseToTargetPos == true)
            {
                _playermonTransform.position = _startPosition;
                _playermonOrientation.localScale = _startRotation;
                _animation.PlayIdleAnimation();

                _isGettingReadyForCrisis = false;
                spaceDenGameplayManagerRef.OnPlayermonReadyForCrisis();
            }

            _needToUpdateSortingOrder = true;
        }

        if (_needToUpdateSortingOrder == true)
        {
            UpdateSortingOrder();
        }

        #region Feed
        if (_isFeeding && !_isFeedingDone)
        {
            if (spaceDenCrisisMissionRef.GetIsMissionRunning() == true)
            {
                if (currentRequestedAction != RequestedAction.NONE && currentRequestedAction != RequestedAction.FEED)
                {
                    spaceDenGameplayManagerRef.OnMissionFailed();
                }
            }

            _feedingCurrentValue += _feedingValuePerSecond * Time.deltaTime;

            if (_feedingCurrentValue >= _feedingTargetValue)
            {
                CancelInvoke( "PlayFeedingSoundEffect" );

                if (currentFillBarProgress != null)
                {
                    currentFillBarProgress.OnComplete();
                }

                _isFeedingDone = true;
                _isFeeding = false;
                spaceDenPlayermonEffectHandlerRef.HideEatingEffect();
                PlayActionSuccessAnimation();

                if (currentRequestedAction == RequestedAction.FEED)
                {
                    OnRequestCompleted();
                    spaceDenGameplayManagerRef.AddLovePoints( _feedingLovePoints );
                }
            }
        }
        else if (shouldActionProgressDecrease == true)
        {
            if (!_isFeedingDone && _feedingCurrentValue > 0.0f)
            {
                _feedingCurrentValue -= _feedingValuePerSecond * Time.deltaTime;

                if (_feedingCurrentValue < 0.0f)
                {
                    _feedingCurrentValue = 0.0f;
                }
            }
        }
        #endregion

        #region Bath
        if (_cleaning && !_cleaned)
        {
            if (spaceDenCrisisMissionRef.GetIsMissionRunning() == true)
            {
                if (currentRequestedAction != RequestedAction.NONE && currentRequestedAction != RequestedAction.BATH)
                {
                    spaceDenGameplayManagerRef.OnMissionFailed();
                }
            }

            _cleanliness += _cleanlinessPerSecond * Time.deltaTime;

            if (_cleanliness >= _maxCleanliness)
            {
                if (currentFillBarProgress != null)
                {
                    currentFillBarProgress.OnComplete();
                }

                _cleaned = true;
                _cleaning = false;
                CancelInvoke( "HideBathingEffect" );
                spaceDenPlayermonEffectHandlerRef.HideBathingEffect();
                PlayActionSuccessAnimation();

                if (currentRequestedAction == RequestedAction.BATH)
                {
                    OnRequestCompleted();
                    spaceDenGameplayManagerRef.AddLovePoints( _cleanlinessLovePoints );
                }
            }
        }
        else if (shouldActionProgressDecrease == true)
        {
            if (!_cleaned && _cleanliness > 0.0f)
            {
                _cleanliness -= _cleanlinessPerSecond * Time.deltaTime;

                if (_cleanliness < 0.0f)
                {
                    _cleanliness = 0.0f;
                }
            }
        }
        #endregion

        #region Play
        if (_playing && !_happy)
        {
            if (spaceDenCrisisMissionRef.GetIsMissionRunning() == true)
            {
                if (currentRequestedAction != RequestedAction.NONE && currentRequestedAction != RequestedAction.PLAY)
                {
                    spaceDenGameplayManagerRef.OnMissionFailed();
                }
            }

            _enjoyment += _enjoymentPerSecond * Time.deltaTime;

            if (_enjoyment >= _maxEnjoyment)
            {
                if (currentFillBarProgress != null)
                {
                    currentFillBarProgress.OnComplete();
                }

                _happy = true;
                _playing = false;
                CancelInvoke( "HidePlayingEffect" );
                spaceDenPlayermonEffectHandlerRef.HidePlayingEffect();
                PlayActionSuccessAnimation();

                if (currentRequestedAction == RequestedAction.PLAY)
                {
                    OnRequestCompleted();
                    spaceDenGameplayManagerRef.AddLovePoints( _enjoymentLovePoints );
                }
            }
        }
        else if (shouldActionProgressDecrease == true)
        {
            if (!_happy && _enjoyment > 0.0f)
            {
                _enjoyment -= _enjoymentPerSecond * Time.deltaTime;

                if (_enjoyment < 0.0f)
                {
                    _enjoyment = 0.0f;
                }
            }
        }
        #endregion

        if (currentRequestedAction == RequestedAction.NONE)
        {
            _isFeedingDone = true;
            _cleaned = true;
            _happy = true;
        }

        if (animatorRef.GetCurrentAnimatorStateInfo( 0 ).IsName( "Idle" ) == true)
        {
            if (currentRequestedAction == RequestedAction.NONE)
            {
                PlayIdleAnimation();
            }
        }
    }

    public void SetCurrentFillBarProgress( FillBarProgress targetFillBarProgress )
    {
        currentFillBarProgress = targetFillBarProgress;
    }

    private void UpdateSortingOrder()
    {
        _sortingGroup.sortingOrder = Mathf.RoundToInt( ( Mathf.Abs( _maxYPos - _playermonTransform.position.y ) / _totalYDistance ) * _maxSortingLayers );
        speechBubbleSortingOrderIndex = 0;
    }

    private void SetToIdle()
    {
        _isIdle = true;
        currentIdleDuration = _idleDuration + ( Random.value * _idleDurationRandom );
        _needToRandomizeTargetPos = true;
    }

    private void PlayActionSuccessAnimation()
    {
        SoundManager.Instance.PlaySoundEffect( _successSFX );
        spaceDenPlayermonEffectHandlerRef.ShowLoveEffect( loveParticle, _sortingGroup );

        if (spaceDenGameplayManagerRef.GetIsShowingCrisisMissionResult() == false)
        {
            if (_isRejecting == true)
            {
                OnRejectingAnimationFinished();
            }

            _animation.PlayActionSuccesAnimation( () => { PlayIdleAnimation(); } );
        }
    }

    private void PlaySoundEffect( AudioClip targetClip, ref AudioClip currentClip, ref float audioStartTime )
    {
        bool _canPlaySound = false;
        if (currentClip == null)
        {
            _canPlaySound = true;
        }
        else if (Time.realtimeSinceStartup - audioStartTime > currentClip.length)
        {
            _canPlaySound = true;
        }

        if (_canPlaySound == true)
        {
            SoundManager.Instance.PlaySoundEffect( targetClip );
            currentClip = targetClip;
            audioStartTime = Time.realtimeSinceStartup;
        }
    }

    #region SpaceDen Actions
    /*
    public void FeedPlayermon()
    {
        _smelling = false;
        _animation.PlayEatingAnimation(PlayActionSuccessAnimation);
        spaceDenPlayermonEffectHandlerRef.ShowEatingEffect( eatingParticle );

        //AudioManager.instance.Play("FeedingPlayermon");

        PlaySoundEffect(_feedingSFX);

        if (spaceDenCrisisMissionRef.GetIsMissionRunning() == true)
        {
            if (currentRequestedAction != RequestedAction.NONE)
            {
                if (currentRequestedAction == RequestedAction.FEED)
                {
                    OnRequestCompleted();
                }
                else
                {
                    spaceDenGameplayManagerRef.OnMissionFailed();
                }
            }
        }
    }

    public void StartSmelling()
    {
        if (_smelling)
        {
            return;
        }

        _smelling = true;
        _animation.PlaySmellingAnimation();
    }

    public void StopSmelling()
    {
        _smelling = false;
        _animation.PlayIdleAnimation();
    }
    */

    private void RejectAction( RequestedAction targetAction )
    {
        if (_isRejecting == false)
        {
            if (spaceDenPlayermonEmojiRef.GetIsShowingHappy() == false)
            {
                if (_lastRejectingAction != targetAction || Time.time - _lastRejectingTime >= _rejectingCooldown)
                {
                    _isRejecting = true;
                    _animation.PlayRejectingAnimation( OnRejectingAnimationFinished );
                    spaceDenPlayermonEmojiRef.ShowAngry( ref speechBubbleSortingOrderIndex );

                    _lastRejectingAction = targetAction;
                    _lastRejectingTime = Time.time;

                    StartCoroutine( RunRejectingAnimationChecking() );
                }
            }
        }
    }

    private void OnRejectingAnimationFinished()
    {
        _isRejecting = false;

        if (currentRequestedAction == RequestedAction.NONE)
        {
            _needToRandomizeTargetPos = false;
            _isWalkingTriggered = true;
            _isIdle = true;
        }
        else
        {
            _animation.PlayCrisisIdlesAnimation();
        }
    }

    private IEnumerator RunRejectingAnimationChecking()
    {
        yield return new WaitForSeconds( 0.1f );

        while (_isRejecting == true)
        {
            if (animatorRef.GetCurrentAnimatorStateInfo( 0 ).IsName( "Rejecting" ) == false)
            {
                OnRejectingAnimationFinished();
                yield break;
            }

            yield return new WaitForSeconds( 0.02f );
        }
    }
    
    public void StartFeeding()
    {
        if (spaceDenGameplayManagerRef.GetIsShowingCrisisMissionResult() == true)
        {
            return;
        }

        if (_isFeedingDone == true)
        {
            if (_readyForCrisis == false)
            {
                RejectAction( RequestedAction.FEED );
            }

            return;
        }

        if (_isRejecting == true)
        {
            OnRejectingAnimationFinished();
        }

        _isFeeding = true;
        _animation.PlayEatingAnimation();
        spaceDenPlayermonEffectHandlerRef.ShowEatingEffect( eatingParticle, _sortingGroup );

        CancelInvoke( "PlayFeedingSoundEffect" );
        PlayFeedingSoundEffect();
    }

    private void PlayFeedingSoundEffect()
    {
        AudioClip _clip = _feedingSFX[ Random.Range( 0, _feedingSFX.Length ) ];
        PlaySoundEffect( _clip, ref _feedingCurrentClip, ref _feedingAudioStartTime );
        Invoke( "PlayFeedingSoundEffect", _clip.length );
    }

    public void StopFeeding()
    {
        CancelInvoke( "PlayFeedingSoundEffect" );

        if (spaceDenGameplayManagerRef.GetIsShowingCrisisMissionResult() == true)
        {
            return;
        }

        if (_isFeedingDone == true)
        {
            return;
        }

        if (_isRejecting == true)
        {
            OnRejectingAnimationFinished();
        }

        _isFeeding = false;
        spaceDenPlayermonEffectHandlerRef.HideEatingEffect();
    }

    public void StartBathing()
    {
        if (spaceDenGameplayManagerRef.GetIsShowingCrisisMissionResult() == true)
        {
            return;
        }

        if (_cleaned)
        {
            if (_readyForCrisis == false)
            {
                RejectAction( RequestedAction.BATH );
            }

            return;
        }

        if (_isRejecting == true)
        {
            OnRejectingAnimationFinished();
        }

        _cleaning = true;
        CancelInvoke( "StopBathing" );
        _animation.PlayBathingAnimation( StopBathing );
        Invoke( "StopBathing", delayToStopIncreasingProgress );
        CancelInvoke( "HideBathingEffect" );
        spaceDenPlayermonEffectHandlerRef.ShowBathingEffect( bathingParticle, _sortingGroup );

        PlaySoundEffect( _bathingSFX[ Random.Range( 0, _bathingSFX.Length ) ], ref _bathingCurrentClip, ref _bathingAudioStartTime );
    }

    public void StopBathing()
    {
        if (spaceDenGameplayManagerRef.GetIsShowingCrisisMissionResult() == true)
        {
            return;
        }

        if (_cleaned)
        {
            return;
        }

        if (_isRejecting == true)
        {
            OnRejectingAnimationFinished();
        }

        _cleaning = false;
        Invoke( "HideBathingEffect", 0.5f );
    }

    private void HideBathingEffect()
    {
        spaceDenPlayermonEffectHandlerRef.HideBathingEffect();
    }

    public void StartPlaying()
    {
        if (spaceDenGameplayManagerRef.GetIsShowingCrisisMissionResult() == true)
        {
            return;
        }

        if (_happy)
        {
            if (_readyForCrisis == false)
            {
                RejectAction( RequestedAction.PLAY );
            }

            return;
        }

        if (_isRejecting == true)
        {
            OnRejectingAnimationFinished();
        }

        _playing = true;
        CancelInvoke( "StopPlaying" );
        _animation.PlayPlayingAnimation();
        Invoke( "StopPlaying", delayToStopIncreasingProgress );
        CancelInvoke( "HidePlayingEffect" );
        spaceDenPlayermonEffectHandlerRef.ShowPlayingEffect( playingParticle, _sortingGroup );

        PlaySoundEffect( _playingSFX[ Random.Range( 0, _playingSFX.Length ) ], ref _playingCurrentClip, ref _playingAudioStartTime );
    }

    public void StopPlaying()
    {
        if (spaceDenGameplayManagerRef.GetIsShowingCrisisMissionResult() == true)
        {
            return;
        }

        if (_happy)
        {
            return;
        }

        if (_isRejecting == true)
        {
            OnRejectingAnimationFinished();
        }

        _playing = false;
        Invoke( "HidePlayingEffect", 0.5f );
    }

    private void HidePlayingEffect()
    {
        spaceDenPlayermonEffectHandlerRef.HidePlayingEffect();
    }
    #endregion

    #region Crisis Mission

    public void GetReadyForCrisis()
    {
        HideRequest();

        if (_playermonTransform.position.x > _startPosition.x)
        {
            _playermonOrientation.localScale = new Vector3( 1.0f, 1.0f, 1.0f );
        }
        else
        {
            _playermonOrientation.localScale = new Vector3( -1.0f, 1.0f, 1.0f );
        }

        _readyForCrisis = true;

        if (Vector2.Distance( _playermonTransform.position, _startPosition ) > _minDistanceToTargetPos)
        {
            _animation.PlayRunningAnimation();
            _isGettingReadyForCrisis = true;
        }
        else
        {
            _playermonTransform.position = _startPosition;
            _playermonOrientation.localScale = _startRotation;
            _animation.PlayIdleAnimation();

            _isGettingReadyForCrisis = false;
            spaceDenGameplayManagerRef.OnPlayermonReadyForCrisis();
        }
    }

    public void CrisisOver()
    {
        _readyForCrisis = false;
        _isFeedingDone = true;
        _cleaned = true;
        _happy = true;

        spaceDenPlayermonEmojiRef.HideEmoji();
        PlayIdleAnimation();
        SetToIdle();
    }

    public void ShowRequest( int actionType, int sequenceNumber = 0, float timeLimit = 0 )
    {
        if (_isRejecting == true)
        {
            OnRejectingAnimationFinished();
        }

        _isWalkingTriggered = false;

        float _startX = -0.5f * requestBubbleDistance * requestList.Count;
        for (int i = 0; i < requestList.Count; i++)
        {
            LeanTween.moveLocalX( requestList[ i ].gameObject, _startX + ( i * requestBubbleDistance ), 0.3f ).setEaseOutCirc();
        }

        GameObject _requestObj = Instantiate( spaceDenPlayermonRequestPrefabObject );
        Transform _requestTransform = _requestObj.transform;
        _requestTransform.SetParent( requestContainer, false );

        SpaceDenPlayermonRequest _request = _requestObj.GetComponent<SpaceDenPlayermonRequest>();
        _request.ShowRequest( spaceDenGameplayManagerRef, ref speechBubbleSortingOrderIndex, ( RequestedAction )actionType, sequenceNumber, timeLimit );
        requestList.Add( _request );
        _requestTransform.localPosition = new Vector3( 0.5f * requestBubbleDistance * ( requestList.Count - 1 ), 0.0f, 0.0f );

        UpdateCurrentRequest();
        _animation.PlayCrisisIdlesAnimation();
    }

    private bool UpdateCurrentRequest()
    {
        if (requestList.Count > 0)
        {
            SpaceDenPlayermonRequest _request = requestList[ 0 ];
            if (currentRequest != _request)
            {
                currentRequest = _request;
                currentRequestedAction = currentRequest.GetTargetRequestedAction();

                switch ( currentRequestedAction )
                {
                    case RequestedAction.FEED:

                        _isFeedingDone = false;
                        _feedingCurrentValue = 0;

                        break;

                    case RequestedAction.PLAY:

                        _happy = false;
                        _enjoyment = 0;

                        break;

                    case RequestedAction.BATH:

                        _cleaned = false;
                        _cleanliness = 0;

                        break;
                }
            }

            return true;
        }

        return false;
    }

    public void OnRequestCompleted()
    {
        spaceDenPlayermonEmojiRef.ShowHappy( ref speechBubbleSortingOrderIndex );

        bool _needToPlayIdleAnimation = false;
        if (spaceDenCrisisMissionRef.GetIsMissionRunning() == true)
        {
            double _timeTakenInSeconds = Time.realtimeSinceStartupAsDouble - currentRequest.GetRequestStartTime();
            bool _isMissionComplete = spaceDenGameplayManagerRef.OnPlayermonRequestCompletedForCrisis( playermonIndex, currentRequestedAction, _timeTakenInSeconds );
            if (_isMissionComplete == false)
            {
                _needToPlayIdleAnimation = true;
            }
        }
        else
        {
            spaceDenGameplayManagerRef.OnPlayermonRequestCompleted( playermonIndex, currentRequestedAction );
            _needToPlayIdleAnimation = true;
        }

        HideRequest();

        if (UpdateCurrentRequest() == false)
        {
            if (_needToPlayIdleAnimation == true)
            {
                PlayIdleAnimation();
            }
        }
    }

    public void HideRequest()
    {
        currentRequestedAction = RequestedAction.NONE;

        if (currentRequest != null)
        {
            requestList.Remove( currentRequest );
            Destroy( currentRequest.gameObject );
            currentRequest = null;

            float _startX = -0.5f * requestBubbleDistance * ( requestList.Count - 1 );
            for (int i = 0; i < requestList.Count; i++)
            {
                LeanTween.moveLocalX( requestList[ i ].gameObject, _startX + ( i * requestBubbleDistance ), 0.3f ).setEaseOutCirc();
            }
        }
    }

    public void HideAllRequests()
    {
        for (int i = 0; i < requestList.Count; i++)
        {
            Destroy( requestList[ i ].gameObject );
        }
        requestList.Clear();
    }

    public void PlayCrisisSuccessAnimation()
    {
        _animation.PlayCrisisSuccessAnimation();
        spaceDenPlayermonEmojiRef.ShowHappy( ref speechBubbleSortingOrderIndex, false );
    }

    public void PlayCrisisFailedAnimation()
    {
        _animation.PlayCrisisFailAnimation();
        spaceDenPlayermonEmojiRef.ShowSad( ref speechBubbleSortingOrderIndex, false );
    }

    public RequestedAction GetCurrentRequestedAction()
    {
        return currentRequestedAction;
    }

    #endregion

    public void PlayIdleAnimation()
    {
        if (spaceDenGameplayManagerRef.GetIsShowingCrisisMissionResult() == false)
        {
            if (spaceDenCrisisMissionRef.GetIsMissionRunning() == true && currentRequestedAction != RequestedAction.NONE)
            {
                _animation.PlayCrisisIdlesAnimation();
            }
            else
            {
                _animation.PlayIdleAnimation();
            }
        }
    }

    public int GetPlayermonIndex()
    {
        return playermonIndex;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using ServerApiResponse;
using Newtonsoft.Json;

public class SpaceDenGameplayManager : MonoBehaviour
{
    [Header( "Settings" )]
    [SerializeField] private float apiCallingRetryingInterval = 10.0f;
    [SerializeField] private bool isUsingTestData = false;

    [Header( "References" )]
    [SerializeField] private Creature[] creatures;
    [SerializeField] private GameObject[] dummyCreatureObjects;
    [SerializeField] private SpaceDenCrisisMission crisisMissionRef;
    [SerializeField] private DragDrop[] itemDragDropComponents;
    [SerializeField] private DecorativeItem[] decorativeItems;

    [Header( "UI" )]
    [SerializeField] private CustomFillBar _loveRankFillBar = null;
    [SerializeField] private Text _loveRankText = null;
    [SerializeField] private Text _sgemAmountText = null;
    [SerializeField] private SpaceDenInstructionPanel instructionPanel;
    [SerializeField] private SpaceDenCrisisMissionPanel crisisMissionPanel;
    [SerializeField] private SpaceDenCrisisMissionResultPanel crisisMissionResultPanel;
    [SerializeField] private PopUpMessageBoxBasic itemPurchasePanel;
    [SerializeField] private PopUpMessageBoxBasic insufficientFundPanel;
    [SerializeField] private PopUpMessageBoxBasic maxLoveRankReachedPanel;
    [SerializeField] private PopUpMessageBoxBasic errorMessagePanel;
    [SerializeField] private Button settingButton;
    [SerializeField] private Button instructionButton;
    [SerializeField] private Button giftBoxButton;
    [SerializeField] private Text giftBoxNumberLabel;
    [SerializeField] private GameObject loadingScreenObject;
    [SerializeField] private SettingPanel settingPanelRef;

    [Header( "Sound" )]
    [SerializeField] private AudioClip _normalBGM = null;
    [SerializeField] private AudioClip _crisisBGM = null;
    [SerializeField] private AudioClip crisisActivatedClip = null;
    [SerializeField] private AudioClip crisisSuccessClip = null;
    [SerializeField] private AudioClip crisisFailedClip = null;
    [SerializeField] private AudioClip decorativeItemPopUpClip = null;
    [SerializeField] private AudioClip normalMessagePopUpClip = null;
    [SerializeField] private AudioClip errorMessagePopUpClip = null;

    private GetSpaceDenProgressResponse_Data spaceDenData = null;
    private GetUserProfileResponse_Data_BasicUserProfile currentUserProfile = null;
    private GetSpaceDenProgressResponse_Data_SpaceDenProgress currentSpaceDenProgress = null;

    private bool isSpaceDenStarted = false;
    private float spaceDenActionSubmissionStartTime = 0.0f;
    private float playermonRequestInterval = 10.0f;
    private float playermonRequestIntervalRandom = 5.0f;
    private float playerActionSubmissionInterval = 30.0f;

    private List<SpaceDenPlayermon> spaceDenPlayermons = null;
    private List<CompletedPlayermonRequest> completedPlayermonRequestList = null;
    private List<CrisisCompletedPlayermonRequest> crisisCompletedPlayermonRequestList = null;
    private bool isInCrisisMissionMode = false;
    private bool isShowingCrisisMissionResult = false;
    private float currentLovePoints = 0.0f;
    private float maximumLovePoints = 0.0f;
    private bool hasMaximumLoveRankReached = false;
    private bool isCrisisMissionStarted = false;
    private int crisisReadyCount = 0;
    private GameObject giftBoxButtonObject = null;
    private List<int> currentUnlockedDecorativeItemIdList = new List<int>();

    private readonly string GET_SPACE_DEN_PROGRESS = ServerApiManager.DOMAIN + "playermon/spaceDen/progress";
    private readonly string ACKNOWLEDGE_THAT_USER_HAS_SEEN_SPACEDEN = ServerApiManager.DOMAIN + "";
    private readonly string ACTIVATE_CRISIS_IN_SPACEDEN = ServerApiManager.DOMAIN + "playermon/spaceDen/activateCrisis";
    private readonly string SUBMIT_SPACEDEN_CRISIS_RESULT = ServerApiManager.DOMAIN + "";
    private readonly string SUBMIT_SPACEDEN_ACTION = ServerApiManager.DOMAIN + "";
    private readonly string UNLOCK_DECORATIVE_ITEM_IN_SPACEDEN = ServerApiManager.DOMAIN + "";

    void Awake()
    {
        crisisMissionPanel.SetUp( this );
        crisisMissionResultPanel.SetUp( this );
        giftBoxButtonObject = giftBoxButton.gameObject;
        settingPanelRef.onExitSpaceDenButtonClicked = ExitScene;
    }

    void Start()
    {
        SoundManager.Instance.PlayBackgroundMusic( _normalBGM );
        RequestSpaceDenProgress();
    }

    void Update()
    {
        if (isSpaceDenStarted == true && isInCrisisMissionMode == false && hasMaximumLoveRankReached == false)
        {
            if (Time.time - spaceDenActionSubmissionStartTime > playerActionSubmissionInterval)
            {
                spaceDenActionSubmissionStartTime = Time.time;
                SubmitCompletedRequestList();
            }
        }
    }

    private void UpdateUserProfileInformation( GetUserProfileResponse_Data_BasicUserProfile userProfile, GetSpaceDenProgressResponse_Data_SpaceDenProgress spaceDenProgress )
    {
        currentUserProfile = userProfile;

        _sgemAmountText.text = userProfile.sgem_tokens.ToString();
        UpdateSpaceDenProgressInformation( spaceDenProgress );
    }

    private void UpdateSpaceDenProgressInformation( GetSpaceDenProgressResponse_Data_SpaceDenProgress spaceDenProgress )
    {
        currentSpaceDenProgress = spaceDenProgress;

        _loveRankText.text = "Rank " + spaceDenProgress.current_love_rank.ToString();
        currentLovePoints = spaceDenProgress.current_love_points;
        maximumLovePoints = spaceDenProgress.maximum_love_points;
        UpdateLoveRankFillBar();

        crisisMissionPanel.UpdateProgress( spaceDenProgress );

        int _giftBoxNumber = spaceDenProgress.gift_boxes;
        giftBoxNumberLabel.text = _giftBoxNumber.ToString();

        if (_giftBoxNumber > 0)
        {
            if (giftBoxButtonObject.activeSelf == false)
            {
                giftBoxButton.transform.localScale = Vector3.zero;
                giftBoxButton.interactable = false;
                giftBoxButtonObject.SetActive( true );
                LeanTween.scale( giftBoxButton.gameObject, Vector3.one, 0.5f ).setEaseOutBack().setOnComplete( () =>
                {
                    giftBoxButton.interactable = true;
                } );
            }
        }
        else
        {
            if (giftBoxButtonObject.activeSelf == true)
            {
                giftBoxButton.interactable = false;
                LeanTween.scale( giftBoxButton.gameObject, Vector3.zero, 0.5f ).setEaseInBack().setOnComplete( () =>
                {
                    giftBoxButtonObject.SetActive( false );
                } );
            }
        }

        int[] _unlockedDecorativeItemIds = spaceDenProgress.unlocked_decorative_item_ids;
        for (int i = 0; i < _unlockedDecorativeItemIds.Length; i++)
        {
            int _unlockedDecorativeItemId = _unlockedDecorativeItemIds[ i ];
            for (int j = 0; j < decorativeItems.Length; j++)
            {
                DecorativeItem _item = decorativeItems[ i ];
                if (_item.GetItemId() == _unlockedDecorativeItemId)
                {
                    if (currentUnlockedDecorativeItemIdList.Contains( _unlockedDecorativeItemId ) == false)
                    {
                        _item.Unlock();
                        currentUnlockedDecorativeItemIdList.Add( _unlockedDecorativeItemId );
                        SoundManager.Instance.PlaySoundEffect( decorativeItemPopUpClip );
                    }
                    else if (_item.GetIsShowing() == false)
                    {
                        _item.Show();
                    }

                    break;
                }
            }
        }

        if (hasMaximumLoveRankReached == false)
        {
            if (spaceDenProgress.current_love_rank >= spaceDenProgress.maximum_love_rank)
            {
                hasMaximumLoveRankReached = true;
                StartCoroutine( DelayToShowMaxLoveRankReachedPanel() );
            }
        }
    }

    private IEnumerator DelayToShowMaxLoveRankReachedPanel()
    {
        yield return new WaitUntil( () => isInCrisisMissionMode == false );
        yield return new WaitUntil( () => errorMessagePanel.GetIsShowing() == false );
        maxLoveRankReachedPanel.Show();
        PlayNormalMessagePopUpSound();
    }

    private void UpdateLoveRankFillBar()
    {
        _loveRankFillBar.UpdateFillAmount( currentLovePoints / maximumLovePoints );
    }

    public void GoToGameScene()
    {
        SceneControlManager.GoToGameScene();
    }

    private void DelayToTriggerPlayermonToRequest()
    {
        float _randomInterval = playermonRequestInterval + ( UnityEngine.Random.value * playermonRequestIntervalRandom );
        Invoke( "TriggerPlayermonToRequest", _randomInterval );
    }

    private void TriggerPlayermonToRequest()
    {
        List<SpaceDenPlayermon> _availablePlayermons = new List<SpaceDenPlayermon>();
        for (int i = 0; i < spaceDenPlayermons.Count; i++)
        {
            SpaceDenPlayermon _spaceDenPlayermon = spaceDenPlayermons[ i ];
            if (_spaceDenPlayermon.GetCurrentRequestedAction() == SpaceDenPlayermon.RequestedAction.NONE)
            {
                _availablePlayermons.Add( _spaceDenPlayermon );
            }
        }

        if (_availablePlayermons.Count > 0)
        {
            _availablePlayermons[ UnityEngine.Random.Range( 0, _availablePlayermons.Count ) ].ShowRequest( UnityEngine.Random.Range( 1, 4 ) );
        }

        DelayToTriggerPlayermonToRequest();
    }

    public void StartCrisisMission()
    {
        isInCrisisMissionMode = true;
        loadingScreenObject.SetActive( true );

        CancelInvoke( "TriggerPlayermonToRequest" );
        settingButton.interactable = false;
        instructionButton.interactable = false;
        giftBoxButton.interactable = false;
        RequestForCrisisMission();
    }

    public void OnPlayermonReadyForCrisis()
    {
        if (isCrisisMissionStarted == true)
        {
            crisisReadyCount++;
            if (crisisReadyCount >= spaceDenPlayermons.Count)
            {
                isCrisisMissionStarted = false;
                crisisMissionRef.SetIsMissionReadyToPlay( true );
            }
        }
    }

    public void AddLovePoints( int amount )
    {
        if (hasMaximumLoveRankReached == false)
        {
            currentLovePoints += amount;
            UpdateLoveRankFillBar();

            if (currentLovePoints >= currentSpaceDenProgress.maximum_love_points)
            {
                SubmitCompletedRequestList();
            }
        }
    }

    public void OnPlayermonRequestCompleted( int playermonIndex, SpaceDenPlayermon.RequestedAction targetRequestedAction )
    {
        completedPlayermonRequestList.Add( new CompletedPlayermonRequest( playermonIndex, ( int )targetRequestedAction, GameTimeManager.Instance.GetCurrentUnixTimestampInMilliseconds() ) );
    }

    public bool OnPlayermonRequestCompletedForCrisis( int playermonIndex, SpaceDenPlayermon.RequestedAction targetRequestedAction, double timeTakenInSeconds )
    {
        int _actionType = ( int )targetRequestedAction;
        if (crisisMissionRef.CheckPlayermonRequest( playermonIndex, _actionType ) == true)
        {
            crisisCompletedPlayermonRequestList.Add( new CrisisCompletedPlayermonRequest( playermonIndex, _actionType, timeTakenInSeconds, GameTimeManager.Instance.GetCurrentUnixTimestampInMilliseconds() ) );
        }
        else
        {
            OnMissionFailed();
        }

        if (crisisMissionRef.IsMissionComplete() == true)
        {
            loadingScreenObject.SetActive( true );
            CancelAllItemDragging();
            crisisMissionRef.StopMission();
            SubmitCrisisMissionResult();

            return true;
        }

        return false;
    }

    public void OnMissionComplete( int rewardedSgemAmount )
    {
        isShowingCrisisMissionResult = true;
        crisisMissionResultPanel.ShowMissionComplete( rewardedSgemAmount );

        for (int i = 0; i < spaceDenPlayermons.Count; i++)
        {
            SpaceDenPlayermon _spaceDenPlayermon = spaceDenPlayermons[ i ];
            _spaceDenPlayermon.PlayCrisisSuccessAnimation();
        }

        SoundManager.Instance.PlayBackgroundMusic( crisisSuccessClip );
    }

    public void OnMissionCompleteTwo()
    {
        isShowingCrisisMissionResult = true;
        crisisMissionResultPanel.ShowMissionCompleteWithoutPlayermon();

        for (int i = 0; i < spaceDenPlayermons.Count; i++)
        {
            SpaceDenPlayermon _spaceDenPlayermon = spaceDenPlayermons[ i ];
            _spaceDenPlayermon.PlayCrisisSuccessAnimation();
        }

        SoundManager.Instance.PlayBackgroundMusic( crisisSuccessClip );
    }

    public void OnMissionFailed()
    {
        CancelAllItemDragging();
        crisisMissionRef.StopMission();
        isShowingCrisisMissionResult = true;
        crisisMissionResultPanel.ShowMissionFailed();

        for (int i = 0; i < spaceDenPlayermons.Count; i++)
        {
            SpaceDenPlayermon _spaceDenPlayermon = spaceDenPlayermons[ i ];
            _spaceDenPlayermon.HideAllRequests();
            _spaceDenPlayermon.PlayCrisisFailedAnimation();
        }

        SoundManager.Instance.PlayBackgroundMusic( _normalBGM );
        SoundManager.Instance.PlaySoundEffect( crisisFailedClip );
    }

    public void OnMissionResultPanelClosed( bool isMissionComplete )
    {
        isShowingCrisisMissionResult = false;

        for (int i = 0; i < spaceDenPlayermons.Count; i++)
        {
            SpaceDenPlayermon _spaceDenPlayermon = spaceDenPlayermons[ i ];
            _spaceDenPlayermon.CrisisOver();
            //_spaceDenPlayermon.PlayIdleAnimation();
        }

        if (isMissionComplete == true)
        {
            SoundManager.Instance.PlayBackgroundMusic( _normalBGM );
        }
        else
        {
            crisisMissionPanel.EnableCrisisButton();
        }

        settingButton.interactable = true;
        instructionButton.interactable = true;
        giftBoxButton.interactable = true;
        DelayToTriggerPlayermonToRequest();
        isInCrisisMissionMode = false;
    }

    private void CancelAllItemDragging()
    {
        for (int i = 0; i < itemDragDropComponents.Length; i++)
        {
            DragDrop _itemDragDropComponent = itemDragDropComponents[ i ];
            _itemDragDropComponent.CancelDrag();
            _itemDragDropComponent.GetFillBarProgress().Reset();
        }

        for (int i = 0; i < spaceDenPlayermons.Count; i++)
        {
            SpaceDenPlayermon _spaceDenPlayermon = spaceDenPlayermons[ i ];
            _spaceDenPlayermon.StopFeeding();
            _spaceDenPlayermon.StopPlaying();
            _spaceDenPlayermon.StopBathing();
        }
    }

    public SpaceDenCrisisMission GetCrisisMissionRef()
    {
        return crisisMissionRef;
    }

    public bool GetIsShowingCrisisMissionResult()
    {
        return isShowingCrisisMissionResult;
    }

    public void ClickToShowInstruction()
    {
        SoundManager.Instance.PlayPositiveClickingClip();
        instructionPanel.Show();
    }

    public void OnInstructionPanelClosed()
    {

    }

    public void ClickToViewMarketplace()
    {
        SoundManager.Instance.PlayPositiveClickingClip();
        ExternalLinkManager.Instance.OpenMarketplaceUrl();
    }

    public void ClickToOpenItemPurchasePanel()
    {
        if (itemPurchasePanel.GetIsPlayingAnimation() == true)
        {
            return;
        }

        SoundManager.Instance.PlayPositiveClickingClip();
        itemPurchasePanel.Show();
        PlayNormalMessagePopUpSound();
    }

    public void ClickToCloseItemPurchasePanel()
    {
        if (itemPurchasePanel.GetIsPlayingAnimation() == true)
        {
            return;
        }

        SoundManager.Instance.PlayNegativeClickingClip();
        itemPurchasePanel.Hide();
    }

    int _itemId = 1;
    public void ClickToOpenGiftBox()
    {
        if (itemPurchasePanel.GetIsPlayingAnimation() == true)
        {
            return;
        }

        loadingScreenObject.SetActive( true );

        SoundManager.Instance.PlayPositiveClickingClip();
        itemPurchasePanel.Hide();
        UnlockDecorativeItemInSpaceDen();

        /*
        for (int i = 0; i < decorativeItems.Length; i++)
        {
            DecorativeItem _item = decorativeItems[ i ];
            if (_item.GetItemId() == _itemId)
            {
                giftBoxNumberLabel.text = ( 10 - _itemId ).ToString();
                _item.Unlock();
                _itemId++;

                SoundManager.Instance.PlaySoundEffect( decorativeItemPopUpClip );

                break;
            }
        }

        if (_itemId > 10)
        {
            giftBoxButton.interactable = false;
            LeanTween.scale( giftBoxButton.gameObject, Vector3.zero, 0.5f ).setEaseInBack().setOnComplete( () =>
            {
                giftBoxButton.gameObject.SetActive( false );
            } );
        }
        */
    }

    public void ClickToCloseMaxLoveRankReachedPanel()
    {
        SoundManager.Instance.PlayPositiveClickingClip();
        maxLoveRankReachedPanel.Hide();
    }

    public void ClickToCloseErrorMessagePanel()
    {
        SoundManager.Instance.PlayPositiveClickingClip();
        errorMessagePanel.Hide();
    }

    public void ClickToOpenSettingPanel()
    {
        SoundManager.Instance.PlayPositiveClickingClip();
        settingPanelRef.OpenSettingPanel();
    }

    public void ClickToExitScene()
    {
        SoundManager.Instance.PlayPositiveClickingClip();
        ExitScene();
    }

    private void ExitScene()
    {
        SceneControlManager.GoToMainMenuScene();
    }

    public void PlayNormalMessagePopUpSound()
    {
        SoundManager.Instance.PlaySoundEffect( normalMessagePopUpClip );
    }

    public void PlayErrorMessagePopUpSound()
    {
        SoundManager.Instance.PlaySoundEffect( errorMessagePopUpClip );
    }

#region API Processes

    // Get SpaceDen Progress
    private void RequestSpaceDenProgress()
    {
        if (isUsingTestData == true)
        {
            TextAsset _textAsset = Resources.Load<TextAsset>( "TestData/GetSpaceDenProgress_" + UserProfileManager.Instance.GetUserAccessToken() );
            if (_textAsset == null)
            {
                _textAsset = Resources.Load<TextAsset>( "TestData/GetSpaceDenProgress_tester_5" );
            }

            OnSpaceDenProgressRequestingComplete( UnityWebRequest.Result.Success, _textAsset.text );
        }
        else
        {
            PlayermonInResponse[] _playermons = null;
            TeamInResponse _selectedTeam = UserProfileManager.Instance.GetSelectedTeam();
            if (_selectedTeam != null)
            {
                _playermons = _selectedTeam.playermons;
            }
            else
            {
                _playermons = UserProfileManager.Instance.GetRandomPlayermons();
            }

            WWWForm _wwwForm = new WWWForm();
            if (_playermons.Length > 0)
            {
                _wwwForm.AddField( "playermon_one_id", _playermons[ 0 ].id );
            }
            if (_playermons.Length > 1)
            {
                _wwwForm.AddField( "playermon_two_id", _playermons[ 1 ].id );
            }
            if (_playermons.Length > 2)
            {
                _wwwForm.AddField( "playermon_three_id", _playermons[ 2 ].id );
            }

            ServerApiManager.Post( GET_SPACE_DEN_PROGRESS, UserProfileManager.GetServerApiHeaders( true, ServerApiManager.HeaderContentType.APPLICATION_WWW_FORM ), _wwwForm, OnSpaceDenProgressRequestingComplete );
        }
    }

    private void OnSpaceDenProgressRequestingComplete( UnityWebRequest.Result result, string resultText )
    {
        Debug.Log( "resultText = " + resultText );

        bool _isProcessSuccessful = false;
        if (result == UnityWebRequest.Result.Success)
        {
            GetSpaceDenProgressResponse _response = JsonConvert.DeserializeObject<GetSpaceDenProgressResponse>( resultText );
            if (_response.status == 1)
            {
                _isProcessSuccessful = true;

                spaceDenData = _response.data;

                playermonRequestInterval = spaceDenData.playermon_request_interval_in_seconds;
                playermonRequestIntervalRandom = spaceDenData.playermon_request_interval_random_in_seconds;
                playerActionSubmissionInterval = spaceDenData.player_action_submission_interval_in_seconds;

                GetUserProfileResponse_Data_BasicUserProfile _userProfile = spaceDenData.user_profile;
                GetSpaceDenProgressResponse_Data_SpaceDenProgress _spaceDenProgress = spaceDenData.spaceden_progress;
                GetSpaceDenProgressResponse_Data_SpaceDenAction[] _spaceDenActions = spaceDenData.spaceden_actions;

                int[] _unlockedDecorativeItemIds = _spaceDenProgress.unlocked_decorative_item_ids;
                for (int i = 0; i < _unlockedDecorativeItemIds.Length; i++)
                {
                    currentUnlockedDecorativeItemIdList.Add( _unlockedDecorativeItemIds[ i ] );
                }
                UpdateUserProfileInformation( _userProfile, _spaceDenProgress );

                spaceDenPlayermons = new List<SpaceDenPlayermon>();
                PlayermonInResponse[] _playermons = spaceDenData.playermons;
                for (int i = 0; i < _playermons.Length; i++)
                {
                    SpaceDenPlayermon _spaceDenPlayermon = null;

                    PlayermonInResponse _playermon = _playermons[ i ];
                    if (_playermon.id != -1)
                    {
                        Creature _creature = creatures[ i ];
                        _creature.SetUp( new CreatureData( _playermon, true, true ), 1.0f, false );
                        _creature.GetPlayermonActionsRef().SetDefaultSortingLayer( "Playermon", i );
                        _spaceDenPlayermon = _creature.GetSpaceDenPlayermonRef();

                        _creature.gameObject.SetActive( true );
                    }
                    else
                    {
                        GameObject _dummyCreatureObj = dummyCreatureObjects[ i ];
                        _dummyCreatureObj.GetComponent<PlayermonActions>().SetDefaultSortingLayer( "Playermon", i );
                        _spaceDenPlayermon = _dummyCreatureObj.GetComponent<SpaceDenPlayermon>();

                        _dummyCreatureObj.SetActive( true );
                    }

                    _spaceDenPlayermon.SetUp( this, _playermon.index, _spaceDenActions );
                    spaceDenPlayermons.Add( _spaceDenPlayermon );
                }

                crisisMissionRef.SetUp( spaceDenPlayermons );
                completedPlayermonRequestList = new List<CompletedPlayermonRequest>();
                DelayToTriggerPlayermonToRequest();

                instructionPanel.SetUp( this, spaceDenData );
                if (spaceDenData.is_first_time == true)
                {
                    instructionPanel.Show( true );
                }

                GetSpaceDenProgressResponse_Data_SpaceDenCrisis_SgemRewardForCompletion _sgemReward = spaceDenData.spaceden_crisis.sgem_reward_for_completion;
                int _numberOfActualPlayers = spaceDenData.number_of_actual_playermons;
                int _rewardedSgemAmount = 0;
                if (_numberOfActualPlayers == 0)
                {
                    _rewardedSgemAmount = _sgemReward.zero_actual_playermon;
                }
                else if (_numberOfActualPlayers == 1)
                {
                    _rewardedSgemAmount = _sgemReward.one_actual_playermon;
                }
                else if (_numberOfActualPlayers == 2)
                {
                    _rewardedSgemAmount = _sgemReward.two_actual_playermons;
                }
                else if (_numberOfActualPlayers == 3)
                {
                    _rewardedSgemAmount = _sgemReward.three_actual_playermons;
                }

                crisisMissionPanel.SetCrisisReward( _rewardedSgemAmount );

                isSpaceDenStarted = true;
                spaceDenActionSubmissionStartTime = Time.time;

                loadingScreenObject.SetActive( false );
            }
        }

        if (_isProcessSuccessful == false)
        {
            Invoke( "RequestSpaceDenProgress", apiCallingRetryingInterval );
        }
    }

    // Acknowledge that User Has Seen SpaceDen
    private void AcknowledgeThatUserHasSeenSpaceDen()
    {
        if (isUsingTestData == true)
        {
            OnAcknowledgingThatUserHasSeenSpaceDenComplete( UnityWebRequest.Result.Success, Resources.Load<TextAsset>( "TestData/SuccessfulResponse" ).text );
        }
        else
        {

        }
    }

    private void OnAcknowledgingThatUserHasSeenSpaceDenComplete( UnityWebRequest.Result result, string resultText )
    {
        Debug.Log( "resultText = " + resultText );

        bool _isProcessSuccessful = false;
        if (result == UnityWebRequest.Result.Success)
        {
            ServerApiManager.ApiResponse _response = JsonConvert.DeserializeObject<ServerApiManager.ApiResponse>( resultText );
            if (_response.status == 1)
            {
                _isProcessSuccessful = true;
            }
        }

        if (_isProcessSuccessful == false)
        {

        }
    }

    // Submit Actions that Earn Love Points
    private void SubmitCompletedRequestList()
    {
        if (completedPlayermonRequestList.Count > 0)
        {
            string _resultString = JsonConvert.SerializeObject( completedPlayermonRequestList );

            Debug.Log( "_resultString = " + _resultString );

            if (isUsingTestData == true)
            {
                OnSubmittingCompletedRequestListComplete( UnityWebRequest.Result.Success, Resources.Load<TextAsset>( "TestData/SpaceDenActionSubmission" ).text );
            }
            else
            {

            }
        }
    }

    private void OnSubmittingCompletedRequestListComplete( UnityWebRequest.Result result, string resultText )
    {
        Debug.Log( "resultText = " + resultText );

        bool _isProcessSuccessful = false;
        if (result == UnityWebRequest.Result.Success)
        {
            GetSpaceDenProgressUpdateResponse _response = JsonConvert.DeserializeObject<GetSpaceDenProgressUpdateResponse>( resultText );
            if (_response.status == 1)
            {
                _isProcessSuccessful = true;
                completedPlayermonRequestList = new List<CompletedPlayermonRequest>();
                UpdateSpaceDenProgressInformation( _response.data.spaceden_progress );
            }
        }

        if (_isProcessSuccessful == false)
        {

        }
    }

    // Activate Crisis in SpaceDen
    private void RequestForCrisisMission()
    {
        if (isUsingTestData == true)
        {
            OnRequestingForCrisisMissionComplete( UnityWebRequest.Result.Success, Resources.Load<TextAsset>( "TestData/SpaceDenCrisisMission_" + UnityEngine.Random.Range( 1, 11 ) ).text );
        }
        else
        {
            ServerApiManager.Get( ACTIVATE_CRISIS_IN_SPACEDEN, UserProfileManager.GetServerApiHeaders( false ), OnRequestingForCrisisMissionComplete );
        }
    }

    private void OnRequestingForCrisisMissionComplete( UnityWebRequest.Result result, string resultText )
    {
        Debug.Log( "resultText = " + resultText );

        bool _isProcessSuccessful = false;
        if (result == UnityWebRequest.Result.Success)
        {
            GetSpaceDenCrisisResponse _response = JsonConvert.DeserializeObject<GetSpaceDenCrisisResponse>( resultText );
            if (_response.status == 1)
            {
                _isProcessSuccessful = true;

                SoundManager.Instance.PlaySoundEffect( crisisActivatedClip );
                SoundManager.Instance.PlayBackgroundMusic( _crisisBGM );
                crisisMissionRef.SetIsMissionReadyToPlay( false );

                isCrisisMissionStarted = true;
                crisisReadyCount = 0;
                for (int i = 0; i < spaceDenPlayermons.Count; i++)
                {
                    spaceDenPlayermons[ i ].GetReadyForCrisis();
                }

                crisisCompletedPlayermonRequestList = new List<CrisisCompletedPlayermonRequest>();
                crisisMissionRef.StartMission( _response.data );
            }
        }

        if (_isProcessSuccessful == false)
        {
            errorMessagePanel.Show();
            PlayErrorMessagePopUpSound();

            crisisMissionPanel.EnableCrisisButton();
            settingButton.interactable = true;
            instructionButton.interactable = true;
            giftBoxButton.interactable = true;
            DelayToTriggerPlayermonToRequest();
            isInCrisisMissionMode = false;
        }

        loadingScreenObject.SetActive( false );
    }

    // Submit the Result for the Crisis in SpaceDen
    private void SubmitCrisisMissionResult()
    {
        string _resultString = JsonConvert.SerializeObject( crisisCompletedPlayermonRequestList );

        Debug.Log( "_resultString = " + _resultString );

        if (isUsingTestData == true)
        {
            TextAsset _textAsset = Resources.Load<TextAsset>( "TestData/SpaceDenCrisisMissionComplete_" + UserProfileManager.Instance.GetUserAccessToken() );
            if (_textAsset == null)
            {
                _textAsset = Resources.Load<TextAsset>( "TestData/SpaceDenCrisisMissionComplete" );
            }

            OnSubmittingCrisisMissionResultComplete( UnityWebRequest.Result.Success, _textAsset.text );
        }
        else
        {

        }
    }

    private void OnSubmittingCrisisMissionResultComplete( UnityWebRequest.Result result, string resultText )
    {
        bool _isProcessSuccessful = false;
        if (result == UnityWebRequest.Result.Success)
        {
            GetSpaceDenCrisisResultResponse _response = JsonConvert.DeserializeObject<GetSpaceDenCrisisResultResponse>( resultText );
            if (_response.status == 1)
            {
                _isProcessSuccessful = true;

                GetSpaceDenCrisisResultResponse_Data _responseData = _response.data;
                GetSpaceDenProgressResponse_Data_SpaceDenProgress _spaceDenProgress = _responseData.spaceden_progress;
                UpdateUserProfileInformation( _responseData.user_profile, _spaceDenProgress );

                if (_responseData.is_mission_complete == 1)
                {
                    if (_responseData.number_of_actual_playermons > 0)
                    {
                        OnMissionComplete( _spaceDenProgress.crisis_sgem_tokens_rewarded );
                    }
                    else
                    {
                        OnMissionCompleteTwo();
                    }
                }
                else
                {
                    OnMissionFailed();
                }

                loadingScreenObject.SetActive( false );
            }
        }

        if (_isProcessSuccessful == false)
        {
            Invoke( "SubmitCrisisMissionResult", apiCallingRetryingInterval );
        }
    }

    // Unlock Decorative Item in SpaceDen
    private void UnlockDecorativeItemInSpaceDen()
    {
        if (isUsingTestData == true)
        {
            OnUnlockingDecorativeItemInSpaceDenComplete( UnityWebRequest.Result.Success, Resources.Load<TextAsset>( "TestData/UnlockDecorativeItemInSpaceDen" ).text );
        }
        else
        {

        }
    }

    private void OnUnlockingDecorativeItemInSpaceDenComplete( UnityWebRequest.Result result, string resultText )
    {
        Debug.Log( "resultText = " + resultText );

        bool _isProcessSuccessful = false;
        if (result == UnityWebRequest.Result.Success)
        {
            GetSpaceDenProgressUpdateResponse _response = JsonConvert.DeserializeObject<GetSpaceDenProgressUpdateResponse>( resultText );
            if (_response.status == 1)
            {
                _isProcessSuccessful = true;

                GetSpaceDenProgressUpdateResponse_Data _responseData = _response.data;
                GetSpaceDenProgressResponse_Data_SpaceDenProgress _spaceDenProgress = _responseData.spaceden_progress;
                UpdateUserProfileInformation(_responseData.user_profile, _spaceDenProgress);
            }
        }

        if (_isProcessSuccessful == false)
        {
            errorMessagePanel.Show();
            PlayErrorMessagePopUpSound();
        }

        loadingScreenObject.SetActive( false );
    }

#endregion

#region Inner Classes 

    public class CompletedPlayermonRequest
    {
        public int playermon_index;
        public int action_type;
        public long timestamp_in_ms;

        public CompletedPlayermonRequest( int playermonIndex, int actionType, long timestampInMilliseconds )
        {
            playermon_index = playermonIndex;
            action_type = actionType;
            timestamp_in_ms = timestampInMilliseconds;
        }
    }

    public class CrisisCompletedPlayermonRequest
    {
        public int playermon_index;
        public int action_type;
        public long time_taken_in_ms;
        public long timestamp_in_ms;

        public CrisisCompletedPlayermonRequest( int playermonIndex, int actionType, double timeTakenInSeconds, long timestampInMilliseconds )
        {
            playermon_index = playermonIndex;
            action_type = actionType;
            time_taken_in_ms = (int)Math.Round( timeTakenInSeconds * 1000 );
            timestamp_in_ms = timestampInMilliseconds;
        }
    }

    [Serializable]
    public class DecorativeItem
    {
        [SerializeField] private int itemId = 0;
        [SerializeField] private GameObject itemObject = null;

        public void Show()
        {
            itemObject.SetActive( true );
        }

        public void Unlock()
        {
            itemObject.transform.localScale = Vector3.zero;
            itemObject.SetActive( true );
            LeanTween.scale( itemObject, Vector3.one, 0.3f ).setEaseOutCirc().setDelay( 0.3f );
        }

        public int GetItemId()
        {
            return itemId;
        }

        public bool GetIsShowing()
        {
            return itemObject.activeSelf;
        }
    }

#endregion
}

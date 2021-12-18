using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using ServerApiResponse;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    [Header( "Settings" )]
    [SerializeField] private float announcementShowingInterval = 5.0f;
    [SerializeField] private float spaceshipHoveringHeight = 0.5f;
    [SerializeField] private float spaceshipHoveringDuration = 1.0f;

    [Header( "UI" )]
    [SerializeField] private Creature creaturePrefab;
    [SerializeField] private Transform[] creatureStandingPoints;
    [SerializeField] private RawImage announcementRawImage;
    [SerializeField] private TextMeshProUGUI usernameLabel;
    [SerializeField] private Text titleLabel;
    [SerializeField] private TextMeshProUGUI sgemAmountLabel;
    [SerializeField] private TextMeshProUGUI energyBarLabel;
    [SerializeField] private Text energyCountdownTimerLabel;
    [SerializeField] private SettingPanel settingPanelRef;

    [Header( "References" )]
    [SerializeField] private GameObject spaceshipObject;

    [Header( "Sound" )]
    [SerializeField] private AudioClip backgroundMusicClip;

    private bool isEnergyCountdownTimerRunning = false;

    void Awake()
    {
        GetUserProfileResponse_Data_UserProfile _userProfileData = UserProfileManager.Instance.GetUserProfileData();
        usernameLabel.text = _userProfileData.username;
        titleLabel.text = _userProfileData.title;
        sgemAmountLabel.text = _userProfileData.sgem_tokens.ToString();
        energyBarLabel.text = _userProfileData.current_energy_points.ToString() + "/" + _userProfileData.maximum_energy_points.ToString();

        isEnergyCountdownTimerRunning = ( _userProfileData.current_energy_points < _userProfileData.maximum_energy_points );

        if (isEnergyCountdownTimerRunning == true)
        {
            GameTimeManager.Instance.SetCountdownTime( _userProfileData.energy_update_remaining_seconds );
            GameTimeManager.Instance.StartToUpdate();
        }
        else
        {
            energyCountdownTimerLabel.gameObject.SetActive( false );
        }

        GameObject _creaturePrefabObject = creaturePrefab.gameObject;

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

        for (int i = 0; i < _playermons.Length; i++)
        {
            GameObject _creatureObject = Instantiate( _creaturePrefabObject );
            Creature _creature = _creatureObject.GetComponent<Creature>();
            _creature.SetUp( new CreatureData( _playermons[ i ], true ), 1.5f, false );

            _creatureObject.transform.position = creatureStandingPoints[ i ].position;
            _creature.GetPlayermonActionsRef().SetDefaultSortingLayer( "Playermon", i );
        }
    }

    void Start()
    {
        if (SceneControlManager.GetLastSceneName() != SceneControlManager.GAME_LOADING_SCENE_NAME)
        {
            SoundManager.Instance.PlayBackgroundMusic( backgroundMusicClip );
        }

        if (AnnouncementManager.Instance.GetAnnouncementCount() > 0)
        {
            announcementRawImage.gameObject.SetActive( true );
            StartCoroutine( RunLoadingAnnouncements() );
        }

        LeanTween.moveLocalY( spaceshipObject, spaceshipObject.transform.localPosition.y + spaceshipHoveringHeight, spaceshipHoveringDuration ).setLoopPingPong();
    }

    void Update()
    {
        if (isEnergyCountdownTimerRunning == true)
        {
            TimeSpan _timeSpan = GameTimeManager.Instance.GetRemainingTime();
            energyCountdownTimerLabel.text = string.Format( "{0:D2} : {1:D2} : {2:D2}", _timeSpan.Hours, _timeSpan.Minutes, _timeSpan.Seconds );
        }
    }

    private IEnumerator RunLoadingAnnouncements()
    {
        AnnouncementManager _announcementManager = AnnouncementManager.Instance;
        int _announcementCount = _announcementManager.GetAnnouncementCount();

        while ( true )
        {
            for (int i = 0; i < _announcementCount; i++)
            {
                yield return new WaitUntil( () => _announcementManager.GetAnnouncementImage( i ) != null );
                announcementRawImage.texture = _announcementManager.GetAnnouncementImage( i );
                yield return new WaitForSeconds( announcementShowingInterval );
            }
        }
    }

    public void ClickToOpenTutorialPage()
    {
        SoundManager.Instance.PlayPositiveClickingClip();
        ExternalLinkManager.Instance.OpenTutorialUrl();
    }

    public void ClickToOpenSettingPanel()
    {
        SoundManager.Instance.PlayPositiveClickingClip();
        settingPanelRef.OpenSettingPanel();
    }

    public void ClickToTeamManagement()
    {
        SoundManager.Instance.PlayPositiveClickingClip();
        SceneControlManager.GoToTeamManagementScene();
    }

    public void ClickToPlayermonManagement()
    {
        SoundManager.Instance.PlayPositiveClickingClip();
        SceneControlManager.GoToPlayermonManagementScene();
    }

    public void ClickToSpaceDenGame()
    {
        SoundManager.Instance.PlayPositiveClickingClip();
        SceneControlManager.GoToSpaceDenGameScene();
    }

    public void ClickToAdventureMode()
    {
        SoundManager.Instance.PlayPositiveClickingClip();
        SceneControlManager.GoToBattleGameScene();
    }

    public void ClickToBattlegroundMode()
    {
        SoundManager.Instance.PlayPositiveClickingClip();
        SceneControlManager.GoToBattleGameScene();
    }

    public void ClickToLogOut()
    {
        SoundManager.Instance.PlayPositiveClickingClip();
        SceneControlManager.GoToUserLoginScene();
    }
}

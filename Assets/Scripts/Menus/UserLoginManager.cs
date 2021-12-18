using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using ServerApiResponse;
using Newtonsoft.Json;

public class UserLoginManager : MonoBehaviour
{
    [Header( "Settings" )]
    [SerializeField] private float apiCallingRetryingInterval = 10.0f;
    [SerializeField] private float loginPanelMovingDuration = 0.3f;
    [SerializeField] private bool isUsingTestData = false;

    [Header( "UI" )]
    [SerializeField] private GameObject loginPanelObject;
    [SerializeField] private Transform loginPanelTargetPoint;
    [SerializeField] private InputField accessTokenInputField;
    [SerializeField] private PopUpMessageBox resultMessageBox;
    [SerializeField] private PopUpMessageBoxBasic forceUpdatePanel;
    [SerializeField] private Text forceUpdateMessageLabel;
    [SerializeField] private GameObject loadingScreenObject;

    [Header( "Systems" )]
    [SerializeField] private Transform barcodeDecoderContainer;
    [SerializeField] private BarcodeDecoder barcodeDecoderPrefab;
    [SerializeField] private GameObject barcodeDecoderCancelButtonObject;
    [SerializeField] private FileBrowserSystem fileBrowserSystemRef;

    [Header( "Sound" )]
    [SerializeField] private AudioClip backgroundMusicClip;
    [SerializeField] private AudioClip loginSuccessClip;
    [SerializeField] private AudioClip errorMessageClip;
    [SerializeField] private AudioClip messagePanelClip;
    [SerializeField] private AudioClip messageButtonClip;

    private readonly string MINIMUM_VERSION_API = ServerApiManager.DOMAIN + "playermon/playermonConfig/min-app-version";
    private readonly string USER_LOGIN_API = ServerApiManager.DOMAIN + "playermon/auth/game/userProfile";

    private Vector3 loginPanelStartPoint;
    private GameObject barcodeDecoderPrefabObject = null;
    private BarcodeDecoder barcodeDecoderComponent = null;
    private string appDownloadUrl = "";

    void Awake()
    {
        barcodeDecoderPrefabObject = barcodeDecoderPrefab.gameObject;
        fileBrowserSystemRef.onResultCallback = OnQrCodeSelected;

        // Somehow the code needs to run this first so that the GetUserProfileResponse JSON data
        // can be deserialized in the OnProcessingLoginComplete() function.
        JsonConvert.DeserializeObject<GetUserProfileResponse>( Resources.Load<TextAsset>( "TestData/UserProfile_tester_1" ).text );
    }

    void Start()
    {
        if (SceneControlManager.GetLastSceneName() != SceneControlManager.MAIN_MENU_SCENE_NAME)
        {
            SoundManager.Instance.PlayBackgroundMusic( backgroundMusicClip );
        }

        RetrieveMinimumVersion();
    }

    private void RetrieveMinimumVersion()
    {
        if (isUsingTestData == true)
        {
            OnRetrievingMinimumVersionComplete( UnityWebRequest.Result.Success, Resources.Load<TextAsset>( "TestData/MinimumAppVersion" ).text );
        }
        else
        {
            Dictionary<string,string> _headers = ServerApiManager.GetHeaders( false );
            ServerApiManager.Get( MINIMUM_VERSION_API, _headers, OnRetrievingMinimumVersionComplete );
        }
    }

    private void OnRetrievingMinimumVersionComplete( UnityWebRequest.Result result, string resultText )
    {
        Debug.Log( "resultText = " + resultText );

        bool _isProcessSuccessful = false;
        if (result == UnityWebRequest.Result.Success)
        {
            GetMinimumAppVersionResponse _response = JsonConvert.DeserializeObject<GetMinimumAppVersionResponse>( resultText );
            if (_response.status == 1)
            {
                _isProcessSuccessful = true;

                GetMinimumAppVersionResponse_Data _responseData = _response.data;
                appDownloadUrl = _responseData.app_download_url;

                string _minimumAppVersion = _responseData.minimum_app_version;
                bool _needToUpdate = false;
                try
                {
                    Version _currentVersion = new Version( Application.version );
                    Version _minimumRequiredVersion = new Version( _minimumAppVersion );
                    int _result = _currentVersion.CompareTo( _minimumRequiredVersion );
                    if (_result < 0)
                    {
                        _needToUpdate = true;
                    }
                }
                catch (Exception e)
                {
                    Debug.Log( "Version is invalid format: " + e.ToString() );
                }

                if (_needToUpdate == true)
                {
                    forceUpdateMessageLabel.text = "This version (" + Application.version + ") is out of date. Please update to the latest version.";
                    forceUpdatePanel.Show();
                    SoundManager.Instance.PlaySoundEffect( messagePanelClip );
                }
                else
                {
                    loginPanelStartPoint = loginPanelObject.transform.position;
                    loginPanelObject.SetActive( true );
                    LeanTween.move( loginPanelObject, loginPanelTargetPoint.position, loginPanelMovingDuration ).setEaseOutCirc();
                }

                loadingScreenObject.SetActive( false );
            }
        }

        if (_isProcessSuccessful == false)
        {
            Invoke( "apiCallingRetryingInterval", apiCallingRetryingInterval );
        }
    }

    public void ClickToOpenAppDownloadPage()
    {
        SoundManager.Instance.PlaySoundEffect( messageButtonClip );
        Application.OpenURL( appDownloadUrl );
    }

    private void SetToScreenAutoRotation()
    {
        if (Input.deviceOrientation == DeviceOrientation.LandscapeLeft
            || Input.deviceOrientation == DeviceOrientation.Portrait)
        {
            Screen.orientation = ScreenOrientation.LandscapeLeft;
            Screen.orientation = ScreenOrientation.AutoRotation;
        }
        else
        {
            Screen.orientation = ScreenOrientation.LandscapeRight;
            Screen.orientation = ScreenOrientation.AutoRotation;
        }
    }

    private void OnQrCodeScanned( string result )
    {
        SetToScreenAutoRotation();
        Destroy( barcodeDecoderComponent.gameObject );
        barcodeDecoderCancelButtonObject.SetActive( false );

        accessTokenInputField.text = result;
        ProcessLogin( result );
    }

    private void OnQrCodeSelected( bool success, string result )
    {
        SetToScreenAutoRotation();

        if (success == true)
        {
            accessTokenInputField.text = result;
            ProcessLogin( result );
        }
        else
        {
            ShowResultMessage( result );
        }
    }

    public void ClickToScanQrCode()
    {
        SoundManager.Instance.PlayPositiveClickingClip();

        Screen.orientation = ScreenOrientation.Portrait;
        HideResultMessage();
        SetUpQrCodeScanner();
        barcodeDecoderCancelButtonObject.SetActive( true );
    }

    private void SetUpQrCodeScanner()
    {
        GameObject _barcodeDecoderObj = Instantiate( barcodeDecoderPrefabObject );
        _barcodeDecoderObj.transform.SetParent( barcodeDecoderContainer, false );

        barcodeDecoderComponent = _barcodeDecoderObj.GetComponent<BarcodeDecoder>();
        barcodeDecoderComponent.onResultCallback = OnQrCodeScanned;
        barcodeDecoderComponent.onResetRequired = ResetQrCodeScanner;
    }

    private void ResetQrCodeScanner()
    {
        Destroy( barcodeDecoderComponent.gameObject );
        SetUpQrCodeScanner();
    }

    public void ClickToCancelQrCodeScanning()
    {
        SoundManager.Instance.PlayPositiveClickingClip();

        SetToScreenAutoRotation();
        Destroy( barcodeDecoderComponent.gameObject );
        barcodeDecoderCancelButtonObject.SetActive( false );
    }

    public void ClickToSelectQrCode()
    {
        SoundManager.Instance.PlayPositiveClickingClip();

        HideResultMessage();
        fileBrowserSystemRef.OpenFileBrowser();
    }

    public void ClickToLogIn()
    {
        SoundManager.Instance.PlayPositiveClickingClip();

        if (string.IsNullOrEmpty( accessTokenInputField.text ) == true)
        {
            ShowResultMessage( "Please enter your access token" );
        }
        else
        {
            HideResultMessage();
            ProcessLogin( accessTokenInputField.text );
        }
    }

    private void ProcessLogin( string accessToken )
    {
        UserProfileManager.Instance.SetUserAccessToken( accessToken );

        if (isUsingTestData == true)
        {
            TextAsset _userProfileTestData = Resources.Load<TextAsset>( "TestData/UserProfile_" + accessToken );
            if (_userProfileTestData != null)
            {
                OnProcessingLoginComplete( UnityWebRequest.Result.Success, _userProfileTestData.text );
            }
            else
            {
                OnProcessingLoginComplete( UnityWebRequest.Result.DataProcessingError, "" );
            }
        }
        else
        {
            Dictionary<string,string> _headers = ServerApiManager.GetHeaders( true, ServerApiManager.HeaderContentType.APPLICATION_WWW_FORM );
            _headers.Add( "Authorization", "Bearer " + accessToken );

            WWWForm _wwwForm = new WWWForm();
            _wwwForm.AddField( "device_id", SystemInfo.deviceUniqueIdentifier );
            _wwwForm.AddField( "app_version", Application.version );

            ServerApiManager.Post( USER_LOGIN_API, _headers, _wwwForm, OnProcessingLoginComplete );
        }
    }

    private void OnProcessingLoginComplete( UnityWebRequest.Result result, string resultText )
    {
        Debug.Log( "resultText = " + resultText );

        bool _isProcessSuccessful = false;
        if (result == UnityWebRequest.Result.Success)
        {
            GetUserProfileResponse _response = JsonConvert.DeserializeObject<GetUserProfileResponse>( resultText );
            if (_response.status == 1)
            {
                _isProcessSuccessful = true;

                GetUserProfileResponse_Data _responseData = _response.data;
                GameTimeManager.Instance.SetUpCurrentTime( _responseData );
                ExternalLinkManager.Instance.SetUp( _responseData.website_url, _responseData.terms_of_use_url, _responseData.tutorial_url, _responseData.marketplace_url );
                AnnouncementManager.Instance.SetUpAnnouncement( _responseData );
                UserProfileManager.Instance.SetUserLoginSignature( _responseData.signature );
                UserProfileManager.Instance.SetUpUserProfile( _responseData.user_profile );

                LeanTween.move( loginPanelObject, loginPanelStartPoint, loginPanelMovingDuration ).setEaseOutCirc().setOnComplete( SceneControlManager.GoToGameLoadingScene );
                SoundManager.Instance.PlaySoundEffect( loginSuccessClip );
            }
        }

        if (_isProcessSuccessful == false)
        {
            ShowResultMessage( "Login failed" );
        }
    }

    private void ShowResultMessage( string resultMessage )
    {
        resultMessageBox.Show( resultMessage );
        SoundManager.Instance.PlaySoundEffect( errorMessageClip );
    }

    private void HideResultMessage()
    {
        resultMessageBox.Hide();
    }
}

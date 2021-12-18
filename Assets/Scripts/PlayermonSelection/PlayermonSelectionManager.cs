using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using ServerApiResponse;
using Newtonsoft.Json;
using TMPro;

public class PlayermonSelectionManager : MonoBehaviour
{
    [Header( "Settings" )]
    [SerializeField] private int itemsPerPage = 10;

    [Header( "References" )]
    [SerializeField] private TextMeshProUGUI totalPlayermonLabel;

    private readonly string PLAYERMON_API = ServerApiManager.DOMAIN + "playermon/playermon";

    void Start()
    {
        RequestForPlayermonList( 1, itemsPerPage );
        RequestForPlayermon( 1 );
    }

    private void RequestForPlayermonList( int startIndex, int count )
    {
        string _uri = PLAYERMON_API + "?start_index=" + startIndex + "&count=" + count;

        Debug.Log( "_uri = " + _uri );

        ServerApiManager.Get( _uri, UserProfileManager.GetServerApiHeaders( false ), OnRequestingForPlayermonListComplete );
    }

    private void OnRequestingForPlayermonListComplete( UnityWebRequest.Result result, string resultText )
    {
        Debug.Log( "resultText = " + resultText );

        bool _isProcessSuccessful = false;
        if (result == UnityWebRequest.Result.Success)
        {
            GetPlayermonListResponse _response = JsonConvert.DeserializeObject<GetPlayermonListResponse>( resultText );
            if (_response.status == 1)
            {
                _isProcessSuccessful = true;

                GetPlayermonListResponse_Data _responseData = _response.data;

                if (_responseData.total_playermons > 1)
                {
                    totalPlayermonLabel.text = "Total " + _responseData.total_playermons.ToString() + " Playermon";
                }
                else
                {
                    totalPlayermonLabel.text = "Total " + _responseData.total_playermons.ToString() + " Playermons";
                }
            }
        }

        if (_isProcessSuccessful == false)
        {

        }
    }

    private void RequestForPlayermon( int playermonId )
    {
        ServerApiManager.Get( PLAYERMON_API + "/" + playermonId, UserProfileManager.GetServerApiHeaders( false ), OnRequestingForPlayermonComplete );
    }

    private void OnRequestingForPlayermonComplete( UnityWebRequest.Result result, string resultText )
    {
        Debug.Log( "resultText = " + resultText );

        bool _isProcessSuccessful = false;
        if (result == UnityWebRequest.Result.Success)
        {
            GetPlayermonListResponse _response = JsonConvert.DeserializeObject<GetPlayermonListResponse>( resultText );
            if (_response.status == 1)
            {
                _isProcessSuccessful = true;

                GetPlayermonListResponse_Data _responseData = _response.data;
            }
        }

        if (_isProcessSuccessful == false)
        {

        }
    }

    public void ClickToExitScene()
    {
        SoundManager.Instance.PlayNegativeClickingClip();
        SceneControlManager.GoToMainMenuScene();
    }
}

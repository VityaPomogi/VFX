using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using ServerApiResponse;
using Newtonsoft.Json;

public class TeamSelectionManager : MonoBehaviour
{
    private readonly string TEAM_API = ServerApiManager.DOMAIN + "playermon/playermon-team";

    private string targetTeamId = "";

	void Start()
	{
        StartCoroutine( RunTest() );
	}

    private IEnumerator RunTest()
	{
        RequestForTeamList();
        yield return new WaitForSeconds( 3.0f );
        CreateNewTeam( "Good", 1, 1, 2, 2, 3, 3 );
        yield return new WaitForSeconds( 3.0f );
        EditExistingTeam( targetTeamId, "Best", 1, 2, 2, 3, 3, 4 );
        yield return new WaitForSeconds( 3.0f );
        SelectTeam( targetTeamId );
        yield return new WaitForSeconds( 3.0f );
        RemoveTeam( targetTeamId );
    }

    private void RequestForTeamList()
    {
        ServerApiManager.Get( TEAM_API, UserProfileManager.GetServerApiHeaders( false ), OnRequestingForTeamListComplete );
    }

    private void OnRequestingForTeamListComplete( UnityWebRequest.Result result, string resultText )
    {
        Debug.Log( "OnRequestingForTeamListComplete: resultText = " + resultText );

        bool _isProcessSuccessful = false;
        if (result == UnityWebRequest.Result.Success)
        {
            GetTeamListResponse _response = JsonConvert.DeserializeObject<GetTeamListResponse>( resultText );
            if (_response.status == 1)
            {
                _isProcessSuccessful = true;

                GetTeamListResponse_Data _responseData = _response.data;
            }
        }

        if (_isProcessSuccessful == false)
        {

        }
    }

    private void CreateNewTeam( string teamName,
                                int playermonOneId, int playermonOnePosition,
                                int playermonTwoId, int playermonTwoPosition,
                                int playermonThreeId, int playermonThreePosition )
    {
        Debug.Log( "CreateNewTeam" );

        Dictionary<string,string> _headers = UserProfileManager.GetServerApiHeaders( true, ServerApiManager.HeaderContentType.APPLICATION_JSON );
        CreateNewTeamRequestBody _requestBody = new CreateNewTeamRequestBody( teamName, playermonOneId, playermonOnePosition, playermonTwoId, playermonTwoPosition, playermonThreeId, playermonThreePosition );
        string _postData = JsonConvert.SerializeObject( _requestBody );
        ServerApiManager.Post( TEAM_API, _headers, _postData, OnCreatingNewTeamComplete );
    }

    private void OnCreatingNewTeamComplete( UnityWebRequest.Result result, string resultText )
    {
        Debug.Log( "OnCreatingNewTeamComplete: resultText = " + resultText );

        bool _isProcessSuccessful = false;
        if (result == UnityWebRequest.Result.Success)
        {
            GetTeamResponse _response = JsonConvert.DeserializeObject<GetTeamResponse>( resultText );
            if (_response.status == 1)
            {
                _isProcessSuccessful = true;

                GetTeamResponse_Data _responseData = _response.data;
                targetTeamId = _responseData.team.id;
            }
        }

        if (_isProcessSuccessful == false)
        {

        }
    }

    private void EditExistingTeam(  string teamId, string teamName,
                                    int playermonOneId, int playermonOnePosition,
                                    int playermonTwoId, int playermonTwoPosition,
                                    int playermonThreeId, int playermonThreePosition )
    {
        Debug.Log( "EditExistingTeam" );

        Dictionary<string,string> _headers = UserProfileManager.GetServerApiHeaders( true, ServerApiManager.HeaderContentType.APPLICATION_JSON );
        EditExistingTeamRequestBody _requestBody = new EditExistingTeamRequestBody( teamId, teamName, playermonOneId, playermonOnePosition, playermonTwoId, playermonTwoPosition, playermonThreeId, playermonThreePosition );
        string _postData = JsonConvert.SerializeObject( _requestBody );
        ServerApiManager.Patch( TEAM_API, _headers, _postData, OnEditingExistingTeamComplete );
    }

    private void OnEditingExistingTeamComplete( UnityWebRequest.Result result, string resultText )
    {
        Debug.Log( "OnEditingExistingTeamComplete: resultText = " + resultText );

        bool _isProcessSuccessful = false;
        if (result == UnityWebRequest.Result.Success)
        {
            GetTeamResponse _response = JsonConvert.DeserializeObject<GetTeamResponse>( resultText );
            if (_response.status == 1)
            {
                _isProcessSuccessful = true;

                GetTeamResponse_Data _responseData = _response.data;
            }
        }

        if (_isProcessSuccessful == false)
        {

        }
    }

    private void SelectTeam( string teamId )
    {
        Dictionary<string,string> _headers = UserProfileManager.GetServerApiHeaders( true, ServerApiManager.HeaderContentType.APPLICATION_WWW_FORM );

        WWWForm _wwwForm = new WWWForm();
        _wwwForm.AddField( "team_id", teamId );

    //    ServerApiManager.Patch( TEAM_API, _headers, _wwwForm, OnSelectingTeamComplete );
    }

    private void OnSelectingTeamComplete( UnityWebRequest.Result result, string resultText )
    {
        Debug.Log( "OnSelectingTeamComplete: resultText = " + resultText );

        bool _isProcessSuccessful = false;
        if (result == UnityWebRequest.Result.Success)
        {
            GetTeamResponse _response = JsonConvert.DeserializeObject<GetTeamResponse>( resultText );
            if (_response.status == 1)
            {
                _isProcessSuccessful = true;

                GetTeamResponse_Data _responseData = _response.data;
            }
        }

        if (_isProcessSuccessful == false)
        {

        }
    }

    private void RemoveTeam( string teamId )
    {
        Dictionary<string,string> _headers = UserProfileManager.GetServerApiHeaders( true, ServerApiManager.HeaderContentType.APPLICATION_JSON );
        ServerApiManager.Delete( TEAM_API + "/" + teamId, _headers, OnRemovingTeamComplete );
    }

    private void OnRemovingTeamComplete( UnityWebRequest.Result result, string resultText )
    {
        Debug.Log( "OnRemovingTeamComplete: resultText = " + resultText );

        bool _isProcessSuccessful = false;
        if (result == UnityWebRequest.Result.Success)
        {
            GetTeamResponse _response = JsonConvert.DeserializeObject<GetTeamResponse>( resultText );
            if (_response.status == 1)
            {
                _isProcessSuccessful = true;

                GetTeamResponse_Data _responseData = _response.data;
            }
        }

        if (_isProcessSuccessful == false)
        {

        }
    }

#region Inner Classes

    public class CreateNewTeamRequestBody
    {
        public string team_name = "";
        public int playermon_one_id = 0;
        public int playermon_one_position = 0;
        public int playermon_two_id = 0;
        public int playermon_two_position = 0;
        public int playermon_three_id = 0;
        public int playermon_three_position = 0;

        public CreateNewTeamRequestBody(    string teamName,
                                            int playermonOneId, int playermonOnePosition,
                                            int playermonTwoId, int playermonTwoPosition,
                                            int playermonThreeId, int playermonThreePosition )
        {
            team_name = teamName;
            playermon_one_id = playermonOneId;
            playermon_one_position = playermonOnePosition;
            playermon_two_id = playermonTwoId;
            playermon_two_position = playermonTwoPosition;
            playermon_three_id = playermonThreeId;
            playermon_three_position = playermonThreePosition;
        }
    }

    public class EditExistingTeamRequestBody
    {
        public string team_id = "";
        public string team_name = "";
        public int playermon_one_id = 0;
        public int playermon_one_position = 0;
        public int playermon_two_id = 0;
        public int playermon_two_position = 0;
        public int playermon_three_id = 0;
        public int playermon_three_position = 0;

        public EditExistingTeamRequestBody( string teamId, string teamName,
                                            int playermonOneId, int playermonOnePosition,
                                            int playermonTwoId, int playermonTwoPosition,
                                            int playermonThreeId, int playermonThreePosition)
        {
            team_id = teamId;
            team_name = teamName;
            playermon_one_id = playermonOneId;
            playermon_one_position = playermonOnePosition;
            playermon_two_id = playermonTwoId;
            playermon_two_position = playermonTwoPosition;
            playermon_three_id = playermonThreeId;
            playermon_three_position = playermonThreePosition;
        }
    }

#endregion
}

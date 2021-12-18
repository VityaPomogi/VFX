using System.Collections.Generic;
using UnityEngine;
using ServerApiResponse;

public class UserProfileManager : Singleton<UserProfileManager>
{
    private string userAccessToken = "";
    private string userLoginSignature = "";
    private GetUserProfileResponse_Data_UserProfile userProfileData = null;
    private TeamInResponse selectedTeam = null;
    private PlayermonInResponse[] randomPlayermons = null;

    public void SetUserAccessToken( string accessToken )
    {
        userAccessToken = accessToken;
    }

    public string GetUserAccessToken()
    {
        return userAccessToken;
    }

    public void SetUserLoginSignature( string loginSignature )
    {
        userLoginSignature = loginSignature;
    }

    public string GetUserLoginSignature()
    {
        return userLoginSignature;
    }

    public void SetUpUserProfile( GetUserProfileResponse_Data_UserProfile userProfileData )
    {
        this.userProfileData = userProfileData;

        selectedTeam = userProfileData.selected_team;
        randomPlayermons = userProfileData.random_playermons;
    }

    public TeamInResponse GetSelectedTeam()
    {
        /*
        for (int i = 0; i < userProfileTeams.Length; i++)
        {
            TeamInResponse _team = userProfileTeams[ i ];
            if (_team.is_selected == true)
            {
                return _team;
            }
        }

        return null;
        */

        return selectedTeam;
    }

    public PlayermonInResponse[] GetRandomPlayermons()
    {
        return randomPlayermons;
    }

    public GetUserProfileResponse_Data_UserProfile GetUserProfileData()
    {
        return userProfileData;
    }

    /*
    public TeamInResponse[] GetUserProfileTeams()
    {
        return userProfileTeams;
    }
    */

    public static Dictionary<string,string> GetServerApiHeaders( bool hasParameters, ServerApiManager.HeaderContentType contentType = ServerApiManager.HeaderContentType.NONE )
    {
        Dictionary<string, string> _headers = ServerApiManager.GetHeaders( hasParameters, contentType );
        _headers.Add( "Authorization", "Bearer " + UserProfileManager.Instance.GetUserAccessToken() );
        _headers.Add( "device_id", SystemInfo.deviceUniqueIdentifier );
        _headers.Add( "signature", UserProfileManager.Instance.GetUserLoginSignature() );
        return _headers;
    }
}

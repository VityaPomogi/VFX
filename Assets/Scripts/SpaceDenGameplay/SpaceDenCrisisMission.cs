using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ServerApiResponse;

public class SpaceDenCrisisMission : MonoBehaviour
{
    [SerializeField] private float startDelay = 0.5f;

    private List<SpaceDenPlayermon> spaceDenPlayermons = null;
    private GetSpaceDenCrisisResponse_Data currentMissionData = null;
    private GetSpaceDenCrisisResponse_Data_Request[] playermonRequests = null;
    private bool isMissionRunning = false;
    private bool isMissionReadyToPlay = false;
    private int completedRequests = 0;

    public void SetUp( List<SpaceDenPlayermon> spaceDenPlayermons )
    {
        this.spaceDenPlayermons = spaceDenPlayermons;
    }

    public void StartMission( GetSpaceDenCrisisResponse_Data missionData )
    {
        currentMissionData = missionData;
        playermonRequests = currentMissionData.requests_in_sequence;
        completedRequests = 0;

        isMissionRunning = true;
        StartCoroutine( "RunMission" );
    }

    private IEnumerator RunMission()
    {
        float _timeLimitPerRequest = currentMissionData.time_limit_in_seconds_per_request;
        float _intervalPerRequest = currentMissionData.interval_in_seconds_per_request;

        yield return new WaitUntil( () => ( isMissionReadyToPlay == true ) );
        yield return new WaitForSeconds( startDelay );

        for (int i = 0; i < playermonRequests.Length; i++)
        {
            GetSpaceDenCrisisResponse_Data_Request _request = playermonRequests[ i ];
            for (int j = 0; j < spaceDenPlayermons.Count; j++)
            {
                SpaceDenPlayermon _spaceDenPlayermon = spaceDenPlayermons[ j ];
                if (_spaceDenPlayermon.GetPlayermonIndex() == _request.playermon_index)
                {
                    _spaceDenPlayermon.ShowRequest( _request.action_type, i + 1, _timeLimitPerRequest );
                    break;
                }
            }

            yield return new WaitForSeconds( _intervalPerRequest );
        }
    }

    public void StopMission()
    {
        StopCoroutine( "RunMission" );
        isMissionRunning = false;
    }

    public bool CheckPlayermonRequest( int playermonIndex, int actionType )
    {
        GetSpaceDenCrisisResponse_Data_Request _playermonRequest = playermonRequests[ completedRequests ];
        if (_playermonRequest.playermon_index == playermonIndex && _playermonRequest.action_type == actionType)
        {
            completedRequests++;
            return true;
        }

        return false;
    }

    public bool IsMissionComplete()
    {
        if (completedRequests >= playermonRequests.Length)
        {
            return true;
        }

        return false;
    }

    public bool GetIsMissionRunning()
    {
        return isMissionRunning;
    }

    public void SetIsMissionReadyToPlay( bool isMissionReadyToPlay )
    {
        this.isMissionReadyToPlay = isMissionReadyToPlay;
    }
}

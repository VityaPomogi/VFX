using System;
using UnityEngine;
using ServerApiResponse;

public class GameTimeManager : Singleton<GameTimeManager>
{
    private DateTime startLocalTime;
    private double startTime;

    private bool isReadyToUpdate = false;
    private double countdownTime = 0;
    private double countdownStartTime = 0;
    private double remainingTime = 0;

    void Update()
    {
        if (isReadyToUpdate == true)
        {
            UpdateRemainingTime();
        }
    }

    private void UpdateRemainingTime()
    {
        remainingTime = countdownTime - ( Time.realtimeSinceStartupAsDouble - countdownStartTime );
    }

    public void StartToUpdate()
    {
        isReadyToUpdate = true;
    }

    public void SetUpCurrentTime( GetUserProfileResponse_Data responseData )
    {
        startLocalTime = DateTimeHelper.GetDateTimeFromUnixTimeMilliseconds( responseData.current_time_in_ms, true );
        startTime = Time.realtimeSinceStartupAsDouble;
    }

    public void SetCountdownTime( double totalSeconds )
    {
        countdownTime = totalSeconds;
        countdownStartTime = Time.realtimeSinceStartupAsDouble;
        UpdateRemainingTime();
    }

    public TimeSpan CompareToCurrentTime( string dateTimeString )
    {
        return ( GetLocalTimeFromString( dateTimeString ) - GetCurrentTime() );
    }

    public DateTime GetCurrentTime()
    {
        return startLocalTime.AddSeconds( Time.realtimeSinceStartupAsDouble - startTime );
    }

    public long GetCurrentUnixTimestampInMilliseconds()
    {
        return DateTimeHelper.GetUnixTimeMillisecondsFromDateTime( GetCurrentTime() );
    }

    private DateTime GetLocalTimeFromString( string dateTimeString )
    {
        return DateTime.Parse( dateTimeString ).ToLocalTime();
    }

    public TimeSpan GetRemainingTime()
    {
        return TimeSpan.FromSeconds( remainingTime );
    }
}

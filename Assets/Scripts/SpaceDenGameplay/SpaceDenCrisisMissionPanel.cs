using System;
using UnityEngine;
using UnityEngine.UI;
using ServerApiResponse;

public class SpaceDenCrisisMissionPanel : MonoBehaviour
{
    [SerializeField] private Text crisisRewardLabel = null;
    [SerializeField] private Text crisisCooldownLabel = null;
    [SerializeField] private GameObject crisisTitleObject = null;
    [SerializeField] private MyButton crisisButton = null;

    private SpaceDenGameplayManager spaceDenGameplayManagerRef = null;
    private bool isCrisisMissionReady = false;
    private TimeSpan remainingTime = new TimeSpan();
    private bool isReadyToUpdate = false;

    private GameObject crisisCooldownLabelObject = null;

    public void SetUp( SpaceDenGameplayManager spaceDenGameplayManagerRef )
    {
        this.spaceDenGameplayManagerRef = spaceDenGameplayManagerRef;
        crisisCooldownLabelObject = crisisCooldownLabel.gameObject;
        DisableCrisisButton();
    }

    public void SetCrisisReward( int sgemAmount )
    {
        crisisRewardLabel.text = sgemAmount.ToString();
    }

    public void UpdateProgress( GetSpaceDenProgressResponse_Data_SpaceDenProgress spaceDenProgress )
    {
        GameTimeManager.Instance.SetCountdownTime( spaceDenProgress.crisis_cooldown_remaining_seconds );
        if (GameTimeManager.Instance.GetRemainingTime().TotalSeconds > 0)
        {
            GameTimeManager.Instance.StartToUpdate();
            crisisCooldownLabelObject.SetActive( true );
            crisisTitleObject.SetActive( false );
            DisableCrisisButton();
            isCrisisMissionReady = false;
        }

        isReadyToUpdate = true;
    }

    void Update()
    {
        if (isReadyToUpdate == true)
        {
            remainingTime = GameTimeManager.Instance.GetRemainingTime();

            if (remainingTime.TotalSeconds <= 0)
            {
                crisisCooldownLabel.text = "";

                if (isCrisisMissionReady == false)
                {
                    crisisCooldownLabelObject.SetActive( false );
                    crisisTitleObject.SetActive( true );
                    EnableCrisisButton();
                    isCrisisMissionReady = true;
                }
            }
            else
            {
                crisisCooldownLabel.text = string.Format( "{0:D2}h {1:D2}m {2:D2}s", remainingTime.Hours, remainingTime.Minutes, remainingTime.Seconds );
            }
        }
    }

    public void ClickToStartCrisisMission()
    {
        SoundManager.Instance.PlayPositiveClickingClip();

        DisableCrisisButton();
        spaceDenGameplayManagerRef.StartCrisisMission();
    }

    public void EnableCrisisButton()
    {
        crisisButton.SetIsInteractable( true );
    }

    public void DisableCrisisButton()
    {
        crisisButton.SetIsInteractable( false );
    }
}

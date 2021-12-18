using UnityEngine;
using UnityEngine.UI;
using ServerApiResponse;

public class SpaceDenInstructionPanel : MonoBehaviour
{
    [SerializeField] private PopUpMessageBoxBasic messageBox;
    [SerializeField] private Text greetingLabel;
    [SerializeField] private Text onePlayermonLabel;
    [SerializeField] private Text twoPlayermonLabel;
    [SerializeField] private Text threePlayermonLabel;

    private SpaceDenGameplayManager spaceDenGameplayManagerRef;
    private bool isFirstTime = false;

    public void SetUp( SpaceDenGameplayManager spaceDenGameplayManagerRef, GetSpaceDenProgressResponse_Data spaceDenData )
    {
        this.spaceDenGameplayManagerRef = spaceDenGameplayManagerRef;

        greetingLabel.text = "Hi <color=#FFFF00>" + spaceDenData.user_profile.username + "</color>,";

        GetSpaceDenProgressResponse_Data_SpaceDenCrisis_SgemRewardForCompletion _sgemRewardForCompletion = spaceDenData.spaceden_crisis.sgem_reward_for_completion;
        onePlayermonLabel.text = "Completing with <color=#FFFF00>1</color> Playermon will earn <color=#FFFF00>" + _sgemRewardForCompletion.one_actual_playermon.ToString() + " SGEM tokens</color>.";
        twoPlayermonLabel.text = "Completing with <color=#FFFF00>2</color> Playermons will earn <color=#FFFF00>" + _sgemRewardForCompletion.two_actual_playermons.ToString() + " SGEM tokens</color>.";
        threePlayermonLabel.text = "Completing with <color=#FFFF00>3</color> Playermons will earn <color=#FFFF00>" + _sgemRewardForCompletion.three_actual_playermons.ToString() + " SGEM tokens</color>.";
    }

    public void Show( bool isFirstTime = false )
    {
        this.isFirstTime = isFirstTime;
        messageBox.Show();
        spaceDenGameplayManagerRef.PlayNormalMessagePopUpSound();
    }

    public void Hide()
    {
        messageBox.Hide();
    }

    public void ClickToClosePanel()
    {
        SoundManager.Instance.PlayNegativeClickingClip();
        Hide();

        if (isFirstTime == true)
        {
            spaceDenGameplayManagerRef.OnInstructionPanelClosed();
        }
    }
}

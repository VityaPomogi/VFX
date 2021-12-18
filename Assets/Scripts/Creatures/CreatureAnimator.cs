using UnityEngine;

public class CreatureAnimator : MonoBehaviour
{
    [SerializeField] private Animator animatorRef;
    [SerializeField] private string idleAnimationName = "";
    [SerializeField] private string readyAnimationName = "";
    [SerializeField] private string walkingAnimationName = "";
    [SerializeField] private string runningAnimationName = "";
    [SerializeField] private string jumpingAnimationName = "";
    [SerializeField] private string attackingAnimationNameOne = "";
    [SerializeField] private string attackingAnimationNameTwo = "";
    [SerializeField] private string gettingHitAnimationNameOne = "";
    [SerializeField] private string gettingHitAnimationNameTwo = "";
    [SerializeField] private string weakAnimationName = "";
    [SerializeField] private string faintedAnimationName = "";
    [SerializeField] private string dyingAnimationName = "";

    public void PlayIdleAnimation()
    {
        animatorRef.speed = 1.0f;
        animatorRef.Play( idleAnimationName );
    }

    public void PlayReadyAnimation()
    {
        animatorRef.speed = 1.5f;
        animatorRef.Play( readyAnimationName );
    }

    public void PlayWalkingAnimation()
    {
        animatorRef.speed = 1.0f;
        animatorRef.Play( walkingAnimationName );
    }

    public void PlayRunningAnimation()
    {
        animatorRef.speed = 1.0f;
        animatorRef.Play( runningAnimationName );
    }

    public void PlayJumpingAnimation()
    {
        animatorRef.speed = 1.0f;
        animatorRef.Play( jumpingAnimationName );
    }

    public void PlayAttackingAnimationOne()
    {
        animatorRef.speed = 1.0f;
        animatorRef.Play( attackingAnimationNameOne );
    }

    public void PlayAttackingAnimationTwo()
    {
        animatorRef.speed = 1.0f;
        animatorRef.Play( attackingAnimationNameTwo );
    }

    public void PlayGettingHitAnimationOne()
    {
        animatorRef.speed = 1.0f;
        animatorRef.Play( gettingHitAnimationNameOne );
    }

    public void PlayGettingHitAnimationTwo()
    {
        animatorRef.speed = 1.0f;
        animatorRef.Play( gettingHitAnimationNameTwo );
    }

    public void PlayWeakAnimation()
    {
        animatorRef.speed = 1.0f;
        animatorRef.Play( weakAnimationName );
    }

    public void PlayFaintedAnimation()
    {
        animatorRef.speed = 1.2f;
        animatorRef.Play( faintedAnimationName );
    }

    public void PlayDyingAnimation()
    {
        animatorRef.speed = 1.0f;
        animatorRef.Play( dyingAnimationName );
    }
}

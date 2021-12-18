using System;
using UnityEngine;

public class PlayermonAnimations : MonoBehaviour
{
    [Header( "Settings" )]
    [SerializeField] private int minimumLoopsToPlayIdleTrigger = 3;
    [SerializeField] private float chanceToPlayIdleTrigger = 0.5f;

    [Header( "References" )]
    [SerializeField] private Animator _animator = null;
    [SerializeField] private Creature creatureRef = null;
    [SerializeField] private Animator eyeAnimator = null;

    [Header( "General Animation Name" )]
    [SerializeField] private string _idleAnimation = "Idle";
    [SerializeField] private string _idleTriggerAnimation = "IdleTrigger";
    [SerializeField] private string _walkingAnimation = "Walk";
    [SerializeField] private string _runningAnimation = "Run";

    [Header( "Battle Animation Name" )]
    [SerializeField] private string _readyAnimation = "Ready";
    [SerializeField] private string _jumpingAnimation = "Jump";
    [SerializeField] private string _attackingAnimation = "Attack_1";
    [SerializeField] private string _gettingHitAnimation = "GettingHit_1";

    [Header( "SpaceDen Animation Name" )]
    [SerializeField] private string _smellingAnimation = "Smelling";
    [SerializeField] private string _eatingAnimation = "Eating";
    [SerializeField] private string _bathingAnimation = "Bathing";
    [SerializeField] private string _playingAnimation = "Playing";
    [SerializeField] private string _actionSuccessAnimation = "ActionSuccess01";
    [SerializeField] private string _crisisFailAnimation = "CrisisFail";
    [SerializeField] private string _crisisSuccessAnimation = "CrisisSuccess";
    [SerializeField] private string _crisisIdleAnimation = "CrisisIdle";
    [SerializeField] private string _rejectingAnimation = "Rejecting";

    private Action OnAnimationCompletedCallback = null;
    private Action OnAnimationFrameCallback = null;

    private bool shouldEyeBlink = false;
    private int idleAnimationLoopCount = 0;

    void Start()
    {
        _animator.Play( _idleAnimation, 0, UnityEngine.Random.value * 0.5f );
    }

    private void PlayAnimation( string animationName, Action onAnimationCompletedCallback = null, Action onAnimationFrameCallback = null )
    {
        shouldEyeBlink = true;
        _animator.Play( animationName );

        if (animationName == "Attack_1" || animationName == "Attack_2")
        {
            _animator.speed = 1.5f;
        }
        else
        {
            _animator.speed = 1.0f;
        }

        OnAnimationCompletedCallback = onAnimationCompletedCallback;
        OnAnimationFrameCallback = onAnimationFrameCallback;
    }

    private void ActionCallback(Action actionCallback)
    {
        if (actionCallback != null)
        {
            actionCallback();
        }
    }

#region General Animations

    public void PlayIdleAnimation( Action onAnimationCompletedCallback = null )
    {
        PlayAnimation( _idleAnimation, onAnimationCompletedCallback );
    }

    public void PlayIdleTriggerAnimation( Action onAnimationCompletedCallback = null )
    {
        PlayAnimation( _idleTriggerAnimation, onAnimationCompletedCallback );
    }

    public void PlayWalkingAnimation( Action onAnimationCompletedCallback = null )
    {
        PlayAnimation( _walkingAnimation, onAnimationCompletedCallback );
    }

    public void PlayRunningAnimation( Action onAnimationCompletedCallback = null )
    {
        PlayAnimation( _runningAnimation, onAnimationCompletedCallback );
    }

    #endregion

    #region Battle Animations

    public void PlayReadyAnimation(Action onAnimationCompletedCallback = null)
    {
        PlayAnimation(_readyAnimation, onAnimationCompletedCallback);
    }

    public void PlayJumpAnimation(Action onAnimationCompletedCallback = null, Action onAnimationFrameCallback = null)
    {
        PlayAnimation(_jumpingAnimation, onAnimationCompletedCallback, onAnimationFrameCallback);
    }

    public void PlayAttackAnimation(Action onAnimationCompletedCallback = null, Action onAnimationFrameCallback = null)
    {
        PlayAnimation(_attackingAnimation, onAnimationCompletedCallback, onAnimationFrameCallback);
    }

    public void PlayGetHitAnimation(Action onAnimationCompletedCallback = null)
    {
        PlayAnimation(_gettingHitAnimation, onAnimationCompletedCallback);
    }

#endregion

#region SpaceDen Animations

    public void PlaySmellingAnimation(Action onAnimationCompletedCallback = null)
    {
        PlayAnimation(_smellingAnimation, onAnimationCompletedCallback);
    }

    public void PlayEatingAnimation(Action onAnimationCompletedCallback = null)
    {
        PlayAnimation(_eatingAnimation, onAnimationCompletedCallback);
    }

    public void PlayBathingAnimation(Action onAnimationCompletedCallback = null)
    {
        PlayAnimation(_bathingAnimation, onAnimationCompletedCallback);
    }

    public void PlayPlayingAnimation(Action onAnimationCompletedCallback = null)
    {
        PlayAnimation(_playingAnimation, onAnimationCompletedCallback);
    }

    public void PlayActionSuccesAnimation(Action onAnimationCompletedCallback = null)
    {
        PlayAnimation(_actionSuccessAnimation, onAnimationCompletedCallback);
    }

    public void PlayCrisisFailAnimation(Action onAnimationCompletedCallback = null)
    {
        PlayAnimation(_crisisFailAnimation, onAnimationCompletedCallback);
    }

    public void PlayCrisisSuccessAnimation(Action onAnimationCompletedCallback = null)
    {
        PlayAnimation(_crisisSuccessAnimation, onAnimationCompletedCallback);
    }

    public void PlayCrisisIdlesAnimation(Action onAnimationCompletedCallback = null)
    {
        PlayAnimation(_crisisIdleAnimation, onAnimationCompletedCallback);
    }

    public void PlayRejectingAnimation(Action onAnimationCompletedCallback = null)
    {
        PlayAnimation( _rejectingAnimation, onAnimationCompletedCallback );
    }

#endregion

#region Animation Event

    private void AnimationFrameCallback()
    {
        Action _action = OnAnimationFrameCallback;
        ActionCallback( OnAnimationFrameCallback );

        if (OnAnimationFrameCallback == _action)
        {
            OnAnimationFrameCallback = null;
        }
    }

    private void AnimationCompletedCallback()
    {
        Action _action = OnAnimationCompletedCallback;
        ActionCallback( OnAnimationCompletedCallback );

        if (OnAnimationCompletedCallback == _action)
        {
            OnAnimationCompletedCallback = null;
        }
    }

#endregion

    public void PlayEyeAnimation( string animationName )
    {
        if (creatureRef != null)
        {
            if (animationName == "Blinking")
            {
                if (creatureRef.GetCurrentHealthMode() == Creature.HealthMode.DEAD
                    || creatureRef.GetCurrentHealthMode() == Creature.HealthMode.WEAK)
                {
                    return;
                }
                else if (shouldEyeBlink == false)
                {
                    shouldEyeBlink = true;
                    return;
                }
                else
                {
                    shouldEyeBlink = false;
                }
            }
            else if (animationName == "Weak")
            {
                if (creatureRef.GetIsAttacking() == true)
                {
                    return;
                }
            }
        }

        if (eyeAnimator != null)
        {
            eyeAnimator.Play( animationName );
        }
    }

    public void OnIdleAnimationLoopDone()
    {
        idleAnimationLoopCount++;

        if (idleAnimationLoopCount >= minimumLoopsToPlayIdleTrigger)
        {
            if (UnityEngine.Random.value < chanceToPlayIdleTrigger)
            {
                idleAnimationLoopCount = 0;
                PlayIdleTriggerAnimation();
            }
        }
    }

    public Animator GetAnimator()
    {
        return _animator;
    }
}

using UnityEngine;

public class Creature : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 10.0f;
    [SerializeField] private bool isClickable = true;

    [SerializeField] private SwappableCreatureV3 swappableCreatureRef;
    [SerializeField] private CreatureAnimator animatorRef;
    [SerializeField] private CreatureHealthBar healthBar;
    [SerializeField] private CreatureClassIcon classIcon;
    [SerializeField] private CreatureStatusBar statusBar;
    [SerializeField] private CreatureShieldBar shieldBar;
    [SerializeField] private CreatureEventTrigger eventTriggerRef;

    [SerializeField] private Transform creatureContainer;
    [SerializeField] private Transform creatureTransform;
    [SerializeField] private GameObject creatureObject;
    [SerializeField] private GameObject headsUpDisplayContainer;

    [SerializeField] private PlayermonActions playermonActionsRef;
    [SerializeField] private FloatingDisplayLabel floatingDisplayLabelPrefab;
    [SerializeField] private DisplayPowerUp displayPowerUpPrefab;
    [SerializeField] private Transform floatingDisplayLabelPoint;
    [SerializeField] private Transform hittingPoint;
    [SerializeField] private Transform deathFirePoint;
    [SerializeField] private Transform markingPoint;
    [SerializeField] private Transform passiveTriggerPoint;
    [SerializeField] private Transform shieldPoint;
    [SerializeField] private GameObject[] hiddenObjectsOnDeath;

    [Header("Sound")]
    [SerializeField] private AudioClip _clickedSFX = null;

    [Header( "Visual Effects" )]
    [SerializeField] private GameObject hittingParticle;
    [SerializeField] private GameObject deathFire;
    [SerializeField] private GameObject markedParticle;
    [SerializeField] private GameObject stunningParticle;
    [SerializeField] private GameObject poisoningParticle;
    [SerializeField] private GameObject passiveTriggerEffect;
    [SerializeField] private GameObject shieldEffect;

    [Header( "SpaceDen" )]
    [SerializeField] private SpaceDenPlayermon spaceDenPlayermonRef;

    private CreatureData targetCreatureData = null;
    private HealthMode currentHealthMode = HealthMode.HEALTHY;
    private bool isAttacking = false;
    private int damageTaken = 0;
    private bool willBeStunned = false;
    private bool isStunned = false;
    private bool willBePoisoned = false;
    private bool isPoisoned = false;
    private bool isDefending = false;

    private GameObject floatingDisplayLabelPrefabObject = null;
    private GameObject displayPowerUpPrefabObject = null;
    private SpriteRenderer[] creatureSpriteRenderers = null;

    public enum HealthMode
    {
        HEALTHY,
        WEAK,
        DEAD
    }

    void Awake()
    {
        floatingDisplayLabelPrefabObject = floatingDisplayLabelPrefab.gameObject;
        displayPowerUpPrefabObject = displayPowerUpPrefab.gameObject;
        creatureSpriteRenderers = creatureObject.GetComponentsInChildren<SpriteRenderer>( true );
        playermonActionsRef.onBeingIdle = OnBeingIdle;
        playermonActionsRef.onGettingHit = OnGettingHit;
        playermonActionsRef.onAttackStarted = OnAttackStarted;
        playermonActionsRef.onAttackEnded = OnAttackEnded;
        eventTriggerRef.SetUp( this );
    }

    public void SetUp( CreatureData targetCreatureData, float containerScale = 1.0f, bool hasHeadsUpDisplay = true )
    {
        this.targetCreatureData = targetCreatureData;
        healthBar.SetUp( targetCreatureData.GetHitpoint() );

        if (targetCreatureData.GetIsPlayer() == true)
        {
            creatureTransform.localScale = new Vector3( -1.0f, 1.0f, 1.0f );
        }
        else
        {
            shieldBar.SetToRightSide();
        }

        swappableCreatureRef.SetSwappableBodyParts( targetCreatureData.GetHeadId(), targetCreatureData.GetEyeId(), targetCreatureData.GetArmId(), targetCreatureData.GetLegId(), targetCreatureData.GetBodyId(), targetCreatureData.GetChestObjectId(), targetCreatureData.GetTailObjectId(), "A" );
        classIcon.SetClassIcon( targetCreatureData.GetClassType() );

        creatureContainer.localScale = new Vector3( containerScale, containerScale, 1.0f );
        headsUpDisplayContainer.SetActive( hasHeadsUpDisplay );
    }

    public void AddStatus( Sprite iconSprite, int counterNumber, Color32 labelOutlineColor )
    {
        statusBar.AddStatus( iconSprite, counterNumber, labelOutlineColor );
    }

    public void SetDamageTaken( int damageTaken )
    {
        this.damageTaken = damageTaken;
    }

    public void SetBeingMarked()
    {
        BattleGameplayManager.Instance.PlayTargetMarkingAudioClip();

        GameObject _markedParticleObj = Instantiate( markedParticle );
        _markedParticleObj.transform.position = markingPoint.position;
        Destroy( _markedParticleObj, 5.0f );

        AddStatus( Resources.Load<Sprite>( "StatusIcons/Icon_Target" ), 2, new Color32( 141, 141, 141, 255 ) );
    }

    public void SetWillBeStunned( bool willBeStunned )
    {
        this.willBeStunned = willBeStunned;
    }

    public void SetWillBePoisoned( bool willBePoisoned )
    {
        this.willBePoisoned = willBePoisoned;
    }

    public void ShowHittingEffect()
    {
        GameObject _hittingParticleObj = Instantiate( hittingParticle );
        _hittingParticleObj.transform.position = hittingPoint.position;
        Destroy( _hittingParticleObj, 5.0f );
    }

    public void ShowFloatingDisplayLabel( string displayText )
    {
        GameObject _floatingDisplayLabelObject = Instantiate( floatingDisplayLabelPrefabObject );
        _floatingDisplayLabelObject.transform.position = floatingDisplayLabelPoint.position;
        _floatingDisplayLabelObject.GetComponent<FloatingDisplayLabel>().ShowLabel( displayText, 1.4f, 2.0f, 1.0f );
    }

    public void ShowFloatingDisplayPowerUp( Sprite iconSprite, string displayText, Color32 labelOutlineColor )
    {
        GameObject _displayPowerUpObject = Instantiate( displayPowerUpPrefabObject );
        _displayPowerUpObject.transform.position = floatingDisplayLabelPoint.position;

        DisplayPowerUp _displayPowerUp = _displayPowerUpObject.GetComponent<DisplayPowerUp>();
        _displayPowerUp.SetUp( iconSprite, displayText, labelOutlineColor );
        _displayPowerUp.GetFloatingDisplayObjectRef().Show( 1.4f, 2.0f, 1.0f );
    }

    public void ShowPassiveTriggerEvent()
    {
        BattleGameplayManager.Instance.PlayPowerUpAudioClip();

        GameObject _passiveTriggerEventObj = Instantiate( passiveTriggerEffect );
        _passiveTriggerEventObj.transform.position = passiveTriggerPoint.position;
        Destroy( _passiveTriggerEventObj, 5.0f );
    }

    public void ShowShieldEffect()
    {
        GameObject _shieldEffectObj = Instantiate( shieldEffect );
        _shieldEffectObj.transform.position = shieldPoint.position;
        isDefending = true;
    }

    private void OnBeingIdle()
    {
        SetCurrentHealthMode( currentHealthMode );
    }

    private void OnGettingHit()
    {
        BattleGameplayManager.Instance.PlayHittingAudioClip();
        ShowHittingEffect();

        if (willBeStunned == true)
        {
            BattleGameplayManager.Instance.PlayStunningAudioClip();

            GameObject _stunningParticleObj = Instantiate( stunningParticle );
            _stunningParticleObj.transform.position = hittingPoint.position;
            Destroy( _stunningParticleObj, 5.0f );

            willBeStunned = false;
            isStunned = true;

            AddStatus( Resources.Load<Sprite>( "StatusIcons/Icon_Stunned" ), 2, new Color32( 131, 8, 205, 255 ) );
        }
        if (willBePoisoned == true)
        {
            BattleGameplayManager.Instance.PlayPoisoningAudioClip();

            GameObject _poisoningParticleObj = Instantiate( poisoningParticle );
            _poisoningParticleObj.transform.position = markingPoint.position;

            willBePoisoned = false;
            isPoisoned = true;
            AddStatus( Resources.Load<Sprite>( "StatusIcons/Icon_Poison" ), 3, new Color32( 20, 107, 19, 255 ) );
        }

        TakeDamage( damageTaken );
    }

    private void OnAttackStarted()
    {
        isAttacking = true;
    }

    private void OnAttackEnded()
    {
        isAttacking = false;
    }

    public void MoveToTargetPosition( Vector3 targetPosition )
    {
        float _moveDuration = Vector3.Distance( this.transform.position, targetPosition ) / moveSpeed;
        LeanTween.move( this.gameObject, targetPosition, _moveDuration ).setOnComplete( OnTargetPositionReached );
        animatorRef.PlayRunningAnimation();
    }

    private void OnTargetPositionReached()
    {
        OnBeingIdle();
    }

    public void TakeDamage( float amount )
    {
        float _remainingAmount = shieldBar.MinusShieldNumber( amount );
        if (_remainingAmount > 0)
        {
            healthBar.ReduceHitpoint( _remainingAmount );
        }

        UpdateAnimation();
        ShowFloatingDisplayLabel( "-" + Mathf.RoundToInt( amount ).ToString() );
    }

    public void Heal( float amount )
    {
        healthBar.IncreaseHitpoint( amount );
        UpdateAnimation();
    }

    public void Die()
    {
        GameObject _deathFireObj = Instantiate( deathFire );
        _deathFireObj.transform.position = deathFirePoint.position;

        for (int i = 0; i < hiddenObjectsOnDeath.Length; i++)
        {
            hiddenObjectsOnDeath[ i ].SetActive( false );
        }
    }

    public void SetOpacityLevel( float opacityLevel )
    {
        for (int i = 0; i < creatureSpriteRenderers.Length; i++)
        {
            SpriteRenderer _spriteRenderer = creatureSpriteRenderers[ i ];
            Color _color = _spriteRenderer.color;
            _color.a = opacityLevel;
            _spriteRenderer.color = _color;
        }
    }

    private void UpdateAnimation()
    {
        if (healthBar.IsEmpty() == false)
        {
            if (currentHealthMode != HealthMode.WEAK)
            {
                if (healthBar.IsRedBar() == true)
                {
                    SetCurrentHealthMode( HealthMode.WEAK );
                }
                else
                {
                    SetCurrentHealthMode( HealthMode.HEALTHY );
                }
            }
            else
            {
                if (healthBar.IsRedBar() == false)
                {
                    SetCurrentHealthMode( HealthMode.HEALTHY );
                }
                else
                {
                    SetCurrentHealthMode( HealthMode.WEAK );
                }
            }
        }
        else
        {
            SetCurrentHealthMode( HealthMode.DEAD );
        }
    }

    private void SetCurrentHealthMode( HealthMode currentHealthMode )
    {
        this.currentHealthMode = currentHealthMode;

        switch ( currentHealthMode )
        {
            case HealthMode.HEALTHY:

                if (isStunned == true)
                {
                    animatorRef.PlayFaintedAnimation();
                }
                else
                {
                    animatorRef.PlayIdleAnimation();
                }

                break;

            case HealthMode.WEAK:

                if (isStunned == true)
                {
                    animatorRef.PlayFaintedAnimation();
                }
                else
                {
                    animatorRef.PlayWeakAnimation();
                }

                break;

            case HealthMode.DEAD:

                animatorRef.PlayDyingAnimation();

                break;
        }
    }

    public HealthMode GetCurrentHealthMode()
    {
        return currentHealthMode;
    }

    public void OnClicked()
    {
        if (isClickable == true)
        {
            animatorRef.PlayReadyAnimation();
            SoundManager.Instance.PlaySoundEffect( _clickedSFX );
        }
    }

    public CreatureData GetTargetCreatureData()
    {
        return targetCreatureData;
    }

    public CreatureAnimator GetAnimatorRef()
    {
        return animatorRef;
    }

    public PlayermonActions GetPlayermonActionsRef()
    {
        return playermonActionsRef;
    }

    public CreatureStatusBar GetStatusBar()
    {
        return statusBar;
    }

    public CreatureShieldBar GetShieldBar()
    {
        return shieldBar;
    }

    public bool GetIsAttacking()
    {
        return isAttacking;
    }

#region SpaceDen

    public SpaceDenPlayermon GetSpaceDenPlayermonRef()
    {
        return spaceDenPlayermonRef;
    }

#endregion
}

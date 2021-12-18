using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleGameplayManager : MonoBehaviour
{
    [Header( "Data" )]
    [SerializeField] private CharacterScriptableObject characterDatabase;
    [SerializeField] private SkillScriptableObject skillDatabase;

    [Header( "UI" )]
    [SerializeField] private ActionPointGauge actionPointGaugeRef;
    [SerializeField] private CardDeck cardDeckRef;
    [SerializeField] private BattleSequence battleSequenceRef;
    [SerializeField] private HighlightedSkillCard highlightedSkillCardRef;
    [SerializeField] private PointPositionManager pointPositionManagerRef;
    [SerializeField] private Text roundNumberLabel;
    [SerializeField] private Text opponentUsernameLabel;
    [SerializeField] private TextMeshProUGUI opponentRemainingCardLabel;
    [SerializeField] private TextMeshProUGUI usedCardLabel;
    [SerializeField] private MyButton endTurnButton;

    [Header( "Prefab" )]
    [SerializeField] private SkillCard skillCardPrefab;

    [Header( "Sounds" )]
    [SerializeField] private AudioClip backgroundMusicAudioClip;
    [SerializeField] private AudioClip cardDrawnAudioClip;
    [SerializeField] private AudioClip cardHoveredAudioClip;
    [SerializeField] private AudioClip cardSelectedAudioClip;
    [SerializeField] private AudioClip cardDeselectedAudioClip;
    [SerializeField] private AudioClip hittingAudioClip;
    [SerializeField] private AudioClip targetMarkingAudioClip;
    [SerializeField] private AudioClip powerUpAudioClip;
    [SerializeField] private AudioClip stunningAudioClip;
    [SerializeField] private AudioClip poisoningAudioClip;
    [SerializeField] private AudioClip poisonDamageTakingAudioClip;
    [SerializeField] private AudioClip stunnedAudioClip;
    [SerializeField] private AudioClip shieldAppearingAudioClip;

    [Header( "Testing" )]
    [SerializeField] private List<CreatureData> creatureDataList = new List<CreatureData>();
    [SerializeField] private Color32 bonusMoraleOutlineColor;

    private List<Creature> creatures = new List<Creature>();
    private int roundNumber = 0;
    private GamePhase currentGamePhase = GamePhase.NONE;

    private List<SkillInfo> skillInfoList = new List<SkillInfo>();
    private GameObject skillCardPrefabGameObject = null;

    private static BattleGameplayManager _instance;
    public static BattleGameplayManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<BattleGameplayManager>();
            }

            return _instance;
        }
    }

    public enum GamePhase
    {
        NONE,
        NEW_ROUND,
        PLANNING,
        EXECUTING,
        ENDED
    }

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            Destroy( this.gameObject );
        }

        skillCardPrefabGameObject = skillCardPrefab.gameObject;
    }

    void Start()
    {
        skillInfoList.Add( new SkillInfo( 1, CreatureData.ClassTypes.MECH, 1, 60, 30 ) );
        skillInfoList.Add( new SkillInfo( 2, CreatureData.ClassTypes.COMET, 2, 300, 50 ) );
        skillInfoList.Add( new SkillInfo( 3, CreatureData.ClassTypes.COMET, 1, 75, 25 ) );
        skillInfoList.Add( new SkillInfo( 4, CreatureData.ClassTypes.MYTHOS, 0, 0, 0 ) );
        skillInfoList.Add( new SkillInfo( 5, CreatureData.ClassTypes.MYTHOS, 0, 0, 0 ) );
        skillInfoList.Add( new SkillInfo( 6, CreatureData.ClassTypes.INFERNO, 2, 90, 50 ) );
        skillInfoList.Add( new SkillInfo( 7, CreatureData.ClassTypes.INFERNO, 1, 45, 30 ) );
        skillInfoList.Add( new SkillInfo( 8, CreatureData.ClassTypes.AQUA, 1, 45, 20 ) );
        skillInfoList.Add( new SkillInfo( 9, CreatureData.ClassTypes.AQUA, 1, 30, 30 ) );
        skillInfoList.Add( new SkillInfo( 10, CreatureData.ClassTypes.MECH, 0, 0, 0 ) );
        skillInfoList.Add( new SkillInfo( 11, CreatureData.ClassTypes.MECH, 1, 40, 15 ) );
        skillInfoList.Add( new SkillInfo( 12, CreatureData.ClassTypes.MECH, 0, 0, 0 ) );
        skillInfoList.Add( new SkillInfo( 13, CreatureData.ClassTypes.MECH, 1, 25, 15 ) );
        skillInfoList.Add( new SkillInfo( 14, CreatureData.ClassTypes.NATURE, 1, 0, 75 ) );
        skillInfoList.Add( new SkillInfo( 15, CreatureData.ClassTypes.NATURE, 1, 0, 75 ) );
        skillInfoList.Add( new SkillInfo( 16, CreatureData.ClassTypes.NATURE, 1, 30, 50 ) );

        // ---------- Creatures ----------

        creatureDataList.Sort( ( x, y ) =>
        {
            if (x.GetSequence() < y.GetSequence())
            {
                return -1;
            }
            else if (x.GetSequence() > y.GetSequence())
            {
                return 1;
            }

            return 0;
        } );

        for (int i = 0; i < creatureDataList.Count; i++)
        {
            creatures.Add( pointPositionManagerRef.SpawnCreatureOnPoint( creatureDataList[ i ] ) );
        }

        // ---------------------------------

        // ---------- Skill Cards ----------

        List<CardDeck.CardDeckSkill> _skillIdList = new List<CardDeck.CardDeckSkill>();

        /*
        for (int i = 0; i < creatureDataList.Count; i++)
        {
            CreatureData _creatureData = creatureDataList[ i ];
            if (_creatureData.GetIsPlayer() == true)
            {
                List<int> _creatureSkillIdList = _creatureData.GetSkillIdList();
                for (int j = 0; j < _creatureSkillIdList.Count; j++)
                {
                    for (int k = 0; k < 2; k++)
                    {
                        _skillIdList.Add( new CardDeck.CardDeckSkill( _creatureData, _creatureSkillIdList[ j ] ) );
                    }
                }
            }
        }
        */

        _skillIdList.Add( new CardDeck.CardDeckSkill( creatureDataList[ 3 ], 1 ) );
        _skillIdList.Add( new CardDeck.CardDeckSkill( creatureDataList[ 3 ], 1 ) );
        _skillIdList.Add( new CardDeck.CardDeckSkill( creatureDataList[ 0 ], 8 ) );
        _skillIdList.Add( new CardDeck.CardDeckSkill( creatureDataList[ 0 ], 1 ) );
        _skillIdList.Add( new CardDeck.CardDeckSkill( creatureDataList[ 4 ], 14 ) );

        _skillIdList.Add( new CardDeck.CardDeckSkill( creatureDataList[ 3 ], 1 ) );
        _skillIdList.Add( new CardDeck.CardDeckSkill( creatureDataList[ 0 ], 1 ) );
        _skillIdList.Add( new CardDeck.CardDeckSkill( creatureDataList[ 4 ], 1 ) );

        for (int i = 0; i < 16; i++)
        {
            _skillIdList.Add( new CardDeck.CardDeckSkill( creatureDataList[ 3 ], 1 ) );
        }

        /*
        _skillIdList.Add( new CardDeck.CardDeckSkill( creatureDataList[ 3 ], 2 ) );
        _skillIdList.Add( new CardDeck.CardDeckSkill( creatureDataList[ 4 ], 3 ) );
        _skillIdList.Add( new CardDeck.CardDeckSkill( creatureDataList[ 4 ], 1 ) );
        _skillIdList.Add( new CardDeck.CardDeckSkill( creatureDataList[ 5 ], 1 ) );
        _skillIdList.Add( new CardDeck.CardDeckSkill( creatureDataList[ 5 ], 16 ) );
        */

        cardDeckRef.SetUp( _skillIdList );
        cardDeckRef.SetNumberOfRemainingCards( _skillIdList.Count );
        cardDeckRef.GetOnHandCardManagerRef().SetUp( creatureDataList );

        // ---------------------------------

        SoundManager.Instance.PlayBackgroundMusic( backgroundMusicAudioClip );

        SetRoundNumber( 1 );
        actionPointGaugeRef.SetUp( 10, 5 );
        battleSequenceRef.SetUp( creatureDataList );

        Invoke( "StartGame", 0.1f );
    }

    private void StartGame()
    {
        cardDeckRef.DrawCards( 5 );

        currentGamePhase = GamePhase.NEW_ROUND;
        Invoke( "StartPlanningPhase", 0.5f );
    }

    private void StartPlanningPhase()
    {
        currentGamePhase = GamePhase.PLANNING;
        endTurnButton.SetIsInteractable( true );
    }

    public void HighlightCreature( Creature targetCreature )
    {
        for (int i = 0; i < creatures.Count; i++)
        {
            Creature _creature = creatures[ i ];
            if (_creature != targetCreature)
            {
                _creature.SetOpacityLevel( 0.2f );
            }
        }
    }

    public void ResetAllCreatureOpacityLevels()
    {
        for (int i = 0; i < creatures.Count; i++)
        {
            creatures[ i ].SetOpacityLevel( 1.0f );
        }
    }

    public void ClickToExitScene()
    {
        SceneControlManager.GoToMainMenuScene();
    }

    public void ClickToExecuteActions()
    {
        SoundManager.Instance.PlayPositiveClickingClip();

        endTurnButton.SetIsInteractable( false );
        currentGamePhase = GamePhase.EXECUTING;
        StartCoroutine( RunExecutingActions() );
    }

    private IEnumerator RunExecutingActions()
    {
        /*
        for (int i = 0; i < creatures.Count; i++)
        {
            Creature _creature = creatures[ i ];

            if (_creature.GetCurrentHealthMode() != Creature.HealthMode.DEAD)
            {
                int _deathCount = 0;
                for (int j = 0; j < creatures.Count; j++)
                {
                    Creature _c = creatures[ j ];
                    if (_c.GetTargetCreatureData().GetIsPlayer() != _creature.GetTargetCreatureData().GetIsPlayer()
                           && _c.GetCurrentHealthMode() == Creature.HealthMode.DEAD)
                    {
                        _deathCount++;
                    }
                }

                if (_deathCount < 3)
                {
                    Creature _target = null;
                    do
                    {
                        _target = creatures[ Random.Range( 0, creatures.Count ) ];
                    }
                    while (_creature.GetTargetCreatureData().GetIsPlayer() == _target.GetTargetCreatureData().GetIsPlayer()
                           || _target.GetCurrentHealthMode() == Creature.HealthMode.DEAD);

                    _creature.GetPlayermonActionsRef().SetTargetEnemy( _target.gameObject );
                    _creature.GetPlayermonActionsRef().AttackTarget();
                    yield return new WaitForSeconds( 4.0f );
                }
            }
        }

        SetRoundNumber( roundNumber + 1 );
        */

        opponentRemainingCardLabel.text = "23";
        battleSequenceRef.AddCardToOpponentCharacter( creatureDataList[ 1 ], GetSkillInfo( 1 ) );
        yield return new WaitForSeconds( 0.1f );
        opponentRemainingCardLabel.text = "22";
        battleSequenceRef.AddCardToOpponentCharacter( creatureDataList[ 1 ], GetSkillInfo( 16 ) );
        yield return new WaitForSeconds( 0.1f );
        opponentRemainingCardLabel.text = "21";
        battleSequenceRef.AddCardToOpponentCharacter( creatureDataList[ 2 ], GetSkillInfo( 3 ) );
        yield return new WaitForSeconds( 0.2f );

        battleSequenceRef.RemoveCardsFromHand();
        battleSequenceRef.ShowOpacityLevelOnExecution();

        // -------------------- Setup Started --------------------

        creatures[ 0 ].GetShieldBar().SetShieldNumber( 20 );
        creatures[ 1 ].GetShieldBar().SetShieldNumber( 80 );
        creatures[ 2 ].GetShieldBar().SetShieldNumber( 25 );
        creatures[ 3 ].GetShieldBar().SetShieldNumber( 30 );
        creatures[ 4 ].GetShieldBar().SetShieldNumber( 75 );
        SoundManager.Instance.PlaySoundEffect( shieldAppearingAudioClip );
        yield return new WaitForSeconds( 0.5f );

        // -------------------- Setup Ended --------------------

        // -------------------- First Attack Started --------------------

        battleSequenceRef.ShowCharacterActions( 0 );
        yield return new WaitForSeconds( 0.5f );

        Creature _playerCreature = creatures[ 0 ];
        Creature _opponentCreature = creatures[ 1 ];

        _playerCreature.GetAnimatorRef().PlayReadyAnimation();
        yield return new WaitForSeconds( 0.6f );
        _opponentCreature.SetBeingMarked();
        yield return new WaitForSeconds( 0.5f );

        _playerCreature.GetPlayermonActionsRef().SetTargetEnemy( _opponentCreature.gameObject );
        _opponentCreature.SetDamageTaken( 45 );
        _playerCreature.GetPlayermonActionsRef().AttackTarget();
        yield return new WaitForSeconds( 3.5f );
        battleSequenceRef.HideCurrentCharacterActions();
        usedCardLabel.text = "1";

        // -------------------- First Attack Ended --------------------

        battleSequenceRef.ResetOpacityLevel();
        yield return new WaitForSeconds( 0.1f );
        battleSequenceRef.ShowOpacityLevelOnExecution();

        // -------------------- Second Attack Started --------------------

        battleSequenceRef.ShowCharacterActions( 1 );
        yield return new WaitForSeconds( 0.5f );

        _opponentCreature = creatures[ 1 ];
        _opponentCreature.MoveToTargetPosition( pointPositionManagerRef.GetPointSetPositionByPositionId( false, 5 ) );
        yield return new WaitForSeconds( 0.1f );
        creatures[ 2 ].GetPlayermonActionsRef().JumpToTargetPosition( pointPositionManagerRef.GetPointSetPositionByPositionId( false, 1 ) );
        yield return new WaitForSeconds( 0.1f );
        creatures[ 5 ].GetPlayermonActionsRef().JumpToTargetPosition( pointPositionManagerRef.GetPointSetPositionByPositionId( false, 4 ) );
        yield return new WaitForSeconds( 0.8f );

        _playerCreature = creatures[ 4 ];
        _opponentCreature.GetPlayermonActionsRef().SetTargetEnemy( _playerCreature.gameObject );
        _playerCreature.SetDamageTaken( 30 );
        _playerCreature.SetWillBeStunned( true );
        _opponentCreature.GetPlayermonActionsRef().SetStayThereAfterAttack( true );
        _opponentCreature.GetPlayermonActionsRef().AttackTarget();
        yield return new WaitForSeconds( 2.5f );
        _playerCreature.SetDamageTaken( 60 );
        _opponentCreature.GetPlayermonActionsRef().SetStayThereAfterAttack( false );
        _opponentCreature.GetPlayermonActionsRef().Attack();
        yield return new WaitForSeconds( 2.5f );
        battleSequenceRef.HideCurrentCharacterActions();

        // -------------------- Second Attack Ended --------------------

        battleSequenceRef.ResetOpacityLevel();
        yield return new WaitForSeconds( 0.1f );
        battleSequenceRef.ShowOpacityLevelOnExecution();

        // -------------------- Third Attack Started --------------------

        battleSequenceRef.ShowCharacterActions( 2 );
        yield return new WaitForSeconds( 0.5f );

        _opponentCreature = creatures[ 2 ];
        _playerCreature = creatures[ 4 ];
        _opponentCreature.GetPlayermonActionsRef().SetTargetEnemy( _playerCreature.gameObject );
        _playerCreature.SetDamageTaken( 75 );
        _playerCreature.SetWillBePoisoned( true );
        _opponentCreature.GetPlayermonActionsRef().AttackTarget();
        yield return new WaitForSeconds( 3.5f );
        battleSequenceRef.HideCurrentCharacterActions();

        // -------------------- Third Attack Ended --------------------

        battleSequenceRef.ResetOpacityLevel();
        yield return new WaitForSeconds( 0.1f );
        battleSequenceRef.ShowOpacityLevelOnExecution();

        // -------------------- Fourth Attack Started --------------------

        battleSequenceRef.ShowCharacterActions( 3 );
        yield return new WaitForSeconds( 0.5f );

        _playerCreature = creatures[ 3 ];
        _opponentCreature = creatures[ 1 ];
        _playerCreature.GetPlayermonActionsRef().SetTargetEnemy( _opponentCreature.gameObject );
        _opponentCreature.SetDamageTaken( 60 );
        _playerCreature.GetPlayermonActionsRef().SetStayThereAfterAttack( true );
        _playerCreature.GetPlayermonActionsRef().AttackTarget();
        yield return new WaitForSeconds( 2.0f );
        _playerCreature.ShowPassiveTriggerEvent();
        _playerCreature.ShowFloatingDisplayPowerUp( Resources.Load<Sprite>( "StatusIcons/Icon_Morale_Up" ), "+5 Morale", bonusMoraleOutlineColor );
        _playerCreature.GetPlayermonActionsRef().SetStayThereAfterAttack( false );
        yield return new WaitForSeconds( 1.5f );
        _playerCreature.GetPlayermonActionsRef().JumpToTargetPosition( pointPositionManagerRef.GetPointSetPositionByPositionId( true, 3 ) );

        yield return new WaitForSeconds( 1.0f );
        battleSequenceRef.HideCurrentCharacterActions();
        usedCardLabel.text = "2";

        // -------------------- Fourth Attack Ended --------------------

        battleSequenceRef.ResetOpacityLevel();
        yield return new WaitForSeconds( 0.1f );
        battleSequenceRef.ShowOpacityLevelOnExecution();

        // -------------------- Fifth Attack Started --------------------

        battleSequenceRef.ShowCharacterActions( 4, false );
        yield return new WaitForSeconds( 0.5f );

        _playerCreature = creatures[ 4 ];
        _playerCreature.TakeDamage( 30 );
        _playerCreature.GetStatusBar().GetStatus( 0 ).MinusCounterNumber();
        SoundManager.Instance.PlaySoundEffect( poisonDamageTakingAudioClip );
        yield return new WaitForSeconds( 0.8f );
        _playerCreature.ShowFloatingDisplayLabel( "Stunned" );
        SoundManager.Instance.PlaySoundEffect( stunnedAudioClip );
        yield return new WaitForSeconds( 2.0f );

        /*
        _playerCreature.GetAnimatorRef().PlayReadyAnimation();
        yield return new WaitForSeconds( 0.6f );
        _playerCreature.ShowShieldEffect();

        yield return new WaitForSeconds( 1.5f );
        */

        battleSequenceRef.HideCurrentCharacterActions();
        usedCardLabel.text = "3";

        // -------------------- Fifth Attack Ended --------------------

        for (int i = 0; i < creatures.Count; i++)
        {
            creatures[ i ].GetShieldBar().SetShieldNumber( 0 );
        }

        battleSequenceRef.ResetOpacityLevel();
        yield return new WaitForSeconds( 0.1f );
        NextTurn();
    }

    public void NextTurn()
    {
        SetRoundNumber( roundNumber + 1 );
        cardDeckRef.DrawCards( 3 );

        currentGamePhase = GamePhase.NEW_ROUND;
        Invoke( "StartPlanningPhase", 0.5f );
    }

    public void OnCreatureSpawned( Creature targetCreature )
    {
        creatures.Add( targetCreature );
    }

    private void SetRoundNumber( int roundNumber )
    {
        this.roundNumber = roundNumber;
        roundNumberLabel.text = string.Format( "ROUND {0}", roundNumber );
    }

    public SkillCard CreateCard( CreatureData targetCreatureData, SkillInfo targetSkillInfo )
    {
        GameObject _skillCardObj = Instantiate( skillCardPrefabGameObject );
        SkillCard _skillCardComponent = _skillCardObj.GetComponent<SkillCard>();
        _skillCardComponent.SetUp( targetCreatureData, skillDatabase.GetSkillData( targetSkillInfo.GetSkillId() ), targetSkillInfo );

        return _skillCardComponent;
    }

    public GamePhase GetCurrentGamePhase()
    {
        return currentGamePhase;
    }

    public BattleSequence GetBattleSequenceRef()
    {
        return battleSequenceRef;
    }

    public ActionPointGauge GetActionPointGaugeRef()
    {
        return actionPointGaugeRef;
    }

    public HighlightedSkillCard GetHighlightedSkillCardRef()
    {
        return highlightedSkillCardRef;
    }

    public CharacterScriptableObject GetCharacterDatabase()
    {
        return characterDatabase;
    }

    public Creature GetCreature( int creatureId )
    {
        for (int i = 0; i < creatures.Count; i++)
        {
            Creature _creature = creatures[ i ];
            if (_creature.GetTargetCreatureData().GetCreatureId() == creatureId)
            {
                return _creature;
            }
        }

        return null;
    }

    public SkillInfo GetSkillInfo( int skillId )
    {
        for (int i = 0; i < skillInfoList.Count; i++)
        {
            SkillInfo _skillInfo = skillInfoList[ i ];
            if (_skillInfo.GetSkillId() == skillId)
            {
                return _skillInfo;
            }
        }

        return null;
    }

    public Sprite GetSkillImage( int skillId )
    {
        return GetSkillImage( skillDatabase.GetSkillData( skillId ) );
    }

    public Sprite GetSkillImage( SkillScriptableObject.SkillData targetSkillData )
    {
        return Resources.Load<Sprite>( skillDatabase.GetSkillImageFolderPath() + targetSkillData.GetSkillImageFileName() );
    }

    public void PlayCardDrawnAudioClip()
    {
        SoundManager.Instance.PlaySoundEffect( cardDrawnAudioClip );
    }

    public void PlayCardHoveredAudioClip()
    {
        SoundManager.Instance.PlaySoundEffect( cardHoveredAudioClip );
    }

    public void PlayCardSelectedAudioClip()
    {
        SoundManager.Instance.PlaySoundEffect( cardSelectedAudioClip );
    }

    public void PlayCardDeselectedAudioClip()
    {
        SoundManager.Instance.PlaySoundEffect( cardDeselectedAudioClip );
    }

    public void PlayHittingAudioClip()
    {
        SoundManager.Instance.PlaySoundEffect( hittingAudioClip );
    }

    public void PlayTargetMarkingAudioClip()
    {
        SoundManager.Instance.PlaySoundEffect( targetMarkingAudioClip );
    }

    public void PlayPowerUpAudioClip()
    {
        SoundManager.Instance.PlaySoundEffect( powerUpAudioClip );
    }

    public void PlayStunningAudioClip()
    {
        SoundManager.Instance.PlaySoundEffect( stunningAudioClip );
    }

    public void PlayPoisoningAudioClip()
    {
        SoundManager.Instance.PlaySoundEffect( poisoningAudioClip );
    }

#region Skill Info

    public class SkillInfo
    {
        private int skillId = 0;
        private CreatureData.ClassTypes classType = CreatureData.ClassTypes.NONE;
        private int actionPointNumber = 0;
        private int attackNumber = 0;
        private int shieldNumber = 0;

        public SkillInfo( int skillId, CreatureData.ClassTypes classType, int actionPointNumber, int attackNumber, int shieldNumber )
        {
            this.skillId = skillId;
            this.classType = classType;
            this.actionPointNumber = actionPointNumber;
            this.attackNumber = attackNumber;
            this.shieldNumber = shieldNumber;
        }

        public int GetSkillId()
        {
            return skillId;
        }

        public CreatureData.ClassTypes GetClassType()
        {
            return classType;
        }

        public int GetActionPointNumber()
        {
            return actionPointNumber;
        }

        public int GetAttackNumber()
        {
            return attackNumber;
        }

        public int GetShieldNumber()
        {
            return shieldNumber;
        }
    }

#endregion
}

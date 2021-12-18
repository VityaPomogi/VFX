using UnityEngine;

public class SkillCard : MonoBehaviour
{
    [SerializeField] private float rollAnimationDuration = 0.2f;
    [SerializeField] private float rollDistance = 50.0f;
    [SerializeField] private float moveAnimationDuration = 0.2f;
    [SerializeField] private float malfunctionAlphaValue = 0.5f;
    [SerializeField] private SkillCardDisplayInfo displayInfo;
    [SerializeField] private CanvasGroup canvasGroupRef;

    private CreatureData targetCreatureData = null;
    private SkillScriptableObject.SkillData targetSkillData = null;
    private Vector2 onHandPosition;
    private Vector2 rollOverPos;

    private bool isSelected = false;
    private bool isClickable = false;
    private bool isRolledOver = false;

    private RectTransform thisRectTransform;
    private BattleSequence battleSequenceRef;
    private HighlightedSkillCard highlightedSkillCardRef;
    private OnHandCardForCharacter onHandCardForCharacterRef;
    private Transform originalParent;

    public void SetUp( CreatureData targetCreatureData, SkillScriptableObject.SkillData targetSkillData, BattleGameplayManager.SkillInfo targetSkillInfo )
    {
        this.targetCreatureData = targetCreatureData;
        this.targetSkillData = targetSkillData;

        displayInfo.SetUp( targetSkillData, targetSkillInfo );

        thisRectTransform = this.GetComponent<RectTransform>();
        battleSequenceRef = BattleGameplayManager.Instance.GetBattleSequenceRef();
        highlightedSkillCardRef = BattleGameplayManager.Instance.GetHighlightedSkillCardRef();
    }

    public void SetOnHandCardForCharacterRef( OnHandCardForCharacter onHandCardForCharacterRef, Transform container )
    {
        this.onHandCardForCharacterRef = onHandCardForCharacterRef;
        thisRectTransform.SetParent( container, false );
        originalParent = thisRectTransform.parent;
    }

    public void RemoveOnHandCardForCharacterRef()
    {
        this.onHandCardForCharacterRef = null;
    }

    public OnHandCardForCharacter GetOnHandCardForCharacterRef()
    {
        return onHandCardForCharacterRef;
    }

    public void SetParent( Transform parent, bool worldPositionStays )
    {
        thisRectTransform.SetParent( parent, worldPositionStays );
    }

    public void MoveTo( Vector3 targetPosition, float moveDuration )
    {
        LeanTween.move( thisRectTransform, targetPosition, moveDuration ).setEaseOutCirc();
    }

    public void SetIsClickable( bool isClickable )
    {
        this.isClickable = isClickable;
    }

    public void SetOnHandPosition( Vector2 onHandPosition )
    {
        this.onHandPosition = onHandPosition;
        rollOverPos = onHandPosition + new Vector2( 0, rollDistance );
    }

    public Vector2 GetOnHandPosition()
    {
        return onHandPosition;
    }

    public void UpdateOnHandPosition( float animationDuration )
    {
        if (isSelected == false)
        {
            isClickable = false;
            LeanTween.move( thisRectTransform, new Vector3( onHandPosition.x, onHandPosition.y, 0.0f ), animationDuration ).setEase( LeanTweenType.easeOutCirc )
                .setOnComplete( () =>
                {
                    isClickable = true;
                    if (isRolledOver == true)
                    {
                        ProcessRollOverEvent();
                    }
                } );
        }
    }

    public void MoveToTopLayer()
    {
        thisRectTransform.SetAsLastSibling();
    }

    public void OnClick()
    {
        if (BattleGameplayManager.Instance.GetCurrentGamePhase() == BattleGameplayManager.GamePhase.PLANNING)
        {
            if (isClickable == true)
            {
                if (isSelected == false)
                {
                    BattleGameplayManager.Instance.PlayCardSelectedAudioClip();

                    isSelected = true;
                    BattleGameplayManager.Instance.GetActionPointGaugeRef().MinusActionPoints( displayInfo.GetTargetSkillInfo().GetActionPointNumber() );
                    highlightedSkillCardRef.Hide();
                    battleSequenceRef.MoveCardToSequenceCharacter( this );
                    BattleGameplayManager.Instance.ResetAllCreatureOpacityLevels();
                }
                else
                {
                    BattleGameplayManager.Instance.PlayCardDeselectedAudioClip();

                    isSelected = false;
                    BattleGameplayManager.Instance.GetActionPointGaugeRef().AddActionPoints( displayInfo.GetTargetSkillInfo().GetActionPointNumber() );
                    thisRectTransform.SetParent( originalParent, true );
                    battleSequenceRef.MoveCardToHand( this );
                }

                onHandCardForCharacterRef.SetSkillCardOnHandPositions();
                onHandCardForCharacterRef.UpdateSkillCardPositions( moveAnimationDuration );
            }
        }
    }

    public void OnRollOver()
    {
        if (BattleGameplayManager.Instance.GetCurrentGamePhase() == BattleGameplayManager.GamePhase.PLANNING)
        {
            isRolledOver = true;

            if (isClickable == true)
            {
                if (isSelected == false)
                {
                    ProcessRollOverEvent();
                }
            }
        }
    }

    private void ProcessRollOverEvent()
    {
        BattleGameplayManager.Instance.PlayCardHoveredAudioClip();

        ShowHighlightedSkillCard();
        battleSequenceRef.AddTemporaryCardToSequenceCharacter( this );
        BattleGameplayManager.Instance.HighlightCreature( onHandCardForCharacterRef.GetTargetCreature() );
    }

    private void ShowHighlightedSkillCard()
    {
        LeanTween.move( thisRectTransform, rollOverPos, rollAnimationDuration );
        highlightedSkillCardRef.Show( displayInfo );
    }

    public void OnRollOut()
    {
        if (BattleGameplayManager.Instance.GetCurrentGamePhase() == BattleGameplayManager.GamePhase.PLANNING)
        {
            isRolledOver = false;

            if (isClickable == true)
            {
                if (isSelected == false)
                {
                    LeanTween.move( thisRectTransform, onHandPosition, rollAnimationDuration );
                    highlightedSkillCardRef.Hide();
                    battleSequenceRef.RemoveTemporaryCardFromSequenceCharacter( this );
                    BattleGameplayManager.Instance.ResetAllCreatureOpacityLevels();
                }
            }
        }
    }

    public void MoveToHand()
    {
        LeanTween.move( thisRectTransform, onHandPosition, moveAnimationDuration ).setOnComplete( () => isClickable = true );
    }

    public void ShowMalfunction()
    {
        canvasGroupRef.alpha = malfunctionAlphaValue;
    }

    public bool GetIsSelected()
    {
        return isSelected;
    }

    public CreatureData GetTargetCreatureData()
    {
        return targetCreatureData;
    }
}

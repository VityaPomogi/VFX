using System.Collections.Generic;
using UnityEngine;

public class BattleSequence : MonoBehaviour
{
    [SerializeField] private float moveAnimationDuration = 0.2f;
    [SerializeField] private float opacityLevelOnExecution = 0.5f;
    [SerializeField] private BattleSequenceCharacter[] sequenceCharacters;
    [SerializeField] private ActionExecutionPanel actionExecutionPanelRef;
    [SerializeField] private CanvasGroup canvasGroupRef;
    [SerializeField] private Transform opponentCardStartPoint;

    public void SetUp( List<CreatureData> creatureDataList )
    {
        for (int i = 0; i < creatureDataList.Count; i++)
        {
            CreatureData _creatureData = creatureDataList[ i ];
            sequenceCharacters[ i ].SetUp( _creatureData );
        }
    }

    public void MoveCardToSequenceCharacter( SkillCard targetSkillCard )
    {
        targetSkillCard.SetIsClickable( false );
        GetSequenceCharacters( targetSkillCard.GetTargetCreatureData().GetCreatureId() ).AddSkill( targetSkillCard, moveAnimationDuration );
    }

    public void MoveCardToHand( SkillCard targetSkillCard )
    {
        targetSkillCard.SetIsClickable( false );
        GetSequenceCharacters( targetSkillCard.GetTargetCreatureData().GetCreatureId() ).RemoveSkill( targetSkillCard, moveAnimationDuration );
        targetSkillCard.MoveToHand();
    }

    public void AddTemporaryCardToSequenceCharacter( SkillCard targetSkillCard )
    {
        GetSequenceCharacters( targetSkillCard.GetTargetCreatureData().GetCreatureId() ).AddTemporarySkill( moveAnimationDuration );
    }

    public void RemoveTemporaryCardFromSequenceCharacter( SkillCard targetSkillCard )
    {
        GetSequenceCharacters( targetSkillCard.GetTargetCreatureData().GetCreatureId() ).RemoveTemporarySkill( moveAnimationDuration );
    }

    public void AddCardToOpponentCharacter( CreatureData targetCreatureData, BattleGameplayManager.SkillInfo targetSkillInfo )
    {
        BattleSequenceCharacter _sequenceCharacter = GetSequenceCharacters( targetCreatureData.GetCreatureId() );
        SkillCard _skillCard = BattleGameplayManager.Instance.CreateCard( targetCreatureData, targetSkillInfo );
        _skillCard.transform.position = opponentCardStartPoint.position;
        _sequenceCharacter.AddSkill( _skillCard, moveAnimationDuration );
    }

    public void RemoveCardsFromHand()
    {
        for (int i = 0; i < sequenceCharacters.Length; i++)
        {
            BattleSequenceCharacter _sequenceCharacter = sequenceCharacters[ i ];
            List<SkillCard> _skillCardList = _sequenceCharacter.GetSkillCardList();
            for (int j = 0; j < _skillCardList.Count; j++)
            {
                SkillCard _skillCard = _skillCardList[ j ];
                OnHandCardForCharacter _onHandCardForCharacter = _skillCard.GetOnHandCardForCharacterRef();
                if (_onHandCardForCharacter != null)
                {
                    _onHandCardForCharacter.RemoveCard( _skillCard );
                    _skillCard.RemoveOnHandCardForCharacterRef();
                }
            }
        }
    }

    public void ShowCharacterActions( int sequenceIndex, bool isAbleToFunction = true )
    {
        BattleSequenceCharacter _sequenceCharacter = sequenceCharacters[ sequenceIndex ];
        actionExecutionPanelRef.ShowActions( _sequenceCharacter.GetSkillCardList(), isAbleToFunction );
    }

    public void HideCurrentCharacterActions()
    {
        actionExecutionPanelRef.HideCurrentActions();
    }

    public void ShowOpacityLevelOnExecution()
    {
        canvasGroupRef.alpha = opacityLevelOnExecution;
    }

    public void ResetOpacityLevel()
    {
        canvasGroupRef.alpha = 1.0f;
    }

    public BattleSequenceCharacter GetSequenceCharacters( int creatureId )
    {
        for (int i = 0; i < sequenceCharacters.Length; i++)
        {
            BattleSequenceCharacter _sequenceCharacter = sequenceCharacters[ i ];
            if (_sequenceCharacter.GetTargetCreatureData().GetCreatureId() == creatureId)
            {
                return _sequenceCharacter;
            }
        }

        return null;
    }
}

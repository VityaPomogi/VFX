using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnHandCardForCharacter : MonoBehaviour
{
    [SerializeField] private float cardDistance = 100.0f;
    [SerializeField] private Text sequenceNumberLabel;
    [SerializeField] private Transform cardContainer;
    [SerializeField] private InBoxCharacter inBoxCharacterRef;

    private CreatureData targetCreatureData = null;
    private List<SkillCard> skillCardList = new List<SkillCard>();
    private Creature targetCreature = null;

    private RectTransform thisRectTransform;

    void Awake()
    {
        thisRectTransform = this.GetComponent<RectTransform>();
    }

    public void SetUp( CreatureData targetCreatureData )
    {
        this.targetCreatureData = targetCreatureData;
        SetSequenceNumber( targetCreatureData.GetSequence() );
        inBoxCharacterRef.SetUp( true, targetCreatureData.GetHeadId(), targetCreatureData.GetEyeId(), targetCreatureData.GetBodyId() );
        targetCreature = BattleGameplayManager.Instance.GetCreature( targetCreatureData.GetCreatureId() );
    }

    public void SetPositionX( float posX )
    {
        thisRectTransform.anchoredPosition = new Vector2( posX, thisRectTransform.anchoredPosition.y );
    }

    public void SetSequenceNumber( int sequenceNumber )
    {
        sequenceNumberLabel.text = sequenceNumber.ToString();
    }

    public SkillCard CreateCard( BattleGameplayManager.SkillInfo targetSkillInfo )
    {
        SkillCard _skillCardComponent = BattleGameplayManager.Instance.CreateCard( targetCreatureData, targetSkillInfo );
        _skillCardComponent.SetOnHandCardForCharacterRef( this, cardContainer );
        skillCardList.Add( _skillCardComponent );

        SetSkillCardOnHandPositions();

        return _skillCardComponent;
    }

    public void RemoveCard( SkillCard targetSkillCard)
    {
        skillCardList.Remove( targetSkillCard );
    }

    public void SetSkillCardOnHandPositions()
    {
        List<SkillCard> _cards = new List<SkillCard>();
        for (int i = 0; i < skillCardList.Count; i++)
        {
            SkillCard _skillCard = skillCardList[ i ];
            if (_skillCard.GetIsSelected() == false)
            {
                _cards.Add( _skillCard );
            }
        }

        float _startX = -0.5f * ( _cards.Count - 1 ) * cardDistance;
        for (int i = 0; i < _cards.Count; i++)
        {
            SkillCard _card = _cards[ i ];
            _card.SetOnHandPosition( new Vector2( _startX + ( i * cardDistance ), 0.0f ) );
            _card.MoveToTopLayer();
        }
    }

    public void UpdateSkillCardPositions( float animationDuration )
    {
        for (int i = 0; i < skillCardList.Count; i++)
        {
            skillCardList[ i ].UpdateOnHandPosition( animationDuration );
        }
    }

    public CreatureData GetTargetCreatureData()
    {
        return targetCreatureData;
    }

    public Creature GetTargetCreature()
    {
        return targetCreature;
    }
}

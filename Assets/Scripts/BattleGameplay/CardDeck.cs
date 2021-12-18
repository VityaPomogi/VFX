using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CardDeck : MonoBehaviour
{
    [SerializeField] private OnHandCardManager onHandCardManagerRef;
    [SerializeField] private TextMeshProUGUI remainingCardLabel;
    [SerializeField] private TextMeshProUGUI usedCardLabel;

    private List<CardDeckSkill> cardDeckSkillList = new List<CardDeckSkill>();
    private List<CardDeckSkill> dynamicCardDeckSkillList = new List<CardDeckSkill>();
    private int numberOfRemainingCards = 0;

    public class CardDeckSkill
    {
        private CreatureData targetCreatureData;
        private int skillId;

        public CardDeckSkill( CreatureData targetCreatureData, int skillId )
        {
            this.targetCreatureData = targetCreatureData;
            this.skillId = skillId;
        }

        public CreatureData GetTargetCreatureData()
        {
            return targetCreatureData;
        }

        public int GetSkillId()
        {
            return skillId;
        }
    }

    public void SetUp( List<CardDeckSkill> cardDeckSkillList )
    {
        this.cardDeckSkillList = cardDeckSkillList;
        ShuffleCardDeck();
    }

    public void ShuffleCardDeck()
    {
        dynamicCardDeckSkillList = new List<CardDeckSkill>();
        for (int i = 0; i < cardDeckSkillList.Count; i++)
        {
            dynamicCardDeckSkillList.Add( cardDeckSkillList[ i ] );
        }

    //    dynamicCardDeckSkillList.Shuffle();
    }

    public void SetNumberOfRemainingCards( int numberOfRemainingCards )
    {
        this.numberOfRemainingCards = numberOfRemainingCards;
        remainingCardLabel.text = numberOfRemainingCards.ToString();
    }

    private void MinusCards( int amount )
    {
        SetNumberOfRemainingCards( numberOfRemainingCards - amount );
    }

    public void DrawCards( int numberOfCards )
    {
        if (numberOfCards > numberOfRemainingCards)
        {
            numberOfCards = numberOfRemainingCards;
        }

        if (numberOfCards > 0)
        {
            for (int i = 0; i < numberOfCards; i++)
            {
                CardDeckSkill _cardDeckSkill = dynamicCardDeckSkillList[ 0 ];
                int _skillId = _cardDeckSkill.GetSkillId();
                BattleGameplayManager.SkillInfo _skillInfo = BattleGameplayManager.Instance.GetSkillInfo( _skillId );
                onHandCardManagerRef.DrawCard( _cardDeckSkill.GetTargetCreatureData(), _skillInfo );
                dynamicCardDeckSkillList.RemoveAt( 0 );
            }

            MinusCards( numberOfCards );
        }

        BattleGameplayManager.Instance.PlayCardDrawnAudioClip();
    }

    public OnHandCardManager GetOnHandCardManagerRef()
    {
        return onHandCardManagerRef;
    }
}

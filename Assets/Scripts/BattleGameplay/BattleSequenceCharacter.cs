using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleSequenceCharacter : MonoBehaviour
{
    [SerializeField] private float leftSequenceX = 0.0f;
    [SerializeField] private float rightCharacterX = 0.0f;
    [SerializeField] private float cardPositionY = 0.0f;
    [SerializeField] private float cardDistance = 100.0f;
    [SerializeField] private RectTransform characterTransform;
    [SerializeField] private RectTransform sequenceNumberTransform;
    [SerializeField] private Text sequenceNumberLabel;
    [SerializeField] private Image boxImage;
    [SerializeField] private Sprite playerBoxSprite;
    [SerializeField] private Sprite enemyBoxSprite;
    [SerializeField] private InBoxCharacter inBoxCharacterRef;
    [SerializeField] private Transform cardContainer;

    private CreatureData targetCreatureData = null;
    private List<SkillCard> skillCardList = new List<SkillCard>();

    public void SetUp( CreatureData targetCreatureData )
    {
        this.targetCreatureData = targetCreatureData;

        bool _isPlayer = targetCreatureData.GetIsPlayer();
        boxImage.sprite = ( _isPlayer == true ) ? playerBoxSprite : enemyBoxSprite;
        sequenceNumberLabel.text = targetCreatureData.GetSequence().ToString();

        bool _isFacingRight = false;
        if (_isPlayer == true)
        {
            _isFacingRight = true;
            characterTransform.anchoredPosition = new Vector2( rightCharacterX, characterTransform.anchoredPosition.y );
            sequenceNumberTransform.anchoredPosition = new Vector2( leftSequenceX, sequenceNumberTransform.anchoredPosition.y );
        }

        inBoxCharacterRef.SetUp( _isFacingRight, targetCreatureData.GetHeadId(), targetCreatureData.GetEyeId(), targetCreatureData.GetBodyId() );
    }

    public void AddSkill( SkillCard targetSkillCard, float moveAnimationDuration )
    {
        skillCardList.Add( targetSkillCard );
        targetSkillCard.transform.SetParent( cardContainer, true );

        RectTransform _skillCardRect = targetSkillCard.GetComponent<RectTransform>();
        LeanTween.move( _skillCardRect, new Vector3( 0, cardPositionY, 0 ), moveAnimationDuration ).setEase( LeanTweenType.easeOutCirc );
        LeanTween.scale( _skillCardRect, Vector3.one, moveAnimationDuration ).setEase( LeanTweenType.easeOutCirc ).setOnComplete( () => MakeCardClickable( targetSkillCard ) );

        UpdateCardPositions( skillCardList.Count - 1, 0, moveAnimationDuration );
    }

    public void RemoveSkill( SkillCard targetSkillCard, float moveAnimationDuration )
    {
        skillCardList.Remove( targetSkillCard );
        UpdateCardPositions( skillCardList.Count, 1, moveAnimationDuration );
    }

    public void AddTemporarySkill( float moveAnimationDuration )
    {
        UpdateCardPositions( skillCardList.Count, 0, moveAnimationDuration );
    }

    public void RemoveTemporarySkill( float moveAnimationDuration )
    {
        UpdateCardPositions( skillCardList.Count, 1, moveAnimationDuration );
    }

    public void ClearSkillList()
    {
        skillCardList.Clear();
    }

    private void UpdateCardPositions( int cardCount, int offset, float moveAnimationDuration )
    {
        for (int i = 0; i < cardCount; i++)
        {
            RectTransform _skillCardRect = skillCardList[ i ].GetComponent<RectTransform>();
            LeanTween.cancel( _skillCardRect );
            LeanTween.move( _skillCardRect, new Vector3( 0, cardPositionY - ( cardCount - i - offset ) * cardDistance, 0 ), moveAnimationDuration );
        }
    }

    private void MakeCardClickable( SkillCard targetSkillCard )
    {
        targetSkillCard.SetIsClickable( true );
    }

    public CreatureData GetTargetCreatureData()
    {
        return targetCreatureData;
    }

    public List<SkillCard> GetSkillCardList()
    {
        return skillCardList;
    }
}

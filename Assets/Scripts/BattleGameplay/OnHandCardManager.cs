using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnHandCardManager : MonoBehaviour
{
    [SerializeField] private float drawAnimationDuration = 0.5f;
    [SerializeField] private OnHandCardForCharacter[] onHandCardForCharacters;
    [SerializeField] private Transform cardDrawingPoint;
    [SerializeField] private RectTransform startPoint;
    [SerializeField] private RectTransform endPoint;

    private Vector3 cardDrawingPos;

    void Awake()
    {
        cardDrawingPos = cardDrawingPoint.position;
    }

    public void SetUp( List<CreatureData> creatureDataList )
    {
        float _startX = startPoint.anchoredPosition.x;
        float _distance = endPoint.anchoredPosition.x - _startX;
        float _distributionDistance = _distance / ( onHandCardForCharacters.Length + 1 );
        int _creatureIndex = 0;
        for (int i = 0; i < onHandCardForCharacters.Length; i++)
        {
            OnHandCardForCharacter _onHandCardForCharacter = onHandCardForCharacters[ i ];
            _onHandCardForCharacter.SetPositionX( _startX + ( ( i + 1 ) * _distributionDistance ) );

            for (int j = _creatureIndex; j < creatureDataList.Count; j++)
            {
                CreatureData _creatureData = creatureDataList[ j ];
                if (_creatureData.GetIsPlayer() == true)
                {
                    _onHandCardForCharacter.SetUp( _creatureData );
                    _creatureIndex = j + 1;
                    break;
                }
            }
        }
    }

    public void DrawCard( CreatureData targetCreatureData, BattleGameplayManager.SkillInfo targetSkillInfo )
    {
        OnHandCardForCharacter _onHandCardForCharacter = GetOnHandCardForCharacter( targetCreatureData );
        SkillCard _skillCard = _onHandCardForCharacter.CreateCard( targetSkillInfo );

        RectTransform _skillCardRect = _skillCard.GetComponent<RectTransform>();
        _skillCardRect.position = cardDrawingPos;
        _skillCardRect.localScale = Vector3.zero;

        LeanTween.scale( _skillCardRect, Vector3.one, drawAnimationDuration ).setEase( LeanTweenType.easeOutCirc );

        _onHandCardForCharacter.UpdateSkillCardPositions( drawAnimationDuration );
    }

    public OnHandCardForCharacter GetOnHandCardForCharacter( CreatureData targetCreatureData )
    {
        for (int i = 0; i < onHandCardForCharacters.Length; i++)
        {
            OnHandCardForCharacter _onHandCardForCharacter = onHandCardForCharacters[ i ];
            if (_onHandCardForCharacter.GetTargetCreatureData().GetCreatureId() == targetCreatureData.GetCreatureId())
            {
                return _onHandCardForCharacter;
            }
        }

        return null;
    }
}

using System.Collections.Generic;
using UnityEngine;

public class ActionExecutionPanel : MonoBehaviour
{
    [SerializeField] private float cardDistance = 100.0f;
    [SerializeField] private float moveDuration = 0.3f;

    private List<SkillCard> currentSkillCardList;

    public void ShowActions( List<SkillCard> skillCardList, bool isAbleToFunction )
    {
        this.currentSkillCardList = skillCardList;

        float _startX = ( ( -0.5f * skillCardList.Count ) + 0.5f ) * cardDistance;
        for (int i = 0; i < skillCardList.Count; i++)
        {
            SkillCard _skillCard = skillCardList[ i ];

            if (isAbleToFunction == false)
            {
                _skillCard.ShowMalfunction();
            }

            _skillCard.SetParent( this.transform, true );
            _skillCard.MoveTo( new Vector3( _startX + ( i * cardDistance ), 0.0f ), moveDuration );
        }
    }

    public void HideCurrentActions()
    {
        for (int i = 0; i < currentSkillCardList.Count; i++)
        {
            Destroy( currentSkillCardList[ i ].gameObject );
        }

        currentSkillCardList.Clear();
    }
}

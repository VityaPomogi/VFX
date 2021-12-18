using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureStatusBar : MonoBehaviour
{
    [SerializeField] private float statusIconDistance = 1.0f;
    [SerializeField] private float statusIconAnimationTime = 0.3f;
    [SerializeField] private CreatureStatus creatureStatusPrefab;

    private GameObject creatureStatusPrefabObject = null;
    private List<CreatureStatus> creatureStatusList = null;

    void Awake()
    {
        creatureStatusPrefabObject = creatureStatusPrefab.gameObject;
        creatureStatusList = new List<CreatureStatus>();
    }

    public void AddStatus( Sprite iconSprite, int counterNumber, Color32 labelOutlineColor )
    {
        GameObject _creatureStatusObj = Instantiate( creatureStatusPrefabObject );
        _creatureStatusObj.transform.SetParent( this.transform, false );
        _creatureStatusObj.transform.localPosition = new Vector3( creatureStatusList.Count * statusIconDistance, 0, 0 );
        _creatureStatusObj.transform.localScale = Vector3.zero;

        CreatureStatus _creatureStatus = _creatureStatusObj.GetComponent<CreatureStatus>();
        _creatureStatus.SetUp( iconSprite, counterNumber, labelOutlineColor );

        creatureStatusList.Add( _creatureStatus );

        LeanTween.scale( _creatureStatusObj, Vector3.one, statusIconAnimationTime ).setEase( LeanTweenType.easeOutBack );
    }

    public CreatureStatus GetStatus( int indexInList )
    {
        return creatureStatusList[ indexInList ];
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointPositionManager : MonoBehaviour
{
    [SerializeField] private BattleGameplayManager battleGameplayManagerRef;

    [Header( "Points" )]
    [SerializeField] private Vector3 positionOffset = Vector3.zero;
    [SerializeField] private bool isPointIndicatorShown = false;
    [SerializeField] private PointSet[] _pointPos1 = null;
    [SerializeField] private PointSet[] _pointPos2 = null;

    [Header("Playermon")]
    [SerializeField] private GameObject _playermonPrefab = null;
    [SerializeField] private int _totalPlayermonPerTeam = 3;

    [Header("Debug")]
    [SerializeField] private bool _spawn = false;

    private List<GameObject> _team1Playermons = new List<GameObject>();
    private List<GameObject> _team2Playermons = new List<GameObject>();

    private List<int> _randomedIndex1 = new List<int>();
    private List<int> _randomedIndex2 = new List<int>();

    void Awake()
    {
        for (int i = 0; i < _pointPos1.Length; i++)
        {
            _pointPos1[ i ].ShowPointIndicator( isPointIndicatorShown );
        }
        for (int i = 0; i < _pointPos2.Length; i++)
        {
            _pointPos2[ i ].ShowPointIndicator( isPointIndicatorShown );
        }
    }

    public Creature SpawnCreatureOnPoint( CreatureData targetCreatureData )
    {
        GameObject _creatureObject = Instantiate( _playermonPrefab );

        PointSet _pointSet = GetPointSetByPositionId( targetCreatureData.GetIsPlayer(), targetCreatureData.GetPositionId() );
        _creatureObject.transform.position = _pointSet.position + positionOffset;
        _creatureObject.GetComponent<PlayermonActions>().SetDefaultSortingLayer( _pointSet.sortingLayerName, _pointSet.sortingOrder );
        _creatureObject.name = string.Format( "Team1_{0}_{1}", _pointSet.sortingLayerName, _pointSet.sortingOrder.ToString() );

        Creature _creature = _creatureObject.GetComponent<Creature>();
        _creature.SetUp( targetCreatureData );

        _team1Playermons.Add( _creatureObject );

        return _creature;
    }

    public PointSet GetPointSetByPositionId( bool isPlayer, int positionId )
    {
        PointSet[] _pointSets = ( isPlayer == true ) ? _pointPos1 : _pointPos2;
        for (int i = 0; i < _pointSets.Length; i++)
        {
            PointSet _pointSet = _pointSets[ i ];
            if (_pointSet.GetPositionId() == positionId)
            {
                return _pointSet;
            }
        }

        return null;
    }

    public Vector3 GetPointSetPositionByPositionId( bool isPlayer, int positionId )
    {
        return ( GetPointSetByPositionId( isPlayer, positionId ).GetPoint().position + positionOffset );
    }

    /*
    void Start()
    {
        SpawnPlayermonsOnPoint();
    }
    */

    private void Update()
    {
        if (_spawn)
        {
            _spawn = false;
            SpawnPlayermonsOnPoint();
        }
    }

    public void SpawnPlayermonsOnPoint()
    {
        ResetList();

        if (_totalPlayermonPerTeam > _pointPos1.Length)
        {
            _totalPlayermonPerTeam = _pointPos1.Length;
        }

        for (int i = 0; i < _totalPlayermonPerTeam * 2; i++)
        {
            GameObject go = Instantiate(_playermonPrefab);

            bool _isFacingRight = false;
            if (i < _totalPlayermonPerTeam)
            {
                int index = GetNewPointPositionIndex(true);

                if (index == -1)
                {
                    continue;
                }

                _isFacingRight = true;

                go.transform.position = _pointPos1[index].position;
                go.GetComponent<PlayermonActions>().SetDefaultSortingLayer(_pointPos1[index].sortingLayerName, _pointPos1[index].sortingOrder);
                go.name = string.Format("Team1_{0}_{1}", _pointPos1[index].sortingLayerName, _pointPos1[index].sortingOrder.ToString());

                _team1Playermons.Add(go);
            }
            else
            {
                int index = GetNewPointPositionIndex(false);

                if (index == -1)
                {
                    continue;
                }

                _isFacingRight = false;

                go.transform.position = _pointPos2[index].position;
                go.GetComponent<PlayermonActions>().SetDefaultSortingLayer(_pointPos2[index].sortingLayerName, _pointPos2[index].sortingOrder);
                go.name = string.Format("Team2_{0}_{1}", _pointPos2[index].sortingLayerName, _pointPos2[index].sortingOrder.ToString());

                _team2Playermons.Add(go);
            }

            /*
            Creature _creature = go.GetComponent<Creature>();
            _creature.SetUp( Random.Range( 300, 500 ), _isFacingRight );

            battleGameplayManagerRef.OnCreatureSpawned( _creature );
            */
        }

        SetTargetEnemyForAllPlayermon();
    }

    private void SetTargetEnemyForAllPlayermon()
    {
        for (int i = 0; i < _totalPlayermonPerTeam; i++)
        {
            _team1Playermons[i].GetComponent<PlayermonActions>().SetTargetEnemy(_team2Playermons[Random.Range(0, _team2Playermons.Count)]);
            _team2Playermons[i].GetComponent<PlayermonActions>().SetTargetEnemy(_team1Playermons[Random.Range(0, _team2Playermons.Count)]);
        }
    }

    private int GetNewPointPositionIndex(bool isTeam1)
    {
        int index = -1;

        if (isTeam1 ? (_randomedIndex1.Count >= _pointPos1.Length) : (_randomedIndex2.Count >= _pointPos2.Length))
        {
            return index;
        }

        if (_totalPlayermonPerTeam == _pointPos1.Length)
        {
            index = (isTeam1 ? _randomedIndex1 : _randomedIndex2).Count;
        }
        else
        {
            index = Random.Range(0, (isTeam1 ? _pointPos1 : _pointPos2).Length);
        }

        if ((isTeam1 ? _randomedIndex1 : _randomedIndex2).Contains(index))
        {
            return GetNewPointPositionIndex(isTeam1);
        }
        else
        {
            (isTeam1 ? _randomedIndex1 : _randomedIndex2).Add(index);
            return index;
        }

        //if (isTeam1)
        //{
        //    if (_totalPlayermonPerTeam == _pointPos1.Length)
        //    {
        //        index = _randomedIndex1.Count;
        //    }
        //    else
        //    {
        //        index = Random.Range(0, (isTeam1 ? _pointPos1 : _pointPos2).Length);
        //    }

        //    if (_randomedIndex1.Contains(index))
        //    {
        //        return GetNewPointPositionIndex(true);
        //    }
        //    else
        //    {
        //        (isTeam1 ? _randomedIndex1 : _randomedIndex2).Add(index);
        //        return index;
        //    }
        //}
        //else
        //{
        //    index = Random.Range(0, _pointPos2.Length);
        //    if (_randomedIndex2.Contains(index))
        //    {
        //        return GetNewPointPositionIndex(false);
        //    }
        //    else
        //    {
        //        _randomedIndex2.Add(index);
        //        return index;
        //    }
        //}
    }

    private void ResetList()
    {
        if (_team1Playermons.Count > 0)
        {
            foreach (GameObject go in _team1Playermons)
            {
                Destroy(go);
            }
        }

        if (_team2Playermons.Count > 0)
        {
            foreach (GameObject go in _team2Playermons)
            {
                Destroy(go);
            }
        }

        _team1Playermons.Clear();
        _team2Playermons.Clear();
        _randomedIndex1.Clear();
        _randomedIndex2.Clear();
    }


    [System.Serializable]
    public class PointSet
    {
        [SerializeField] private int positionId = 0;
        [SerializeField] private Transform _point = null;
        [SerializeField] private string _sortingLayerName = "Default";
        [SerializeField] private int _sortingOrder = 0;
        [SerializeField] private GameObject pointIndicator;

        public Vector3 position => _point != null ? _point.position : Vector3.zero;
        public string sortingLayerName => _sortingLayerName ??= "Default";
        public int sortingOrder => _sortingOrder;

        public void ShowPointIndicator( bool isShown )
        {
            pointIndicator.SetActive( isShown );
        }

        public int GetPositionId()
        {
            return positionId;
        }

        public Transform GetPoint()
        {
            return _point;
        }
    }
}

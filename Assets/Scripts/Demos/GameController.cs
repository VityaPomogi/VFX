using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private bool randomizeOnStart = false;
    [SerializeField] private PointPositionManager pointPositionManagerRef;
    [SerializeField] private SwappableCreatureV3[] swappableCreatures;
    [SerializeField] private Animator[] animators;
    [SerializeField] private string[] animationNames;
    [SerializeField] private Creature[] creatures;

    private int animationIndex = 1;

    void Start()
    {
        if (randomizeOnStart == true)
        {
            ClickToRandomize();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown( KeyCode.Q ) == true)
        {
            ClickToDamageAllCreatures();
        }
        else if (Input.GetKeyDown( KeyCode.W ) == true)
        {
            ClickToHealAllCreatures();
        }
        else if (Input.GetKeyDown( KeyCode.R ) == true)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene( UnityEngine.SceneManagement.SceneManager.GetActiveScene().name );
        }
    }

    public void ClickToRandomize()
    {
        if (swappableCreatures.Length == 0)
        {
            swappableCreatures = GameObject.FindObjectsOfType<SwappableCreatureV3>();
        }

        for (int i = 0; i < swappableCreatures.Length; i++)
        {
            swappableCreatures[ i ].Randomize();
        }
    }

    public void ClickToChangePose()
    {
        if (animators.Length == 0)
        {
            animators = GameObject.FindObjectsOfType<Animator>();
        }

        for (int i = 0; i < animators.Length; i++)
        {
            animators[ i ].Play( animationNames[ animationIndex ] );
        }
        animationIndex = ( animationIndex + 1 ) % animationNames.Length;
    }

    public void ClickToChangePositions()
    {
        swappableCreatures = new SwappableCreatureV3[ 0 ];
        animators = new Animator[ 0 ];
        pointPositionManagerRef.SpawnPlayermonsOnPoint();
        StartCoroutine( DelayToRandomize() );
    }

    private IEnumerator DelayToRandomize()
    {
        yield return null;
        ClickToRandomize();
    }

    public void ClickToDamageAllCreatures()
    {
        if (creatures.Length == 0)
        {
            creatures = GameObject.FindObjectsOfType<Creature>();
        }

        for (int i = 0; i < creatures.Length; i++)
        {
            creatures[ i ].TakeDamage( Random.Range( 10.0f, 100.0f ) );
        }
    }

    public void ClickToHealAllCreatures()
    {
        if (creatures.Length == 0)
        {
            creatures = GameObject.FindObjectsOfType<Creature>();
        }

        for (int i = 0; i < creatures.Length; i++)
        {
            creatures[ i ].Heal( Random.Range( 10.0f, 100.0f ) );
        }
    }
}

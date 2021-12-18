using UnityEngine;
using System.Collections;

/// <summary>
/// Convenience class that creates an ephemeral MonoBehaviour instance through which static classes can call StartCoroutine.
/// </summary>
public class Coroutiner
{
    public static Coroutine StartCoroutine( IEnumerator iterationResult )
    {
        // Create GameObject with MonoBehaviour to handle task.
        GameObject _routineHandlerGO = new GameObject( "Coroutiner" );
        CoroutinerInstance routineHandler = _routineHandlerGO.AddComponent( typeof( CoroutinerInstance ) ) as CoroutinerInstance;
        return routineHandler.ProcessWork( iterationResult );
    }
}

public class CoroutinerInstance : MonoBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad( this );
    }

    public Coroutine ProcessWork( IEnumerator iterationResult )
    {
        return StartCoroutine( DestroyWhenComplete( iterationResult ) );
    }

    public IEnumerator DestroyWhenComplete( IEnumerator iterationResult )
    {
        yield return StartCoroutine( iterationResult );
        Destroy( this.gameObject );
    }
}

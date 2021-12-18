using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TestServerApiManager : MonoBehaviour
{
    void Start()
    {
        TestGet();
        TestPost();
        TestPut();

        Debug.Log( "ipv4 = " + IPManager.GetIP( IPManager.ADDRESSFAM.IPv4 ) );
        Debug.Log( "ipv6 = " + IPManager.GetIP( IPManager.ADDRESSFAM.IPv6 ) );
    }

    private void TestGet()
    {
        ServerApiManager.Get( "https://jsonplaceholder.typicode.com/posts", OnGet );
    }

    private void TestPost()
    {
        Dictionary<string,string> _headers = ServerApiManager.GetHeaders( true );

        WWWForm _parameters = new WWWForm();
        _parameters.AddField( "title", "foo" );
        _parameters.AddField( "body", "bar" );
        _parameters.AddField( "userId", 1 );

        ServerApiManager.Post( "https://jsonplaceholder.typicode.com/posts", _headers, _parameters, OnPost );
    }

    private void TestPut()
    {
        Dictionary<string,string> _headers = ServerApiManager.GetHeaders( true );

        WWWForm _parameters = new WWWForm();
        _parameters.AddField( "id", 1 );
        _parameters.AddField( "title", "foo2" );
        _parameters.AddField( "body", "bar2" );
        _parameters.AddField( "userId", 2 );

        ServerApiManager.Put( "https://jsonplaceholder.typicode.com/posts/1", _headers, _parameters, OnPut );
    }

    private void OnGet( UnityWebRequest.Result result, string resultText )
    {
        if (result == UnityWebRequest.Result.Success)
        {
            Debug.Log( "[ GET - Success ]\n" + resultText );
        }
        else
        {
            Debug.Log( "[ GET - Failed ]\n" + resultText );
        }
    }

    private void OnPost( UnityWebRequest.Result result, string resultText )
    {
        if (result == UnityWebRequest.Result.Success)
        {
            Debug.Log( "[ POST - Success ]\n" + resultText );
        }
        else
        {
            Debug.Log( "[ POST - Failed ]\n" + resultText );
        }
    }

    private void OnPut( UnityWebRequest.Result result, string resultText )
    {
        if (result == UnityWebRequest.Result.Success)
        {
            Debug.Log( "[ PUT - Success ]\n" + resultText );
        }
        else
        {
            Debug.Log( "[ PUT - Failed ]\n" + resultText );
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class ServerApiManager : MonoBehaviour
{
    private const DomainType targetDomainType = DomainType.DEVELOPMENT;
    private const string productionDomain = "";
    private const string developmentDomain = "https://api-staging.playermon.com/api/";
    private const string apiKey = "ebb36ae9-2e45-4d38-9490-1378bcc778bb";

    private const string headerContentTypeApplicationWwwForm = "application/x-www-form-urlencoded";
    private const string headerContentTypeApplicationJson = "application/json";
    private const string headerContentTypeMultipartFormData = "multipart/form-data";

    public static string DOMAIN
    {
        get
        {
            if (targetDomainType == DomainType.PRODUCTION)
            {
                return productionDomain;
            }

            return developmentDomain;
        }
    }

    private enum DomainType
    {
        PRODUCTION,
        DEVELOPMENT
    }

    public enum HeaderContentType
    {
        NONE,
        APPLICATION_WWW_FORM,
        APPLICATION_JSON,
        MULTIPART_FORM_DATA
    }

#region Unity Web Request

    public static void Get( string uri, Action<UnityWebRequest.Result,string> onComplete = null )
    {
        Get( uri, null, onComplete );
    }

    public static void Get( string uri, Dictionary<string,string> headers, Action<UnityWebRequest.Result,string> onComplete = null )
    {
        UnityWebRequest _webRequest = UnityWebRequest.Get( uri );
        Coroutiner.StartCoroutine( RunProcessingWebRequest( _webRequest, headers, onComplete ) );
    }

    public static void Post( string uri, Dictionary<string,string> headers, WWWForm parameters, Action<UnityWebRequest.Result,string> onComplete = null )
    {
        UnityWebRequest _webRequest = UnityWebRequest.Post( uri, parameters );
        Coroutiner.StartCoroutine( RunProcessingWebRequest( _webRequest, headers, onComplete ) );
    }

    public static void Post( string uri, Dictionary<string,string> headers, List<IMultipartFormSection> parameters, Action<UnityWebRequest.Result,string> onComplete = null )
    {
        UnityWebRequest _webRequest = UnityWebRequest.Post( uri, parameters );
        Coroutiner.StartCoroutine( RunProcessingWebRequest( _webRequest, headers, onComplete ) );
    }

    public static void Post( string uri, Dictionary<string,string> headers, string postData, Action<UnityWebRequest.Result,string> onComplete = null )
    {
        UnityWebRequest _webRequest = GetWebRequestForJsonFormat( uri, "POST", headers, postData );
        Coroutiner.StartCoroutine( RunProcessingWebRequest( _webRequest, headers, onComplete ) );
    }

    public static void Put( string uri, Dictionary<string,string> headers, WWWForm parameters, Action<UnityWebRequest.Result,string> onComplete = null )
    {
        Put( uri, headers, parameters.data, onComplete );
    }

    public static void Put( string uri, Dictionary<string,string> headers, byte[] byteData, Action<UnityWebRequest.Result,string> onComplete = null )
    {
        UnityWebRequest _webRequest = UnityWebRequest.Put( uri, byteData );
        Coroutiner.StartCoroutine( RunProcessingWebRequest( _webRequest, headers, onComplete ) );
    }

    public static void Patch( string uri, Dictionary<string,string> headers, WWWForm parameters, Action<UnityWebRequest.Result,string> onComplete = null )
    {
        UnityWebRequest _webRequest = UnityWebRequest.Put( uri, parameters.data );
        _webRequest.method = "PATCH";
        Coroutiner.StartCoroutine( RunProcessingWebRequest( _webRequest, headers, onComplete ) );
    }

    public static void Patch( string uri, Dictionary<string,string> headers, string postData, Action<UnityWebRequest.Result,string> onComplete = null )
    {
        UnityWebRequest _webRequest = GetWebRequestForJsonFormat( uri, "PATCH", headers, postData );
        Coroutiner.StartCoroutine( RunProcessingWebRequest( _webRequest, headers, onComplete ) );
    }

    public static void Delete( string uri, Dictionary<string,string> headers, Action<UnityWebRequest.Result,string> onComplete = null )
    {
        UnityWebRequest _webRequest = GetWebRequestForJsonFormat( uri, "DELETE", headers );
        Coroutiner.StartCoroutine( RunProcessingWebRequest( _webRequest, headers, onComplete ) );
    }

    private static IEnumerator RunProcessingWebRequest( UnityWebRequest webRequest, Dictionary<string,string> headers, Action<UnityWebRequest.Result,string> onComplete )
    {
        using ( webRequest )
        {
            SetUpWebRequestHeaders( webRequest, headers );
            yield return webRequest.SendWebRequest();
            ProcessResult( webRequest, onComplete );
        }
    }

    private static void SetUpWebRequestHeaders( UnityWebRequest webRequest, Dictionary<string,string> headers )
    {
        if (headers != null)
        {
            foreach (KeyValuePair<string,string> _header in headers)
            {
                webRequest.SetRequestHeader( _header.Key, _header.Value );
            }
        }
    }

    private static void ProcessResult( UnityWebRequest webRequest, Action<UnityWebRequest.Result,string> onComplete )
    {
        UnityWebRequest.Result _result = webRequest.result;
        switch (_result)
        {
            case UnityWebRequest.Result.ConnectionError:
            case UnityWebRequest.Result.DataProcessingError:
            case UnityWebRequest.Result.ProtocolError:

                if (onComplete != null)
                {
                    onComplete( _result, webRequest.error );
                }

                break;

            case UnityWebRequest.Result.Success:

                if (onComplete != null)
                {
                    onComplete( _result, webRequest.downloadHandler.text );
                }

                break;
        }
    }

    private static UnityWebRequest GetWebRequestForJsonFormat( string uri, string methodName, Dictionary<string,string> headers, string postData = "" )
    {
        byte[] _byteData = null;
        if (postData != "")
        {
            _byteData = Encoding.UTF8.GetBytes( postData );
        }

        UnityWebRequest _webRequest = new UnityWebRequest( uri );
        foreach (KeyValuePair<string,string> _item in headers)
        {
            _webRequest.SetRequestHeader( _item.Key, _item.Value );
        }
        _webRequest.method = methodName;

        if (_byteData != null)
        {
            _webRequest.uploadHandler = new UploadHandlerRaw( _byteData );
        }

        _webRequest.downloadHandler = new DownloadHandlerBuffer();

        return _webRequest;
    }

#endregion

#region WWW

    public static void PutUsingWWW( string uri, Dictionary<string,string> headers, byte[] byteData, Action<string> onComplete = null )
    {
        Coroutiner.StartCoroutine( RunProcessingWWW( uri, byteData, headers, onComplete ) );
    }

    private static IEnumerator RunProcessingWWW( string uri, byte[] postData, Dictionary<string,string> headers, Action<string> onComplete )
    {
        WWW _www = new WWW( uri, postData, headers );
        yield return _www;

        if (onComplete != null)
        {
            onComplete( _www.text );
        }
    }

#endregion

    public static Dictionary<string,string> GetHeaders( bool hasParameters, HeaderContentType contentType = HeaderContentType.NONE )
    {
        Dictionary<string,string> _headers = new Dictionary<string,string>();
        AppendApiKeyToHeaders( _headers );

        if (hasParameters == true)
        {
            string _contentTypeValue = "";

            switch( contentType )
            {
                case HeaderContentType.APPLICATION_WWW_FORM:

                    _contentTypeValue = headerContentTypeApplicationWwwForm;

                    break;

                case HeaderContentType.APPLICATION_JSON:

                    _contentTypeValue = headerContentTypeApplicationJson;

                    break;

                case HeaderContentType.MULTIPART_FORM_DATA:

                    _contentTypeValue = headerContentTypeMultipartFormData;

                    break;
            }

            _headers.Add( "Content-type", _contentTypeValue );
        }

        return _headers;
    }

    public static Dictionary<string,string> AppendApiKeyToHeaders( Dictionary<string,string> headers )
    {
        headers.Add( "X-API-KEY", apiKey );
        return headers;
    }

    public class ApiResponse
    {
        public int status { set; get; }
        public string message { set; get; }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

public class PlayermonImageGenerator : MonoBehaviour
{
    [Header( "Settings" )]
    [SerializeField] private float configRetrievingInterval = 10.0f;
    [SerializeField] private float taskRequestingInterval = 10.0f;
    [SerializeField] private float uploadRetryingInterval = 10.0f;
    [SerializeField] private int imageWidth = 512;
    [SerializeField] private int imageHeight = 512;
    [SerializeField] private bool isUsingTestData = false;
    [SerializeField] private bool isDebugLogEnabled = false;

    [Header( "References" )]
    [SerializeField] private Camera targetCamera = null;
    [SerializeField] private SwappableCreatureV3 swappableCreatureRef = null;

    private readonly string CONFIG_RETRIEVING_API = ServerApiManager.DOMAIN + "playermon/renderingEngine/config";
    private readonly string TASK_REQUESTING_API = ServerApiManager.DOMAIN + "playermon/renderingEngine/task";
    private readonly string IMAGE_UPLOADING_API = ServerApiManager.DOMAIN + "playermon/renderingEngine/uploadImage";

    private int playermonId = 0;
    private byte[] screenshotPNG = null;

    void Awake()
    {
        Application.runInBackground = true;
        Debug.unityLogger.logEnabled = isDebugLogEnabled;
    }

    void Start()
    {
        RetrieveConfiguration();
    }

    private void RetrieveConfiguration()
    {
        Debug.Log( "Retrieve Configuration API = " + CONFIG_RETRIEVING_API );

        if (isUsingTestData == true)
        {
            RequestForTask();
        }
        else
        {
            ServerApiManager.Get( CONFIG_RETRIEVING_API, ServerApiManager.GetHeaders( false ), OnRetrievingConfigurationComplete );
        }
    }

    private void OnRetrievingConfigurationComplete( UnityWebRequest.Result result, string resultText )
    {
        Debug.Log( "Result Text = " + resultText );

        bool _isProcessSuccessful = false;
        if (result == UnityWebRequest.Result.Success)
        {
            ConfigResponse _configResponse = JsonConvert.DeserializeObject<ConfigResponse>( resultText );
            if (_configResponse.status == 1)
            {
                _isProcessSuccessful = true;

                ConfigResponse_Data _configResponseData = _configResponse.data;
                taskRequestingInterval = _configResponseData.task_requesting_interval;
                uploadRetryingInterval = _configResponseData.upload_retrying_interval;

                RequestForTask();
            }
        }

        if (_isProcessSuccessful == false)
        {
            Debug.Log( "Failed" );
            Invoke( "RetrieveConfiguration", configRetrievingInterval );
        }
    }

    private void RequestForTask()
    {
        Debug.Log( "Task Request API = " + TASK_REQUESTING_API );

        if (isUsingTestData == true)
        {
            OnRequestingForTaskComplete( UnityWebRequest.Result.Success, Resources.Load<TextAsset>( "TestData/PlayermonImageGenerator" ).text );
        }
        else
        {
            ServerApiManager.Get( TASK_REQUESTING_API, ServerApiManager.GetHeaders( false ), OnRequestingForTaskComplete );
        }
    }

    private void OnRequestingForTaskComplete( UnityWebRequest.Result result, string resultText )
    {
        Debug.Log( "Result Text = " + resultText );

        bool _isProcessSuccessful = false;
        if (result == UnityWebRequest.Result.Success)
        {
            TaskResponse _taskResponse = JsonConvert.DeserializeObject<TaskResponse>( resultText );
            if (_taskResponse.status == 1)
            {
                _isProcessSuccessful = true;
                screenshotPNG = null;
                ProcessTask( _taskResponse.data );
            }
        }

        if (_isProcessSuccessful == false)
        {
            Debug.Log( "Failed" );
            Invoke( "RequestForTask", taskRequestingInterval );
        }
    }

    private void ProcessTask( TaskResponse_Data responseData )
    {
        playermonId = responseData.id;

        TaskResponse_Data_BodyParts[] _bodyParts = responseData.swappable_body_parts;

        string _headId = "";
        string _eyeId = "";
        string _armId = "";
        string _legId = "";
        string _bodyId = "";
        string _chestObjectId = "";
        string _tailObjectId = "";

        for (int i = 0; i < _bodyParts.Length; i++)
        {
            TaskResponse_Data_BodyParts _bodyPart = _bodyParts[ i ];
            switch ( _bodyPart.category )
            {
                case "head":            _headId = _bodyPart.label;          break;
                case "eye":             _eyeId = _bodyPart.label;           break;
                case "arm":             _armId = _bodyPart.label;           break;
                case "leg":             _legId = _bodyPart.label;           break;
                case "body":            _bodyId = _bodyPart.label;          break;
                case "chest_object":    _chestObjectId = _bodyPart.label;   break;
                case "tail_object":     _tailObjectId = _bodyPart.label;    break;
            }
        }

        swappableCreatureRef.SetSwappableBodyParts( _headId, _eyeId, _armId, _legId, _bodyId, _chestObjectId, _tailObjectId, "A" );

        Invoke( "UploadImage", 0.05f );
    }

    private void UploadImage()
    {
        Debug.Log( "Image Uploading API = " + ( IMAGE_UPLOADING_API + "/" + playermonId.ToString() ) );

        if (isUsingTestData == true)
        {
            OnImageUploadingComplete( "{\"status\":1}" );
        }
        else
        {
            if (screenshotPNG == null)
            {
                screenshotPNG = GetScreenshotPNG( targetCamera, imageWidth, imageHeight );
            }

            WWWForm _wwwForm = new WWWForm();
            _wwwForm.AddBinaryData( "file", screenshotPNG, "screenshot.png", "image/png" );

            Dictionary<string,string> _headers = _wwwForm.headers;
            ServerApiManager.AppendApiKeyToHeaders( _headers );

            ServerApiManager.PutUsingWWW( IMAGE_UPLOADING_API + "/" + playermonId, _headers, _wwwForm.data, OnImageUploadingComplete );
        }
    }

    private void OnImageUploadingComplete( string resultText )
    {
        Debug.Log( "Result Text = " + resultText );

        ServerApiManager.ApiResponse _response = JsonConvert.DeserializeObject<ServerApiManager.ApiResponse>( resultText );

        bool _isProcessSuccessful = false;
        if (_response != null)
        {
            if (_response.status == 1)
            {
                _isProcessSuccessful = true;
                screenshotPNG = null;
                Invoke( "RequestForTask", taskRequestingInterval );
            }
        }

        if (_isProcessSuccessful == false)
        {
            Debug.Log( "Failed" );
            Invoke( "UploadImage", uploadRetryingInterval );
        }
    }

    private byte[] GetScreenshotPNG( Camera cam, int width, int height )
    {
        // Depending on your render pipeline, this may not work.
        var bak_cam_targetTexture = cam.targetTexture;
        var bak_cam_clearFlags = cam.clearFlags;
        var bak_RenderTexture_active = RenderTexture.active;

        var tex_transparent = new Texture2D( width, height, TextureFormat.ARGB32, false );
        // Must use 24-bit depth buffer to be able to fill background.
        var render_texture = RenderTexture.GetTemporary( width, height, 24, RenderTextureFormat.ARGB32 );
        var grab_area = new Rect( 0, 0, width, height );

        RenderTexture.active = render_texture;
        cam.targetTexture = render_texture;
        cam.clearFlags = CameraClearFlags.SolidColor;

        // Simple: use a clear background
        cam.backgroundColor = Color.clear;
        cam.Render();
        tex_transparent.ReadPixels( grab_area, 0, 0 );
        tex_transparent.Apply();

        // Encode the resulting output texture to a byte array then write to the file
        byte[] _pngShot = ImageConversion.EncodeToPNG( tex_transparent );

        cam.clearFlags = bak_cam_clearFlags;
        cam.targetTexture = bak_cam_targetTexture;
        RenderTexture.active = bak_RenderTexture_active;
        RenderTexture.ReleaseTemporary( render_texture );
        Texture2D.Destroy( tex_transparent );

        return _pngShot;
    }

    public class ConfigResponse : ServerApiManager.ApiResponse
    {
        public ConfigResponse_Data data { set; get; }
    }

    public class ConfigResponse_Data
    {
        public float task_requesting_interval { set; get; }
        public float upload_retrying_interval { set; get; }
    }

    public class TaskResponse : ServerApiManager.ApiResponse
    {
        public TaskResponse_Data data { set; get; }
    }

    public class TaskResponse_Data
    {
        public int id { set; get; }
        public TaskResponse_Data_BodyParts[] swappable_body_parts { set; get; }
        public string image { set; get; }
    }

    public class TaskResponse_Data_BodyParts
    {
        public string category { set; get; }
        public string label { set; get; }
    }
}

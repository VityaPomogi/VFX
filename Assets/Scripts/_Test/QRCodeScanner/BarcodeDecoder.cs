using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using ZXing;

public class BarcodeDecoder : MonoBehaviour
{
    [Header( "Settings" )]
    [SerializeField] private float scanningInterval = 1.0f;
    [SerializeField] private float statusCheckingDelay = 2.0f;
    [SerializeField] private int _requestedWidth = 640;
    [SerializeField] private int _requestedHeight = 480;

    [Header( "UI" )]
    [SerializeField] private RawImage _displayImage = null;
    [SerializeField] private Text _progressText = null;
    [SerializeField] private Text _resultText = null;

    private BarcodeReader _barcodeReader = null;
    private WebCamTexture _camTexture = null;

    private Color32[] _rawRGB = null;
    private int _width = 0;
    private int _height = 0;

    private int _dotCount = 0;
    private int _scanCount = 0;

    public Action<string> onResultCallback = null;
    public Action onResetRequired = null;

    private void Awake()
    {
        _barcodeReader = new BarcodeReader();
        _barcodeReader.AutoRotate = true;
        _barcodeReader.Options.TryHarder = true;

        _camTexture = new WebCamTexture( _requestedWidth, _requestedHeight );

        _displayImage.rectTransform.sizeDelta = new Vector2( _requestedWidth, _requestedHeight );
        _displayImage.texture = _camTexture;
        _displayImage.material.mainTexture = _camTexture;

#if UNITY_ANDROID && !UNITY_EDITOR

        _displayImage.transform.Rotate( Vector3.back, 90.0f );

#endif
    }

    private void Start()
    {
        _camTexture.Play();
        _width = _camTexture.width;
        _height = _camTexture.height;
        _rawRGB = _camTexture.GetPixels32();

        StartCoroutine( "Decode" );
        Invoke( "CheckScanningStatus", statusCheckingDelay );
    }

    private void CheckScanningStatus()
    {
        Color32[] _pixelData = null;
        bool _hasError = false;
        try
        {
            _pixelData = _camTexture.GetPixels32();
        }
        catch
        {
            _hasError = true;
        }

        if (_scanCount < 2 || _hasError == true || _pixelData == null || _pixelData.Length < 300)
        {
            StopCoroutine( "Decode" );

            if (onResetRequired != null)
            {
                onResetRequired();
            }
        }
    }

    private void Update()
    {
        if (_rawRGB == null && _camTexture.isPlaying == true)
        {
            _rawRGB = _camTexture.GetPixels32();
        }
    }

    private void OnApplicationQuit()
    {
        _camTexture.Stop();
    }

    private IEnumerator Decode()
    {
        while ( true )
        {
            var result = _barcodeReader.Decode( _rawRGB, _width, _height );

            if (result != null)
            {
                if (_resultText != null)
                {
                    _resultText.text = result.Text;
                }

                if (onResultCallback != null)
                {
                    onResultCallback( result.Text );
                }
            }
            else
            {
                if (_resultText != null)
                {
                    _resultText.text = "Failed to decode";
                }
            }

            _rawRGB = null;

            if (_progressText != null)
            {
                _dotCount++;
                if (_dotCount > 3)
                {
                    _dotCount = 0;
                }

                _progressText.text = "Scanning";

                for (int i = 0; i < _dotCount; i++)
                {
                    _progressText.text += ".";
                }
            }

            _scanCount++;
            yield return new WaitForSeconds( scanningInterval );
        }
    }
}

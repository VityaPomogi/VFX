using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using ZXing;
using Kakera;
using SFB;


public class FileBrowserSystem : MonoBehaviour
{
    [Header("Unimgpicker Controller")]
    [SerializeField] private Unimgpicker _imagePicker = null;

    [Header("UI")]
    [SerializeField] private RawImage _rawImage = null;
    [SerializeField] private Text _resultText = null;

    private string _path = string.Empty;

    public System.Action<bool,string> onResultCallback = null;

    public void Awake()
    {
        _imagePicker.Completed += (string path) =>
        {
            StartCoroutine(LoadImage(path));
        };
    }

    public void WriteResult(string[] paths)
    {
        if (paths.Length == 0)
        {
            return;
        }

        _path = string.Empty;
        foreach (var p in paths)
        {
            _path += p + "\n";
        }
    }

    private IEnumerator LoadImage(string path)
    {
        string url = "file://" + path;
        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url))
        {
            yield return request.SendWebRequest();

#if UNITY_2020_2_OR_NEWER
            if (request.result != UnityWebRequest.Result.Success)
#else
            if (request.isNetworkError || request.isHttpError)
#endif
            {
                Debug.Log(request.error);

                if (onResultCallback != null)
                {
                    onResultCallback( false, request.error );
                }
            }
            else
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(request);

                if (_rawImage != null)
                {
                    _rawImage.texture = texture;
                }

                //Debug.Log(texture.isReadable);

                //Debug.Log(texture.mipmapCount);

                //if (texture.Resize(1080, 2340))
                //{
                //    texture.Apply();
                //}

                DecodeImage(texture.GetPixels32(), texture.width, texture.height);
            }
        }
    }

    private void DecodeImage(Color32[] rawRGB, int width, int height)
    {
        var barcodeReader = new BarcodeReader { AutoRotate = true };
        barcodeReader.Options.TryHarder = true;

        Result result = barcodeReader.Decode(rawRGB, width, height);
        if (result != null)
        {
            if (_resultText != null)
            {
                _resultText.text = result.Text;
            }

            if (onResultCallback != null)
            {
                onResultCallback( true, result.Text );
            }
        }
        else
        {
            if (_resultText != null)
            {
                _resultText.text = "Failed to decode";
            }

            if (onResultCallback != null)
            {
                onResultCallback( false, "Failed to decode" );
            }
        }
    }

    public void OpenFileBrowser()
    {
        var extensions = new[] {
            new ExtensionFilter("Image Files", "png", "jpg", "jpeg" ),
        };

#if UNITY_EDITOR
        StandaloneFileBrowser.OpenFilePanelAsync("Select Image", "", extensions, false, (string[] paths) => { WriteResult(paths); StartCoroutine(LoadImage(_path)); });
#elif UNITY_STANDALONE
        StandaloneFileBrowser.OpenFilePanelAsync("Select Image", "", extensions, false, (string[] paths) => { WriteResult(paths); StartCoroutine(LoadImage(_path)); });
#elif UNITY_IOS
        _imagePicker.Show("Select Image", "unimgpicker");
#elif UNITY_ANDROID
        _imagePicker.Show("Select Image", "unimgpicker");
#endif
    }
}

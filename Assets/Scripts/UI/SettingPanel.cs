using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingPanel : MonoBehaviour
{
    [System.Serializable]
    public enum GraphicQuality
    {
        Low,
        Medium,
        High
    }

    [System.Serializable]
    public class Resolution
    {
        [SerializeField] private int _width = 0;
        [SerializeField] private int _height = 0;

        public int Width => _width;
        public int Height => _height;
        public string Text => string.Format("{0} x {1}", _width, _height);
    }

    [Header("Setting Panel")]
    [SerializeField] private GameObject _settingPanel = null;
    [SerializeField] private GameObject container = null;
    [SerializeField] private GameObject blocker = null;

    [Header("Background Music")]
    [SerializeField] private TextMeshProUGUI _backgroundMusicText = null;
    [SerializeField] private Slider _backgroundMusicSlider = null;

    [Header("Background Music")]
    [SerializeField] private TextMeshProUGUI _soundEffectText = null;
    [SerializeField] private Slider _soundEffectSlider = null;

    [Header("Graphic Quality")]
    [SerializeField] private Toggle _lowToggle = null;
    [SerializeField] private Toggle _mediumToggle = null;
    [SerializeField] private Toggle _highToggle = null;

    [Header("Resolution")]
    [SerializeField] private TMP_Dropdown _resolutionDropdown = null;
    [SerializeField] private Resolution[] _resolutionSelection = null;

    [Header( "References" )]
    [SerializeField] private CameraFollowScreenSize[] cameraFollowScreenSizeComponents;
    [SerializeField] private CanvasScalerAdjuster[] canvasScalerAdjusterComponents;

    //public string SelectedResolution = string.Empty;

    public static bool HasSettingSetUp { get; private set; } = false;
    public static float BackgroundMusicVolume { get; private set; } = 100.0f;
    public static float SoundEffectVolume { get; private set; } = 100.0f;
    public static GraphicQuality CurrentGraphicQuality { get; private set; } = GraphicQuality.Medium;
    public static int CurrentResolutionIndex { get; private set; } = 0;

    private float _backgroundMusicVolume = 100.0f;
    private float _soundEffectVolume = 100.0f;
    private GraphicQuality _graphicQuality = GraphicQuality.Medium;
    private int _resolutionIndex = 0;

    public Action onScreenResolutionChanged = null;
    public Action onExitSpaceDenButtonClicked = null;

    void Awake()
    {
        if (HasSettingSetUp == false)
        {
            BackgroundMusicVolume = 100.0f;
            SoundEffectVolume = 100.0f;

            if (Application.isMobilePlatform == true)
            {
                CurrentGraphicQuality = GraphicQuality.Medium;
            }
            else
            {
                CurrentGraphicQuality = GraphicQuality.High;
            }

            CurrentResolutionIndex = 0;
            HasSettingSetUp = true;
        }

        _backgroundMusicVolume = BackgroundMusicVolume;
        _soundEffectVolume = SoundEffectVolume;
        _graphicQuality = CurrentGraphicQuality;
        _resolutionIndex = CurrentResolutionIndex;

        _resolutionDropdown.options.Clear();
    }

    public void Initialize()
    {
        _backgroundMusicVolume = BackgroundMusicVolume;
        _soundEffectVolume = SoundEffectVolume;
        _graphicQuality = CurrentGraphicQuality;
        _resolutionIndex = CurrentResolutionIndex;

        _resolutionDropdown.options.Clear();

        _backgroundMusicSlider.value = BackgroundMusicVolume;
        _soundEffectSlider.value = SoundEffectVolume;

        switch (CurrentGraphicQuality)
        {
            case GraphicQuality.Low:
                _lowToggle.isOn = true;
                break;

            case GraphicQuality.Medium:
                _mediumToggle.isOn = true;
                break;

            case GraphicQuality.High:
                _highToggle.isOn = true;
                break;
        }

        List<string> options = new List<string>();

        foreach (Resolution resolution in _resolutionSelection)
        {
            options.Add(resolution.Text);
        }

        _resolutionDropdown.AddOptions(options);
        _resolutionDropdown.value = CurrentResolutionIndex;
    }

    public void OpenSettingPanel()
    {
        blocker.SetActive( true );
        _settingPanel.SetActive(true);
        container.transform.localScale = Vector3.zero;
        container.SetActive( true );
        LeanTween.scale( container, Vector3.one, 0.3f ).setEaseOutBack().setOnComplete( HideBlocker );
        Initialize();
    }

    private void HideBlocker()
    {
        blocker.SetActive( false );
    }

    public void ClickToCloseSettingPanel()
    {
        SoundManager.Instance.PlayNegativeClickingClip();
        CloseSettingPanel();
    }

    private void CloseSettingPanel()
    {
        blocker.SetActive( true );
        LeanTween.scale( container, Vector3.zero, 0.3f ).setEaseInBack().setOnComplete( HideSettingPanel );
    }

    private void HideSettingPanel()
    {
        container.SetActive( false );
        _settingPanel.SetActive( false );
    }

    public void UpdateBackgroundMusicVolume( float value )
    {
        _backgroundMusicVolume = value;
        _backgroundMusicText.text = _backgroundMusicVolume.ToString();
        SoundManager.Instance.SetBackgroundMusicAudioSourceVolume( value * 0.01f );
    }

    public void UpdateSoundEffectVolume( float value )
    {
        _soundEffectVolume = value;
        _soundEffectText.text = _soundEffectVolume.ToString();
        SoundManager.Instance.SetSoundEffectAudioSourceVolume( value * 0.01f );
    }

    public void UpdateGraphicQuality()
    {
        SoundManager.Instance.PlayPositiveClickingClip();

        if (_lowToggle.isOn && _graphicQuality != GraphicQuality.Low)
        {
            _graphicQuality = GraphicQuality.Low;
        }
        else if (_mediumToggle.isOn && _graphicQuality != GraphicQuality.Medium)
        {
            _graphicQuality = GraphicQuality.Medium;
        }
        else if (_highToggle.isOn && _graphicQuality != GraphicQuality.High)
        {
            _graphicQuality = GraphicQuality.High;
        }

        switch ( _graphicQuality )
        {
            case GraphicQuality.Low:

                if (Application.isMobilePlatform == true)
                {
                    QualitySettings.SetQualityLevel( 0 );
                }
                else
                {
                    QualitySettings.SetQualityLevel( 1 );
                }

                break;

            case GraphicQuality.Medium:

                if (Application.isMobilePlatform == true)
                {
                    QualitySettings.SetQualityLevel( 2 );
                }
                else
                {
                    QualitySettings.SetQualityLevel( 3 );
                }

                break;

            case GraphicQuality.High:

                if (Application.isMobilePlatform == true)
                {
                    QualitySettings.SetQualityLevel( 4 );
                }
                else
                {
                    QualitySettings.SetQualityLevel( 5 );
                }

                break;
        }
    }

    public void UpdateResolution(int value)
    {
        SoundManager.Instance.PlayPositiveClickingClip();

        if (_resolutionIndex != value)
        {
            _resolutionIndex = value;

            Resolution _resolution = _resolutionSelection[ value ];
            Screen.SetResolution( _resolution.Width, _resolution.Height, Screen.fullScreen );

            for (int i = 0; i < cameraFollowScreenSizeComponents.Length; i++)
            {
                cameraFollowScreenSizeComponents[ i ].UpdateSettings();
            }

            for (int i = 0; i < canvasScalerAdjusterComponents.Length; i++)
            {
                canvasScalerAdjusterComponents[ i ].UpdateSettings();
            }

            if (onScreenResolutionChanged != null)
            {
                onScreenResolutionChanged();
            }
        }
    }

    public void SaveChanges()
    {
        SoundManager.Instance.PlayPositiveClickingClip();

        BackgroundMusicVolume = _backgroundMusicVolume;
        SoundEffectVolume = _soundEffectVolume;
        CurrentGraphicQuality = _graphicQuality;
        CurrentResolutionIndex = _resolutionIndex;

        CloseSettingPanel();
    }

    public void ExitSpaceDen()
    {
        SoundManager.Instance.PlayNegativeClickingClip();

        if (onExitSpaceDenButtonClicked != null)
        {
            onExitSpaceDenButtonClicked();
        }
    }
}

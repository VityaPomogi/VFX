using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameLoadingManager : MonoBehaviour
{
    [Header( "Settings" )]
    [SerializeField] private string[] sceneNames;
    [SerializeField] private float dotAnimationInterval = 0.3f;
    [SerializeField] private bool shouldPlaySpaceshipAnimation = true;
    [SerializeField] private float spaceshipTargetPosY = 100.0f;
    [SerializeField] private float spaceshipMoveDuration = 1.0f;

    [Header( "References" )]
    [SerializeField] private Text titleLabel;
    [SerializeField] private RectTransform titleLabelRectTransform;
    [SerializeField] private Text progressLabel;
    [SerializeField] private CustomFillBar progressBar;
    [SerializeField] private GameObject spaceshipObject;
    [SerializeField] private Transform spaceshipTargetPoint;
    [SerializeField] private GameObject progressBarObject;
    [SerializeField] private Transform progressBarTargetPoint;

    private int dotCount = 1;
    private float lastUpdateTime = 0.0f;

    void Start()
    {
        lastUpdateTime = Time.realtimeSinceStartup;
        StartCoroutine( RunLoadingScenes() );
    }

    private IEnumerator RunLoadingScenes()
    {
        LeanTween.move( spaceshipObject, spaceshipTargetPoint.position, 0.3f ).setEaseOutCirc();
        LeanTween.move( progressBarObject, progressBarTargetPoint.position, 0.3f ).setEaseOutCirc();
        yield return new WaitForSeconds( 0.3f );

        if (shouldPlaySpaceshipAnimation == true)
        {
            LeanTween.moveLocalY( spaceshipObject, spaceshipTargetPosY, spaceshipMoveDuration ).setLoopPingPong().setIgnoreTimeScale( true );
        }

        AsyncOperation _asyncOperation = null;
        float _averageProgressRate = 1.0f / sceneNames.Length;
        float _baseProgressRate = 0.0f;
        float _currentProgressRate = 0.0f;

        for (int i = 0; i < sceneNames.Length; i++)
        {
            _asyncOperation = SceneManager.LoadSceneAsync( sceneNames[ i ], LoadSceneMode.Additive );
            _asyncOperation.allowSceneActivation = false;

            _baseProgressRate = i * _averageProgressRate;
            while (_asyncOperation.progress < 0.9f)
            {
                yield return null;
                _currentProgressRate = _baseProgressRate + ( ( _asyncOperation.progress / 0.9f ) * _averageProgressRate );
                progressLabel.text = Mathf.FloorToInt( _currentProgressRate * 100 ) + "%";
                progressBar.UpdateFillAmount( _currentProgressRate );
                UpdateTitleLabel();
            }
        }

        Vector2 _titleLabelPos = titleLabelRectTransform.anchoredPosition;
        _titleLabelPos.x = 0.0f;
        titleLabelRectTransform.anchoredPosition = _titleLabelPos;

        titleLabel.alignment = TextAnchor.MiddleCenter;
        titleLabel.text = "Done!";
        SceneControlManager.GoToMainMenuScene();
    }

    private void UpdateTitleLabel()
    {
        if (Time.realtimeSinceStartup - lastUpdateTime >= dotAnimationInterval)
        {
            string _titleText = "Loading Game Assets";
            for (int i = 0; i < dotCount; i++)
            {
                _titleText += " .";
            }

            titleLabel.text = _titleText;
            dotCount = ( dotCount + 1 ) % 4;
            lastUpdateTime = Time.time;
        }
    }
}

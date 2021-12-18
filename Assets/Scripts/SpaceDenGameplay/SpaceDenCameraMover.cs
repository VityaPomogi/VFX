using UnityEngine;

public class SpaceDenCameraMover : MonoBehaviour
{
    [Header( "Settings" )]
    [SerializeField] private float maximumWidth = 27.0f;
    [SerializeField] private float movementSpeed = 1.0f;
    [SerializeField] private float movingTriggerSize = 0.1f;

    [Header( "References" )]
    [SerializeField] private SettingPanel settingPanelRef;

    private Transform thisTransform;
    private float minX = 0.0f;
    private float maxX = 0.0f;
    private float totalDistance = 0.0f;
    private float movingTriggerLeftX = 0.0f;
    private float movingTriggerRightX = 0.0f;

    private bool isMovingLeft = false;
    private bool isMovingRight = false;

    void Awake()
    {
        settingPanelRef.onScreenResolutionChanged = UpdateSettings;
    }

    void Start()
    {
        thisTransform = this.transform;
        UpdateSettings();
    }

    public void UpdateSettings()
    {
        Camera _camera = Camera.main;
        float _halfHeight = _camera.orthographicSize;
        float _halfWidth = _camera.aspect * _halfHeight;
        float _fullWidth = _halfWidth * 2.0f;
        float _distance = ( maximumWidth - _fullWidth ) * 0.5f;

        minX = -_distance;
        maxX = _distance;
        totalDistance = maxX - minX;

        movingTriggerLeftX = Screen.width * movingTriggerSize;
        movingTriggerRightX = Screen.width * ( 1.0f - movingTriggerSize );

        if (thisTransform == null)
        {
            thisTransform = this.transform;
        }

        Vector3 _pos = thisTransform.position;
        _pos.x = 0.0f;
        thisTransform.position = _pos;
    }

    void Update()
    {
        Vector3 _pos = thisTransform.position;
        if (_pos.x > minX && ( Input.mousePosition.x < movingTriggerLeftX || Input.GetKey( KeyCode.LeftArrow ) == true ) || isMovingLeft == true)
        {
            _pos.x -= movementSpeed * Time.deltaTime;
        }
        else if (_pos.x < maxX && ( Input.mousePosition.x > movingTriggerRightX || Input.GetKey( KeyCode.RightArrow ) == true ) || isMovingRight == true)
        {
            _pos.x += movementSpeed * Time.deltaTime;
        }

        _pos.x = Mathf.Clamp( _pos.x, minX, maxX );
        thisTransform.position = _pos;
    }

    public void MoveToLeft()
    {
        isMovingLeft = true;
        isMovingRight = false;
    }

    public void MoveToRight()
    {
        isMovingLeft = false;
        isMovingRight = true;
    }

    public void StopMoving()
    {
        isMovingLeft = false;
        isMovingRight = false;
    }

    public float GetPositionRate()
    {
        return ( thisTransform.position.x / totalDistance );
    }
}

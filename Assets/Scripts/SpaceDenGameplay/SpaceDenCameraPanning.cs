using UnityEngine;
using UnityEngine.UI;

public class SpaceDenCameraPanning : MonoBehaviour
{
    [Header( "Settings" )]
    [SerializeField] private float minimumDistanceToMove = 10.0f;
    [SerializeField] private float buttonMinX = -55.0f;
    [SerializeField] private float buttonMaxX = 55.0f;
    [SerializeField] private Image cameraPanningButtonImage = null;
    [SerializeField] private Sprite buttonNormalSprite = null;
    [SerializeField] private Sprite buttonTriggeredSprite = null;

    [Header( "References" )]
    [SerializeField] private SpaceDenCameraMover cameraMoverRef;
    [SerializeField] private RectTransform cameraPanningButton;

    private float totalDistance = 0.0f;
    private bool isPanningCamera = false;
    private float mousePosX = 0.0f;
    private bool isMovingLeft = false;
    private bool isMovingRight = false;

    void Awake()
    {
        totalDistance = buttonMaxX - buttonMinX;
    }

    void Update()
    {
        if (isPanningCamera == true)
        {
            if (( isMovingLeft == false || Input.mousePosition.x >= mousePosX + minimumDistanceToMove )
                && Input.mousePosition.x >= cameraPanningButton.position.x + minimumDistanceToMove)
            {
                isMovingLeft = false;
                isMovingRight = true;
                cameraMoverRef.MoveToRight();
            }
            else if (( isMovingRight == false || Input.mousePosition.x <= mousePosX - minimumDistanceToMove )
                && Input.mousePosition.x <= cameraPanningButton.position.x - minimumDistanceToMove)
            {
                isMovingLeft = true;
                isMovingRight = false;
                cameraMoverRef.MoveToLeft();
            }
            else
            {
                isMovingLeft = false;
                isMovingRight = false;
                cameraMoverRef.StopMoving();
            }

            mousePosX = Input.mousePosition.x;
        }
        else
        {
            isMovingLeft = false;
            isMovingRight = false;
            cameraMoverRef.StopMoving();
        }

        cameraPanningButton.anchoredPosition = new Vector2( cameraMoverRef.GetPositionRate() * totalDistance, 0.0f );
    }

    public void StartPanningCamera()
    {
        SoundManager.Instance.PlayPositiveClickingClip();
        isPanningCamera = true;
        cameraPanningButtonImage.sprite = buttonTriggeredSprite;
    }

    public void StopPanningCamera()
    {
        SoundManager.Instance.PlayNegativeClickingClip();
        isPanningCamera = false;
        cameraPanningButtonImage.sprite = buttonNormalSprite;
    }
}

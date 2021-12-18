using UnityEngine;

public class CursorFollower : MonoBehaviour
{
    private RectTransform thisRectTransform = null;
    private float positionOffsetX = 0.0f;
    private float positionOffsetY = 0.0f;

    void Awake()
    {
        thisRectTransform = this.GetComponent<RectTransform>();
        positionOffsetX = Screen.width * 0.5f;
        positionOffsetY = Screen.height * 0.5f;
    }

    void Update()
    {
        thisRectTransform.anchoredPosition = new Vector2( Input.mousePosition.x - positionOffsetX, Input.mousePosition.y - positionOffsetY );
    }
}

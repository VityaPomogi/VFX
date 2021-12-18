using UnityEngine;
using UnityEngine.Events;

public class KeyDownTrigger : MonoBehaviour
{
    [SerializeField] private KeyCode screenshotKey = KeyCode.A;
    [SerializeField] private UnityEvent onKeyDown = null;

    void Update()
    {
        if (Input.GetKeyDown( screenshotKey ) == true)
        {
            if (onKeyDown != null)
            {
                onKeyDown.Invoke();
            }
        }
    }
}

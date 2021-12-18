using UnityEngine;

public class AutoRotate : MonoBehaviour
{
    [SerializeField] private float rotatingSpeed = 360.0f;

    private Transform thisTransform = null;

    void Awake()
    {
        thisTransform = this.transform;
    }

    void Update()
    {
        Vector3 _eulerAngles = thisTransform.localEulerAngles;
        _eulerAngles.z += rotatingSpeed * Time.deltaTime;
        thisTransform.localEulerAngles = _eulerAngles;
    }
}

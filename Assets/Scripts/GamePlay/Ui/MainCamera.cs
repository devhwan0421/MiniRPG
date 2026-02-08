using UnityEngine;

public class MainCamera : MonoBehaviour
{
    public Transform _target;
    public float _smoothTime = 0.15f;
    public Vector3 _offset = new Vector3(0, 0, -10);

    public float _minX;
    public float _maxX;
    public float _minY;
    public float _maxY;

    private Vector3 _currentVelocity = Vector3.zero;

    private void LateUpdate()
    {
        if (_target == null) return;

        Vector3 targetPosition = _target.position + _offset;

        float clampedX = Mathf.Clamp(targetPosition.x, _minX, _maxX);
        float clampedY = Mathf.Clamp(targetPosition.y, _minY, _maxY);

        Vector3 finalPosition = new Vector3(clampedX, clampedY, targetPosition.z);

        transform.position = Vector3.SmoothDamp(transform.position, finalPosition, ref _currentVelocity, _smoothTime);
    }
}
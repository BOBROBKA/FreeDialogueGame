using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float zoomSpeed = 5f;
    

    private Transform _target;
    private Camera _camera;

    private float _targetZoom;

    private void Awake()
    {
        _camera = GetComponent<Camera>();
        _targetZoom = _camera.orthographicSize;
    }

    public void SetTarget(Transform target, float zoom)
    {
        _target = target;
        _targetZoom = zoom;
    }

    private void Update()
    {
        if (_target == null) return;

        transform.position = Vector3.Lerp(
            transform.position,
            new Vector3(
                _target.position.x,
                _target.position.y,
                transform.position.z
            ),
            moveSpeed * Time.deltaTime
        );

        _camera.orthographicSize = Mathf.Lerp(
            _camera.orthographicSize,
            _targetZoom,
            zoomSpeed * Time.deltaTime
        );
    }
}
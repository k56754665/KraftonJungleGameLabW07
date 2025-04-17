using Unity.Cinemachine;
using UnityEngine;

public class CameraZoomController : MonoBehaviour
{
    [SerializeField] float _zoomInLensSize = 10f;
    [SerializeField] float _zoomOutLensSize = 14f;
    [SerializeField] float _zoomSpeed = 5f;

    CinemachineCamera _camera;

    void Start()
    {
        _camera = GetComponent<CinemachineCamera>();
        _camera.Lens.OrthographicSize = _zoomInLensSize;
    }

    private void Update()
    {
        if (InputManager.Instance.IsFocusing)
            _camera.Lens.OrthographicSize = Mathf.Lerp(_camera.Lens.OrthographicSize, _zoomOutLensSize, Time.deltaTime * _zoomSpeed);
        else
            _camera.Lens.OrthographicSize = Mathf.Lerp(_camera.Lens.OrthographicSize, _zoomInLensSize, Time.deltaTime * _zoomSpeed);
    }
}

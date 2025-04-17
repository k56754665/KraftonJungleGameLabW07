using UnityEngine;

public class AimLineController : MonoBehaviour
{
    LineRenderer _line;
    PlayerController _playerController;
    Transform _target;

    [SerializeField] LayerMask _fieldOfViewLayer;
    [SerializeField] float _followSpeed = 10f; // 목표 위치로 얼마나 빨리 따라갈지

    Vector2 _targetPosition; // 목표 위치 캐싱

    void Start()
    {
        _target = transform.GetChild(0);
        _line = GetComponent<LineRenderer>();
        _playerController = FindAnyObjectByType<PlayerController>();
        _target.GetComponent<SpriteRenderer>().enabled = true;
        _line.enabled = true;

        _targetPosition = _target.position;
    }

    void LateUpdate()
    {
        if (_playerController == null)
        {
            _line.enabled = false;
            _target.GetComponent<SpriteRenderer>().enabled = false;
            return;
        }

        Vector2 startPosition = _playerController.transform.position;
        Vector2 pointerPosition = InputManager.Instance.PointerMoveInput;
        Vector2 direction = (pointerPosition - startPosition).normalized;
        float distance = Vector2.Distance(startPosition, pointerPosition);

        RaycastHit2D hit = Physics2D.Raycast(startPosition, direction, distance, _fieldOfViewLayer);

        Vector2 endPosition = pointerPosition;

        if (hit.collider != null)
        {
            endPosition = hit.point;
        }

        // 목표 위치를 업데이트하고, Lerp로 부드럽게 이동
        _targetPosition = Vector2.Lerp(_targetPosition, endPosition, Time.deltaTime * _followSpeed);

        _line.SetPosition(0, startPosition);
        _line.SetPosition(1, _targetPosition);
        _target.transform.position = _targetPosition;
    }
}

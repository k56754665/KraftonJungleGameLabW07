using UnityEngine;

public class AimLineController : MonoBehaviour
{
    LineRenderer _line;
    PlayerController _playerController;
    Transform _target;

    void Start()
    {
        _target = transform.GetChild(0);
        _line = GetComponent<LineRenderer>();
        _playerController = FindAnyObjectByType<PlayerController>();
        _target.GetComponent<SpriteRenderer>().enabled = true;
        _line.enabled = true;
    }

    void Update()
    {
        Vector2 startPosition = _playerController.transform.position;
        Vector2 endPosition = InputManager.Instance.PointerMoveInput;
        _line.SetPosition(0, startPosition);
        _line.SetPosition(1, endPosition);
        _target.transform.position = endPosition;
    }
}

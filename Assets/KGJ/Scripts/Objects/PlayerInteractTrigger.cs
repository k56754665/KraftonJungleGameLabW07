using Define;
using UnityEngine;

public class PlayerInteractTrigger : MonoBehaviour
{
    PlayerController _playerController;

    void Start()
    {
        _playerController = transform.parent.GetComponent<PlayerController>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Object"))
        {
            // Object에 Can 스크립트가 있다면
            Can can = collision.GetComponent<Can>();
            if (can != null)
            {
                if (!can.IsThrowing)
                {
                    can.GetComponentInChildren<EnemyUIController_Script>().ShowUI();
                    _playerController.CurrentTarget = can.gameObject;
                    _playerController.TargetType = Target.Object;
                }
            }

        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Object"))
        {
            Can can = collision.GetComponent<Can>();
            if (can != null)
            {
                can.GetComponentInChildren<EnemyUIController_Script>().HideUI();
            }
        }
    }
}

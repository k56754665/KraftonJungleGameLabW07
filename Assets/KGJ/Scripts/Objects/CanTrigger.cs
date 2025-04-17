using UnityEngine;

public class CanTrigger : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerController>().CurrentTarget = gameObject;
            collision.gameObject.GetComponent<PlayerInteraction>().ShowEKeyUI(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerController>().CurrentTarget = null;
            collision.gameObject.GetComponent<PlayerInteraction>().ShowEKeyUI(false);
        }
    }
}

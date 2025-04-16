using UnityEngine;

public class Water : MonoBehaviour
{
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("dddddd");
            collision.GetComponent<Character>().movespeed /= 2;
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("dddddd");
            collision.GetComponent<Character>().movespeed *= 2;
        }
    }
}

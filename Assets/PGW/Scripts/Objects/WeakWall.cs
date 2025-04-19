using UnityEngine;

public class WeakWall : MonoBehaviour
{
    [SerializeField] int _hp = 1;


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bullet"))
        {
            _hp--;
            if (_hp <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
}

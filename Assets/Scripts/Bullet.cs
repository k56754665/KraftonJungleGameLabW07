using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 100f;
    public Vector3 direction;
    public string from;

    public enum BulletColor
    {
        Yellow,
        Blue,
        Red
    }

    public BulletColor bulletColor;

    GameObject soundwave;
    Rigidbody2D rb;

    private void Start()
    {
        soundwave = Resources.Load<GameObject>("Prefabs/Soundwaves/SoundwaveWalk");
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        rb.linearVelocity = transform.up * speed;
    }


    private void OnTriggerEnter2D(Collider2D _collision)
    {
        if (_collision.gameObject.CompareTag("Field Of View Object"))
        {
            Instantiate(soundwave, transform.position, Quaternion.identity);
            //Debug.Log("Hit Wall");
            Destroy(gameObject);
        }
    }

    public void SetDirection(Vector3 _dir)
    {
        direction = _dir.normalized;
    }
}
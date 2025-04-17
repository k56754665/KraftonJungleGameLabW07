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






    void Update()
    {
        transform.position += -transform.up * speed * Time.deltaTime;
    }


    private void OnTriggerEnter2D(Collider2D _collision)
    {
        if (_collision.gameObject.CompareTag("Field Of View Object"))
        {
            //Debug.Log("Hit Wall");
            Destroy(gameObject);
        }
    }

    public void SetDirection(Vector3 _dir)
    {
        direction = _dir.normalized;
    }
}
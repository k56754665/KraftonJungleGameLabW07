using UnityEngine;

public class Can : MonoBehaviour
{
    PlayerFire _playerFire;
    GameObject _soundWavePrefab;

    void Start()
    {
        _playerFire = FindFirstObjectByType<PlayerFire>();
        _soundWavePrefab = Resources.Load<GameObject>("Prefabs/Soundwaves/SoundwaveRun");
    }

    public void Throw()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.AddForce(transform.up * 10f, ForceMode2D.Impulse);
        rb.AddTorque(100f, ForceMode2D.Impulse);
    }

    public void Activate()
    {
        _playerFire.SetCurrentCan();
        Destroy(gameObject);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Instantiate(_soundWavePrefab, transform.position, Quaternion.identity);
    }
}

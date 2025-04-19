using System.Collections;
using UnityEngine;

public class Can : MonoBehaviour
{
    public bool IsThrowing
    {
        get { return _isThrowing; }
        set { _isThrowing = value; }
    }
    bool _isThrowing = false;

    PlayerFire _playerFire;
    GameObject _soundWavePrefab;

    Coroutine _throwingCoroutine;

    void Start()
    {
        _playerFire = FindFirstObjectByType<PlayerFire>();
        _soundWavePrefab = Resources.Load<GameObject>("Prefabs/Soundwaves/SoundwaveRun");
    }

    public void Throw()
    {
        _isThrowing = true;
        if (_throwingCoroutine == null)
            _throwingCoroutine = StartCoroutine(Throwing());
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.AddForce(-transform.up * 20f, ForceMode2D.Impulse);
        rb.AddTorque(5f, ForceMode2D.Impulse);
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

    IEnumerator Throwing()
    {
        yield return new WaitForSeconds(2f);
        _isThrowing = false;
        _throwingCoroutine = null;
    }
}

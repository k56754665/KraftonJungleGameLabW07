using UnityEngine;

public class ItemSmokeBomb : MonoBehaviour
{
    PlayerFire _playerFire;
    Canvas_Script _canvas;
    public ParticleSystem GetYouParticle;

    public int ammoPlusNum;

    private void Start()
    {
        _playerFire = GameObject.FindFirstObjectByType<PlayerFire>();
        _canvas = GameObject.FindFirstObjectByType<Canvas_Script>();
    }

    private void OnTriggerEnter2D(Collider2D _collision)
    {
        if (_collision.gameObject.CompareTag("Player"))
        {
            _playerFire.smokeBombNumber += ammoPlusNum;
            Instantiate(GetYouParticle, transform.position, transform.rotation);
            _canvas.UpdateGunNumber(_canvas.smokeBombUINum, _playerFire.smokeBombNumber);
            Destroy(this.gameObject);
        }
    }
}

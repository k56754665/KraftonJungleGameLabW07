using UnityEngine;

public class GunAmmo_Script : MonoBehaviour
{
    public enum GunAmmoColor
    {
        Blue,
        Red
    }

    [SerializeField] GunAmmoColor currentGunAmmoColor;
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
            if (currentGunAmmoColor == GunAmmoColor.Blue)
            {
                _playerFire.blueGunNumber += ammoPlusNum;
                Instantiate(GetYouParticle, transform.position, transform.rotation);
                _canvas.UpdateGunNumber(_canvas.blueGunUINum, _playerFire.blueGunNumber);
                Destroy(this.gameObject);
            }
            else if (currentGunAmmoColor == GunAmmoColor.Red)
            {
                _playerFire.redGunNumber += ammoPlusNum;
                Instantiate(GetYouParticle, transform.position, transform.rotation);
                _canvas.UpdateGunNumber(_canvas.redGunUINum, _playerFire.redGunNumber);
                Destroy(this.gameObject);
            }
        }
    }
}

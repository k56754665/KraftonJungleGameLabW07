using UnityEngine;

public class GunAmmo_Script : MonoBehaviour
{
    public enum GunAmmoColor
    {
        Blue,
        Red
    }
    public GunAmmoColor currentGunAmmoColor;
    public Player player;
    public Canvas_Script canvas;
    public ParticleSystem GetYouParticle;

    public int ammoPlusNum;

    private void Start()
    {
        player = GameObject.FindFirstObjectByType<Player>();
        canvas = GameObject.FindFirstObjectByType<Canvas_Script>();
    }


    private void OnTriggerEnter2D(Collider2D _collision)
    {
        if (_collision.gameObject.CompareTag("Player"))
        {
            if (currentGunAmmoColor == GunAmmoColor.Blue)
            {
                player.blueGunNumber += ammoPlusNum;
                Instantiate(GetYouParticle, transform.position, transform.rotation);
                canvas.UpdateGunNumber(canvas.blueGunUINum, player.blueGunNumber);
                Destroy(this.gameObject);
            }
            else if (currentGunAmmoColor == GunAmmoColor.Red)
            {
                player.redGunNumber += ammoPlusNum;
                Instantiate(GetYouParticle, transform.position, transform.rotation);
                canvas.UpdateGunNumber(canvas.redGunUINum, player.redGunNumber);
                Destroy(this.gameObject);
            }
        }
    }
}

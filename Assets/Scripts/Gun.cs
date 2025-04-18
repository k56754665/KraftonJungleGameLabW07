using System.Collections;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public GameObject bullet;
    public GameObject blueBullet;
    public GameObject redBullet;

    [SerializeField] float _blueGunRandom;
    [SerializeField] float _redGunRandom;

    public void BlueGunFire()
    {
        float randomAngle = Random.Range(-_blueGunRandom, _blueGunRandom);
        StartCoroutine(flash());
        GameObject fire_bullet = Instantiate(blueBullet, transform.position, Quaternion.identity);
        fire_bullet.GetComponent<Bullet>().from = transform.parent.gameObject.tag;
        fire_bullet.GetComponent<Bullet>().transform.rotation = transform.parent.rotation * Quaternion.Euler(0f, 0f, randomAngle + 180f);
    }

    public void RedGunFire()
    {
        float randomAngle = Random.Range(-_redGunRandom, _redGunRandom);
        StartCoroutine(flash());
        GameObject fire_bullet = Instantiate(redBullet, transform.position, Quaternion.Euler(0f, 0f, randomAngle));
        fire_bullet.GetComponent<Bullet>().from = transform.parent.gameObject.tag;
        fire_bullet.GetComponent<Bullet>().transform.rotation = transform.parent.rotation * Quaternion.Euler(0f, 0f, randomAngle + 180f);
    }

    IEnumerator flash()
    {
        GameObject flash = transform.parent.Find("Flash").gameObject;
        if (flash)
        {
            flash.SetActive(true);
            yield return new WaitForSeconds(0.1f);
            flash.SetActive(false);
        }
        else 
        {
            Debug.Log("플래시 못찾음");
        }
        
    }
}

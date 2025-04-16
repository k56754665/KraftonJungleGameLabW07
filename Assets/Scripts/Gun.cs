using System.Collections;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public GameObject bullet;
    public GameObject blueBullet;
    public GameObject redBullet;


    public void BlueGunFire()
    {
        StartCoroutine(flash());
        GameObject fire_bullet = Instantiate(blueBullet, transform.position, Quaternion.identity);
        fire_bullet.GetComponent<Bullet>().SetDirection(-transform.up);
        fire_bullet.GetComponent<Bullet>().from = transform.parent.gameObject.tag;
        fire_bullet.GetComponent<Bullet>().transform.rotation = transform.parent.rotation;
        //Instantiate(bullet, transform.position, transform.parent.rotation);
    }

    public void RedGunFire()
    {
        StartCoroutine(flash());
        GameObject fire_bullet = Instantiate(redBullet, transform.position, Quaternion.identity);
        fire_bullet.GetComponent<Bullet>().SetDirection(-transform.up);
        fire_bullet.GetComponent<Bullet>().from = transform.parent.gameObject.tag;
        fire_bullet.GetComponent<Bullet>().transform.rotation = transform.parent.rotation;
        //Instantiate(bullet, transform.position, transform.parent.rotation);
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

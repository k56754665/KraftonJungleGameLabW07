using System;
using UnityEngine;

public class PlayerFire : MonoBehaviour
{
    public enum GunType
    {
        BlueGun,
        RedGun
    }

    Canvas_Script _canvas;
    GunType currentGunType;

    [SerializeField] GameObject gun;
    [SerializeField] GameObject soundwaveBlueGun;
    [SerializeField] GameObject soundwaveRedGun;

    public int blueGunNumber;
    public int redGunNumber;

    void Start()
    {
        _canvas = GameObject.FindFirstObjectByType<Canvas_Script>();
        _canvas.UpdateGunNumber(_canvas.blueGunUINum, blueGunNumber);
        _canvas.UpdateGunNumber(_canvas.redGunUINum, redGunNumber);

        currentGunType = GunType.BlueGun; // 초기 총 종류 설정
        InputManager.Instance.fireAction += PlayerGunFire; // 총 발사
        InputManager.Instance.changeWeaponAction += CheckMouseWheel; // 총 변경

    }

    public void CheckMouseWheel(Vector2 vector)
    {
        if (vector.y > 0f) // 위로 스크롤
        {
            SwitchGun(1);
            Debug.Log("Current Gun: " + currentGunType);
        }
        else if (vector.y < 0f) // 아래로 스크롤
        {
            SwitchGun(-1);
            Debug.Log("Current Gun: " + currentGunType);
        }
    }

    void SwitchGun(int WheelDirection)
    {
        // 총 종류를 변경 direction은 휠 방향을 의미
        int gunCount = System.Enum.GetValues(typeof(GunType)).Length;
        currentGunType = (GunType)(((int)currentGunType + WheelDirection + gunCount) % gunCount);

        // 현재 총 HUD 변경
        if (currentGunType == GunType.BlueGun)
        {
            _canvas.TurnOff(_canvas.redGunUI);
            _canvas.TurnOn(_canvas.blueGunUI);
            _canvas.UpdateGunNumber(_canvas.blueGunUINum, blueGunNumber);
        }
        else if (currentGunType == GunType.RedGun)
        {
            _canvas.TurnOff(_canvas.blueGunUI);
            _canvas.TurnOn(_canvas.redGunUI);
            _canvas.UpdateGunNumber(_canvas.redGunUINum, redGunNumber);
        }
    }

    public void PlayerGunFire()
    {
        if (currentGunType == GunType.BlueGun && blueGunNumber > 0)
        {
            gun.GetComponent<Gun>().BlueGunFire();
            Instantiate(soundwaveBlueGun, transform.position, transform.rotation);
            blueGunNumber -= 1;
            _canvas.UpdateGunNumber(_canvas.blueGunUINum, blueGunNumber);
            Debug.Log("총 발사!");
        }
        else if (currentGunType == GunType.RedGun && redGunNumber > 0)
        {
            gun.GetComponent<Gun>().RedGunFire();
            Instantiate(soundwaveRedGun, transform.position, transform.rotation);
            redGunNumber -= 1;
            _canvas.UpdateGunNumber(_canvas.redGunUINum, redGunNumber);
            Debug.Log("총 발사!");
        }
    }
}

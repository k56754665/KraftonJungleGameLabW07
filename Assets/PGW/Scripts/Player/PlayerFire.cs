using UnityEngine;
using Define;

public class PlayerFire : MonoBehaviour
{
    Canvas_Script _canvas;
    GunType currentGunType;

    [SerializeField] GameObject gun;
    [SerializeField] GameObject soundwaveBlueGun;
    [SerializeField] GameObject soundwaveRedGun;
    GameObject _can; // Can Prefab
    GameObject _canObject;

    public int blueGunNumber;
    public int redGunNumber;
    

    bool _canFire; // 총 발사 가능 여부
    public bool CanFire { get { return _canFire; } set { _canFire = value; } }

    void Start()
    {
        _canvas = GameObject.FindFirstObjectByType<Canvas_Script>();
        _canvas.UpdateGunNumber(_canvas.blueGunUINum, blueGunNumber);
        _canvas.UpdateGunNumber(_canvas.redGunUINum, redGunNumber);

         _can = Resources.Load<GameObject>("Prefabs/KGJ/Can"); // Can Prefab 로드

        currentGunType = GunType.BlueGun; // 초기 총 종류 설정
        InputManager.Instance.fireAction += PlayerGunFire; // 총 발사
        InputManager.Instance.changeWeaponAction += CheckMouseWheel; // 총 변경

        SavePointManager.Instance.OnSaveEvent += SavePointFire;
        SavePointManager.Instance.OnLoadEvent += LoadSavePointFire;

        _canFire = true; // 총 발사 가능 상태로 초기화
    }

    public void CheckMouseWheel(Vector2 vector)
    {
        if (currentGunType == GunType.Can) // 오브젝트를 들고 있는 상태
        {
            return;
        }

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
            _canvas.HideCanImage();
            _canvas.TurnOff(_canvas.redGunUI);
            _canvas.TurnOn(_canvas.blueGunUI);
            _canvas.UpdateGunNumber(_canvas.blueGunUINum, blueGunNumber);
        }
        else if (currentGunType == GunType.RedGun)
        {
            _canvas.HideCanImage();
            _canvas.TurnOff(_canvas.blueGunUI);
            _canvas.TurnOn(_canvas.redGunUI);
            _canvas.UpdateGunNumber(_canvas.redGunUINum, redGunNumber);
        }
    }

    public void SetCurrentCan()
    {
        currentGunType = GunType.Can; // 오브젝트를 들고 있는 상태
        _canvas.ShowCanImage();
        _canvas.TurnOff(_canvas.redGunUI);
        _canvas.TurnOff(_canvas.blueGunUI);
    }

    public void PlayerGunFire()
    {
         // 총 발사 불가능 상태일 때는 아무것도 하지 않음
        if (!_canFire) return;

        if (currentGunType == GunType.Can)
        {
            // TODO : can을 던지는 로직
            _canObject = Instantiate(_can, transform.position, transform.rotation);
            _canObject.GetComponent<Can>().Throw();
            _canvas.HideCanImage();
            currentGunType = GunType.BlueGun;
            _canvas.TurnOff(_canvas.redGunUI);
            _canvas.TurnOn(_canvas.blueGunUI);
            _canvas.UpdateGunNumber(_canvas.blueGunUINum, blueGunNumber);
        }
        else if (currentGunType == GunType.BlueGun && blueGunNumber > 0)
        {
            gun.GetComponent<Gun>().BlueGunFire();
            Instantiate(soundwaveBlueGun, transform.position, transform.rotation);
            blueGunNumber -= 1;
            _canvas.UpdateGunNumber(_canvas.blueGunUINum, blueGunNumber);
        }
        else if (currentGunType == GunType.RedGun && redGunNumber > 0)
        {
            gun.GetComponent<Gun>().RedGunFire();
            Instantiate(soundwaveRedGun, transform.position, transform.rotation);
            redGunNumber -= 1;
            _canvas.UpdateGunNumber(_canvas.redGunUINum, redGunNumber);
        }
    }

    /// <summary>
    /// 현재 총알 수를 세이브 포인트에 저장하는 함수
    /// </summary>
    void SavePointFire()
    {
        SavePointManager.Instance.SaveBlueGunNumber = blueGunNumber;
        SavePointManager.Instance.SaveRedGunNumber = redGunNumber;
    }

    /// <summary>
    /// 총알 수를 세이브 포인트에 저장된 값으로 변경해주는 함수
    /// </summary>
    void LoadSavePointFire()
    {
        blueGunNumber = SavePointManager.Instance.SaveBlueGunNumber;
        redGunNumber = SavePointManager.Instance.SaveRedGunNumber;
    }

    private void OnDestroy()
    {
        InputManager.Instance.fireAction -= PlayerGunFire; // 총 발사
        InputManager.Instance.changeWeaponAction -= CheckMouseWheel; // 총 변경
        SavePointManager.Instance.OnSaveEvent -= SavePointFire;
        SavePointManager.Instance.OnLoadEvent -= LoadSavePointFire;
    }
}

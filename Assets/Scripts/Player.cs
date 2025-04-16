using UnityEngine;
using CodeMonkey.Utils;

public class Player : MonoBehaviour
{
    public enum PlayerState 
    {
        Walk,
        Run,
        Save
    }

    public enum GunType
    {
        BlueGun,
        RedGun
    }

    public PlayerState currentState = PlayerState.Walk;

    public float movespeed;
    public int hp;
    public int maxHp;
    protected float runMultiply = 1;

    // 총
    public GameObject gun;
    public GunType currentGunType;
    public Transform firePoint;
    public int blueGunNumber;
    public int redGunNumber;

    public Canvas_Script canvas;
    public GameManager gameManager;
    public ParticleSystem deathParticle;
    public ParticleSystem healParticle;
    public Rigidbody2D playerRb;

    // 시야각 가져오기
    [SerializeField] private FieldOfView_Script fieldOfView;

    // 음파 가져오기
    public GameObject soundwaveWalk;
    public GameObject soundwaveRun;
    public GameObject soundwaveBlueGun;
    public GameObject soundwaveRedGun;

    private float lastSoundwaveTime = 0f; // 마지막 음파 생성 시간
    private float soundwaveInterval = 0.4f; // 음파 생성 간격 (1초)

    public GameObject targetPatient;
    public bool isCanSave = false;

    public float stamina;
    public float holdKeyTime = 0f;





    void Start()
    {
        currentGunType = GunType.BlueGun; // 초기 총 종류 설정

        hp = maxHp;
        stamina = 99999f;
        runMultiply = 1;

        gameManager = GameObject.FindFirstObjectByType<GameManager>();
        canvas = GameObject.FindFirstObjectByType<Canvas_Script>();
        playerRb = GetComponent<Rigidbody2D>();

        canvas.UpdateGunNumber(canvas.blueGunUINum, blueGunNumber);
        canvas.UpdateGunNumber(canvas.redGunUINum, redGunNumber);
    }
    void Update()
    {
        // 마우스 휠 스크롤 입력 처리
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        CheckMouseWheel(scrollInput);

        SetHealth();


        if (!gameManager.isgameover)
        {
            // 플레이어 상태변화
            if (Input.GetKeyDown(KeyCode.LeftShift) && stamina > 0.3f) 
            {
                currentState = PlayerState.Run;
            }
            if (Input.GetKeyUp(KeyCode.LeftShift) && currentState == PlayerState.Run) 
            {
                currentState = PlayerState.Walk;
            }

            // 플레이어 상태에 따른 속도 배수 변화
            switch (currentState) 
            {
                case PlayerState.Walk:
                    runMultiply = 1f;
                    soundwaveInterval = 0.4f;
                    break;
                case PlayerState.Run:
                    runMultiply = 2f;
                    soundwaveInterval = 0.15f;
                    break;
                case PlayerState.Save:
                    runMultiply = 0;
                    soundwaveInterval = 0.4f;
                    break;
            }

            // stamina
            if (currentState == PlayerState.Run)
            {
                if (stamina > 0)
                {
                    stamina -= Time.deltaTime;
                }
                else 
                {
                    currentState = PlayerState.Walk;
                }
            }
            else 
            {
                stamina += Time.deltaTime/2;
            }
            stamina = Mathf.Clamp(stamina, 0, stamina);

            // 스태바로 정보전달
            // canvas.staminaBar.transform.localScale = new Vector3(stamina/1.5f, 1, 1);

            // 이동
            if (currentState != PlayerState.Save)
            {
                Vector2 moveDirection = Vector2.zero;

                if (Input.GetKey(KeyCode.W))
                {
                    moveDirection += Vector2.up;
                }
                if (Input.GetKey(KeyCode.A))
                {
                    moveDirection += Vector2.left;
                }
                if (Input.GetKey(KeyCode.S))
                {
                    moveDirection += Vector2.down;
                }
                if (Input.GetKey(KeyCode.D))
                {
                    moveDirection += Vector2.right;
                }

                // 음파 생성 조건
                if (moveDirection != Vector2.zero)
                {
                    playerRb.linearVelocity = moveDirection.normalized * movespeed * runMultiply;

                    // 음파 생성
                    if (Time.time - lastSoundwaveTime >= soundwaveInterval) // 음파 생성 간격 확인
                    {
                        GameObject soundwave = currentState == PlayerState.Run ? soundwaveRun : soundwaveWalk;
                        GameObject newSoundwave = Instantiate(soundwave, transform.position, transform.rotation);

                        // 음파 생성 후 마지막 생성 시간 업데이트
                        lastSoundwaveTime = Time.time;
                    }
                }
                else
                {
                    playerRb.linearVelocity = Vector2.zero; // 방향 없을 때 속도 0
                }
            }



            if (isCanSave && targetPatient) 
            {
                if (Input.GetKey(KeyCode.E))
                {
                    targetPatient.GetComponent<Patient_Script>().issaving = 0;
                    currentState = PlayerState.Save;
                    holdKeyTime += Time.deltaTime;

                    if (holdKeyTime >= 1f) 
                    {
                        // gameManager.score += 1;
                        TriggerPatientHeal();
                        Destroy(targetPatient);
                        holdKeyTime = 0;
                        isCanSave = false;
                        gameManager.isGameClear = true;
                        currentState = PlayerState.Walk;
                    }
                }
                else 
                {
                    holdKeyTime = 0;
                    currentState = PlayerState.Walk;
                    targetPatient.GetComponent<Patient_Script>().issaving = 1;
                }

                if (targetPatient) 
                {
                    targetPatient.GetComponent<Patient_Script>().fill.transform.localScale = new Vector3(1, holdKeyTime / 1, 1);
                }
            }

            // 플레이어 각도
            Vector3 mousePosition = Input.mousePosition;
            mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
            Vector2 direction = (mousePosition - transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle + 90));

            // 플레이어 위치, 각도와 Field of View 위치, 각도 동기화
            fieldOfView.SetAimDirection(UtilsClass.GetVectorFromAngle(angle + 90));
            fieldOfView.SetOrigin(this.transform.position);

            // 발사
            if (Input.GetMouseButtonDown(0) && currentState != PlayerState.Save)
            {
                PlayerGunFire();
            }

            // 플레이어 체력이 적으면 UI표시
            if (hp == 1)
            {
                canvas.lowHp_UI.SetActive(true);
            }
            else
            {
                canvas.lowHp_UI.SetActive(false);
            }

            // 자힐
            /*
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (transform.Find("AidKit_Indicator").gameObject.activeSelf && hp < 2)
                {
                    hp++;
                    TriggerSelfHeal();
                    transform.Find("AidKit_Indicator").gameObject.SetActive(false);
                    isHaveAdkit = false;
                }
            }
            */

            //// 자해(테스트용)
            //if (Input.GetKeyDown(KeyCode.Z))
            //{
            //    hp--;
            //}

            // 사망
            if (hp < 1)
            {
                gameManager.Gameover();
                TriggerDeath();
                if (gameObject != null)
                {
                    Destroy(gameObject);
                }
            }
        }
    }

    private void OnDestroy()
    {
        // 죽으면 파티클 생성
        Instantiate(deathParticle, transform.position, transform.rotation);
    }

    // 체력을 설정하는 메서드
    public void SetHealth()
    {
        // 체력이 최대 체력을 초과하지 않도록 제한
        if (hp > maxHp)
        {
            hp = maxHp;
        }
        // 체력이 0 이하로 떨어지지 않도록 설정
        if (hp < 0)
        {
            hp = 0; 
        }
    }
    void TriggerDeath() 
    {
        if (deathParticle != null)
        {
            ParticleSystem death = Instantiate(deathParticle, transform.position, Quaternion.identity);
            death.Play();
            Destroy(death.gameObject, 2f);
        }
    }

    void CheckMouseWheel(float _scrollInput)
    {
        if (_scrollInput > 0f) // 위로 스크롤
        {
            SwitchGun(1);
            Debug.Log("Current Gun: " + currentGunType);
        }
        else if (_scrollInput < 0f) // 아래로 스크롤
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
            canvas.TurnOff(canvas.redGunUI);
            canvas.TurnOn(canvas.blueGunUI);
            canvas.UpdateGunNumber(canvas.blueGunUINum, blueGunNumber);
        }
        else if (currentGunType == GunType.RedGun)
        {
            canvas.TurnOff(canvas.blueGunUI);
            canvas.TurnOn(canvas.redGunUI);
            canvas.UpdateGunNumber(canvas.redGunUINum, redGunNumber);
        }
    }

    void PlayerGunFire()
    {
        if (currentGunType == GunType.BlueGun && blueGunNumber > 0)
        {
            gun.GetComponent<Gun>().BlueGunFire();
            Instantiate(soundwaveBlueGun, transform.position, transform.rotation);
            blueGunNumber -= 1;
            canvas.UpdateGunNumber(canvas.blueGunUINum, blueGunNumber);
            Debug.Log("총 발사!");
        }
        else if (currentGunType == GunType.RedGun && redGunNumber > 0)
        {
            gun.GetComponent<Gun>().RedGunFire();
            Instantiate(soundwaveRedGun, transform.position, transform.rotation);
            redGunNumber -= 1;
            canvas.UpdateGunNumber(canvas.redGunUINum, redGunNumber);
            Debug.Log("총 발사!");
        }
    }

    void TriggerSelfHeal()
    {
        if (healParticle != null)
        {
            ParticleSystem heal = Instantiate(healParticle, transform.position, Quaternion.identity);
            heal.Play();
            Destroy(heal.gameObject, 2f);
        }
    }
    void TriggerPatientHeal()
    {
        if (healParticle != null)
        {
            ParticleSystem heal = Instantiate(healParticle, targetPatient.transform.position, Quaternion.identity);
            heal.Play();
            Destroy(heal.gameObject, 2f);
        }
    }


    private void OnTriggerEnter2D(Collider2D _collision)
    {
        // 총알 맞기
        if (_collision.gameObject.CompareTag("Bullet"))
        {
            Bullet bullet = _collision.gameObject.GetComponent<Bullet>();

            if (bullet.bulletColor == Bullet.BulletColor.Yellow)
            {
                hp -= 1;
                Instantiate(deathParticle, transform.position, transform.rotation);
            }
        }

        //환자 살리기
        if (_collision.gameObject.CompareTag("Patient"))
        {
            isCanSave = true;
            targetPatient = _collision.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject == targetPatient) 
        {
            isCanSave = false;
            holdKeyTime = 0f;
            targetPatient = null;
        }
    }

}
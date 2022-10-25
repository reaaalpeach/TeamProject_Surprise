using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using Cinemachine;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviourPunCallbacks
{
    public PhotonView pv;
    public TextMesh playerName;
    Animator animator;
    PlayerInput playerInput;

    //변경 변수
    CharacterController controller = null;
    VirtualJoystick virtualJoystick = null;
    Camera cam;

    Vector3 joystickDirection;
    Vector3 moveDirection;

    public float speed;
    public float speed_walk = 6f;
    public float speed_run = 20f;

    public float turnSmoothTime = 0.1f;
    public float turnSmoothVelocity;
    

    //공격
    [SerializeField] GameObject atkCooltimePanel;
    float atkCooltime = 3f;
    public GameObject attackFX;
    public Transform attackPos;
    public BoxCollider meleeArea;
    AudioSource audioSource;
    public AudioClip hitSound;

    //Run
    GameObject runCooltimePanel;
    float runTime = 0;
    float runMaxTime = 4f;
    float runCooltime = 3f;
    public ParticleSystem runFX;
    [SerializeField] ParticleSystem runParticle;
    bool isRunPaticlePlay = false;

    // 킬로그에서 사용할 해시테이블
    Hashtable ht = new Hashtable();

    public bool isMove { get; private set; }
    public bool isReady { get; private set; }

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        if (SceneManager.GetActiveScene().name == "GameScene") // 게임씬에서는 닉네임패널 끄기
            playerName.gameObject.SetActive(false);

        if (PhotonNetwork.IsConnected)
            pv.RPC("GetPlayerName", RpcTarget.All);

        if(photonView.IsMine)
        {
            playerInput = GetComponent<PlayerInput>();
            controller = GetComponent<CharacterController>();
            animator = GetComponent<Animator>();
            virtualJoystick = FindObjectOfType<VirtualJoystick>();
            cam = Camera.main;

            //공격
            atkCooltimePanel = GameObject.Find("AtkCoolTime_Panel");
            
            atkCooltimePanel.transform.parent.GetComponent<Button>().onClick.AddListener(() => Attack());
            //run
            runCooltimePanel = GameObject.Find("RunCoolTime_Panel");
            
        }
    }

    void FixedUpdate()
    {
        if (!photonView.IsMine) return;

        Move();
        animator.SetBool("Walk", isMove);

    }

    [PunRPC]
    void GetPlayerName()
    {
        if (photonView.IsMine)
        {
            attackPos.gameObject.name = PhotonNetwork.LocalPlayer.NickName;
            gameObject.name = PhotonNetwork.LocalPlayer.NickName;
            playerName.text = PhotonNetwork.LocalPlayer.NickName;
        }
        else
        {
            attackPos.gameObject.name = pv.Owner.NickName;
            gameObject.name = pv.Owner.NickName;
            playerName.text = pv.Owner.NickName;
        }
    }

    void Move()
    { 
        if (virtualJoystick != null)
        {
            joystickDirection = virtualJoystick.Dir;

            if (joystickDirection.magnitude >= 0.1f)
            {
                isMove = true; // 애니메이션을 위한 bool값

                float targetAngle = Mathf.Atan2(joystickDirection.x, joystickDirection.z) * Mathf.Rad2Deg + cam.transform.eulerAngles.y;
                // Mathf.Atan2(x,y) : x와 y의 값으로 아크탄젠트함수를 이용해 연산해 결과를 라디안값으로 반환
                // Mathf.Rad2Deg : 라디안을 일반각도로 변환
                // cam 회전값이 기본인 상태(cam.eulerAngles.y)에서 플레이어 회전 인풋값(Mathf.Atan2(direction.x, direction.z))을 넣는다
                // AcrTan(x/y)를 의미 -> 유니티 좌표계는 y축 + 방향이 0도로 시계방향 좌표계를 쓰기 때문에 y/x가 아닌 x/y
                // Atan는 절대각을 -π/2 ~ π/2의 라디안 값으로 반환
                // Atan2는 두 점 사이의 상대좌표(x, y)를 받아 절대각을 -π ~ π의 라디안 값으로 반환 -> 데카르트 좌표에 유용. 음수를 허용

                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
                //현재각과 목표 회전값까지 사이의 자연스러운 회전을 위한 각도를 계속해서 계산
                transform.rotation = Quaternion.Euler(0f, angle, 0f);

                moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                //targetangle 방향의 z축 값(앞 방향)만을 취한다

                Run();//뛰는 경우와 걷는 경우 speed값을 여기서 조절해준다
            }
            else
            {
                isMove = false;//애니메이션을 위한 bool값

                moveDirection = Vector3.zero;
            }
            controller.SimpleMove(moveDirection.normalized * speed * Time.deltaTime); //controller.move와 다른 점은 Time.deltaTime를 곱해주지 않아도 됨. 또 지면 방향 설정을 해주면 중력은 자동 계산 해준다
        }
    }

    public void Attack()//버튼 안에 onclick으로 들어가 있다
    {
        animator.SetTrigger("Attack");
        pv.RPC("CheckingAttackArea_Particle", RpcTarget.All);
        atkCooltimePanel.GetComponent<CoolTime>().SetCoolTime(atkCooltime);//쿨타임 패널에 공격 쿨타임 정보 넘겨주기
        atkCooltimePanel.SetActive(true);//쿨타임 패널 활성화. -> 쿨타임 동안 버튼을 비활성화 시키고 쿨타임 시간만큼 돌아가는 패널 위의 슬라이더를 생성함.
    }

    void Run()// runTime = 0 에서 부터 시작하며 달리기 버튼을 눌렀을 때 runMaxTime까지 커질 수 있으며 max 값까지 도달하지 않고 중간에 손을 땐 경우 Time.deltaTime을 계속 빼주면서 0값을 만든다.
    {
        if(playerInput.run)//달리기 버튼을 눌렀을 때
        {
            if (1 < runTime / runMaxTime) //달릴 수 있는 총 시간을 모두 소모한 경우
            {
                playerInput.run = false;//달리지 않는 상태라는 bool값 갱신
                //runTime = runMaxTime;
                runCooltimePanel.GetComponent<CoolTime>().SetCoolTime(runCooltime);//coolTime panel에 달리기 쿨타임 값 전달
                runCooltimePanel.SetActive(true);//쿨타임 패널 활성화 -> 쿨타임 동안 버튼을 비활성화 시키고 쿨타임 시간만큼 돌아가는 패널 위의 슬라이더를 생성함.
            }
            runTime += Time.deltaTime;
        }
        else if(!playerInput.run && runTime > 0)//달리기 버튼에서 손을 땠는데 달릴 수 있는 총 시간을 아직 소모하지 않은 경우
        {
            runTime -= Time.deltaTime;//runTime 충전
            if (runTime < 0)//Time.deltaTime을 빼고 있어서 딱 0이 되지 않기 때문에 0보다 작아지면 0값으로 만들어준다
            {
                runTime = 0f;
            }
        }

        //Particle V2 연속
        //continuous
        if (playerInput.run && !isRunPaticlePlay)
        {
            runParticle.Play();
            isRunPaticlePlay = true;
        }
        else if (!playerInput.run && isRunPaticlePlay)
        {
            runParticle.Stop();
            isRunPaticlePlay = false;
        }

        speed = playerInput.run ? speed_run : speed_walk;//걷는지 뛰는지에 따른 speed 값 조절
    }

    IEnumerator Die()
    {
        animator.SetTrigger("Die");
        //PhotonNetwork.LocalPlayer.CustomProperties["준비완료"] = 0;

        yield return new WaitForSeconds(1.8f);
        PhotonNetwork.Destroy(gameObject);
    }

    [PunRPC]
    void CheckingAttackArea_Particle()
    {
        StartCoroutine(Swing());
    }
    IEnumerator Swing()
    {
        yield return new WaitForSeconds(0.1f);
        if(SceneManager.GetActiveScene().name == "GameScene")
        {
            meleeArea.enabled = true;
        }
        audioSource.PlayOneShot(hitSound);

        yield return new WaitForSeconds(0.4f);
        meleeArea.enabled = false;
        Instantiate(attackFX, attackPos.position, attackPos.rotation);
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Attack"))
        {
            PhotonNetwork.LocalPlayer.CustomProperties.Add("공격", null);
            PhotonNetwork.LocalPlayer.CustomProperties.Add("죽음", null);

            StartCoroutine("Die");

            ht["공격"] = other.gameObject.name;
            ht["죽음"] =  gameObject.name;
        }
        PhotonNetwork.LocalPlayer.SetCustomProperties(ht);
    }
}

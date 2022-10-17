using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using Cinemachine;

public class PlayerController : MonoBehaviourPunCallbacks
{
    public PhotonView pv;
    PlayerInput playerInput;

    public TextMesh playerName;
    Animator animator;
    Rigidbody rb;

    float moveSpeed = 1f;

    //���� ����
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
    //

    //����
    GameObject atkCooltimePanel;
    float atkCooltime = 3f;

    //Run
    GameObject runCooltimePanel;
    float runTime = 0;
    float runMaxTime = 4f;
    float runCooltime = 3f;

    public bool isMove { get; private set; }
    public bool isReady { get; private set; }

    void Awake()
    {
        if(PhotonNetwork.IsConnected)
            pv.RPC("GetPlayerName", RpcTarget.All);
        //������
        /*playerInput = GetComponent<PlayerInput>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();*/
        //������
        if(photonView.IsMine)
        {
            playerInput = GetComponent<PlayerInput>();
            controller = GetComponent<CharacterController>();
            animator = GetComponent<Animator>();
            virtualJoystick = FindObjectOfType<VirtualJoystick>();
            cam = Camera.main;

            //����
            atkCooltimePanel = GameObject.Find("AtkCoolTime_Panel");
            atkCooltimePanel.transform.parent.GetComponent<Button>().onClick.AddListener(() =>
                Attack()
            );
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
            playerName.text = PhotonNetwork.LocalPlayer.NickName;
        else
            playerName.text = pv.Owner.NickName;
    }

    void Move()
    {
        //������
        /*if (playerInput.move != 0 || playerInput.rotate !=0)
        {
            isMove = true;
            Vector3 direction = playerInput.move * Vector3.right + playerInput.rotate * Vector3.forward; // �������ϱ�
            transform.forward = direction;
            rb.velocity = transform.forward * moveSpeed *(playerInput.run ? 8f : 5f);
        }
        else isMove = false;*/

        //���� ��

        if (virtualJoystick != null)
        {
            joystickDirection = virtualJoystick.Dir;

            if (joystickDirection.magnitude >= 0.1f)
            {
                isMove = true;//�ִϸ��̼��� ���� bool��

                float targetAngle = Mathf.Atan2(joystickDirection.x, joystickDirection.z) * Mathf.Rad2Deg + cam.transform.eulerAngles.y;
                //cam ȸ������ �⺻�� ����(cam.eulerAngles.y)���� �÷��̾� ȸ�� ��ǲ��(Mathf.Atan2(direction.x, direction.z))�� �ִ´�
                //AcrTan(x/y)�� �ǹ� -> ����Ƽ ��ǥ��� y�� +������ 0���� �ð���� ��ǥ�踦 ���� ������ y/x�� �ƴ� x/y
                //Atan�� ���밢�� -��/2 ~ ��/2�� ���� ������ ��ȯ
                //Atan2�� �� �� ������ �����ǥ(x, y)�� �޾� ���밢�� -�� ~ ���� ���� ������ ��ȯ -> ��ī��Ʈ ��ǥ�� ����. ������ ���

                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
                //���簢�� ��ǥ ȸ�������� ������ �ڿ������� ȸ���� ���� ������ ����ؼ� ���
                transform.rotation = Quaternion.Euler(0f, angle, 0f);

                moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                //targetangle ������ z�� ��(�� ����)���� ���Ѵ�

                Run();

                controller.SimpleMove(moveDirection.normalized * speed * Time.deltaTime); //controller.move�� �ٸ� ���� Time.deltaTime�� �������� �ʾƵ� ��. �� ���� ���� ������ ���ָ� �߷��� �ڵ� ��� ���ش�
            }
            else
            {
                isMove = false;//�ִϸ��̼��� ���� bool��

                moveDirection = Vector3.zero;
            }

            controller.SimpleMove(moveDirection.normalized * speed * Time.deltaTime); //controller.move�� �ٸ� ���� Time.deltaTime�� �������� �ʾƵ� ��. �� ���� ���� ������ ���ָ� �߷��� �ڵ� ��� ���ش�
        }
    }

    public void Attack()
    {
        animator.SetTrigger("Attack");
        atkCooltimePanel.GetComponent<CoolTime>().SetCoolTime(atkCooltime);
        atkCooltimePanel.SetActive(true);
    }

    void Run()
    {
        if(playerInput.run)
        {
            if(1 < runTime / runMaxTime)
            {
                playerInput.run = false;
                runTime = runMaxTime;
                runCooltimePanel.GetComponent<CoolTime>().SetCoolTime(runCooltime);
                runCooltimePanel.SetActive(true);
            }
            runTime += Time.deltaTime;
        }
        else if(!playerInput.run && runTime > 0)
        {
            if(runTime < 0)
            {
                runTime = 0f;
            }
            else
            {
                runTime -= Time.deltaTime;
            }
        }
        speed = playerInput.run ? speed_run : speed_walk;
    }
}

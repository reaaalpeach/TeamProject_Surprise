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

    public float speed = 6f;

    public float turnSmoothTime = 0.1f;
    public float turnSmoothVelocity;
    //
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
            controller = GetComponent<CharacterController>();
            animator = GetComponent<Animator>();
            virtualJoystick = FindObjectOfType<VirtualJoystick>();
            cam = Camera.main;
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
        
        /*if(virtualJoystick != null)
        {*/
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");

            Vector3 direction = new Vector3(horizontal, 0, vertical).normalized;

            if (direction.magnitude >= 0.1f)
            {
                float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.transform.eulerAngles.y;
                //cam ȸ������ �⺻�� ����(cam.eulerAngles.y)���� �÷��̾� ȸ�� ��ǲ��(Mathf.Atan2(direction.x, direction.z))�� �ִ´�
                //AcrTan(x/y)�� �ǹ� -> ����Ƽ ��ǥ��� y�� +������ 0���� �ð���� ��ǥ�踦 ���� ������ y/x�� �ƴ� x/y
                //Atan�� ���밢�� -��/2 ~ ��/2�� ���� ������ ��ȯ
                //Atan2�� �� �� ������ �����ǥ(x, y)�� �޾� ���밢�� -�� ~ ���� ���� ������ ��ȯ -> ��ī��Ʈ ��ǥ�� ����. ������ ���

                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
                //���簢�� ��ǥ ȸ�������� ������ �ڿ������� ȸ���� ���� ������ ����ؼ� ���
                transform.rotation = Quaternion.Euler(0f, angle, 0f);

                Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                //targetangle ������ z�� ��(�� ����)���� ���Ѵ�

                controller.SimpleMove(moveDir.normalized * speed); //controller.move�� �ٸ� ���� Time.deltaTime�� �������� �ʾƵ� ��. �� ���� ���� ������ ���ָ� �߷��� �ڵ� ��� ���ش�
            }
            else
            {
                Vector3 moveDir = Vector3.zero;
                //moveDir.y -= 6f * Time.deltaTime;

                controller.SimpleMove(moveDir.normalized * speed); //move
            }
        
    }
}

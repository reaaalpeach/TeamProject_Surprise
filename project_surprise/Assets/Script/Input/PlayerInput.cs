using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable; // ����Ƽ���� �����ϴ� hashtable�� ��ġ�� ������ �ʿ�!


// MonoBehaviourPun : photonView ������Ƽ�� ���� ���� ������Ʈ�� Photon View ������Ʈ�� ������ ���� ���
public class PlayerInput : MonoBehaviourPun 
{ 
    public float move { get; private set; }
    public float rotate { get; private set; }
    public bool attack { get; set; }
    public bool run { get; set; }
    public bool ready { get; set; }

    //PhotonView pv;
    //Hashtable ht;


    public void Movement(Vector2 inputDirection)
    {
        Vector2 moveInput = inputDirection;
        move =  moveInput.x;
        rotate = moveInput.y;
    }

    void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;

        //pv = GetComponent<PhotonView>();

        //PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable { { "�غ�", 0 } }); // �ϴ� ������ ���� 0(�غ�� 1, �غ�X�� 1)
        //ht = PhotonNetwork.LocalPlayer.CustomProperties;

        //ready = false;

        //if (GameManager.instance != null && GameManager.instance.isGameOver)
        //{
        //    move = 0;
        //    rotate = 0;
        //    attack = false;
        //    run = false;
        //    return;
        //}
    }

    void Update()
    {
        // ���� ���� ������Ʈ�� ���� ���� ������Ʈ�� ��쿡�� true!!
        // ����Ʈ�÷��̾�(�� ��ǻ�Ϳ� ���̴� �ٸ� �÷��̾�)�� ��쿡�� return��.
        if (!photonView.IsMine) return; 

        Keyboard();
    }
    void Keyboard()
    {
        move = Input.GetAxisRaw("Horizontal");
        rotate = Input.GetAxisRaw("Vertical");
    }

}

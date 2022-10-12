using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

// MonoBehaviourPun : photonView ������Ƽ�� ���� ���� ������Ʈ�� Photon View ������Ʈ�� ������ ���� ���
public class PlayerInput : MonoBehaviourPun //IPunObservable
{ 
    public float move { get; private set; }
    public float rotate { get; private set; }
    public bool attack { get; set; }
    public bool run { get; set; }
    public bool ready { get; set; }

    

    public void Movement(Vector2 inputDirection)
    {
        Vector2 moveInput = inputDirection;
        move =  moveInput.x;
        rotate = moveInput.y;
    }

    void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;

        ready = false;

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
        GameReady();
    }
    void Keyboard()
    {
        move = -1*Input.GetAxisRaw("Horizontal");
        rotate = -1*Input.GetAxisRaw("Vertical");
    }

    //[PunRPC]
    void GameReady()
    {
        if(PhotonNetwork.IsMasterClient && ready)
            PhotonNetwork.LoadLevel("GameScene");
    }

}

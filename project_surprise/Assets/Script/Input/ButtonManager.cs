using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable; // ����Ƽ���� �����ϴ� hashtable�� ��ġ�� ������ �ʿ�!

public class ButtonManager : MonoBehaviourPunCallbacks//,IPunObservable
{
    PhotonView pv;
    PlayerInput playerInput;
    public Text readyText;
    int readyButton = 0;
    int readyCnt = 0;

    [Header("Run")]
    [SerializeField] Button runButton;

    Hashtable temp = new Hashtable();
    void Start()
    {
        playerInput = FindObjectOfType<PlayerInput>();
        temp.Add("�غ�Ϸ�", 0);


        //PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable { { "�غ�", num } }); 
        //ht = PhotonNetwork.LocalPlayer.CustomProperties;

    }

    public void RunButtonDown()
    {
        playerInput.run = true; // PlayerInput�� Update���� �Ź� �˻������ʾƵ��Ǽ� ���⼭ �ִ°� �����Ű��⵵,,
        Debug.Log("��ưDown ����");
    }
    public void RunButtonUp()
    {
        playerInput.run = false;
        Debug.Log("��ưUp ����");
    }
    public void AttackButtonDown()
    {
        playerInput.attack = true;
        Debug.Log("��ưDown ����");
    }
    public void AttackButtonUp()
    {
        playerInput.attack = false;
        Debug.Log("��ưUp ����");
    }
    public void ReadyButton()
    {
        ++readyButton;
        if (readyButton % 2 == 1)
        {
            readyText.text = "�غ�Ϸ�";
            temp["�غ�Ϸ�"] = 1;
        }
        else
        {
            readyText.text = "�غ��ϱ�";
            temp["�غ�Ϸ�"] = 0;
        }
        PhotonNetwork.LocalPlayer.SetCustomProperties(temp);
    }
    void ReadyStatusRenew()
    {
        readyCnt = 0;
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            Debug.Log("PlayerLength : " + PhotonNetwork.PlayerList.Length);
            readyCnt += (int)PhotonNetwork.PlayerList[i].CustomProperties["�غ�Ϸ�"];
        }

        if(PhotonNetwork.IsMasterClient)
        {
            if(readyCnt == PhotonNetwork.PlayerList.Length)
                PhotonNetwork.LoadLevel("GameScene");
        }
        Debug.Log("readyCnt : " + readyCnt);
    }

    public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, Hashtable changedProps) // ������Ƽ ����Ǹ� �ڵ����� ȣ���.
    {
        if(targetPlayer == PhotonNetwork.LocalPlayer)
        {
            if(changedProps != null)
            {
                ReadyStatusRenew();
            }
        }

    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
           ReadyStatusRenew();
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
           ReadyStatusRenew();
    }
}
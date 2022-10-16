using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable; // ����Ƽ���� �����ϴ� hashtable�� ��ġ�� ������ �ʿ�!

public class ButtonManager : MonoBehaviourPunCallbacks
{
    public PhotonView pv;
    PlayerInput playerInput;
    public Text readyText;
    public Text playerList;
    public Transform gridTR;

    Text playerStatus = null;
    int readyButton = 0;
    int readyCnt = 0;
    bool isReady = false;

    Dictionary<string, Text> playerDic = new Dictionary<string, Text>();

    Hashtable temp = new Hashtable();
    Hashtable playerName = new Hashtable();

    void Awake()
    {
        playerInput = FindObjectOfType<PlayerInput>();

        playerName.Add("�г���", PhotonNetwork.LocalPlayer.NickName);
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerName);

        if (PhotonNetwork.IsMasterClient)
        {
            temp.Add("�غ�Ϸ�", 1);
            readyText.gameObject.SetActive(false);
        }
        else
        {
            temp.Add("�غ�Ϸ�", 0);
            readyText.gameObject.SetActive(true);
        }
        PhotonNetwork.LocalPlayer.SetCustomProperties(temp);
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
        if(PhotonNetwork.IsMasterClient) PhotonNetwork.LoadLevel("GameScene");

        ++readyButton;
        if (readyButton % 2 == 1)
        {
            readyText.text = "�غ�Ϸ�!";
            temp["�غ�Ϸ�"] = 1;
        }
        else
        {
            readyText.text = "�غ��Ϸ��� �����ּ���!";
            temp["�غ�Ϸ�"] = 0;
        }
        PhotonNetwork.LocalPlayer.SetCustomProperties(temp);
    }
    void ReadyStatusRenew()
    {
        readyCnt = 0;
        Debug.Log("PlayerLength : " + PhotonNetwork.PlayerList.Length);
        for (int i = 0; i < PhotonNetwork.CurrentRoom.PlayerCount; i++)
        {
            readyCnt += (int)PhotonNetwork.PlayerList[i].CustomProperties["�غ�Ϸ�"];
        }
        Debug.Log("readyCnt : " + readyCnt);
        if (PhotonNetwork.IsMasterClient)
        {
            if (!isReady) // ���� �غ��ư�� �ٷ� Ȱ��ȭ �Ǵ� �� �������� ����
            {
                readyCnt -= 1;
                isReady = true;
            }
            if ((readyCnt == PhotonNetwork.CurrentRoom.PlayerCount) && isReady ) // ������ ����ī��Ʈ�� ���廩�� �ٸ� �÷���� ���� ������
            {
                readyText.gameObject.SetActive(true); // ������ �غ��ư Ȱ��ȭ
                readyText.text = "������ �����Ϸ��� �����ּ���!";
            }
            else if((readyCnt != PhotonNetwork.CurrentRoom.PlayerCount) && isReady) // ��� ���𰡵Ǽ� ������ �غ��ư�� Ȱ��ȭ��µ� ���߿� �ٸ� �÷��̾� ���� �� �ٽ� �غ��ư ���ֱ�
            {
                readyText.gameObject.SetActive(false); // ������ �غ��ư Ȱ��ȭ
            }
        }
    }

    //[PunRPC]
    void PlayerStatus(Photon.Realtime.Player p)
    {
        Debug.Log("�÷��̾� ����Ʈ ����");
        int tmp = (int)p.CustomProperties["�غ�Ϸ�"];
        string readyStatus = new string("");
        readyStatus = tmp == 1 ? "�غ�Ϸ�" : "���� �غ� ��..";

        List<string> playerNameList = new List<string>();

        for (int i = 0; i < PhotonNetwork.CurrentRoom.PlayerCount; i++)
        {
            playerNameList.Add((string)PhotonNetwork.PlayerList[i].CustomProperties["�г���"]);

        }

        Text tempPlayerStatus = null;
        if (!playerNameList.Contains(p.NickName)) // �÷��̾ ���� ���
        {
            playerDic.TryGetValue(p.NickName, out tempPlayerStatus);
            Destroy(tempPlayerStatus);
        }
        else // �÷��̾� ������ ���ŵ� ���
        {
            if (playerDic.ContainsKey(p.NickName) == false) // �÷��̾ ���� ���� ���
            {
                Text playerStatus = Instantiate(playerList, gridTR);

                if (p.IsMasterClient) playerStatus.text = "<color=red>[����]</color>" + p.NickName + "�� " + "<color=red>" + readyStatus + "</color>";
                else playerStatus.text = p.NickName + "�� " + "<color=red>" + readyStatus + "</color>";

                playerDic.Add(p.NickName, playerStatus);
            }
            else // ������ ���� ������ ���ŵ� ���
            {
                playerDic.TryGetValue(p.NickName, out tempPlayerStatus);

                if (p.IsMasterClient) playerStatus.text = "<color=red>[����]</color>" + p.NickName + "�� " + "<color=red>" + readyStatus + "</color>";
                else tempPlayerStatus.text = p.NickName + "�� " + "<color=red>" + readyStatus + "</color>";
            }
        }
    }


    public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, Hashtable changedProps) // ������Ƽ ����Ǹ� �ڵ����� ȣ���.
    {
        ReadyStatusRenew();
        PlayerStatus(targetPlayer);
        //photonView.RPC("PlayerStatus", RpcTarget.All, targetPlayer);
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        ReadyStatusRenew();
        PlayerStatus(newPlayer);
        //photonView.RPC("PlayerStatus", RpcTarget.All, newPlayer);
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        ReadyStatusRenew();
        PlayerStatus(otherPlayer);
        //photonView.RPC("PlayerStatus", RpcTarget.All, otherPlayer);
    }
}
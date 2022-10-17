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

    [Header("PlayerList")]
    [SerializeField] Transform scrollContent;
    [SerializeField] GameObject playerList;

    Hashtable temp = new Hashtable();
    void Start()
    {
        Debug.Log("�� ��ȯ");
        playerInput = FindObjectOfType<PlayerInput>();
        temp.Add("�غ�Ϸ�", 0);
        Debug.Log("�г���" + PhotonNetwork.LocalPlayer.NickName);
        
        PlayerState();
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

    //[PunRPC]
    void PlayerState()
    {
        Transform[] childList = scrollContent.GetComponentsInChildren<Transform>();
        Debug.Log("�ڽĿ�����Ʈ " + childList.Length);
        if(childList != null)
        {
            for (int i = 1; i < childList.Length; i++)
            {
                if(childList[i] != scrollContent)
                {
                    Destroy(childList[i].gameObject);
                    Debug.Log("list ����");
                }
            }
        }

        for(int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            GameObject list = Instantiate(playerList, scrollContent);
            Debug.Log("list ����");
            Text playerName = list.transform.GetChild(0).GetComponent<Text>();
            Text ready = list.transform.GetChild(1).GetComponent<Text>();

            playerName.text = (string)PhotonNetwork.PlayerList[i].CustomProperties["�г���"];
            Debug.Log((string)PhotonNetwork.PlayerList[i].CustomProperties["�г���"]);
            
            bool isReady = 1 == (int)PhotonNetwork.PlayerList[i].CustomProperties["�غ�Ϸ�"];
            ready.text = isReady ? "Ready" : "not Ready";
        }
    }

    public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, Hashtable changedProps) // ������Ƽ ����Ǹ� �ڵ����� ȣ���.
    {
        Debug.Log("�÷��̾� ���� ������Ʈ");
        PlayerState();
        ReadyStatusRenew();
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        Debug.Log("�÷��̾� ����");
        PlayerState();
        ReadyStatusRenew();

    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        Debug.Log("�÷��̾� ����");
        PlayerState();
        ReadyStatusRenew();
    }
}
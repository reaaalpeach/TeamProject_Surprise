using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    private string gameVersion = "1.0"; //  ���ӹ��� : ���� �������� ��Ī�� �� ���

    public Text connectionInfoText; // ��Ʈ��ũ ���� ������ ǥ���� �ؽ�Ʈ
    public Button connectServerButton; // ���� ���� ��ư
    public Button connectRoomButton; // ���� ���� ��ư
    public InputField nicknameInput; // �г��� �Է��� ��ǲ�ʵ�
    public InputField roomNameInput; // ���̸� �Է��� ��ǲ�ʵ�
    public GameObject nicknameInputPanel;
    public GameObject joinRoomPanel;

    public GameObject room;
    public Transform gridTR;
    AudioSource audioSource;
    public AudioClip clickClip;

    private Dictionary<string, GameObject> roomDict = new Dictionary<string, GameObject>(); // �� ����� �����ϱ����� ��ųʸ� �ڷ���(key, value)

    void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true; // ������ ȥ�� ���� �ε��ϸ�, ������ ����� �ڵ����� ��ũ��

        PhotonNetwork.SendRate = 60;
        PhotonNetwork.SerializationRate = 30;

        PhotonNetwork.GameVersion = gameVersion; // ���ӿ� �ʿ��� ���ӹ��� ����

        audioSource = FindObjectOfType<AudioSource>();
    }
    public void Connect()
    {
        audioSource.PlayOneShot(clickClip);

        if (string.IsNullOrEmpty(nicknameInput.text)) // �̸��� ��ǲ�ʵ尡 ����ִٸ�
        {
            nicknameInput.text = PlayerInfo.instance.playerName + $"_{Random.Range(1, 100):000}"; // �������� �̸� �ο�
        }
        PhotonNetwork.LocalPlayer.NickName = nicknameInput.text;

        PhotonNetwork.LocalPlayer.CustomProperties.Add("�г���", PhotonNetwork.LocalPlayer.NickName);
        PhotonNetwork.LocalPlayer.CustomProperties.Add("�غ�Ϸ�", 0);

        PlayerPrefs.SetString("PlayerName", nicknameInput.text);

        PhotonNetwork.ConnectUsingSettings(); // �г��� �Ԏ��ϰ� Go!��ư ������ ������ ������ �����ͼ��� ���� �õ�

        connectionInfoText.text =  "��Ȳ�۹� ����� ��..."; // ���� ������...���� �������� �ڶ�� ��...?
    }

    public override void OnConnectedToMaster() // ���� ������ ������ ���� �����ϸ� �ڵ����� �����
    {
        connectionInfoText.text = "�¶��� : ��Ȳ�۹� ����� ����!"; // ����or���� ���� �Ϸ�!

        nicknameInputPanel.SetActive(false);
        joinRoomPanel.SetActive(true);

        PhotonNetwork.JoinLobby(); // �κ� ���� �븮��Ʈ �ݹ��Լ��� �Ҹ���~!!!!
    }

    // ������ ���� ���ӿ� �����߰ų�, �̹� ������ ������ ���ӵ� ���¿��� ��� ������ ������ ������ �ڵ� ����
    // ������ �����, ������ ���� ���¸� ǥ�ö�� �� ���� ��ư�� ��Ȱ��ȭ. �׸��� ������ ������ ������ �õ�
    public override void OnDisconnected(DisconnectCause cause)
    {
        connectServerButton.interactable = false;
        connectionInfoText.text = "�������� : ��Ȳ�۹� ����� ����!\n�ٽ� ����� ��..."; // ���� ���� ����! �ٽ� ����� ��!
        PhotonNetwork.ConnectUsingSettings();
    }
    public override void OnJoinedLobby()
    {
        Debug.Log("�κ� ����");
    }
    public void ConnectRandomRoom()
    {
        audioSource.PlayOneShot(clickClip);
        connectRoomButton.interactable = false; // �ߺ� ���� �õ��� �������� ��� ��Ȱ��ȭ

        if(PhotonNetwork.IsConnected) // ������ ������ ���� ���̶��(������ ������ ������ �� �� ���¿��� ���ӽõ����ܸ� ��������)
        {
            //�� ���� ����
            connectionInfoText.text = "�������� ���� ��..."; // �������� ���� ��...
            PhotonNetwork.JoinRandomRoom();
        }
        else // �ƴϸ� ������ ������ ���� �õ�
        {
            connectionInfoText.text = "�������� : ��Ȳ�۹� ����� ����!\n�ٽ� ����� ��..."; // ���� ���� ����! �ٽ� ����� ��!
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    // ���� �� ���� ������ ��쿡 �ڵ� ����. ������ �������� ������ ���� ���� �ƴ�!!!
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        connectionInfoText.text = "�� ������ �����! ���ο� ���� Ž�� ��..."; // �� ������ �����! ���ο� ���� Ž�� ��...
        Invoke("CreateRoom", 1.5f);
    }

    public void CreateRoom() // �����
    {
        audioSource.PlayOneShot(clickClip);
        connectionInfoText.text = "���ο� ���� ���� ��...";

        RoomOptions option = new RoomOptions();
        option.IsOpen = true;
        option.IsVisible = true;
        option.MaxPlayers = 10;

        if (string.IsNullOrEmpty(roomNameInput.text)) // ���̸��� ��ǲ�ʵ尡 ����ִٸ�
        {
            roomNameInput.text = $"ROOM_{Random.Range(1, 100):000}"; // �������� �̸� �ο�
        }
        PhotonNetwork.CreateRoom(roomNameInput.text, option);
        //Invoke(GetCreateRoom(roomNameInput.text, option), 1.5f);
    }

    #region �游���Ϸ�
    public override void OnCreatedRoom() // ������Ϸ�Ǹ� �Ҹ�
    {
        Debug.Log("������Ϸ�");
    }
    #endregion
    // �κ� �������� �� �ڵ��� ȣ���
    public override void OnRoomListUpdate(List<RoomInfo> roomList) // ��� ���� �渮��Ʈ ������Ʈ
    {
        Debug.Log("�渮��Ʈ ������Ʈ");

        GameObject tempRoom = null;
        foreach (RoomInfo roomInfo in roomList)
        {
            if(roomInfo.RemovedFromList == true) // ���� ������ ���
            {
                roomDict.TryGetValue(roomInfo.Name, out tempRoom);
                Destroy(tempRoom);
                roomDict.Remove(roomInfo.Name);
            }
            else // �� ������ ���ŵ� ���
            {
                if (roomDict.ContainsKey(roomInfo.Name) == false) // ���� ó�� ������ ���
                {
                    GameObject _room = Instantiate(room, gridTR); // ���� ��Ÿ���� �гλ���
                    SetRoomInfo(_room, roomInfo); // ������ ������Ʈ ��
                    roomDict.Add(roomInfo.Name, _room); // ����������� ��ųʸ��� �־��ֱ�
                }
                else // ���ŵȰ�� (���� ���� ������ ����� ���)
                {
                    roomDict.TryGetValue(roomInfo.Name, out tempRoom); // ���̸��� �´� ���� ������. ���̸��� �ش��ϴ� ���� ������Ʈ��������
                    SetRoomInfo(tempRoom, roomInfo); // �׸��� �ٽ� ���� ����
                }
            }
        }
    }

    void SetRoomInfo(GameObject rommD, RoomInfo roomI) // ������ ���� ��, �� �� ������ ������ ���� �Լ� ��������Ʈ ����
    {
        RoomData roomData = rommD.GetComponent<RoomData>();
        roomData.roomName = roomI.Name;
        roomData.maxPlayer = roomI.MaxPlayers;
        roomData.playerCount = roomI.PlayerCount;
        roomData.UpdateInfo();
        roomData.GetComponent<Button>().onClick.AddListener(delegate { OnClickRoom(roomData.roomName); }); 
        // �͸� �޼��� : �̸��� ���� �޼����� ��ü(����)�� �ִ� ��. �̸� ��������Ʈ�� ��밡��.
        // ��������Ʈ�� �̸� ���ǵ� �޼��带 �����ϴ� ���� �ƴ϶� '�̸�����'�޼��带 ���� �����ϴ� ��!!!
        // => �޼���� ��ſ� delegateŰ����� �Բ� �͸� �޼����� ���¸� ������ ��! delegate(�Ű�����) {����;};
    }
    void OnClickRoom(string roomN) // �̹� �ִ� �濡 �� ��, Ŭ���� ���̸����� ����
    {
        PhotonNetwork.JoinRoom(roomN); // ���̸����� ����
        Debug.Log(roomN);
    }
    public override void OnJoinedRoom() // �� ������ ������ ��� �ڵ����� �����(�ڱⰡ ���� CreateRoom()���� ����), ���⼭ �����غ� �� �÷��̾����� �̵�
    {
        Debug.Log("������Ϸ�");
        connectionInfoText.text = "���� �Լ� ����!";
        Invoke("GetOnJoinedRoom", 1.5f);
        connectionInfoText.text = "�������� ���� ��...";
    }

    void GetOnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("RoomScene");
    }

}



using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable; // 유니티에서 제공하는 hashtable과 겹치기 때문에 필요!
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviourPunCallbacks
{
    public PhotonView pv;
    PlayerInput playerInput;
    public Text readyText;

    Text playerStatus = null;
    int readyButton = 0;
    int readyCnt = 0;

    [Header("Run")]
    [SerializeField] Button runButton;

    [Header("PlayerList")]
    [SerializeField] Transform scrollContent;
    [SerializeField] GameObject playerList;

    Hashtable temp = new Hashtable();

    private void Start()
    {
        playerInput = FindObjectOfType<PlayerInput>();

        if (SceneManager.GetActiveScene().name == "GameScene")
        {
            Debug.Log("GameScene");
            readyText.gameObject.SetActive(false);
        }
        else
        {
            if (PhotonNetwork.IsMasterClient)
                readyText.gameObject.SetActive(false);
            else
                readyText.gameObject.SetActive(true);
        }
        
        PlayerState();
    }

    public void RunButtonDown()
    {
        playerInput.run = true; // PlayerInput의 Update에서 매번 검사하지않아도되서 여기서 넣는게 나은거같기도,,
    }
    public void RunButtonUp()
    {
        playerInput.run = false;
    }
    public void AttackButtonDown()
    {
        playerInput.attack = true;
        Debug.Log("버튼Down 눌림");
    }
    public void AttackButtonUp()
    {
        playerInput.attack = false;
        Debug.Log("버튼Up 눌림");
    }
    public void ReadyButton()
    {
        if(PhotonNetwork.IsMasterClient) PhotonNetwork.LoadLevel("GameScene");

        ++readyButton;
        if (readyButton % 2 == 1)
        {
            readyText.text = "준비완료!";
            temp["준비완료"] = 1;
        }
        else
        {
            readyText.text = "준비하려면 눌러주세요!";
            temp["준비완료"] = 0;
        }
        PhotonNetwork.LocalPlayer.SetCustomProperties(temp);
    }
    void ReadyStatusRenew()
    {
        Debug.Log((string)PhotonNetwork.LocalPlayer.CustomProperties["닉네임"]);
        readyCnt = 0;
        Debug.Log("PlayerLength : " + PhotonNetwork.PlayerList.Length);
        for (int i = 0; i < PhotonNetwork.CurrentRoom.PlayerCount; i++)
        {
            readyCnt += (int)PhotonNetwork.PlayerList[i].CustomProperties["준비완료"];
        }
        Debug.Log("readyCnt : " + readyCnt);
        if (PhotonNetwork.IsMasterClient)
        {
            if ((readyCnt == PhotonNetwork.CurrentRoom.PlayerCount) && (PhotonNetwork.CurrentRoom.PlayerCount > 1)) // 방장의 레디카운트가 방장빼고 다른 플레어어 수와 같으면
            {
                readyText.gameObject.SetActive(true); // 방장의 준비버튼 활성화
                readyText.text = "게임을 시작하려면 눌러주세요!";

                if (SceneManager.GetActiveScene().name == "GameScene")
                    readyText.gameObject.SetActive(false);
            }
            else readyText.gameObject.SetActive(false);
        }
    }

    //[PunRPC]
    void PlayerState()
    {
        Transform[] childList = scrollContent.GetComponentsInChildren<Transform>();
        Debug.Log("자식오브젝트 " + childList.Length);
        if(childList != null)
        {
            for (int i = 1; i < childList.Length; i++)
            {
                if(childList[i] != scrollContent)
                {
                    Destroy(childList[i].gameObject);
                    Debug.Log("list 삭제");
                }
            }
        }

        for(int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            GameObject list = Instantiate(playerList, scrollContent);
            Debug.Log("list 생성");
            Text playerName = list.transform.GetChild(0).GetComponent<Text>();
            Text ready = list.transform.GetChild(1).GetComponent<Text>();

            playerName.text = (string)PhotonNetwork.PlayerList[i].CustomProperties["닉네임"];
            Debug.Log((string)PhotonNetwork.PlayerList[i].CustomProperties["닉네임"]);
            
            bool isReady1 = 1 == (int)PhotonNetwork.PlayerList[i].CustomProperties["준비완료"];
            ready.text = isReady1 ? "Ready" : "";
        }
    }

    public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, Hashtable changedProps) // 프로퍼티 변경되면 자동으로 호출됨.
    {
        Debug.Log("플레이어 상태 업데이트");
        PlayerState();
        ReadyStatusRenew();
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        Debug.Log("플레이어 입장");
        PlayerState();
        ReadyStatusRenew();
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        Debug.Log("플레이어 퇴장");
        PlayerState();
        ReadyStatusRenew();
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        Debug.Log("방에서 나가서 로비로 옴1");
    }

    public override void OnLeftRoom() //  PhotonNetwork.LeaveRoom()이 불리면 자동으로 불림
    {
        SceneManager.LoadScene("Lobby");
        Debug.Log("방에서 나가서 로비로 옴2");
    }
}
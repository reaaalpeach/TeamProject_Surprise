using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using UnityEngine.SceneManagement;

public class KilllogManager : MonoBehaviourPunCallbacks
{
    PlayerInput playerInput;

    [Header("Run")]
    [SerializeField] Button runButton;

    [Header("KillLog_List")]
    [SerializeField] Transform scrollContent1;
    [SerializeField] GameObject killLog;

    private void Start()
    {
        playerInput = FindObjectOfType<PlayerInput>();
    }

    public void RunButtonDown()
    {
        playerInput.run = true; 
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

    public void KillLog()
    {
        GameObject list = Instantiate(killLog, scrollContent1);
        Debug.Log("list 생성");
        Text attackplayerName = list.transform.GetChild(0).GetComponent<Text>();
        Text diePlayerName = list.transform.GetChild(1).GetComponent<Text>();

        attackplayerName.text = (string)PhotonNetwork.LocalPlayer.CustomProperties["공격"];
        diePlayerName.text = (string)PhotonNetwork.LocalPlayer.CustomProperties["죽음"];

        if (attackplayerName.text == "" || diePlayerName.text == "") // 두 번씩 생성되는 경우는 리스트 삭제
        {
            Destroy(list.gameObject);
            Debug.Log("list 삭제");
        }

        PhotonNetwork.LocalPlayer.CustomProperties.Remove("공격"); // 다시 리셋
        PhotonNetwork.LocalPlayer.CustomProperties.Remove("죽음");
    }

    public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, Hashtable changedProps) // 프로퍼티 변경되면 자동으로 호출됨.
    {
        if (SceneManager.GetActiveScene().name == "GameScene")
        {
            Debug.Log("플레이어 킬로그 업데이트");
            KillLog();
        }
    }
}

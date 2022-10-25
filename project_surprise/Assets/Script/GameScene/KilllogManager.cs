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
        Debug.Log("��ưDown ����");
    }
    public void AttackButtonUp()
    {
        playerInput.attack = false;
        Debug.Log("��ưUp ����");
    }

    public void KillLog()
    {
        GameObject list = Instantiate(killLog, scrollContent1);
        Debug.Log("list ����");
        Text attackplayerName = list.transform.GetChild(0).GetComponent<Text>();
        Text diePlayerName = list.transform.GetChild(1).GetComponent<Text>();

        attackplayerName.text = (string)PhotonNetwork.LocalPlayer.CustomProperties["����"];
        diePlayerName.text = (string)PhotonNetwork.LocalPlayer.CustomProperties["����"];

        if (attackplayerName.text == "" || diePlayerName.text == "") // �� ���� �����Ǵ� ���� ����Ʈ ����
        {
            Destroy(list.gameObject);
            Debug.Log("list ����");
        }

        PhotonNetwork.LocalPlayer.CustomProperties.Remove("����"); // �ٽ� ����
        PhotonNetwork.LocalPlayer.CustomProperties.Remove("����");
    }

    public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, Hashtable changedProps) // ������Ƽ ����Ǹ� �ڵ����� ȣ���.
    {
        if (SceneManager.GetActiveScene().name == "GameScene")
        {
            Debug.Log("�÷��̾� ų�α� ������Ʈ");
            KillLog();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable; // ����Ƽ���� �����ϴ� hashtable�� ��ġ�� ������ �ʿ�!


public class CameraSetup : MonoBehaviourPunCallbacks
{
    List<GameObject> onPlayerList = new List<GameObject>();
    void Start()
    {
        if(photonView.IsMine)
        {
            CinemachineFreeLook cam = FindObjectOfType<CinemachineFreeLook>();
            cam.Follow = transform;
            cam.LookAt = transform;
        }
    }

    public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, Hashtable changedProps)
    {
        
    }
}

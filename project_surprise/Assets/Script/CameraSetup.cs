using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable; // 유니티에서 제공하는 hashtable과 겹치기 때문에 필요!


public class CameraSetup : MonoBehaviourPunCallbacks
{
    List<GameObject> playerList = new List<GameObject>();
    void Start()
    {
        if(photonView.IsMine)
        {
            CinemachineFreeLook cam = FindObjectOfType<CinemachineFreeLook>();
            cam.Follow = transform;
            cam.LookAt = transform;
        }

        GameObject[] tmpList = GameObject.FindGameObjectsWithTag("Player");
        for(int i = 0; i < tmpList.Length; i++)
        {
            playerList.Add(tmpList[i]);
        }
    }

    public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, Hashtable changedProps)
    {
        
    }
}

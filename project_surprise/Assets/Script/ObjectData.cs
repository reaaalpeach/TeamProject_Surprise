using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class ObjectData : MonoBehaviourPun
{
    PhotonView pv;
    string objectName;
    void Start()
    {
        if (PhotonNetwork.IsConnected)
            pv.RPC("GetName", RpcTarget.All);

    }

    public string GetObjectName()
    {
        return objectName;
    }

    [PunRPC]
    void GetName()
    {
        if (photonView.IsMine)
            objectName = PhotonNetwork.LocalPlayer.NickName;
        else
            objectName = pv.Owner.NickName;
    }
}

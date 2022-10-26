using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class GamePlayManager : MonoBehaviourPunCallbacks
{
    public Transform[] startPos;
    //public Camera mainCam;

    [SerializeField] Transform leftUp;
    [SerializeField] Transform rightDown;

    void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;

        CreatePlayer();
    }

    void CreatePlayer()
    {
        if (PlayerInfo.instance.GetCurrentPlayer().name == "°¨ÀÚ")
        {
            if(SceneManager.GetActiveScene().name == "Room")
            {
                PhotonNetwork.Instantiate("Potato", startPos[Random.Range(0, startPos.Length)].position, Quaternion.Euler(new Vector3(0, 0, 0)));
            }
            else if(SceneManager.GetActiveScene().name == "GameScene")
            {
                float randX = Random.Range((float)leftUp.position.x, (float)rightDown.position.x);
                float randZ = Random.Range((float)rightDown.position.z, (float)leftUp.position.z);
                Vector3 randPos = new Vector3(randX, 0, randZ);
                Vector3 randRot = new Vector3(0f, Random.Range(0, 360f), 0f);

                PhotonNetwork.Instantiate("Potato", randPos, Quaternion.Euler(randRot));
            }

        }
        else
        {
            if(SceneManager.GetActiveScene().name == "Room")
            {
                PhotonNetwork.Instantiate("SweetPotato", startPos[Random.Range(0, startPos.Length)].position, Quaternion.Euler(new Vector3(0, 0, 0)));
            }
            else if (SceneManager.GetActiveScene().name == "GameScene")
            {
                float randX = Random.Range((float)leftUp.position.x, (float)rightDown.position.x);
                float randZ = Random.Range((float)rightDown.position.z, (float)leftUp.position.z);
                Vector3 randPos = new Vector3(randX, 0, randZ);
                Vector3 randRot = new Vector3(0f, Random.Range(0, 360f), 0f);

                PhotonNetwork.Instantiate("SweetPotato", randPos, Quaternion.Euler(randRot));
            }
        }
    }

}

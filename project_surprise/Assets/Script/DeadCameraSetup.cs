using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Photon.Pun;

public class DeadCameraSetup : MonoBehaviourPunCallbacks
{
    [SerializeField] List<GameObject> playerList = new List<GameObject>();
    [SerializeField] CinemachineFreeLook cam;

    private void OnEnable()
    {
        cam = FindObjectOfType<CinemachineFreeLook>();

        GameObject[] tmpList = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < tmpList.Length; i++)
        {
            playerList.Add(tmpList[i]);
        }
    }

    private void Start()
    {
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

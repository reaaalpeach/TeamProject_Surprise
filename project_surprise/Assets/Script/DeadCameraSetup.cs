using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using Photon.Pun;

public class DeadCameraSetup : MonoBehaviourPunCallbacks
{
    [SerializeField] List<GameObject> playerList = new List<GameObject>();
    [SerializeField] GameObject nextBtnObj;//카메라를 다른 캐릭터로 바꿀때 쓸 버튼 오브젝트
    [SerializeField] CinemachineFreeLook cam;
    int playerlistId;

    private void OnEnable()//PlayerController.cs에서 활성화 시켜줄것.
    {
        cam = FindObjectOfType<CinemachineFreeLook>();
        nextBtnObj.SetActive(true);

        GameObject[] tmpList = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < tmpList.Length; i++)
        {
            playerList.Add(tmpList[i]);
        }
    }

    private void Start()
    {
        nextBtnObj.SetActive(false);//처음에는 버튼 비활성화
        gameObject.SetActive(false);

        playerlistId = 0;
    }

    public void FindOnPlayer()
    {
        //playerList.RemoveAll(player => player == null);//죽으면 사라지는 플레이어를 리스트에서 삭제

        while(playerList[playerlistId] == null)//혹시나 죽었을 때 오브젝트가 리스트에서 삭제되는 것이 느릴 때를 방지한 코드
        {
            playerlistId++;
            if (playerList.Count <= playerlistId)
            {
                playerlistId = 0;
            }
        }
        Debug.Log("List : " + playerlistId);
        cam.Follow = playerList[playerlistId].transform;
        cam.LookAt = playerList[playerlistId].transform;
        playerlistId++;
        if (playerList.Count <= playerlistId)
        {
            playerlistId = 0;
        }
    }
}

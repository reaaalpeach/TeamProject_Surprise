using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using Photon.Pun;

public class DeadCameraSetup : MonoBehaviourPunCallbacks
{
    [SerializeField] List<GameObject> playerList = new List<GameObject>();
    [SerializeField] GameObject nextBtnObj;//ī�޶� �ٸ� ĳ���ͷ� �ٲܶ� �� ��ư ������Ʈ
    [SerializeField] CinemachineFreeLook cam;
    int playerlistId;

    private void OnEnable()//PlayerController.cs���� Ȱ��ȭ �����ٰ�.
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
        nextBtnObj.SetActive(false);//ó������ ��ư ��Ȱ��ȭ
        gameObject.SetActive(false);

        playerlistId = 0;
    }

    public void FindOnPlayer()
    {
        //playerList.RemoveAll(player => player == null);//������ ������� �÷��̾ ����Ʈ���� ����

        while(playerList[playerlistId] == null)//Ȥ�ó� �׾��� �� ������Ʈ�� ����Ʈ���� �����Ǵ� ���� ���� ���� ������ �ڵ�
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    PlayerInput playerInput;
    public Text readyText;
    int readyButton = 0;

    void Start()
    {
        playerInput = FindObjectOfType<PlayerInput>();
    }
    public void RunButtonDown()
    {
        playerInput.run = true; // PlayerInput�� Update���� �Ź� �˻������ʾƵ��Ǽ� ���⼭ �ִ°� �����Ű��⵵,,
        Debug.Log("��ưDown ����");
    }
    public void RunButtonUp()
    {
        playerInput.run = false;
        Debug.Log("��ưUp ����");
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

    public void ReadyButton()
    {
        readyButton++;

        if(readyButton % 2 == 1)
        {
            playerInput.ready = true;
            readyText.text = "�غ�";
            Debug.Log(playerInput.ready);
        }
        else
        {
            playerInput.ready = false;
            readyText.text = "";
            Debug.Log(playerInput.ready);
        }
    }
}
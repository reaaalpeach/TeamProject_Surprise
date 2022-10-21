using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class AIMovement : MonoBehaviour
{
    //State
    enum State { Idle, walk };

    //Walk
    [Header("Walk")]

    [Tooltip("�ȴ� �ӵ�")]
    [SerializeField] float walkSpeed;

    [Tooltip("�ȴ��� �Ǵ�")]
    [SerializeField] bool isWalking;

    [Tooltip("ȸ���ؾ� �ϴ� ����")]
    Vector3 rotDir;

    //Time
    [Tooltip("��� Ȥ�� �ȱ� aciton�� �󸶳� �� ���ΰ�. 0.5��~6�� ����")]
    float actionTime_Rand_Dot5to6
    {
        get
        {
            return UnityEngine.Random.Range(0.5f, 6f);
        }
    }
    [Tooltip("�ൿ�� ����ð�. actionTime�� �޾Ƽ� Time.deltaTime�� 0�� �ɶ����� ����")]
    [SerializeField] float currentTime;

    //component
    Rigidbody rigid;
    Animator animator;
    Collider col;
    PhotonView pv;

    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        currentTime = actionTime_Rand_Dot5to6;
    }

    // Update is called once per frame
    void Update()
    {
        TimeLapse();
        Move();
    }


    #region �ൿ�� �����ϰ� ���� ������
    void TimeLapse()
    {
        currentTime -= Time.deltaTime;
        if(currentTime <= 0)
        {
            ActionReset();
        }
    }

    void ActionReset()
    {
        isWalking = false;
        animator.SetBool("Walk", isWalking);

        rotDir.Set(0f, transform.rotation.eulerAngles.y + UnityEngine.Random.Range(-180f, 180f), 0f);
        ChoiceNextRandomAction();
    }

    private void ChoiceNextRandomAction()
    {
        int randNum = UnityEngine.Random.Range(0, 2); //0 ���, 1 �ȱ�

        if (randNum == (int)State.Idle)
        {
            Idle();
        }
        else if(randNum == (int)State.walk)
        {
            Walk();
        }
    }

    private void Idle()
    {
        currentTime = actionTime_Rand_Dot5to6;
        animator.SetBool("Walk", isWalking);
    }

    private void Walk()
    {
        currentTime = actionTime_Rand_Dot5to6;

        isWalking = true;
        animator.SetBool("Walk", isWalking);
    }
    #endregion

    #region �������� ������
    void Move()
    {
        if(isWalking)
        {
            rigid.MovePosition(rigid.position + transform.forward * walkSpeed * Time.deltaTime);//�����θ� ����

            Vector3 smoothRot = Vector3.Lerp(transform.eulerAngles, rotDir, 0.01f);
            rigid.MoveRotation(Quaternion.Euler(smoothRot));
        }
    }
    #endregion

    void Check_Die()
    {
        StartCoroutine("Die");
    }

    IEnumerator Die()
    {
        animator.SetTrigger("Die");

        yield return new WaitForSeconds(1.5f);
        PhotonNetwork.Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Attack"))
        {
            StartCoroutine("Die");
        }
    }
}

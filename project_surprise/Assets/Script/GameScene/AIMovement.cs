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

    [Tooltip("걷는 속도")]
    [SerializeField] float walkSpeed;

    [Tooltip("걷는지 판단")]
    [SerializeField] bool isWalking;

    [Tooltip("회전해야 하는 방향")]
    Vector3 rotDir;

    //Time
    [Tooltip("대기 혹은 걷기 aciton을 얼마나 할 것인가. 0.5초~6초 랜덤")]
    float actionTime_Rand_Dot5to6
    {
        get
        {
            return UnityEngine.Random.Range(0.5f, 6f);
        }
    }
    [Tooltip("행동의 경과시간. actionTime을 받아서 Time.deltaTime을 0이 될때까지 뺀다")]
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


    #region 행동을 선택하고 값을 지정함
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
        int randNum = UnityEngine.Random.Range(0, 2); //0 대기, 1 걷기

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

    #region 실질적인 움직임
    void Move()
    {
        if(isWalking)
        {
            rigid.MovePosition(rigid.position + transform.forward * walkSpeed * Time.deltaTime);//앞으로만 전진

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

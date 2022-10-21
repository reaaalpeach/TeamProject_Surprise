using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpawnAI : MonoBehaviour
{
    [Header("������ġ ����")]//XZ��ǥ�����
    [SerializeField] Transform leftUp;
    [SerializeField] Transform rightDown;

    [Header("AI ������Ʈ")]
    [Tooltip("������ �� ��")]
    [SerializeField] int enemyNum;

    // Start is called before the first frame update
    void Start()
    {
        if(PhotonNetwork.IsMasterClient)
        {
            for(int i = 0; i < enemyNum; i++)
            {
                float randX = Random.Range((float)leftUp.position.x, (float)rightDown.position.x);
                float randZ = Random.Range((float)rightDown.position.z, (float)leftUp.position.z);
                Vector3 randPos = new Vector3(randX, 0, randZ);
                Vector3 randRot = new Vector3(0f, Random.Range(0, 360f), 0f);
                int random = Random.Range(0, 2);
                if (random == 0)
                    PhotonNetwork.Instantiate("PotatoAI", randPos, Quaternion.Euler(randRot));
                else PhotonNetwork.Instantiate("SweetPotatoAI", randPos, Quaternion.Euler(randRot));
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

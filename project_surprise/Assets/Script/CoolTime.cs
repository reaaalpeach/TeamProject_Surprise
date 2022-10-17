using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoolTime : MonoBehaviour
{
    float coolTime = 3;
    Slider slider;

    Button button;

    //time
    float current;
    float percent;

    private void Awake()
    {
        button = transform.parent.GetComponent<Button>();//�θ������Ʈ�� ��ư

        slider = GetComponentInChildren<Slider>();
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        slider.maxValue = 1;
        button.interactable = false;//��Ÿ���̹Ƿ� ��ư ��Ȱ��ȭ
    }

    public void SetCoolTime(float time)
    {
        coolTime = time;
    }

    private void Update()
    {
        current += Time.deltaTime;
        if(percent < 1)
        {
            percent = current / coolTime;
            slider.value = percent;
            //�����̴��� maxValue���� 1�̹Ƿ�
            //coolTime ���� �پ��ص� �ϰ��ǰ� ����� �� ����

        }
        else
        {
            current = 0;
            percent = 0;
            button.interactable = true;//��Ÿ�� ���ķ� ��ư Ȱ��ȭ
            gameObject.SetActive(false);
        }
    }
}

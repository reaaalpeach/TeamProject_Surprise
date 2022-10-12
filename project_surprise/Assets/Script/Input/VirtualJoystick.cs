using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems; // Ű����, ���콺, ��ġ�� �̺�Ʈ�� ������Ʈ�� ���� �� �ִ� ��� ����

public class VirtualJoystick : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField]
    private RectTransform lever;
    private RectTransform rectTransform; // ���̽�ƽ�� RectTransform

    [SerializeField, Range(10, 150)] // Range()�� ���� �ȿ����� ���� ������ �����ϰ�!
    private float leverRange;

    private Vector2 inputDirection;
    private bool isInput;

    private PlayerInput input;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        input = FindObjectOfType<PlayerInput>();
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        ControlJoystickLever(eventData);
        isInput = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        ControlJoystickLever(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        lever.anchoredPosition = Vector2.zero; //���� ���� ���̽�ƽ�� �߽����� ���ư���
        isInput = false;
        input.Movement(Vector2.zero);
    }

    private void ControlJoystickLever(PointerEventData eventData)
    {
        var inputPos = eventData.position - rectTransform.anchoredPosition;
        var inputVector = inputPos.magnitude < leverRange ? inputPos : inputPos.normalized * leverRange;
        lever.anchoredPosition = inputVector; // inputVector : �ػ󵵸� ������� ������� ���̶� ĳ���Ϳ����ӿ� ���� �ʹ� ū ���̶� �ʹ� ������ ������ ����
        inputDirection = -1*(inputVector / leverRange); // ����, 0~1������ ����ȭ�� ���� ĳ���� ���������� �����ϱ�����
        // ĳ������ �������� ���̽�ƽ�� ���� ����� �ݴ�εǼ� -1����
    }

    private void InputControlVector() // ĳ���Ϳ��� �Էº��� ����
    {
        input.Movement(inputDirection);
    }

    void Update()
    {
        if (isInput)
        {
            InputControlVector();
        }
    }
}
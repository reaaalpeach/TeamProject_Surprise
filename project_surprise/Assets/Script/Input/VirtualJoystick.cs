using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems; // Ű����, ���콺, ��ġ�� �̺�Ʈ�� ������Ʈ�� ���� �� �ִ� ��� ����

public class VirtualJoystick : MonoBehaviour, IDragHandler, IEndDragHandler
{
    [SerializeField]
    private RectTransform lever;
    private RectTransform rectTransform; // ���̽�ƽ�� RectTransform

    [HideInInspector]
    public Vector3 Dir { get; private set; }

    [SerializeField, Range(10, 150)] // Range()�� ���� �ȿ����� ���� ������ �����ϰ�!
    private float leverRange;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void OnDrag(PointerEventData eventData)
    {
        lever.position = eventData.position;
        lever.localPosition = Vector3.ClampMagnitude(eventData.position - (Vector2)transform.position, rectTransform.rect.width * 0.5f);

        Dir = new Vector3(lever.localPosition.x, 0, lever.localPosition.y).normalized;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        lever.localPosition = Vector3.zero;
        Dir = Vector3.zero;
    }
}
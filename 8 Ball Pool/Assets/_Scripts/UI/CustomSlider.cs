using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class CustomSlider : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private RectTransform cueTip;
    [SerializeField] private RectTransform rTrans;
    private bool isDragging;
    private Vector3 delta;
    private float cueMoveArea;
    private Vector3 startPos;

    public float Value;


    private void Awake()
    {
        rTrans = GetComponent<RectTransform>();
        startPos = cueTip.localPosition;
        cueMoveArea = rTrans.rect.height;
    }

    private void OnEnable()
    {
        cueTip.localPosition = startPos;
        Value = 0;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;
        delta = Vector3.zero;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isDragging)
        {
            delta.y = eventData.delta.y;
            cueTip.localPosition = ClampPositionToParent(cueTip.localPosition += delta);
            Value = Mathf.Clamp01(Mathf.Abs(cueTip.localPosition.y - cueMoveArea / 2) / cueMoveArea);
        }
    }

    private Vector3 ClampPositionToParent(Vector3 cueTipLocalPosition)
    {
        float minY = -rTrans.rect.height * 0.5f;
        float maxY = rTrans.rect.height * 0.5f;

        float clampedY = Mathf.Clamp(cueTipLocalPosition.y, minY, maxY);

        return new Vector2(cueTipLocalPosition.x, clampedY);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchmakingSlot : MonoBehaviour
{
    [SerializeField] private float scrollSpeed = 1.0f;
    [SerializeField] private RectTransform startImage;

    //References
    private RectTransform _contentRect;
    private VerticalLayoutGroup _verticalLayoutGroup;

    private float _distanceForLooping;
    private Vector2 _startPosition;

    private void Start()
    {
        _contentRect = GetComponent<RectTransform>();
        _verticalLayoutGroup = GetComponent<VerticalLayoutGroup>();

        _distanceForLooping = CalculateDistanceForLooping();
        _startPosition = _contentRect.anchoredPosition;

        Debug.Log(_distanceForLooping);
    }

    private float CalculateDistanceForLooping()
    {
        int childCount = transform.childCount;
        float height = 0;

        for (int i = 0; i < childCount - 1; i++)
        {
            RectTransform child = transform.GetChild(i) as RectTransform;
            height += child.rect.height + Mathf.Abs(_verticalLayoutGroup.spacing);
        }

        return height;
    }

    private void Update()
    {
        _contentRect.anchoredPosition -= Time.deltaTime * new Vector2(0, scrollSpeed);

        if (Mathf.Abs(_contentRect.anchoredPosition.y - _startPosition.y) >= _distanceForLooping)
        {
            _contentRect.anchoredPosition = _startPosition;
        }
    }
}
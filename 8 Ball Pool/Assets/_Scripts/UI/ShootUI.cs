using UnityEngine;

public class ShootUI : MonoBehaviour

{
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] float slideDuration = 0.5f;
    [SerializeField] float fadeDuration = 0.5f;
    [SerializeField] LeanTweenType slideEaseType = LeanTweenType.easeOutQuint;
    [SerializeField] LeanTweenType fadeEaseType = LeanTweenType.linear;

    private void OnEnable()
    {
        canvasGroup.alpha = 0f;

        LeanTween.moveX(rectTransform, -875, slideDuration).setEase(slideEaseType);

        LeanTween.alphaCanvas(canvasGroup, 1f, fadeDuration).setEase(fadeEaseType);
    }

    private void OnDisable()
    {
        LeanTween.moveX(rectTransform, Screen.width, slideDuration).setEase(slideEaseType);

        LeanTween.alphaCanvas(canvasGroup, 0f, fadeDuration).setEase(fadeEaseType);
    }
}
using UnityEngine;

namespace _Scripts.TestSubjects.CoreGameTest.Player
{
    public class StartArea : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private SpriteRenderer spriteRenderer;

        [Header("Settings")]
        [SerializeField] private float fadeInDuration = 1.0f;
        [SerializeField] private float fadeOutDuration = 1.0f;
        [SerializeField] private float fadeValue = 0.3f;

        private Color originalColor;

        private void OnEnable()
        {
            originalColor = spriteRenderer.color;

            spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);

            LeanTween.alpha(spriteRenderer.gameObject, 1, fadeInDuration)
                .setEase(LeanTweenType.easeInOutSine)
                .setOnComplete(() =>
                {
                    LeanTween.alpha(spriteRenderer.gameObject, fadeValue, fadeOutDuration)
                        .setEase(LeanTweenType.easeInOutSine)
                        .setOnComplete(() => { Debug.Log("Fade-out complete"); });
                });
        }
    }
}
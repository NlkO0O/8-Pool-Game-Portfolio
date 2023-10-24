using UnityEngine;

namespace _Scripts.UI
{
    public class HandHint : MonoBehaviour
    {
        [SerializeField] private Transform whiteBall;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private float fadeInDuration = 1.0f;
        [SerializeField] private float finalAlpha = 1.0f;

        void OnEnable()
        {
            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
            }

            Color initialColor = spriteRenderer.color;
            initialColor.a = 0.0f;
            spriteRenderer.color = initialColor;

            LeanTween.alpha(spriteRenderer.gameObject, finalAlpha, fadeInDuration)
                .setEase(LeanTweenType.easeInQuad);
        }

        private void LateUpdate()
        {
            transform.position = whiteBall.position;
        }
    }
}
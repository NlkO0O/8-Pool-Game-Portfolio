using UnityEngine;

namespace _Scripts.UI
{
    public class BannerScale : MonoBehaviour
    {
        [SerializeField] private bool staysActive;
        [SerializeField] private float scaleFactor = 1f;
        [SerializeField] private float delay = 1f;

        private void OnEnable()
        {
            LTDescr scaleTween = LeanTween.scale(gameObject, new Vector3(scaleFactor, scaleFactor, 1f), 0.5f)
                .setEase(LeanTweenType.easeOutElastic)
                .setOnComplete(Delay);
        }

        private void Delay()
        {
            if (staysActive) return;

            Invoke(nameof(DeactivateObject), delay);
        }

        private void DeactivateObject()
        {
            gameObject.SetActive(false);
        }
    }
}
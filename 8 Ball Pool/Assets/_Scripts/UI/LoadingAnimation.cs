using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LoadingAnimation : MonoBehaviour
{
    public TextMeshProUGUI textMeshProText;
    public float minScale = 0.8f;
    public float maxScale = 1.2f;
    public float duration = 1.0f; 

    private void Start()
    {
        ScaleText();
    }

    private void ScaleText()
    {
        LeanTween.scale(textMeshProText.gameObject, new Vector3(maxScale, maxScale, 1f), duration)
            .setEase(LeanTweenType.easeInOutQuad)
            .setOnComplete(() =>
            {
                LeanTween.scale(textMeshProText.gameObject, new Vector3(minScale, minScale, 1f), duration)
                    .setEase(LeanTweenType.easeInOutQuad)
                    .setOnComplete(ScaleText);
            });
    }
}

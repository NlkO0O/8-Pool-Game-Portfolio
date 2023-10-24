using _Scripts.Game.CoreGame;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class SlotUI : NetworkBehaviour
{
    [SerializeField] private Sprite solidBalls;
    [SerializeField] private Sprite stripeBalls;

    public int Number;

    public Image image;
    public float fadeDuration = 1.0f;

    public void SetUp(BallType ballType)
    {
        image.sprite = ballType switch
        {
            BallType.Stripe => stripeBalls,
            BallType.Solid => solidBalls,
            _ => null
        };

        if (ballType == BallType.Stripe) Number += 8;
    }

    public void DeactivatedPottedSlot()
    {
        Color startColor = image.color;
        startColor.a = 1.0f;
        image.color = startColor;


        LeanTween.value(gameObject, UpdateImageAlpha, 1.0f, 0.0f, fadeDuration)
            .setOnUpdate((alpha) =>
            {
                Color currentColor = image.color;
                currentColor.a = alpha;
                image.color = currentColor;
            })
            .setOnComplete(() => { image.enabled = false; });
    }

    private void UpdateImageAlpha(float alpha)
    {
        Color currentColor = image.color;
        currentColor.a = alpha;
        image.color = currentColor;
    }
}
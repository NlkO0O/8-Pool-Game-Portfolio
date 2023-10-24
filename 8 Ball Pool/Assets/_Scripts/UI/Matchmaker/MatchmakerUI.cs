using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts.Networking.Client;
using Unity.VisualScripting;
using UnityEngine;

public class MatchmakerUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform elementLeft;
    [SerializeField] private RectTransform elementRight;
    [SerializeField] private RectTransform elementTop;
    [SerializeField] private RectTransform elementBot;

    [Header("Settings")]
    [SerializeField] float animationDuration = 1.0f;
    [SerializeField] float delayBetweenAnimations = 0.5f;
    [SerializeField] float middleYPosition = 0f;
    [SerializeField] private float topXPosition = 0f;
    [SerializeField] private float rightXPosition = 200f;
    [SerializeField] private float leftXPosition = -200f;
    [SerializeField] private float botYPosition = -10;

    private Vector2 elementLeftStartPosition;
    private Vector2 elementRightStartPosition;
    private Vector2 elementTopStartPosition;
    private Vector2 elementBotStartPosition;

    private void OnEnable()
    {
        Debug.Log("enabled");

        elementLeftStartPosition = elementLeft.anchoredPosition;
        elementRightStartPosition = elementRight.anchoredPosition;
        elementTopStartPosition = elementTop.anchoredPosition;
        elementBotStartPosition = elementBot.anchoredPosition;

        AnimateElement(elementLeft, new Vector3(leftXPosition, middleYPosition, 0f));
        AnimateElement(elementRight, new Vector3(rightXPosition, middleYPosition, 0f));
        AnimateElement(elementTop, new Vector3(topXPosition, middleYPosition, 0f));
        AnimateElement(elementBot, new Vector3(0, botYPosition, 0));
    }

    private void AnimateElement(RectTransform element, Vector3 targetPosition)
    {
        LeanTween.move(element, targetPosition, animationDuration)
            .setEaseOutBack()
            .setDelay(delayBetweenAnimations);

        LeanTween.alphaCanvas(element.GetComponent<CanvasGroup>(), 1f, animationDuration)
            .setDelay(delayBetweenAnimations);
    }

    private void OnDisable()
    {
        elementLeft.anchoredPosition = elementLeftStartPosition;
        elementRight.anchoredPosition = elementRightStartPosition;
        elementTop.anchoredPosition = elementTopStartPosition;
        elementBot.anchoredPosition = elementBotStartPosition;
    }
}
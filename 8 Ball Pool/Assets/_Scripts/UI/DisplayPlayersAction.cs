using System.Collections;
using System.Collections.Generic;
using _Scripts.UI.Matchmaker;
using TMPro;
using Unity.Netcode;
using UnityEngine;

namespace _Scripts.UI
{
    public class DisplayPlayersAction : NetworkBehaviour
    {
        [Header("References")]
        [SerializeField] private TextMeshProUGUI actionText;
        [SerializeField] private RectTransform rectTransform;
        [Header("Settings")]
        [SerializeField] private Vector3 endPoint;
        [SerializeField] private float popUpTime = 1;

        private Vector3 startPoint;
        private string opponentName;
        private Queue<DisplayObject> messagesQueue;
        private bool isProcessing;
        private Coroutine currentCoroutine;


        public void Start()
        {
            gameObject.SetActive(IsLocalPlayer);

            if (rectTransform == null)
            {
                rectTransform = GetComponent<RectTransform>();
            }

            startPoint = rectTransform.anchoredPosition;
            messagesQueue = new Queue<DisplayObject>();
        }

        [ClientRpc]
        public void UpdateActionTextClientRpc(MessageType messageType, string playerName)
        {
            if (!gameObject.activeSelf) return;

            messagesQueue.Enqueue(new DisplayObject(messageType, playerName));

            if (isProcessing || messagesQueue.Count == 0) return;
            
            currentCoroutine = StartCoroutine(ProcessMessages());
        }

        private IEnumerator ProcessMessages()
        {
            isProcessing = true;

            var currentMessage = messagesQueue.Dequeue();
            actionText.text = currentMessage.DisplayText();

            float t = 0f;

            while (t < popUpTime)
            {
                t += Time.deltaTime;
                rectTransform.anchoredPosition = Vector3.Lerp(startPoint, endPoint, t / popUpTime);
                yield return null;
            }

            yield return new WaitForSecondsRealtime(1f);

            t = 0;

            while (t < popUpTime)
            {
                t += Time.deltaTime;
                rectTransform.anchoredPosition = Vector3.Lerp(endPoint, startPoint, t / popUpTime);
                yield return null;
            }

            isProcessing = false;

            StopCoroutine(currentCoroutine);

            if (messagesQueue.Count > 0) StartCoroutine(ProcessMessages());
        }
    }

    public enum MessageType
    {
        PottedCueBall,
        Breaking,
        PlayersTurn,
        IllegalShot,
        TimedOut
    }
}
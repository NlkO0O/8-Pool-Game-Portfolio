using System;
using System.Linq;
using _Scripts.Game.CoreGame;
using _Scripts.UI;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.TestSubjects.CoreGameTest.Player
{
    public class PoolPlayerTest : NetworkBehaviour
    {
        [Header("References")]
        [SerializeField] private DisplayPlayersAction displayPlayersAction;
        [SerializeField] private RectTransform playerHud;
        [SerializeField] private Image avatar;
        [SerializeField] private TextMeshProUGUI nameTag;
        [SerializeField] private Image timer;
        [SerializeField] private RectTransform pottedBallsUI;
        [SerializeField] private SlotUI[] ballsUI;
        [SerializeField] private GameObject solidsBanner;
        [SerializeField] private GameObject stripesBanner;
        [SerializeField] private GameObject winBanner;
        [SerializeField] private GameObject loseBanner;

        [Header("Settings")]
        [SerializeField] private float maxTime = 30;

        public NetworkVariable<BallType> Balltype = new NetworkVariable<BallType>();
        public NetworkVariable<float> TimeRemaining = new NetworkVariable<float>();

        public string playerName;

        public int playersPottedBallsCount = 0;

        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                TimeRemaining.Value = maxTime;
                if (OwnerClientId == 0)
                {
                    playerName = "0 Niko";
                }
                else
                {
                    playerName = "1 Ana";
                }
            }

            if (IsClient)
            {
                TimeRemaining.OnValueChanged += HandleTimeRemaining;
                Balltype.OnValueChanged += HandleValueChanged;
            }

            if (IsLocalPlayer) return;

            var playerHudPos = playerHud.localPosition;
            playerHudPos.x *= -1;
            playerHud.localPosition = playerHudPos;

            var nameTagPos = nameTag.rectTransform.localPosition;
            nameTagPos.x *= -1;
            nameTag.rectTransform.localPosition = nameTagPos;

            var pottedBallsPos = pottedBallsUI.GetComponent<RectTransform>().localPosition;
            pottedBallsPos.x *= -1;
            pottedBallsUI.GetComponent<RectTransform>().localPosition = pottedBallsPos;

            nameTag.alignment = TextAlignmentOptions.Left;
        }

        private void HandleValueChanged(BallType oldType, BallType newType)
        {
            FillUpPottedBallsUI(newType);

            if (!IsOwner) return;

            switch (newType)
            {
                case BallType.Stripe:
                    stripesBanner.SetActive(true);
                    break;
                case BallType.Solid:
                    solidsBanner.SetActive(true);
                    break;
            }
        }

        private void HandleTimeRemaining(float previousValue, float newValue)
        {
            timer.fillAmount = newValue / maxTime;
        }

        private void FillUpPottedBallsUI(BallType ballType)
        {
            foreach (var slot in ballsUI)
            {
                slot.SetUp(ballType);
            }
        }

        [ClientRpc]
        public void HandleBallPottedClientRpc(int number)
        {
            try
            {
                if (number >= 9)
                {
                    ballsUI[number - 9].DeactivatedPottedSlot();
                }
                else
                {
                    ballsUI[number - 1].DeactivatedPottedSlot();
                }
            }
            catch (IndexOutOfRangeException e)
            {
                Debug.Log(e.Message);
            }
        }

        [ClientRpc]
        public void HandleGameOverClientRpc(ulong clientId)
        {
            if (clientId == OwnerClientId)
            {
                winBanner.SetActive(IsOwner);
            }
            else
            {
                loseBanner.SetActive(IsOwner);
            }
        }

        public void DisplayAction(MessageType messageType, string userName)
        {
            displayPlayersAction.UpdateActionTextClientRpc(messageType, userName);
        }

        private void RearrangePottedBalls(SlotUI elementToMove)
        {
            if (!ballsUI.Contains(elementToMove))
            {
                Debug.Log("Element not found");
                return;
            }

            var index = Array.IndexOf(ballsUI, elementToMove);

            SlotUI[] rearrangedArray = new SlotUI[ballsUI.Length];

            for (int i = 0; i < index; i++)
            {
                rearrangedArray[i] = ballsUI[i];
            }

            for (int i = index; i < ballsUI.Length - 1; i++)
            {
                rearrangedArray[i] = ballsUI[i + 1];
            }

            rearrangedArray[ballsUI.Length - 1] = elementToMove;

            ballsUI = rearrangedArray;
        }
    }
}
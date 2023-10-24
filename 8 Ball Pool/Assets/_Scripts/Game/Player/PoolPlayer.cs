using _Scripts.Game.CoreGame;
using _Scripts.Networking.Host;
using _Scripts.Networking.Shared;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.Game.Player
{
    public class PoolPlayer : NetworkBehaviour
    {
        [SerializeField] private float maxTime = 30;

        public NetworkVariable<BallType> BallType = new NetworkVariable<BallType>();
        public NetworkVariable<float> TimeRemaining = new NetworkVariable<float>();
        public NetworkVariable<FixedString32Bytes> PlayerName = new NetworkVariable<FixedString32Bytes>();
        public PlayerType PlayerType;

        private GameObject myChart;
        private Image timerFill;
        private TextMeshProUGUI nameTag;
        private Image pottedBallsUI;

        public override void OnNetworkSpawn()
        {
            if (IsClient)
            {
                myChart = IsLocalPlayer
                    ? GameObject.FindGameObjectWithTag("Me")
                    : GameObject.FindGameObjectWithTag("Opponent");

                timerFill = myChart.transform.Find("Timer").GetComponent<Image>();
                nameTag = myChart.transform.Find("NameTag").GetComponent<TextMeshProUGUI>();
            }

            PlayerName.OnValueChanged += HandlePlayerNameChanged;
            TimeRemaining.OnValueChanged += HandleTimeRemaining;

            if (IsServer)
            {
                UserData userData =
                    HostSingleTon.Instance.GameManager.NetworkServer.GetUserDataByClientId(OwnerClientId);

                PlayerName.Value = userData.userName;
            }

            HandlePlayerNameChanged(string.Empty, PlayerName.Value);
        }

        private void HandleTimeRemaining(float previousValue, float newValue)
        {
            timerFill.fillAmount = newValue / maxTime;
        }

        private void HandlePlayerNameChanged(FixedString32Bytes previousValue, FixedString32Bytes newValue)
        {
            nameTag.text = newValue.ToString();
        }
    }

    public enum PlayerType
    {
        Player1,
        Player2
    }
}
using System.Collections;
using _Scripts.Game.CoreGame;
using _Scripts.Game.CoreGameSystems;
using _Scripts.Game.Player;
using Unity.Netcode;
using UnityEngine;

namespace _Scripts.Game.States
{
    public class SetUpTableState : GameState
    {
        public SetUpTableState(GameManager gameManager, PoolPlayer player) : base(gameManager, player)
        {
        }

        public override void Enter()
        {
            gameManager.StartCoroutine(Start(
                $"{currentPlayer.PlayerName.Value.ToString()}  is  {currentPlayer.PlayerType} and is breaking..."));
        }

        public override void Update()
        {
            currentPlayer.TimeRemaining.Value -= Time.deltaTime;
        }

        public override void Exit()
        {
            Debug.Log($"Exiting {this}");
        }

        private IEnumerator Start(string message)
        {
            yield return new WaitForSecondsRealtime(2f);

            GameHUD.Singleton.DisplayTextClientRpc(message);
            TableSet.Instance.SetUpTable();

            WhiteBall.Instance.GetComponent<NetworkObject>().ChangeOwnership(currentPlayer.OwnerClientId);
            WhiteBall.Instance.playerInControl = currentPlayer;

            CueController.Instance.GetComponent<NetworkObject>().ChangeOwnership(currentPlayer.OwnerClientId);
            CueController.Instance.ActivateCueClientRpc(true);

            GivePlayerHint(true, currentPlayer.OwnerClientId);

            WhiteBall.Instance.ShouldFreeze.Value = false;
            WhiteBall.Instance.HandleStateChanged(false, false);

            currentPlayer.TimeRemaining.Value = 30f;
        }

        private void GivePlayerHint(bool state, ulong clientId)
        {
            ClientRpcParams clientRpcParams = new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new ulong[] { clientId }
                }
            };

            WhiteBall.Instance.ShowHintClientRpc(state, clientRpcParams);
        }
    }
}
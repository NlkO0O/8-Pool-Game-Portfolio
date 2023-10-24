using System;
using System.Threading.Tasks;
using _Scripts.Game.CoreGame;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.TestSubjects.CoreGameTest.Player
{
    public class PlayerObjectives : NetworkBehaviour
    {
        public WhiteBallTest WhiteBallTest;
        [SerializeField] private CueControllerTest cueControllerTest;
        [SerializeField] private Trajectory trajectoryControllerTest;
        [SerializeField] private GameObject shootButtonTest;
        [SerializeField] private NetworkObject nObject;

        public Action<ulong> OnShotTaken;

        public override void OnNetworkSpawn()
        {
            WhiteBallTest.OnBallInHand += SendCueStateToServerRpc;
        }

        #region ClientCode

        [ClientRpc]
        private void HintClientRpc()
        {
            if (!IsOwner) return;

            WhiteBallTest.handHintTest.SetActive(true);
            WhiteBallTest.handHintTest.transform.position = WhiteBallTest.transform.position
                                                            + new Vector3(0, 0.5f, 0);
        }

        [ClientRpc]
        public void ShowCueClientRpc(bool state)
        {
            if (IsOwner) shootButtonTest.SetActive(state);

            cueControllerTest.Enable(state);
            cueControllerTest.SetAlpha(IsOwner);

            trajectoryControllerTest.gameObject.SetActive(state);
            trajectoryControllerTest.SetAlpha(IsOwner);
        }

        [ClientRpc]
        public void UpdateDelegatesClientRpc()
        {
            WhiteBallTest.UpdateDelegates();
        }

        public void Shoot()
        {
            SendToServerServerRpc();
        }

        #endregion


        #region ServerCode

        public void SetUpShooter(ulong clientId, BallType ballType, bool isMovable = false)
        {
            ChangeOwner(clientId);
            ShowCueClientRpc(true);
            WhiteBallTest.TargetBall = ballType;

            if (isMovable)
            {
                HintClientRpc();
                WhiteBallTest.SetDraggable(true);
                ShowCueClientRpc(false);
            }

            if (WhiteBallTest.MyBall.IsPotted)
            {
                WhiteBallTest.MoveToCenter();
                WhiteBallTest.GetComponent<Rigidbody>().velocity = Vector3.zero;
                WhiteBallTest.MyBall.IsPotted = false;
            }

            WhiteBallTest.Fouled = Foul.NoFoul;
        }

        private void ChangeOwner(ulong clientId)
        {
            nObject.ChangeOwnership(clientId);
        }

        [ServerRpc]
        private void SendToServerServerRpc()
        {
            SendToServerAsync(cueControllerTest.cue.right);
        }

        [ServerRpc]
        private void SendCueStateToServerRpc(bool state)
        {
            ShowCueClientRpc(state);
        }

        private async void SendToServerAsync(Vector3 direction)
        {
            ShowCueClientRpc(false);

            WhiteBallTest.ApplyCueForce(direction);

            WhiteBallTest.handHintTest.SetActive(false);

            await Task.Delay(500);

            OnShotTaken?.Invoke(OwnerClientId);
        }

        #endregion
    }
}
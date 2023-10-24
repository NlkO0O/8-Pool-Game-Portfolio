using _Scripts.TestSubjects.CoreGameTest;
using _Scripts.TestSubjects.CoreGameTest.Player;
using _Scripts.UI;
using UnityEngine;

namespace _Scripts.TestSubjects.StatesTest
{
    public class ShootState : GameStateTest
    {
        private PlayerObjectives playerObjs;

        public ShootState(GameManagerTest gameManager, PoolPlayerTest player) : base(gameManager, player)
        {
        }

        public override void Enter()
        {
            playerObjs = gameManager.playerObjectives;

            ShowMessage(gameManager.isBreaking ? MessageType.Breaking : MessageType.PlayersTurn);

            playerObjs.SetUpShooter(currentPlayer.OwnerClientId, currentPlayer.Balltype.Value, gameManager.isBreaking);

            if (gameManager.isBreaking)
            {
                gameManager.playerObjectives.ShowCueClientRpc(true);
            }

            currentPlayer.TimeRemaining.Value = 30f;
        }

        public override void Update()
        {
            if (currentPlayer.TimeRemaining.Value <= 0)
            {
                playerObjs.WhiteBallTest.Fouled = Foul.TimedOut;
                ShowMessage(MessageType.TimedOut);
                gameManager.ChangeState(new PenaltyStateTest(gameManager, opponentPlayer));
                return;
            }

            currentPlayer.TimeRemaining.Value -= Time.deltaTime;
        }

        public override void Exit()
        {
            currentPlayer.TimeRemaining.Value = 30f;

            gameManager.playerObjectives.WhiteBallTest.SetDraggable(false);
        }
    }
}
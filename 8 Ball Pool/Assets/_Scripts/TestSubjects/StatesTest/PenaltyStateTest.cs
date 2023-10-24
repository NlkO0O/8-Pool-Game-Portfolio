using _Scripts.TestSubjects.CoreGameTest;
using _Scripts.TestSubjects.CoreGameTest.Player;
using UnityEngine;
using MessageType = _Scripts.UI.MessageType;

namespace _Scripts.TestSubjects.StatesTest
{
    public class PenaltyStateTest : GameStateTest
    {
        public PenaltyStateTest(GameManagerTest gameManager, PoolPlayerTest player) : base(gameManager, player)
        {
        }

        public override void Enter()
        {
            var playerObjs = gameManager.playerObjectives;

            ShowMessage(MessageType.PlayersTurn);

            playerObjs.SetUpShooter(currentPlayer.OwnerClientId, currentPlayer.Balltype.Value, true);

            currentPlayer.TimeRemaining.Value = 30f;
        }

        public override void Update()
        {
            if (currentPlayer.TimeRemaining.Value <= 0)
            {
                gameManager.playerObjectives.WhiteBallTest.Fouled = Foul.TimedOut;
                ShowMessage(MessageType.TimedOut);
                gameManager.ChangeState(new PenaltyStateTest(gameManager, opponentPlayer));
                return;
            }

            currentPlayer.TimeRemaining.Value -= Time.deltaTime;
        }

        public override void Exit()
        {
            gameManager.playerObjectives.WhiteBallTest.SetDraggable(false);
        }
    }
}
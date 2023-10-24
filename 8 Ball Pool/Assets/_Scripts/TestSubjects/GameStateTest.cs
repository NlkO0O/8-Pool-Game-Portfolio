using System;
using _Scripts.TestSubjects.CoreGameTest;
using _Scripts.TestSubjects.CoreGameTest.Player;
using _Scripts.UI;

namespace _Scripts.TestSubjects
{
    public abstract class GameStateTest
    {
        protected GameManagerTest gameManager;
        protected PoolPlayerTest currentPlayer;
        protected PoolPlayerTest opponentPlayer;

        protected GameStateTest(GameManagerTest gameManager, PoolPlayerTest player)
        {
            this.gameManager = gameManager;
            currentPlayer = player;
            opponentPlayer = player.OwnerClientId switch
            {
                0 => gameManager.playersDic[1],
                1 => gameManager.playersDic[0],
                _ => null
            };
        }

        public abstract void Enter();
        public abstract void Update();
        public abstract void Exit();

        protected void ShowMessage(MessageType messageType)
        {
            currentPlayer.DisplayAction(messageType, string.Empty);
            opponentPlayer.DisplayAction(messageType, currentPlayer.playerName);
        }
    }
}
using _Scripts.Game.CoreGameSystems;
using _Scripts.Game.Player;
using UnityEngine;

namespace _Scripts.Game.CoreGameSystems
{
    public abstract class GameState
    {
        protected GameManager gameManager;
        protected PoolPlayer currentPlayer;

        protected GameState(GameManager gameManager, PoolPlayer player)
        {
            this.gameManager = gameManager;
            currentPlayer = player;
        }

        public abstract void Enter();
        public abstract void Update();

        public abstract void Exit();
    }
}
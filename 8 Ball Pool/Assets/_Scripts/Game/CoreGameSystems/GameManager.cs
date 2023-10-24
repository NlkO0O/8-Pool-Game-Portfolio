using System.Collections.Generic;
using System.Linq;
using _Scripts.Game.Player;
using _Scripts.Game.States;
using UnityEngine;

namespace _Scripts.Game.CoreGameSystems
{
    public class GameManager : MonoBehaviour
    {
        public List<PoolPlayer> poolPlayers;
        private GameState currentState;

        public void Initialize()
        {
            PoolPlayer player1 = poolPlayers.FirstOrDefault(player => player.PlayerType == PlayerType.Player1);

            currentState = new SetUpTableState(this, player1);

            currentState.Enter();
        }

        private void Update()
        {
            currentState.Update();
        }

        public void ChangeState(GameState newState)
        {
            currentState.Exit();
            currentState = newState;
            currentState.Enter();
        }
    }
}
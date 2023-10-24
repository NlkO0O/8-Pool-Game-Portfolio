using System.Collections.Generic;
using _Scripts.Game.CoreGame;
using _Scripts.TestSubjects.CoreGameTest.Player;
using _Scripts.TestSubjects.CoreGameTest.Table;
using _Scripts.TestSubjects.StatesTest;
using _Scripts.Utils;
using Unity.Netcode;
using UnityEngine;

namespace _Scripts.TestSubjects.CoreGameTest
{
    public class GameManagerTest : SingletonNetwork<GameManagerTest>
    {
        public TableSetTest TableSetTest;
        public PlayerObjectives playerObjectives;
        public BallTest blackBall;

        public Dictionary<ulong, PoolPlayerTest> playersDic = new Dictionary<ulong, PoolPlayerTest>();
        public Dictionary<BallType, PoolPlayerTest> playersWithColors = new Dictionary<BallType, PoolPlayerTest>();

        public List<BallTest> allBalls = new List<BallTest>();
        public List<BallTest> pottedBalls = new List<BallTest>();
        public List<BallTest> pottedBallsBeforeSet = new List<BallTest>();

        public bool isBreaking = true;
        public bool areColorsSet;

        private GameStateTest gameState;

        private void Start()
        {
            NetworkManager.Singleton.OnClientConnectedCallback += HandleStartGame;
        }

        private void HandleStartGame(ulong obj)
        {
            if (!IsServer) return;

            if (NetworkManager.Singleton.ConnectedClients.Count != 2) return;

            foreach (var p in FindObjectsByType<PoolPlayerTest>(FindObjectsSortMode.None))
            {
                playersDic.Add(p.OwnerClientId, p);
            }

            gameState = new SetUpTableStateTest(this, playersDic[0]);

            gameState.Enter();
        }

        private void Update()
        {
            if (!IsServer) return;

            gameState?.Update();
        }

        public void ChangeState(GameStateTest newState)
        {
            if (!IsServer) return;

            gameState.Exit();
            gameState = newState;
            gameState.Enter();
        }
    }
}
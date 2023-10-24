using System.Collections.Generic;
using _Scripts.Game.CoreGame;
using _Scripts.TestSubjects.CoreGameTest.Player;
using _Scripts.Utils;
using Unity.Mathematics;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Scripts.TestSubjects.CoreGameTest.Table
{
    public class TableSetTest : SingletonNetwork<TableSetTest>
    {
        [SerializeField] private NetworkObject whiteBall;
        [SerializeField] private NetworkObject ballPrefab;
        [SerializeField] private GameManagerTest gameManagerTest;

        [SerializeField] private Vector3 blackBallPosition;
        [SerializeField] private List<Vector3> positions;

        public Dictionary<int, Texture2D> loadedTexturesDictionary = new Dictionary<int, Texture2D>();


        public override void OnNetworkSpawn()
        {
            loadedTexturesDictionary = ResourcesLoader.GetTexturesDictionary();
        }

        public void SetTable()
        {
            gameManagerTest.playerObjectives = SpawnWhiteBall();

            for (int i = 0; i <= 15; i++)
            {
                switch (i)
                {
                    case 8:
                    {
                        var blackBall = SetUpBall(blackBallPosition, 8);
                        gameManagerTest.allBalls.Add(blackBall);
                        gameManagerTest.blackBall = blackBall;
                        continue;
                    }
                    case > 0 and < 8:
                    {
                        BallTest ball = SetUpBall(GetRandomBallPosition(), i);
                        gameManagerTest.allBalls.Add(ball);
                        continue;
                    }
                    case > 8 and <= 15:
                    {
                        BallTest ball = SetUpBall(GetRandomBallPosition(), i);
                        gameManagerTest.allBalls.Add(ball);
                        break;
                    }
                }
            }
        }

        private BallTest SetUpBall(Vector3 position, int number)
        {
            NetworkObject ballInstance = Instantiate(ballPrefab, position, quaternion.identity);

            MaterialSetter.SetMaterial(ballInstance, loadedTexturesDictionary[number]);

            ballInstance.Spawn();

            return SetBall(number, ballInstance);
        }

        private BallTest SetBall(int number, NetworkObject ballInstance)
        {
            BallTest ball = ballInstance.GetComponent<BallTest>();

            BallType ballType = GetCorrectTeamColor(number);

            ball.Number.Value = number;

            ball.BallType.Value = ballType;

            return ball;
        }

        private BallType GetCorrectTeamColor(int number) => number switch
        {
            0 => BallType.WhiteBall,
            8 => BallType.BlackBall,
            > 8 => BallType.Stripe,
            < 8 => BallType.Solid
        };

        private Vector3 GetRandomBallPosition()
        {
            int randomIndex = Random.Range(0, positions.Count - 1);

            Vector3 randomPosition = positions[randomIndex];

            positions.Remove(randomPosition);

            return randomPosition;
        }

        public PlayerObjectives SpawnWhiteBall()
        {
            var wBall = Instantiate(whiteBall, new Vector3(-2.5f, -0.6805078f, 0), quaternion.identity);
            wBall.Spawn();
            gameManagerTest.allBalls.Add(wBall.GetComponentInChildren<BallTest>());
            return wBall.GetComponent<PlayerObjectives>();
        }
    }
}
using System.Collections.Generic;
using _Scripts.TestSubjects.CoreGameTest.Table;
using _Scripts.Utils;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Scripts.Game.CoreGame
{
    public class TableSet : NetworkBehaviour
    {
        [Header("References")]
        [SerializeField] private NetworkObject ballPrefab;
        [SerializeField] private NetworkObject whiteBallPrefab;
        [SerializeField] private PottedBalls pottedBalls;

        [Header("Positions")]
        [SerializeField] private Vector3 whiteBallPosition;
        [SerializeField] private Vector3 blackBallPosition;
        [SerializeField] private List<Vector3> positions;

        private Dictionary<int, Texture2D> loadedTexturesDictionary = new Dictionary<int, Texture2D>();
        private static TableSet instance;

        public static TableSet Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<TableSet>();

                    if (instance == null)
                    {
                        Debug.Log("returning null");
                        return null;
                    }
                }

                return instance;
            }
        }
        public Dictionary<BallType, Dictionary<int, Ball>>
            PoolBalls = new Dictionary<BallType, Dictionary<int, Ball>>();
        public WhiteBall WhiteBall;
        public Ball BlackBall;


        public override void OnNetworkSpawn()
        {
            loadedTexturesDictionary = ResourcesLoader.GetTexturesDictionary();

            pottedBalls.gameObject.SetActive(IsServer);
        }

        public void SetUpTable()
        {
            Dictionary<int, Ball> stripes = new Dictionary<int, Ball>();
            Dictionary<int, Ball> fulls = new Dictionary<int, Ball>();

            for (int i = 0; i <= 15; i++)
            {
                if (i == 0)
                {
                    NetworkObject whiteBallInstance =
                        Instantiate(whiteBallPrefab, whiteBallPosition, quaternion.identity);
                    whiteBallInstance.Spawn();
                    WhiteBall = whiteBallInstance.GetComponent<WhiteBall>();

                    SetUpBall(blackBallPosition, 8);
                }

                if (i == 8)
                {
                    BlackBall = SetUpBall(blackBallPosition, 8);
                    continue;
                }

                if (i > 0 && i < 8)
                {
                    Ball ball = SetUpBall(GetRandomBallPosition(), i);
                    fulls.Add(ball.Number.Value, ball);
                    continue;
                }

                if (i > 8 && i <= 15)
                {
                    Ball ball = SetUpBall(GetRandomBallPosition(), i);
                    stripes.Add(ball.Number.Value, ball);
                }
            }

            PoolBalls[BallType.Solid] = fulls;
            PoolBalls[BallType.Stripe] = stripes;
        }

        private Ball SetUpBall(Vector3 position, int number)
        {
            NetworkObject ballInstance = Instantiate(ballPrefab, position, quaternion.identity);

            MaterialSetter.SetMaterial(ballInstance, loadedTexturesDictionary[number]);

            ballInstance.Spawn();

            return SetBall(number, ballInstance);
        }

        private Ball SetBall(int number, NetworkObject ballInstance)
        {
            Ball ball = ballInstance.GetComponent<Ball>();

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

            Vector3 randomedPosition = positions[randomIndex];

            positions.Remove(randomedPosition);

            return randomedPosition;
        }
    }
}
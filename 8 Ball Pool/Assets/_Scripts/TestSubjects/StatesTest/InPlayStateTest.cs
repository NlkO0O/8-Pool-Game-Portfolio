using System.Collections.Generic;
using System.Linq;
using _Scripts.Game.CoreGame;
using _Scripts.TestSubjects.CoreGameTest;
using _Scripts.TestSubjects.CoreGameTest.Player;
using _Scripts.TestSubjects.CoreGameTest.Table;
using _Scripts.UI;
using UnityEngine;

namespace _Scripts.TestSubjects.StatesTest
{
    public class InPlayStateTest : GameStateTest
    {
        private WhiteBallTest whiteBall;
        private List<BallTest> balls;

        public InPlayStateTest(GameManagerTest gameManager, PoolPlayerTest player) : base(gameManager, player)
        {
        }

        public override void Enter()
        {
            whiteBall = gameManager.playerObjectives.WhiteBallTest;
            balls = gameManager.allBalls;
        }

        public override void Update()
        {
            foreach (BallTest ball in balls)
            {
                if (!ball.IsPotted && ball.GetIsMoving()) return;
            }

            if (whiteBall.Fouled == Foul.Potted)
            {
                Debug.Log("Potted cue ball");

                ShowMessage(MessageType.PottedCueBall);
                ExitToPenaltyState();

                return;
            }

            if (whiteBall.TouchCounter == 0)
            {
                whiteBall.Fouled = Foul.NoContact;

                Debug.Log("No Contact");

                ShowMessage(MessageType.IllegalShot);
                ExitToPenaltyState();

                return;
            }

            if (whiteBall.Fouled == Foul.WrongColor)
            {
                Debug.Log("Wrong color boy");

                ShowMessage(MessageType.IllegalShot);
                ExitToPenaltyState();

                return;
            }

            if (CheckIfBumperWasHit())
            {
                HandlePottedBalls();

                return;
            }

            whiteBall.Fouled = Foul.NoBumpers;

            Debug.Log("No bumpers were hit");

            ShowMessage(MessageType.IllegalShot);
            ExitToPenaltyState();
        }

        public override void Exit()
        {
            if (gameManager.pottedBallsBeforeSet.Count > 0 && gameManager.areColorsSet)
            {
                foreach (var ball in gameManager.pottedBallsBeforeSet)
                {
                    HandleBlackBall(ball);

                    HandleBallsUI(ball);
                }

                gameManager.pottedBallsBeforeSet.Clear();
            }
            else if (gameManager.pottedBallsBeforeSet.Count > 0)
            {
                foreach (var ball in gameManager.pottedBallsBeforeSet)
                {
                    HandleBlackBall(ball);
                }
            }

            if (gameManager.pottedBalls.Count > 0 && gameManager.areColorsSet)
            {
                foreach (var ball in gameManager.pottedBalls)
                {
                    HandleBlackBall(ball);

                    HandleBallsUI(ball);

                    balls.Remove(ball);
                }

                gameManager.pottedBalls.Clear();
            }
            else if (gameManager.pottedBalls.Count > 0)
            {
                foreach (var ball in gameManager.pottedBallsBeforeSet)
                {
                    HandleBlackBall(ball);
                }
            }


            foreach (var ball in balls)
            {
                ball.HitBumper = false;
            }

            whiteBall.TouchCounter = 0;

            if (gameManager.isBreaking)
            {
                gameManager.playerObjectives.UpdateDelegatesClientRpc();
                gameManager.isBreaking = false;
            }
        }

        private void HandleBallsUI(BallTest ball)
        {
            var playerToUpdate = currentPlayer.Balltype.Value == ball.BallType.Value
                ? currentPlayer
                : opponentPlayer;

            playerToUpdate.HandleBallPottedClientRpc(ball.Number.Value);
            playerToUpdate.playersPottedBallsCount++;
        }

        private void HandleBlackBall(BallTest ball)
        {
            Debug.Log("Black Ball Was Potted");

            if (ball == gameManager.blackBall)
            {
                var winnerId = currentPlayer.playersPottedBallsCount == 7 && whiteBall.Fouled == Foul.NoFoul
                    ? currentPlayer.OwnerClientId
                    : opponentPlayer.OwnerClientId;

                currentPlayer.HandleGameOverClientRpc(winnerId);
                opponentPlayer.HandleGameOverClientRpc(winnerId);
            }
        }

        private void ExitToPenaltyState()
        {
            gameManager.ChangeState(new PenaltyStateTest(gameManager, opponentPlayer));
        }

        private void HandlePottedBalls()
        {
            if (gameManager.pottedBalls.Count > 0 || gameManager.pottedBallsBeforeSet.Count > 0)
            {
                bool correctColor =
                    gameManager.pottedBalls.FirstOrDefault(t => t.BallType.Value == currentPlayer.Balltype.Value);

                if (gameManager.isBreaking || correctColor)
                {
                    Debug.Log("Ball Dropped During the break");
                    gameManager.ChangeState(new ShootState(gameManager, currentPlayer));
                    return;
                }

                if (!gameManager.areColorsSet)
                {
                    var currentPlayersBallType = gameManager.pottedBalls[0].BallType.Value;
                    Debug.Log(currentPlayer.playerName + $"chose {currentPlayersBallType}");

                    currentPlayer.Balltype.Value = currentPlayersBallType;
                    gameManager.playersWithColors.Add(currentPlayersBallType, currentPlayer);

                    opponentPlayer.Balltype.Value = GetBallType(currentPlayersBallType);
                    gameManager.playersWithColors.Add(GetBallType(currentPlayersBallType), opponentPlayer);

                    gameManager.areColorsSet = true;

                    Debug.Log("We set the colors");
                    gameManager.ChangeState(new ShootState(gameManager, currentPlayer));
                    return;
                }

                Debug.Log("Ball Dropped During the break but still switching to the new one");
                gameManager.ChangeState(new ShootState(gameManager, opponentPlayer));
            }
            else
            {
                Debug.Log("You have zero potted ball");
                gameManager.ChangeState(new ShootState(gameManager, opponentPlayer));
            }
        }

        private BallType GetBallType(BallType currentsType)
        {
            return currentsType switch
            {
                BallType.Solid => BallType.Stripe,
                BallType.Stripe => BallType.Solid,
                _ => BallType.NotSet
            };
        }

        private bool CheckIfBumperWasHit()
        {
            foreach (BallTest ball in balls)
            {
                if (ball.HitBumper || ball.IsPotted) return true;
            }

            return false;
        }
    }
}
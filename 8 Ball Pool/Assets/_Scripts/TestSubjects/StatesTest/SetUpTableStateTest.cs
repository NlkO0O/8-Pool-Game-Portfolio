using System.Collections;
using _Scripts.TestSubjects.CoreGameTest;
using _Scripts.TestSubjects.CoreGameTest.Player;
using UnityEngine;

namespace _Scripts.TestSubjects.StatesTest
{
    public class SetUpTableStateTest : GameStateTest
    {
        public SetUpTableStateTest(GameManagerTest gameManager, PoolPlayerTest player) : base(gameManager, player)
        {
        }

        public override void Enter()
        {
            gameManager.StartCoroutine(StartGame());
        }

        public override void Update()
        {
        }

        public override void Exit()
        {
        }

        private IEnumerator StartGame()
        {
            yield return new WaitForSecondsRealtime(1.5f);

            gameManager.TableSetTest.SetTable();

            gameManager.playerObjectives.OnShotTaken += (ownerId) =>
            {
                gameManager.ChangeState(new InPlayStateTest(gameManager, gameManager.playersDic[ownerId]));
            };

            gameManager.ChangeState(new ShootState(gameManager, currentPlayer));
        }
    }
}
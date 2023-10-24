using _Scripts.Game.CoreGame;
using Unity.Netcode;
using UnityEngine;

namespace _Scripts.TestSubjects.CoreGameTest.Table
{
    public class PottedBalls : NetworkBehaviour
    {
        [SerializeField] private Transform pottedBallReferencePoint;
        private GameManagerTest gameManagerTest;

        private void OnEnable()
        {
            gameManagerTest = GetComponentInParent<GameManagerTest>();
        }

        private void OnCollisionEnter(Collision col)
        {
            var ball = col.transform.GetComponent<BallTest>();

            if (ball.CompareTag("WhiteBall"))
            {
                ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
                return;
            }

            ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
            ball.transform.position = pottedBallReferencePoint.position;
        }
    }
}
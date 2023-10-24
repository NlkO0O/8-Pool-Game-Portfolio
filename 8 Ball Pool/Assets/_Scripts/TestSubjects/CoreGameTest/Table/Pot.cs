using _Scripts.TestSubjects.CoreGameTest.Player;
using Unity.Netcode;
using UnityEngine;

namespace _Scripts.TestSubjects.CoreGameTest.Table
{
    public class Pot : NetworkBehaviour
    {
        [SerializeField] private PhysicMaterial PottedBallMaterial;


        private void OnTriggerEnter(Collider other)
        {
            if (!IsServer) return;

            Debug.Log(other.transform.name);

            var ball = other.GetComponent<BallTest>();
            if (!ball) return;

            var whiteBall = ball.GetComponent<WhiteBallTest>();
            if (whiteBall)
            {
                whiteBall.Fouled = Foul.Potted;
            }
            else
            {
                if (GameManagerTest.Instance.areColorsSet)
                {
                    GameManagerTest.Instance.pottedBalls.Add(ball);
                }
                else
                {
                    GameManagerTest.Instance.pottedBallsBeforeSet.Add(ball);
                }

                other.GetComponent<SphereCollider>().material = PottedBallMaterial;
            }

            ball.IsPotted = true;

            var rb = ball.GetComponent<Rigidbody>();
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
}
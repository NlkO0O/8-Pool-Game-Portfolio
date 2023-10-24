using _Scripts.Game.CoreGame;
using Unity.Netcode;
using UnityEngine;

namespace _Scripts.TestSubjects.CoreGameTest.Table
{
    public class BallTest : NetworkBehaviour
    {
        [SerializeField] private Renderer textureRenderer;
        [SerializeField] private Rigidbody rb;
        [SerializeField] private float stopSpeedThreshold = 0.05f;

        public NetworkVariable<BallType> BallType = new NetworkVariable<BallType>();
        public NetworkVariable<int> Number = new NetworkVariable<int>();

        public bool IsPotted;
        public bool HitBumper;

        public override void OnNetworkSpawn()
        {
            Number.OnValueChanged += HandleNumberUpdated;
        }

        private void HandleNumberUpdated(int oldNumber, int newNumber)
        {
            Material material = new Material(Shader.Find("Standard"));

            material.mainTexture = TableSetTest.Instance.loadedTexturesDictionary[newNumber];

            textureRenderer.material = material;
        }

        protected virtual void Update()
        {
            if (!IsServer) return;

            if (rb.velocity.magnitude > stopSpeedThreshold) return;

            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        public virtual void OnCollisionEnter(Collision other)
        {
            if (other.transform.CompareTag("Bumpers"))
            {
                HitBumper = true;
            }
        }

        public bool GetIsMoving() => rb.velocity != Vector3.zero;
    }
}
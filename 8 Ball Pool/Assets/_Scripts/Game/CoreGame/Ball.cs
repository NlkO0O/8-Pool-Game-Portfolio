using System.Collections.Generic;
using _Scripts.Utils;
using Unity.Netcode;
using UnityEngine;


namespace _Scripts.Game.CoreGame
{
    public class Ball : NetworkBehaviour
    {
        [SerializeField] private Renderer textureRenderer;
        [SerializeField] private Rigidbody rb;

        public NetworkVariable<int> Number = new NetworkVariable<int>();
        public NetworkVariable<bool> ShouldFreeze = new NetworkVariable<bool>();
        public NetworkVariable<BallType> BallType = new NetworkVariable<BallType>();

        private Dictionary<int, Texture2D> loadedTextures;

        public override void OnNetworkSpawn()
        {
            Number.OnValueChanged += HandleNumberUpdated;
            ShouldFreeze.OnValueChanged += HandleStateChanged;

            if (IsClient)
            {
                loadedTextures = ResourcesLoader.GetTexturesDictionary();
            }

            // if (IsServer)
            // {
            //     Transform parent = GameObject.FindWithTag("Balls").transform;
            //     transform.parent = parent;
            // }
        }

        public void HandleStateChanged(bool ignoreValue, bool shouldFreeze)
        {
            if (shouldFreeze)
            {
                rb.constraints = RigidbodyConstraints.FreezePositionX |
                                 RigidbodyConstraints.FreezePositionZ |
                                 RigidbodyConstraints.FreezeRotation;
            }
            else
            {
                rb.constraints = RigidbodyConstraints.None |
                                 RigidbodyConstraints.FreezePositionY;
            }
        }

        private void HandleNumberUpdated(int oldNumber, int newNumber)
        {
            Material material = new Material(Shader.Find("Standard"));

            material.mainTexture = loadedTextures[newNumber];

            textureRenderer.material = material;
        }
    }


    public enum BallType
    {
        NotSet,
        Solid,
        Stripe,
        WhiteBall,
        BlackBall
    }
}
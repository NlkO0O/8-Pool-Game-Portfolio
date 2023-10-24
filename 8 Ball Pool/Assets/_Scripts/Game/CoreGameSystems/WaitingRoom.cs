using System.Collections.Generic;
using _Scripts.Game.Player;
using _Scripts.Networking.Host;
using _Scripts.Networking.Server;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;


namespace _Scripts.Game.CoreGameSystems
{
    public class WaitingRoom : NetworkBehaviour
    {
        [SerializeField] private GameManager gameManager;
        private NetworkServer networkServer;

        public override void OnNetworkSpawn()
        {
            if (!IsHost && !IsServer) return;

            networkServer = HostSingleTon.Instance.GameManager.NetworkServer;

            networkServer.onPlayersReady += Initialize;
        }

        private void Initialize()
        {
            if (!IsHost && !IsServer) return;

            Debug.Log("initializing...");

            GameManager manager = Instantiate(gameManager, new Vector3(0, 0, 0), quaternion.identity);

            // manager.GetComponent<NetworkObject>().Spawn();
            manager.poolPlayers = new List<PoolPlayer>();

            foreach (var p in FindObjectsByType<PoolPlayer>(FindObjectsSortMode.None))
            {
                manager.poolPlayers.Add(p);
            }

            Shuffle(manager.poolPlayers);

            manager.poolPlayers[0].PlayerType = PlayerType.Player1;
            manager.poolPlayers[1].PlayerType = PlayerType.Player2;

            manager.Initialize();

            Destroy(gameObject);
        }

        void Shuffle<T>(List<T> list)
        {
            int n = list.Count;
            for (int i = 0; i < n; i++)
            {
                int r = i + Random.Range(0, n - i);
                T temp = list[i];
                list[i] = list[r];
                list[r] = temp;
            }
        }
    }
}
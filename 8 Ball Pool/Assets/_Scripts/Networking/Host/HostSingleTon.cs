using System;
using System.Threading.Tasks;
using _Scripts.Utils;
using Unity.Netcode;
using UnityEngine;

namespace _Scripts.Networking.Host
{
    public class HostSingleTon : SingletonPersistent<HostSingleTon>
    {
        public HostGameManager GameManager { get; private set; }

        
        public void CreateHost(NetworkObject playerPrefab)
        {
            GameManager = new HostGameManager(playerPrefab);
        }

        private void OnDestroy()
        {
            GameManager?.Dispose();
        }
    }
}
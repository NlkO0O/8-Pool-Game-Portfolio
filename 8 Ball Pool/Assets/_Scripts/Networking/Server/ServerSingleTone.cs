#if UNITY_SERVER || UNITY_EDITOR
using System.Threading.Tasks;
using _Scripts.Networking.Shared;
using Unity.Netcode;
using Unity.Services.Core;
using UnityEngine;

namespace _Scripts.Networking.Server
{
    public class ServerSingleTone : MonoBehaviour
    {
        private static ServerSingleTone _instance;
        public ServerGameManager GameManager { get; private set; }

        public static ServerSingleTone Instance
        {
            get
            {
                if (_instance != null) return _instance;

                _instance = FindObjectOfType<ServerSingleTone>();

                if (_instance == null)
                {
                    return null;
                }

                return _instance;
            }
        }

        void Start()
        {
            DontDestroyOnLoad(gameObject);
        }

        public async Task CreateServer(NetworkObject playerPrefab)
        {
            await UnityServices.InitializeAsync();

            GameManager = new ServerGameManager(ApplicationData.IP(),
                ApplicationData.Port(),
                ApplicationData.QPort(),
                NetworkManager.Singleton,
                playerPrefab);
        }

        private void OnDestroy()
        {
            GameManager?.Dispose();
        }
    }
}
#endif
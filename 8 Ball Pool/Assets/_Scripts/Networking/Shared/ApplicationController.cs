using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using _Scripts.Networking.Client;
using _Scripts.Networking.Host;
using _Scripts.Networking.Server;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

namespace _Scripts.Networking.Shared
{
    public class ApplicationController : MonoBehaviour
    {
        [SerializeField] private ClientSingleTon clientPrefab;
        [SerializeField] private HostSingleTon hostPrefab;
#if UNITY_SERVER || UNITY_EDITOR
        [SerializeField] private ServerSingleTone serverPrefab;
#endif
        [SerializeField] private NetworkObject playerPrefab;

        private ApplicationData appData;
        private const string GAMENAME = "Game";

        async void Start()
        {
            DontDestroyOnLoad(gameObject);

            await LaunchInMode(SystemInfo.graphicsDeviceType == GraphicsDeviceType.Null);
        }

        private async Task LaunchInMode(bool isDedicated)
        {
#if UNITY_SERVER || UNITY_EDITOR
            if (isDedicated)
            {
                Application.targetFrameRate = 60;

                appData = new ApplicationData();

                ServerSingleTone serverSingleTone = Instantiate(serverPrefab);

                StartCoroutine(LoadGameSceneAsync(serverSingleTone));

                return;
            }
#endif

            HostSingleTon hostSingleTon = Instantiate(hostPrefab);
            hostSingleTon.CreateHost(playerPrefab);

            ClientSingleTon clientSingleTon = Instantiate(clientPrefab);
            bool isAuthenticated = await clientSingleTon.CreateClient();

            if (isAuthenticated)
            {
                clientSingleTon.GameManager.GoToMenu();
            }
        }
#if UNITY_SERVER || UNITY_EDITOR
        private IEnumerator LoadGameSceneAsync(ServerSingleTone serverSingleTone)
        {
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(GAMENAME);

            while (!asyncOperation.isDone)
            {
                yield return null;
            }

            Task createServerTask = serverSingleTone.CreateServer(playerPrefab);

            yield return new WaitUntil(() => createServerTask.IsCompleted);

            Task startServerTask = serverSingleTone.GameManager.StartGameServerAsync();

            yield return new WaitUntil(() => startServerTask.IsCompleted);
        }
#endif
    }
}
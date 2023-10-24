#if UNITY_SERVER || UNITY_EDITOR
using System;
using System.Threading.Tasks;
using _Scripts.Networking.Server.Services;
using _Scripts.Networking.Shared;
using Unity.Netcode;
using Unity.Services.Matchmaker.Models;
using UnityEngine;

namespace _Scripts.Networking.Server
{
    public class ServerGameManager : IDisposable
    {
        private string serverIp;
        private int serverPort;
        private int queryPort;
        private NetworkServer networkServer;
        private MultiplayAllocationService multiplayerAllocationService;
        private NetworkObject playerPrefab;
        private const int MaxConnections = 2;

        public ServerGameManager(string serverIp, int serverPort, int queryPort, NetworkManager networkManager,
            NetworkObject playerPrefab)
        {
            this.serverIp = serverIp;
            this.serverPort = serverPort;
            this.queryPort = queryPort;
            this.playerPrefab = playerPrefab;
            networkServer = new NetworkServer(networkManager, playerPrefab, MaxConnections);
            multiplayerAllocationService = new MultiplayAllocationService();
        }

        public async Task StartGameServerAsync()
        {
            await multiplayerAllocationService.BeginServerCheck();

            try
            {
                MatchmakingResults matchmakerPayload = await GetMatchmakerPayload();

                if (matchmakerPayload != null)
                {
                    networkServer.OnUserJoined += UserJoined;
                    networkServer.onUserLeft += UserLeft;
                }
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }

            if (!networkServer.OpenConnection(serverIp, serverPort))
            {
                Debug.LogError("Network Server Failed As Expected");
                return;
            }
        }

        private async Task<MatchmakingResults> GetMatchmakerPayload()
        {
            Task<MatchmakingResults> matchmakerPayloadTask =
                multiplayerAllocationService.SubscribeAndAwaitMatchmakerAllocation();

            if (await Task.WhenAny(matchmakerPayloadTask, Task.Delay(20000)) == matchmakerPayloadTask)
            {
                return matchmakerPayloadTask.Result;
            }

            return null;
        }

        private void UserJoined(UserData user)
        {
            if (multiplayerAllocationService.CurrentPlayers() >= multiplayerAllocationService.MaxPlayers()) return;
            multiplayerAllocationService.AddPlayer();
        }

        private void UserLeft(UserData userData)
        {
            multiplayerAllocationService.RemovePlayer();

            if (multiplayerAllocationService.CurrentPlayers() <= 0)
            {
                Dispose();
                CloseServer();
            }
        }

        private void CloseServer()
        {
            Dispose();
            Application.Quit();
        }

        public int GetCurrentPlayers() => multiplayerAllocationService.CurrentPlayers();

        public int GetMaxPlayers() => multiplayerAllocationService.MaxPlayers();

        public void Dispose()
        {
            multiplayerAllocationService?.Dispose();
            networkServer?.Dispose();
        }
    }
}
#endif
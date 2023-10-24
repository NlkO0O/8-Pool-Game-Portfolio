using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using _Scripts.Networking.Shared;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

namespace _Scripts.Networking.Server
{
    public class NetworkServer : IDisposable
    {
        private int currentPlayers;
        private int maxPlayers;

        public Action<UserData> OnUserJoined;
        public Action<UserData> onUserLeft;

        public Action<string> OnCientLeft;

        private NetworkManager networkManager;
        private NetworkObject playerPrefab;

        private Dictionary<ulong, string> _clientIdToAuth = new Dictionary<ulong, string>();
        private Dictionary<string, UserData> _authIdToUserData = new Dictionary<string, UserData>();

        public Action onPlayersReady;


        public NetworkServer(NetworkManager networkManager, NetworkObject playerPrefab, int maxPlayers)
        {
            this.networkManager = networkManager;
            this.playerPrefab = playerPrefab;
            this.maxPlayers = maxPlayers;

            this.networkManager.ConnectionApprovalCallback += ApprovalCheck;
            this.networkManager.OnServerStarted += OnNetworkReady;
        }

        public bool OpenConnection(string ip, int port)
        {
            UnityTransport transport = networkManager.gameObject.GetComponent<UnityTransport>();
            transport.SetConnectionData(ip, (ushort)port);
            return networkManager.StartServer();
        }

        private void ApprovalCheck(
            NetworkManager.ConnectionApprovalRequest request,
            NetworkManager.ConnectionApprovalResponse response)
        {
            string payload = System.Text.Encoding.UTF8.GetString(request.Payload);
            UserData userData = JsonUtility.FromJson<UserData>(payload);

            _clientIdToAuth[request.ClientNetworkId] = userData.userAuthId;
            _authIdToUserData[userData.userAuthId] = userData;
            OnUserJoined?.Invoke(userData);

            _ = SpawnPLayerDelayed(request.ClientNetworkId);

            response.Approved = true;
            response.CreatePlayerObject = false;
        }

        private async Task SpawnPLayerDelayed(ulong clientId)
        {
            await Task.Delay(500);

            NetworkObject playerInstance = GameObject.Instantiate(playerPrefab);

            playerInstance.SpawnAsPlayerObject(clientId);

            currentPlayers++;

            if (currentPlayers == maxPlayers)
            {
                onPlayersReady?.Invoke();
            }
        }

        private void OnNetworkReady()
        {
            networkManager.OnClientDisconnectCallback += OnClientDisconnect;
        }

        private void OnClientDisconnect(ulong clientId)
        {
            if (_clientIdToAuth.TryGetValue(clientId, out string authId))
            {
                _clientIdToAuth.Remove(clientId);
                _authIdToUserData.Remove(authId);
                onUserLeft?.Invoke(_authIdToUserData[authId]);
                currentPlayers--;
            }
        }

        public void Dispose()
        {
            if (networkManager == null) return;

            networkManager.ConnectionApprovalCallback -= ApprovalCheck;
            networkManager.OnClientDisconnectCallback -= OnClientDisconnect;
            networkManager.OnServerStarted -= OnNetworkReady;

            if (networkManager.IsListening)
            {
                networkManager.Shutdown();
            }
        }

        public UserData GetUserDataByClientId(ulong clientId)
        {
            if (_clientIdToAuth.TryGetValue(clientId, out string authId))
            {
                if (_authIdToUserData.TryGetValue(authId, out UserData data))
                {
                    return data;
                }

                return null;
            }

            return null;
        }
    }
}
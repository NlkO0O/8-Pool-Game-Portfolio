using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using _Scripts.Networking.Server;
using _Scripts.Networking.Shared;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Scripts.Networking.Host
{
    public class HostGameManager : IDisposable
    {
        private Allocation _allocation;
        public NetworkServer NetworkServer;
        private NetworkObject playerPrefab;

        //edit after done back to private joincode
        public string _joinCode;
        private string _lobbyId;
        private const int MaxConnections = 2;
        private const string GAMENAME = "Game";
        private const string MENUNAME = "MainMenu";

        public HostGameManager(NetworkObject playerPrefab)
        {
            this.playerPrefab = playerPrefab;
        }

        public async Task StartHostAsync()
        {
            try
            {
                _allocation = await Relay.Instance.CreateAllocationAsync(MaxConnections);
            }
            catch (Exception e)
            {
                Debug.Log(e);
                return;
            }

            try
            {
                _joinCode = await Relay.Instance.GetJoinCodeAsync(_allocation.AllocationId);
                Debug.Log(_joinCode);
            }
            catch (Exception e)
            {
                Debug.Log(e);
                return;
            }


            UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();

            RelayServerData relayServerData = new RelayServerData(_allocation, "udp");
            transport.SetRelayServerData(relayServerData);

            try
            {
                CreateLobbyOptions lobbyOptions = new CreateLobbyOptions();
                lobbyOptions.Data = new Dictionary<string, DataObject>()
                {
                    {
                        "JoinCode", new DataObject(visibility: DataObject.VisibilityOptions.Member,
                            value: _joinCode)
                    }
                };

                string playerName = PlayerPrefs.GetString(NameSelector.PlayerNameKey);

                Lobby lobby =
                    await Lobbies.Instance.CreateLobbyAsync(playerName,
                        MaxConnections, lobbyOptions);

                _lobbyId = lobby.Id;

                HostSingleTon.Instance.StartCoroutine(HeartBeatLobby(15));
            }
            catch (Exception e)
            {
                Debug.Log(e);
                return;
            }

            NetworkServer = new NetworkServer(NetworkManager.Singleton, playerPrefab, MaxConnections);

            UserData userData = new UserData()
            {
                userName = PlayerPrefs.GetString(NameSelector.PlayerNameKey, "Missing name"),
                userAuthId = AuthenticationService.Instance.PlayerId,
            };

            string payload = JsonUtility.ToJson(userData);
            byte[] payloadBytes = Encoding.UTF8.GetBytes(payload);

            NetworkManager.Singleton.NetworkConfig.ConnectionData = payloadBytes;

            NetworkManager.Singleton.StartHost();

            NetworkManager.Singleton.SceneManager.LoadScene(GAMENAME, LoadSceneMode.Single);
        }

        private IEnumerator HeartBeatLobby(float waitTimeSeconds)
        {
            WaitForSecondsRealtime delay = new WaitForSecondsRealtime(waitTimeSeconds);
            while (true)
            {
                Lobbies.Instance.SendHeartbeatPingAsync(_lobbyId);
                yield return delay;
            }
        }


        public void Dispose()
        {
            ShutDownAsync();
        }

        public async void ShutDownAsync()
        {
            if (string.IsNullOrEmpty(_lobbyId)) return;

            HostSingleTon.Instance.StopCoroutine(nameof(HeartBeatLobby));

            try
            {
                await Lobbies.Instance.DeleteLobbyAsync(_lobbyId);
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }

            _lobbyId = string.Empty;

            NetworkServer?.Dispose();

            NetworkManager.Singleton.SceneManager.LoadScene(MENUNAME, LoadSceneMode.Single);
        }
    }
}
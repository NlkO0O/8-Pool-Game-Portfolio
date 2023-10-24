using System;
using System.Text;
using System.Threading.Tasks;
using _Scripts.Networking.Client.Services;
using _Scripts.Networking.Server;
using _Scripts.Networking.Shared;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClientGameManager : IDisposable
{
    private const string MENUNAME = "MainMenu";

    private JoinAllocation _joinAllocation;
    private NetworkClient _networkClient;
    private MatchplayMatchmaker matchmaker;
    private UserData userData;

    public async Task<bool> InItAsync()
    {
        await UnityServices.InitializeAsync();

        _networkClient = new NetworkClient(NetworkManager.Singleton);

        matchmaker = new MatchplayMatchmaker();

        AuthState authState = await AuthenticationWrapper.DoAuthAsync();

        if (authState == AuthState.Authenticated)
        {
            userData = new UserData()
            {
                userName = PlayerPrefs.GetString(NameSelector.PlayerNameKey, "Missing name"),
                userAuthId = AuthenticationService.Instance.PlayerId,
            };

            return true;
        }

        return false;
    }

    private void StartClient(string ip, int port)
    {
        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        transport.SetConnectionData(ip, (ushort)port);
        ConnectClient();
    }

    public async Task StartClientAsync(string joinCode)
    {
        try
        {
            _joinAllocation = await Relay.Instance.JoinAllocationAsync(joinCode);
        }
        catch (Exception e)
        {
            Debug.Log(e);
            return;
        }

        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();

        RelayServerData relayServerData = new RelayServerData(_joinAllocation, "udp");
        transport.SetRelayServerData(relayServerData);

        ConnectClient();
    }

    public async void MatchMakeAsync(Action<MatchmakerPollingResult> onMatchMakeResponse)
    {
        if (matchmaker.IsMatchmaking)
        {
            Debug.LogWarning("Matchmaking is already in progress...");
            return;
        }

        Debug.Log("We are here before Getting Match Async");

        MatchmakerPollingResult matchResult = await GetMatchAsync();

        onMatchMakeResponse?.Invoke(matchResult);
    }

    public async Task CancelMatchmaking()
    {
        await matchmaker.CancelMatchmaking();
    }

    public void GoToMenu()
    {
        SceneManager.LoadScene(MENUNAME);
    }

    public void Dispose()
    {
        _networkClient?.Dispose();
    }

    private void ConnectClient()
    {
        string payload = JsonUtility.ToJson(userData);
        byte[] payloadBytes = Encoding.UTF8.GetBytes(payload);

        NetworkManager.Singleton.NetworkConfig.ConnectionData = payloadBytes;

        NetworkManager.Singleton.StartClient();
    }

    private async Task<MatchmakerPollingResult> GetMatchAsync()
    {
        MatchmakingResult matchmakingResult = await matchmaker.Matchmake(userData);

        if (matchmakingResult.result == MatchmakerPollingResult.Success)
        {
            StartClient(matchmakingResult.ip, matchmakingResult.port);
        }

        return matchmakingResult.result;
    }

    public void Disconnecet()
    {
        _networkClient.Disconnect();
    }
}
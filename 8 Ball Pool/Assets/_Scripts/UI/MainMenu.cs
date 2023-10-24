using System.Collections;
using System.Collections.Generic;
using _Scripts.Networking.Client;
using _Scripts.Networking.Client.Services;
using _Scripts.Networking.Host;
using TMPro;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Canvas preLobby;
    [SerializeField] private Canvas lobby;
    [SerializeField] private TMP_Text status;
    [SerializeField] private TMP_InputField joinCodeField;

    private bool isMatchmaking;
    private bool isCancelling;

    public void FindMatch()
    {
        lobby.gameObject.SetActive(true);
        preLobby.gameObject.SetActive(false);
        ClientSingleTon.Instance.GameManager.MatchMakeAsync(OnMatchMade);
        isMatchmaking = true;
    }

    private void OnMatchMade(MatchmakerPollingResult result)
    {
        switch (result)
        {
            case MatchmakerPollingResult.Success:
                status.text = "Connecting";
                break;
            case MatchmakerPollingResult.TicketCreationError:
                status.text = "TicketCreationError";
                break;
            case MatchmakerPollingResult.TicketCancellationError:
                status.text = "TicketCancellationError";
                break;
            case MatchmakerPollingResult.MatchAssignmentError:
                status.text = "MatchAssignmentError";
                break;
        }
    }

    public async void CancelMatch()
    {
        if (isCancelling) return;

        if (isMatchmaking)
        {
            isCancelling = true;

            await ClientSingleTon.Instance.GameManager.CancelMatchmaking();

            lobby.gameObject.SetActive(false);
            preLobby.gameObject.SetActive(true);

            isCancelling = false;
            isMatchmaking = false;
        }
    }

    public async void StartHost()
    {
        await HostSingleTon.Instance.GameManager.StartHostAsync();
    }

    public async void StartClient()
    {
        await ClientSingleTon.Instance.GameManager.StartClientAsync(joinCodeField.text);
    }
}
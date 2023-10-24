using _Scripts.Networking.Client;
using _Scripts.Networking.Host;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class GameHUD : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI displayText;

    private static GameHUD _singleton;

    public static GameHUD Singleton
    {
        get
        {
            if (_singleton == null)
            {
                _singleton = FindObjectOfType<GameHUD>();

                if (_singleton == null)
                {
                    GameObject singletonObject = new GameObject(typeof(GameHUD).Name);
                    _singleton = singletonObject.AddComponent<GameHUD>();
                }
            }

            return _singleton;
        }
    }

    public void LeaveGame()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            HostSingleTon.Instance.GameManager.ShutDownAsync();
        }
        else if (NetworkManager.Singleton.IsClient)
        {
            ClientSingleTon.Instance.GameManager.Disconnecet();
        }
    }

    [ClientRpc]
    public void DisplayTextClientRpc(string message)
    {
        displayText.text = message;
    }
}
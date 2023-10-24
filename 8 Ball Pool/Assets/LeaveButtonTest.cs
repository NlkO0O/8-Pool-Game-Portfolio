using Unity.Netcode;
using UnityEngine;

public class LeaveButtonTest : MonoBehaviour
{
    public void ShutDownServer()
    {
        NetworkManager.Singleton.Shutdown();
    }
}
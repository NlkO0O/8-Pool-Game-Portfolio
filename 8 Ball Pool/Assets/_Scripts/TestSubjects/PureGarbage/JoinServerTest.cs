using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Scripts.TestSubjects.PureGarbage
{
    public class JoinServerTest : MonoBehaviour
    {
        [SerializeField] private GameObject buttonClient;
        [SerializeField] private GameObject buttonHost;

        public void StartHost()
        {
            NetworkManager.Singleton.StartHost();
        }

        public void StartClient()
        {
            NetworkManager.Singleton.StartClient();
        }
    }
}
using _Scripts.Networking.Host;
using TMPro;
using UnityEngine;

namespace _Scripts.TestSubjects.PureGarbage
{
    public class DisplayJoinCodeTest : MonoBehaviour
    {
        void Start()
        {
            GetComponent<TextMeshProUGUI>().text = HostSingleTon.Instance.GameManager._joinCode;
        }
    }
}
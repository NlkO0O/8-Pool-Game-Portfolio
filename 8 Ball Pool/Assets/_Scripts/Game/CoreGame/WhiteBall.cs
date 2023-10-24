using _Scripts.Game.Player;
using Unity.Netcode;
using UnityEngine;

public class WhiteBall : NetworkBehaviour
{
    public enum State
    {
        StartGame,
        InPlay,
        Potted,
        Penalty
    }

    [SerializeField] private GameObject handHint;
    [SerializeField] private LayerMask tableLayer;
    [SerializeField] private Vector2 min;
    [SerializeField] private Vector2 max;

    public NetworkVariable<bool> ShouldFreeze = new NetworkVariable<bool>();
    public PoolPlayer playerInControl;
    public bool isDraggable;

    public NetworkVariable<State> state = new NetworkVariable<State>();
    private Rigidbody rb;
    private GameObject startArea;
    private bool isDragging;
    private static WhiteBall instance;
    public static WhiteBall Instance
    {
        get
        {
            if (instance != null) return instance;

            instance = FindObjectOfType<WhiteBall>();

            if (instance == null)
            {
                return null;
            }

            return instance;
        }
    }

    public override void OnNetworkSpawn()
    {
        if (IsClient)
        {
            startArea = GameObject.FindWithTag("StartArea");
        }

        rb = GetComponent<Rigidbody>();
        ShouldFreeze.OnValueChanged += HandleStateChanged;
        state.Value = State.StartGame;
    }

    private void OnMouseDown()
    {
        if (!IsClient || !IsOwner) return;

        isDragging = true;

        handHint.SetActive(false);
        startArea.transform.GetChild(0).gameObject.SetActive(true);
    }

    private void OnMouseUp()
    {
        if (!IsClient || !IsOwner) return;

        isDragging = false;
        startArea.transform.GetChild(0).gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!IsOwner) return;

        switch (state.Value)
        {
            case State.StartGame:
                StartGameLogic();
                break;
            case State.InPlay:
                //do something when in state of playing
                break;
        }
    }

    private void StartGameLogic()
    {
        if (!isDragging)
            return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, float.PositiveInfinity, tableLayer))
        {
            float x = Mathf.Clamp(hit.point.x, min.x, max.x);
            float y = Mathf.Clamp(hit.point.z, min.y, max.y);

            transform.position = new Vector3(x, hit.point.y, y);
        }
    }

    [ClientRpc]
    public void ShowHintClientRpc(bool activityState, ClientRpcParams clientRpcParams = default)
    {
        handHint.SetActive(activityState);
    }

    public void HandleStateChanged(bool ignoreValue, bool shouldFreeze)
    {
        if (shouldFreeze)
        {
            rb.constraints = RigidbodyConstraints.FreezePositionX |
                             RigidbodyConstraints.FreezePositionZ |
                             RigidbodyConstraints.FreezeRotation;
        }
        else
        {
            rb.constraints = RigidbodyConstraints.None |
                             RigidbodyConstraints.FreezePositionY;
        }
    }

    public void Shoot()
    {
        if (!IsClient) return;

        GetComponent<Rigidbody>().AddForce(transform.right * 20f, ForceMode.Impulse);

        if (state.Value == State.StartGame)
        {
            UpdateStateServerRpc(State.InPlay);
        }
    }

    [ServerRpc]
    private void UpdateStateServerRpc(State s)
    {
        state.Value = s;
    }
}
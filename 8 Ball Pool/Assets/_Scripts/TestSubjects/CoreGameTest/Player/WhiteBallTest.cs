using System;
using _Scripts.Game.CoreGame;
using _Scripts.TestSubjects.CoreGameTest.Table;
using Unity.Netcode;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

namespace _Scripts.TestSubjects.CoreGameTest.Player
{
    public class WhiteBallTest : NetworkBehaviour
    {
        [Header("References")]
        [SerializeField] private NetworkObject nObj;
        [SerializeField] private Rigidbody rb;
        [SerializeField] private GameObject inHandIndicator;
        [SerializeField] private CustomSlider slider;

        public GameObject handHintTest;

        [Header("Settings")]
        [SerializeField] private float power;
        [SerializeField] private Vector2 min;
        [SerializeField] private Vector2 max;
        [SerializeField] private LayerMask tableLayer;
        [SerializeField] private LayerMask ballLayer;
        [SerializeField] private LayerMask inHandLayer;

        [Header("Runtime Variables")]
        public BallTest MyBall;
        public int TouchCounter;
        public BallType TargetBall;
        public Foul Fouled;

        public Action<bool> OnBallInHand;

        private NetworkVariable<bool> isDraggable = new NetworkVariable<bool>();
        private NetworkVariable<bool> IsDragging = new NetworkVariable<bool>();

        private GameObject startArea;
        private bool shotStarted;
        private Vector3 positionVector;

        private delegate void OnStateChanged();

        private OnStateChanged onGrab;
        private OnStateChanged onDrop;
        private OnStateChanged onHeld;

        private Camera mainCamera;
        private float radius;
        private Vector3 backToPosition;
        private bool canSet;

        private bool checkUpdate;


        public override void OnNetworkSpawn()
        {
            if (IsClient)
            {
                radius = GetComponent<SphereCollider>().radius * transform.lossyScale.x;

                IsDragging.OnValueChanged += HandleIsDragging;

                startArea = GameObject.FindWithTag("StartArea").transform.GetChild(0).gameObject;

                mainCamera = Camera.main;

                onGrab = () =>
                {
                    if (!IsClient || !IsOwner) return;
                    if (!isDraggable.Value) return;

                    OnBallInHand?.Invoke(false);
                    inHandIndicator.SetActive(true);
                    SetIsDraggingServerRpc(true);

                    if (handHintTest.activeSelf) handHintTest.SetActive(false);
                    startArea.SetActive(true);
                };

                onHeld = () =>
                {
                    Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

                    RaycastHit hit;

                    if (Physics.Raycast(ray, out hit, float.PositiveInfinity, tableLayer))
                    {
                        float x = Mathf.Clamp(hit.point.x, min.x, max.x);
                        float y = Mathf.Clamp(hit.point.z, min.y, max.y);

                        positionVector.x = x;
                        positionVector.z = y;
                        positionVector.y = transform.position.y;

                        MoveOnServerRpc(positionVector);
                    }
                };

                onDrop = () =>
                {
                    if (!IsClient || !IsOwner) return;
                    if (!isDraggable.Value) return;

                    inHandIndicator.SetActive(false);
                    startArea.SetActive(false);
                    SetIsDraggingServerRpc(false);
                    OnBallInHand?.Invoke(true);
                };
            }
        }

        private void HandleIsDragging(bool error, bool isDragging)
        {
            if (IsOwner) return;

            handHintTest.SetActive(isDragging);
        }

        private void OnMouseDown()
        {
            onGrab?.Invoke();
        }

        private void Update()
        {
            if (!IsDragging.Value || !isDraggable.Value) return;

            onHeld?.Invoke();
        }

        private void OnMouseUp()
        {
            onDrop?.Invoke();
        }

        private void OnCollisionEnter(Collision col)
        {
            if (!IsServer) return;

            if (Fouled != Foul.NoFoul) return;

            var ball = col.gameObject.GetComponent<BallTest>();

            if (ball)
            {
                if (TouchCounter != 0) return;

                if (TargetBall != BallType.NotSet)
                {
                    if (ball.BallType.Value != TargetBall) Fouled = Foul.WrongColor;
                }

                TouchCounter++;
            }
            else if (col.transform.CompareTag("Bumpers"))
            {
                if (TouchCounter > 0) MyBall.HitBumper = true;
            }
        }

        public void MoveToCenter()
        {
            transform.position = new Vector3(0, -0.633f, 0);
        }

        public void ApplyCueForce(Vector3 direction)
        {
            rb.AddForce(direction * power * slider.Value, ForceMode.Force);
        }

        public void SetDraggable(bool state)
        {
            isDraggable.Value = state;
        }

        public void UpdateDelegates()
        {
            Debug.Log("<color=green>WE GOT UPDATED DELEGATES</color>");

            onGrab = () =>
            {
                if (!IsClient || !IsOwner) return;
                if (!isDraggable.Value) return;

                checkUpdate = false;
                backToPosition = transform.position;
                if (handHintTest.activeSelf) handHintTest.SetActive(false);
                inHandIndicator.SetActive(true);
                SetIsDraggingServerRpc(true);
                OnBallInHand?.Invoke(false);
            };

            onHeld = () =>
            {
                var tPosition = transform.position;

                Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

                RaycastHit hit;

                var balls = Physics.SphereCastAll(tPosition, radius * 2,
                    Vector3.up, 0, ballLayer);

                canSet = balls.Length == 0;

                Debug.Log($"CanSet variable is {canSet}");

                if (Physics.Raycast(ray, out hit, float.PositiveInfinity, tableLayer))
                {
                    positionVector.x = hit.point.x;
                    positionVector.z = hit.point.z;
                    positionVector.y = tPosition.y;

                    if (checkUpdate) return;
                    MoveOnServerRpc(positionVector);
                }
            };

            onDrop = () =>
            {
                if (!IsClient || !IsOwner) return;
                if (!isDraggable.Value) return;

                inHandIndicator.SetActive(false);
                OnBallInHand?.Invoke(true);

                MoveOnServerRpc(canSet ? positionVector : backToPosition);
                checkUpdate = true;
                SetIsDraggingServerRpc(false);
            };
        }

        [ServerRpc]
        private void MoveOnServerRpc(Vector3 newPos)
        {
            transform.position = newPos;
        }

        [ServerRpc]
        private void SetIsDraggingServerRpc(bool state)
        {
            IsDragging.Value = state;

            int layer = state ? LayerMask.NameToLayer("InHand") : LayerMask.NameToLayer("Default");

            gameObject.layer = layer;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, radius * 2);
        }
    }


    public enum Foul
    {
        NoFoul,
        WrongColor,
        NoContact,
        NoBumpers,
        Potted,
        TimedOut
    }
}
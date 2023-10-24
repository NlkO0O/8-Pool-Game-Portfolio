using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;


namespace _Scripts.Game.Player
{
    public class CueController : NetworkBehaviour
    {
        [Header("References")]
        [SerializeField] private SpriteRenderer cueSprite;
        [SerializeField] private CustomSlider cueSlider;
        [SerializeField] private Trajectory trajectory;

        [Header("Settings")]
        [SerializeField] private float gap;

        private const string WHITEBALL = "WhiteBall";

        private GameObject _ball;
        private Camera _mainCamera;

        private Vector3 touchStartPos;
        private Vector3 touchEndPos;
        private bool isRotating;
        private Vector3 startPos;
        private Vector3 endPos;
        private float powerMultiplier;

        private static CueController instance;
        public static CueController Instance
        {
            get
            {
                if (instance != null) return instance;

                instance = FindObjectOfType<CueController>();

                if (instance == null)
                {
                    return null;
                }

                return instance;
            }
        }

        public override void OnNetworkSpawn()
        {
            if (!IsClient) return;

            _mainCamera = Camera.main;
            //cueSlider.OnValueChanged += HandleValueChanged;
        }

        private void HandleValueChanged(float value)
        {
            cueSprite.transform.position = Vector3.Lerp(startPos, endPos, value);
        }

        public float rotationSpeed = 30f;

        private void Update()
        {
            if (!IsOwner) return;

            if (_ball == null) return;

            float rotateAmount = 0f;

            if (Input.GetKey(KeyCode.W))
            {
                rotateAmount = rotationSpeed * Time.deltaTime;
            }
            else if (Input.GetKey(KeyCode.S))
            {
                rotateAmount = -rotationSpeed * Time.deltaTime;
            }

            cueSprite.transform.RotateAround(_ball.transform.position, Vector3.up, rotateAmount);


            // return;
            //
            // if (!IsOwner || !IsClient) return;
            //
            // if (Input.touchCount > 0)
            // {
            //     Touch touch = Input.GetTouch(0);
            //
            //     if (touch.phase == TouchPhase.Began)
            //     {
            //         touchStartPos = touch.position;
            //
            //         touchStartPos.z = 10f;
            //
            //         isRotating = true;
            //     }
            //
            //     else if (touch.phase == TouchPhase.Moved && isRotating)
            //     {
            //         touchEndPos = touch.position;
            //
            //         float angleDelta = GetAngle(touchStartPos) - GetAngle(touchEndPos);
            //
            //         transform.RotateAround(_ball.transform.position, Vector3.forward, angleDelta);
            //
            //         touchStartPos = touchEndPos;
            //     }
            //
            //     else if (touch.phase == TouchPhase.Ended)
            //     {
            //         isRotating = false;
            //     }
            //  }
        }

        private float GetAngle(Vector3 touchPos)
        {
            touchPos.z = 10;

            float angleDelta;

            Vector2 touchPosWorld = _mainCamera.ScreenToWorldPoint(touchPos);

            Vector2 targetVector = touchPosWorld - (Vector2)_ball.transform.position;

            float dot = Vector3.Dot(targetVector.normalized, Vector2.right);

            float angleRadians = Mathf.Acos(Mathf.Clamp(dot, -1f, 1f));

            if (touchPosWorld.normalized.y < 0)
            {
                angleDelta = 360 - angleRadians * Mathf.Rad2Deg;
            }
            else
            {
                angleDelta = angleRadians * Mathf.Rad2Deg;
            }

            return -angleDelta;
        }

        public void SetUpLerp()
        {
            if (!IsOwner) return;

            startPos = _ball.transform.position;
            endPos = _ball.transform.position - cueSprite.transform.right * gap;
        }

        [ClientRpc]
        public void ActivateCueClientRpc(bool state)
        {
            _ball = GameObject.FindWithTag(WHITEBALL);
            cueSprite.enabled = state;
            cueSprite.transform.position = _ball.transform.position;
            trajectory.gameObject.SetActive(true);

            if (!IsOwner)
            {
                var spriteAlpha = cueSprite.color;
                spriteAlpha.a = 0.5f;
                cueSprite.color = spriteAlpha;
               // trajectory.SetAlpha(0.5f);
                return;
            }

            cueSlider.gameObject.SetActive(state);
        }

        public void Shoot()
        {
            _ball.GetComponent<WhiteBall>().Shoot();
        }
    }
}
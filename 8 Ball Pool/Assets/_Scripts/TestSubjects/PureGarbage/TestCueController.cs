using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace _Scripts.TestSubjects.CoreGameTest
{
    public class TestCueController : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer cueSprite;
        [SerializeField] private float rotationSpeed = 5;
        [SerializeField] private Slider slider;
        [SerializeField] private float gap;
        [SerializeField] private float power;

        private Vector3 touchStartPos;
        private Vector3 touchEndPos;

        private GameObject ball;
        private const string WHITEBALL = "WhiteBall";
        private Camera _mainCamera;

        private Vector3 startPos;
        private Vector3 endPos;
        private float powerMultiplaier;


        private void Start()
        {
            _mainCamera = Camera.main;
            ball = GameObject.FindWithTag(WHITEBALL);
            cueSprite.transform.position = ball.transform.position;
        }

        private void Update()
        {
            float rotateAmount = 0f;

            if (Input.GetKey(KeyCode.W))
            {
                rotateAmount = rotationSpeed * Time.deltaTime;
            }
            else if (Input.GetKey(KeyCode.S))
            {
                rotateAmount = -rotationSpeed * Time.deltaTime;
            }

            cueSprite.transform.RotateAround(ball.transform.position, Vector3.up, rotateAmount);

            cueSprite.transform.position = ball.transform.position;
            //     return;
            //     
            //     if (Input.touchCount > 0)
            //     {
            //         Touch touch = Input.GetTouch(0);
            //
            //         if (touch.phase == TouchPhase.Began)
            //         {
            //             touchStartPos = touch.position;
            //
            //             touchStartPos.z = 10f;
            //
            //             isRotating = true;
            //         }
            //
            //         else if (touch.phase == TouchPhase.Moved && isRotating)
            //         {
            //             touchEndPos = touch.position;
            //
            //             float angleDelta = GetAngle(touchStartPos) - GetAngle(touchEndPos);
            //
            //             cueSprite.transform.RotateAround(ball.transform.position, Vector3.down, angleDelta);
            //
            //             touchStartPos = touchEndPos;
            //         }
            //
            //         else if (touch.phase == TouchPhase.Ended)
            //         {
            //             isRotating = false;
            //         }
            //     }
            // }
            //
            // private float GetAngle(Vector3 touchPos)
            // {
            //     touchPos.z = 10;
            //
            //     float angleDelta;
            //
            //     Vector3 touchPosWorld = _mainCamera.ScreenToWorldPoint(touchPos);
            //
            //     Vector3 targetVector = touchPosWorld - ball.transform.position;
            //
            //     float dot = Vector3.Dot(targetVector.normalized, Vector3.right);
            //
            //     float angleRadians = Mathf.Acos(Mathf.Clamp(dot, -1f, 1f));
            //
            //     if (touchPosWorld.normalized.z < 0)
            //     {
            //         angleDelta = 360 - angleRadians * Mathf.Rad2Deg;
            //     }
            //     else
            //     {
            //         angleDelta = angleRadians * Mathf.Rad2Deg;
            //     }
            //
            //     return -angleDelta;
        }

        public void SetAlpha(bool isClient)
        {
            if (!isClient) return;

            var spriteAlpha = cueSprite.color;
            spriteAlpha.a = 0.5f;
            cueSprite.color = spriteAlpha;
        }

        public void Shoot()
        {
            ball.GetComponent<Rigidbody>().AddForce(cueSprite.transform.right * power, ForceMode.Force);
        }
    }
}
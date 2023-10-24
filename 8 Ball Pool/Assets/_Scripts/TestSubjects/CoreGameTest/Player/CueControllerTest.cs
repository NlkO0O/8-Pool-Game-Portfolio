using UnityEngine;

namespace _Scripts.TestSubjects.CoreGameTest.Player
{
    public class CueControllerTest : MonoBehaviour
    {
        [SerializeField] private float rotationSpeed;
        [SerializeField] public Transform cue;
        [SerializeField] private Transform whiteBall;
        [SerializeField] private SpriteRenderer spriteRenderer;
        private bool canMove;

        void Update()
        {
            if (!canMove) return;

            float rotateAmount = 0f;

            if (Input.GetKey(KeyCode.W))
            {
                rotateAmount = rotationSpeed * Time.deltaTime;
            }
            else if (Input.GetKey(KeyCode.S))
            {
                rotateAmount = -rotationSpeed * Time.deltaTime;
            }

            cue.transform.RotateAround(whiteBall.transform.position, Vector3.up, rotateAmount);

            cue.position = whiteBall.position;
        }

        public void Enable(bool isOwner)
        {
            cue.position = whiteBall.position;
            spriteRenderer.enabled = isOwner;
            canMove = isOwner;
        }

        public void SetAlpha(bool isOwner)
        {
            var spriteAlpha = spriteRenderer.color;
            spriteAlpha.a = isOwner ? 1 : 0.5f;
            spriteRenderer.color = spriteAlpha;
        }
    }
}
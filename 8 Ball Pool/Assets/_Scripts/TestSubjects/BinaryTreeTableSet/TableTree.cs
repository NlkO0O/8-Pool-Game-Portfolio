using UnityEngine;

namespace _Scripts.TestSubjects.BinaryTreeTableSet
{
    public class TableTree : MonoBehaviour
    {
        [SerializeField] private GameObject ball;
        [SerializeField] private Rigidbody[] ballsRigids;
        public BallNode RootBallNode;

        // private async void Start()
        // {
        //     float sphereScale = ball.transform.localScale.x;
        //
        //     float diameter = sphereScale;
        //
        //     await Task.Delay(1000);
        //
        //     Insert(new Vector3(0, -0.11f, 0));
        //
        //     await Task.Delay(1000);
        //
        //     Insert(new Vector3(0 + diameter * Mathf.Sin(Mathf.Deg2Rad * 60), -0.11f, 0 + diameter / 2));
        //
        //     await Task.Delay(1000);
        //
        //     Insert(new Vector3(0 + diameter * Mathf.Sin(Mathf.Deg2Rad * 60), -0.11f, 0 - diameter / 2));
        // }
        //
        // public void Insert(Vector3 data)
        // {
        //     RootBallNode = InsertRec(RootBallNode, data);
        // }
        //
        // private BallNode InsertRec(BallNode root, Vector3 data)
        // {
        //     if (root == null)
        //     {
        //         root = Instantiate(ball, data, Quaternion.identity)
        //             .GetComponent<BallNode>();
        //
        //         return root;
        //     }
        //
        //     if (data.z < root.transform.position.z)
        //     {
        //         root.Left = InsertRec(root.Left, data);
        //     }
        //     else if (data.z > root.transform.position.z)
        //     {
        //         root.Right = InsertRec(root.Right, data);
        //     }
        //
        //     return root;
        // }


        private void Update()
        {
            foreach (var b in ballsRigids)
            {
                if (b.GetComponent<Rigidbody>().velocity != Vector3.zero)
                {
                    // Debug.Log("balls are moving");
                    return;
                }
            }

            //  Debug.Log("stopped");
        }
    }
}
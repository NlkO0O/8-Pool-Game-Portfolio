using UnityEngine;

namespace _Scripts.TestSubjects.BinaryTreeTableSet
{
    public class Table_test : MonoBehaviour
    {
        [Header("Positions")]
        [SerializeField] private GameObject sphere;
        [SerializeField] private GameObject sphere2;

        void Start()
        {
        
        
            float sphereScale = sphere.transform.localScale.x;

            float diameter = sphereScale;

            sphere.transform.position = new Vector3(sphere.transform.position.x + diameter * Mathf.Sin(Mathf.Deg2Rad * 60),
                sphere.transform.position.y,
                sphere.transform.position.z + diameter / 2);


            float sphereScale2 = sphere2.transform.localScale.x;

            float diameter2 = sphereScale;

            sphere2.transform.position = new Vector3(
                sphere2.transform.position.x + diameter2 * Mathf.Sin(Mathf.Deg2Rad * 60),
                sphere2.transform.position.y,
                sphere2.transform.position.z - diameter2 / 2);
        }
    }
}
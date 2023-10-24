using Unity.Netcode;
using UnityEngine;

namespace _Scripts.Utils
{
    public static class MaterialSetter
    {
        public static void SetMaterial(NetworkObject obj, Texture2D texture)
        {
            Material material = new Material(Shader.Find("Standard"));

            material.mainTexture = texture;

            obj.GetComponent<Renderer>().material = material;
        }

        public static void SetMaterial(GameObject obj, Texture2D texture)
        {
            Material material = new Material(Shader.Find("Standard"));

            material.mainTexture = texture;

            obj.GetComponent<Renderer>().material = material;
        }
    }
}
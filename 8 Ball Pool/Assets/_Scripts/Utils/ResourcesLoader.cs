using System.Collections.Generic;
using UnityEngine;

namespace _Scripts.Utils
{
    public static class ResourcesLoader
    {
        private const string TexturesPath = "BallsTextures";

        private static P[] GetArray<P>(string folderPath) where P : UnityEngine.Object
        {
            return Resources.LoadAll<P>(folderPath);
        }

        public static Dictionary<int, Texture2D> GetTexturesDictionary()
        {
            Texture2D[] texturesArray = GetArray<Texture2D>(TexturesPath);

            Dictionary<int, Texture2D> temp = new Dictionary<int, Texture2D>();

            foreach (var t in texturesArray)
            {
                int index = int.Parse(t.name.Split("ballTexture")[1]);
                temp.Add(index, t);
            }

            return temp;
        }
    }
}
using System.IO;
using UnityEngine;
using UnityEditor;

namespace ImersaoVisual.Editor
{
    public class EditorUtils
    {
        public static void SaveAsset(string path, Object asset)
        {
            AssetDatabase.CreateAsset(asset, AssetDatabase.GenerateUniqueAssetPath(path));
        }

        public static void SaveAssetOnCurrentPath(Object asset, string name)
        {
            AssetDatabase.CreateAsset(asset, AssetDatabase.GenerateUniqueAssetPath(GetSelectedFolderPath() + "/" + name));

            EditorUtility.FocusProjectWindow();

            Selection.activeObject = asset;
        }

        public static string GetSelectedFolderPath()
        {
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);

            if (path == "")
            {
                path = "Assets";
            }
            else if (Path.GetExtension(path) != "")
            {
                path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
            }

            return path;
        }

        public static bool IsAPrefab(Object obj)
        {
            return PrefabUtility.GetPrefabParent(obj) == null && PrefabUtility.GetPrefabObject(obj) != null;
        }
    }

}
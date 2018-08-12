#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

public class MakeScriptableObject
{
    [MenuItem("Assets/Create/It")]
    public static void CreateMyAsset()
    {
        AudioList asset = ScriptableObject.CreateInstance<AudioList>();

        AssetDatabase.CreateAsset(asset, "Assets/Grunts.asset");
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();

        Selection.activeObject = asset;
    }
}
#endif
#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

public class MakeScriptableObject
{
    [MenuItem("Assets/Create/It")]
    public static void CreateMyAsset()
    {
        PieceSet asset = ScriptableObject.CreateInstance<PieceSet>();

        AssetDatabase.CreateAsset(asset, "Assets/PieceSet.asset");
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();

        Selection.activeObject = asset;
    }
}
#endif
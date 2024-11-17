using UnityEngine;
using UnityEditor;
using System.IO;

public class AddCollidersToPrefabs : EditorWindow
{
    [MenuItem("Tools/Add Colliders to Prefabs")]
    public static void ShowWindow()
    {
        GetWindow<AddCollidersToPrefabs>("Add Colliders to Prefabs");
    }

    private string folderPath = "Assets/Prefabs"; // Path to the folder containing your prefabs

    void OnGUI()
    {
        GUILayout.Label("Add Colliders to Prefabs", EditorStyles.boldLabel);
        folderPath = EditorGUILayout.TextField("Prefabs Folder", folderPath);

        if (GUILayout.Button("Add Colliders"))
        {
            AddColliders();
        }
    }

    void AddColliders()
    {
        if (string.IsNullOrEmpty(folderPath))
        {
            Debug.LogError("Folder path is empty.");
            return;
        }

        string[] guids = AssetDatabase.FindAssets("t:Prefab", new[] { folderPath });
        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);

            if (prefab != null)
            {
                bool colliderAdded = false;
                if (prefab.GetComponent<Collider>() == null)
                {
                    // Add a BoxCollider by default. Adjust as necessary.
                    prefab.AddComponent<BoxCollider>();
                    colliderAdded = true;
                }

                if (colliderAdded)
                {
                    Debug.Log($"Added collider to {prefab.name}");
                    PrefabUtility.SavePrefabAsset(prefab);
                }
            }
        }

        AssetDatabase.Refresh();
        Debug.Log("Finished adding colliders.");
    }
}

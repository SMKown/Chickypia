using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MeshMerger : MonoBehaviour
{
    void Start()
    {
        MeshFilter[] meshFilters = FindObjectsOfType<MeshFilter>();
        Mesh combinedMesh = new Mesh();

        CombineInstance[] combineInstances = new CombineInstance[meshFilters.Length];
        for (int i = 0; i < meshFilters.Length; i++)
        {
            combineInstances[i].mesh = meshFilters[i].sharedMesh;
            combineInstances[i].transform = meshFilters[i].transform.localToWorldMatrix;
        }
        combinedMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        combinedMesh.CombineMeshes(combineInstances, true, true);

        // ���ο� GameObject ����
        GameObject newGameObject = new GameObject("Merged Mesh");
        MeshFilter meshFilter = newGameObject.AddComponent<MeshFilter>();
        meshFilter.mesh = combinedMesh; // ���յ� �޽��� �Ҵ�

        newGameObject.AddComponent<MeshRenderer>();

#if UNITY_EDITOR
        // �޽� �ڻ����� ����
        string meshPath = "Assets/MergedMesh.asset";
        AssetDatabase.CreateAsset(combinedMesh, meshPath);
        AssetDatabase.SaveAssets();

        // ������ ����
        string prefabPath = "Assets/MergedMesh.prefab"; // ������ ��� ����
        PrefabUtility.SaveAsPrefabAsset(newGameObject, prefabPath);
#endif

        // ������ GameObject ����
        DestroyImmediate(newGameObject);
    }
}

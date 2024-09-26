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

        // 새로운 GameObject 생성
        GameObject newGameObject = new GameObject("Merged Mesh");
        MeshFilter meshFilter = newGameObject.AddComponent<MeshFilter>();
        meshFilter.mesh = combinedMesh; // 결합된 메쉬를 할당

        newGameObject.AddComponent<MeshRenderer>();

#if UNITY_EDITOR
        // 메쉬 자산으로 저장
        string meshPath = "Assets/MergedMesh.asset";
        AssetDatabase.CreateAsset(combinedMesh, meshPath);
        AssetDatabase.SaveAssets();

        // 프리팹 저장
        string prefabPath = "Assets/MergedMesh.prefab"; // 프리팹 경로 설정
        PrefabUtility.SaveAsPrefabAsset(newGameObject, prefabPath);
#endif

        // 생성한 GameObject 삭제
        DestroyImmediate(newGameObject);
    }
}

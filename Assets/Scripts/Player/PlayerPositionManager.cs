using System.Collections.Generic;
using UnityEngine;

public class PlayerPositionManager : MonoBehaviour
{
    public static PlayerPositionManager instance;
    private Dictionary<string, Vector3> lastPositions = new Dictionary<string, Vector3>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SavePosition(string sceneName, Vector3 position)
    {
        lastPositions[sceneName] = position;
    }

    public Vector3 GetSavedPosition(string sceneName)
    {
        return lastPositions.ContainsKey(sceneName) ? lastPositions[sceneName] : Vector3.zero;
    }
}
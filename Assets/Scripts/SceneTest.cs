using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneTest : MonoBehaviour
{
    private SceneLoader SceneLoader;
    private void Awake()
    {
        SceneLoader = GetComponent<SceneLoader>();
    }
    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            SceneLoader.VillageScene();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SceneLoader.CustomScene();
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SceneLoader.FishingScene();
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SceneLoader.Flame01();
        }
    }
}

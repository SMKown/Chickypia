using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameObject option;
    public GameObject soundOption;

    public GameObject M_option;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleOptionMenu();
        }
    }

    #region 게임씬 옵션창
    private void ToggleOptionMenu()
    {
        option.SetActive(!option.activeSelf);
    }

    public void OpenOptionButton()
    {
        option.SetActive(true);
    }

    public void CloseOptionButton()
    {
        option.SetActive(false);
    }
    #endregion

    #region 메인씬 옵션창

    public void M_OpenOptionButton()
    {
        M_option.SetActive(true);
    }

    public void M_CloseOptionButton()
    {
        M_option.SetActive(false);
    }
    #endregion
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [Header("���Ӿ� �ɼ�")]
    public GameObject option;
    public GameObject soundOption;
    [Header("���ξ� �ɼ�")]
    public GameObject M_option;

    public bool isOptionActive = false;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleOptionMenu();
        }

        if (isOptionActive)
        {
            if (EventSystem.current != null && EventSystem.current.currentSelectedGameObject == null)
            {
                Input.ResetInputAxes();
            }
        }
    }

    #region ���Ӿ� �ɼ�â
    private void ToggleOptionMenu()
    {
        //option.SetActive(!option.activeSelf);
        isOptionActive = !isOptionActive;
        option.SetActive(isOptionActive);

        if (isOptionActive)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

    public void OpenOptionButton()
    {
        isOptionActive = true;
        option.SetActive(true);
    }

    public void CloseOptionButton()
    {
        isOptionActive = false;
        option.SetActive(false);
    }
    #endregion

    #region ���ξ� �ɼ�â

    public void M_OpenOptionButton()
    {
        isOptionActive = true;
        M_option.SetActive(true);
    }

    public void M_CloseOptionButton()
    {
        isOptionActive = false;
        M_option.SetActive(false);
    }
    #endregion
}
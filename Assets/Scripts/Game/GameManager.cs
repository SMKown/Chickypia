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
    public GameObject ClickPanel;
    [Header("���ξ� �ɼ�")]
    public GameObject M_option;
    public GameObject License;
    [Header("����Ű")]
    public GameObject open_Button;
    public GameObject operationKey;

    [HideInInspector]public bool isOptionActive = false;
    [HideInInspector]public bool isOperationKeys = false;

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
    }

    #region ����Ű
    public void OpenOperationKeys()
    {
        isOperationKeys = true;
        operationKey.SetActive(true);
        open_Button.SetActive(true);
    }

    public void CloseOperationKeys()
    {
        isOperationKeys = false;
        operationKey.SetActive(false);
    }
    #endregion

    #region ���Ӿ� �ɼ�â
    private void ToggleOptionMenu()
    {
        //option.SetActive(!option.activeSelf);
        isOptionActive = !isOptionActive;
        option.SetActive(isOptionActive);
        ClickPanel.SetActive(isOptionActive);
        if (isOptionActive)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

    public void OpenOptionButton()
    {
        isOptionActive = true;
        option.SetActive(true);
        ClickPanel.SetActive(true);
    }

    public void CloseOptionButton()
    {
        isOptionActive = false;
        option.SetActive(false);
        ClickPanel.SetActive(false);
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
    public void LicenseButton()
    {
        License.SetActive(true);
    }
    public void Close_LicenseButton()
    {
        License.SetActive(false);
    }
    #endregion
}
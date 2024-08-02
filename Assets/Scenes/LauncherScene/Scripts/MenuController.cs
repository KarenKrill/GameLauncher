using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [SerializeField]
    Button _exitButton;
    void OnExitRequest()
    {
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }
    private void OnEnable()
    {
        if (_exitButton != null)
        {
            _exitButton.onClick.AddListener(OnExitRequest);
        }
    }
    private void OnDisable()
    {
        if (_exitButton != null)
        {
            _exitButton.onClick.RemoveListener(OnExitRequest);
        }
    }
}

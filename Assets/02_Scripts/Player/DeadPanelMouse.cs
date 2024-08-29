using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DeadPanelMouse : MonoBehaviour
{
    public GameObject mouse;
    MouseCursorMove mouseCursor;

    private void Awake()
    {
        mouseCursor = mouse.GetComponent<MouseCursorMove>();    
    }


    private void OnEnable()
    {
        mouseCursor.ShowCursor();               // UI 패널 켜지면 마우스 커서 보임
        GameManager.Instance.openUI = true;     // 움직임 제한
    }


    private void OnDisable()
    {
        mouseCursor.HideCursor();                   // UI 패널 꺼지면 마우스 커서 안보임
        GameManager.Instance.openUI = false;        // 움직임 제한
    }
}

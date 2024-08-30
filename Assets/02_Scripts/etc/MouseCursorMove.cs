using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MouseCursorMove : MonoBehaviour
{
    enum MouseState 
    { 
        Lock,
        Confined,
    }

    [SerializeField] static RectTransform cursor;              //화면에 보일커서
    [SerializeField] Image cursorIcon;             //커서의 기본 아이콘

    [SerializeField]MouseState state;                   //현재 마우스 상태
    private void Awake()
    {
        if (cursor == null) { cursor = GetComponent<RectTransform>(); ; }
    }
    void Start()
    {
       
        Cursor.visible = false;                         //커서 비표시
        state = MouseState.Confined;                    //커서 화면내부에 제한
        Cursor.lockState = CursorLockMode.Confined;
        ShowCursor();
    }

    void Update()
    {
        cursor.position = Input.mousePosition;              //UI위치 변화

        //클릭하면
        if (Input.GetMouseButtonDown(0))
        {
            cursorIcon.color = Color.red;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            cursorIcon.color = Color.white;
        }
    }

    //커서가 보이게 하는 메서드
    public static void ShowCursor()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
        cursor.gameObject.SetActive(true);
    }

    //커서를 안보이게하고 중앙에 고정하는 1인칭시 사용할 메서드
    public static void HideCursor()                        
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        cursor.gameObject.SetActive(false);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseCursorMove : MonoBehaviour
{
    enum MouseState { 
        Lock,
        Confined,
    }
    [SerializeField] RectTransform cursor;              //화면에 보일커서
    [SerializeField] GameObject cursorIcon;             //커서의 기본 아이콘
    [SerializeField] GameObject cursorClickIcon;        //클릭했을때 아이콘
    [SerializeField] GameObject cursorClickAbleIcon;    //클릭이 가능한 UI위에 커서가 올라가면 변하는 아이콘

    [SerializeField]MouseState state;                   //현재 마우스 상태
    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;                         //커서 비표시
        state = MouseState.Confined;                    //커서 화면내부에 제한
        Cursor.lockState = CursorLockMode.Confined;
        
    }

    // Update is called once per frame
    void Update()
    {
        cursor.position = Input.mousePosition;              //UI위치 변화
        if (EventSystem.current.IsPointerOverGameObject())  //ui위에 있으면
        {
            cursorClickAbleIcon.SetActive(true);
        }
        else {
            cursorClickAbleIcon.SetActive(false);
        }
        if (Input.GetMouseButtonDown(0)) {                  //클릭하면
            cursorClickIcon.gameObject.SetActive(true);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            cursorClickIcon.gameObject.SetActive(false);
        }

    }

    public void ShowCursor() {                      //커서가 보이게 하는 메서드
        Cursor.lockState = CursorLockMode.Confined;
        cursor.gameObject.SetActive(true);
    }
    public void HideCursor()                        //커서를 안보이게하고 중앙에 고정하는 1인칭시 사용할 메서드
    {
        Cursor.lockState = CursorLockMode.Locked;
        cursor.gameObject.SetActive(false);
    }
}

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
    [SerializeField] RectTransform cursor;              //ȭ�鿡 ����Ŀ��
    [SerializeField] GameObject cursorIcon;             //Ŀ���� �⺻ ������
    [SerializeField] GameObject cursorClickIcon;        //Ŭ�������� ������
    [SerializeField] GameObject cursorClickAbleIcon;    //Ŭ���� ������ UI���� Ŀ���� �ö󰡸� ���ϴ� ������

    [SerializeField]MouseState state;                   //���� ���콺 ����
    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;                         //Ŀ�� ��ǥ��
        state = MouseState.Confined;                    //Ŀ�� ȭ�鳻�ο� ����
        Cursor.lockState = CursorLockMode.Confined;
        
    }

    // Update is called once per frame
    void Update()
    {
        cursor.position = Input.mousePosition;              //UI��ġ ��ȭ
        if (EventSystem.current.IsPointerOverGameObject())  //ui���� ������
        {
            cursorClickAbleIcon.SetActive(true);
        }
        else {
            cursorClickAbleIcon.SetActive(false);
        }
        if (Input.GetMouseButtonDown(0)) {                  //Ŭ���ϸ�
            cursorClickIcon.gameObject.SetActive(true);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            cursorClickIcon.gameObject.SetActive(false);
        }

    }

    public void ShowCursor() {                      //Ŀ���� ���̰� �ϴ� �޼���
        Cursor.lockState = CursorLockMode.Confined;
        cursor.gameObject.SetActive(true);
    }
    public void HideCursor()                        //Ŀ���� �Ⱥ��̰��ϰ� �߾ӿ� �����ϴ� 1��Ī�� ����� �޼���
    {
        Cursor.lockState = CursorLockMode.Locked;
        cursor.gameObject.SetActive(false);
    }
}

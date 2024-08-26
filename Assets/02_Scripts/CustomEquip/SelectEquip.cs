using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor;

public class SelectEquip : MonoBehaviour
{
    public TextMeshProUGUI titleTxt;            // 선택된 메뉴 이름
    public RectTransform selectMenuImage;       // 메뉴 이름 밑 빨간 밑줄
    public TextMeshProUGUI[] menuTxt;           // 메뉴 이름
    public GameObject[] menuPanel;              // 메뉴 패널
    public Stack<GameObject> selectPanelStack = new Stack<GameObject>();
    public GameObject exitWeaponEquip;
    public GameObject playerCharacter;
    AnimIKPlayer animIkPlayer;
    LoadOut loadOut;
    EquipmentsInit equipmentsInit;


    // 마우스 커서 조작
    public MouseCursorMove mouseCursor;

    private void Awake()
    {
        animIkPlayer = playerCharacter.GetComponent<AnimIKPlayer>();
        loadOut = GetComponent<LoadOut>();
        equipmentsInit = playerCharacter.GetComponent<EquipmentsInit>();
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

    private void Update()
    {
        if (this.gameObject.activeSelf == true)
        {
            // ESC 누르면 패널 오프
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                // 만약 켜져있는 패널이 없으면 무기 장착 씬 나가기
                if (selectPanelStack.Count <= 0)
                {
                    exitWeaponEquip.SetActive(true);
                   
                }
                else
                {
                    ClosePanel();
                }
            }
        }
    }


    // 스택에 추가하고 패널 켜주기
    public void PushPanel(GameObject panel)
    {
        selectPanelStack.Push(panel);
        panel.SetActive(true);
    }

    // 스택에서 빼고 패널 꺼주기
    public void ClosePanel()
    {
        if (selectPanelStack.Count > 0)
        {
            GameObject panel = selectPanelStack.Pop();
            
            panel.SetActive(false);
        }
    }

    // 메뉴 버튼에 연결
    public void SelectMenu(int _num)
    {
        ClosePanel();
        switch (_num)
        {
            case 0:
                titleTxt.text = "로드아웃";
                StartCoroutine(ImageMove(_num,65,110));         // 이동할 위치값과 사이즈값은 직접 확인 후 대입함
                animIkPlayer.currentIkIndex = loadOut.equipMainWeaponIndex;
                loadOut.mainWeaponObject[loadOut.equipMainWeaponList[0]].SetActive(true);
                break;

            case 1:
                titleTxt.text = "커스터마이즈";
                StartCoroutine(ImageMove(_num, 190, 160));      // 이동할 위치값과 사이즈값은 직접 확인 후 대입함
                animIkPlayer.currentIkIndex = 4;
                loadOut.mainWeaponObject[loadOut.equipMainWeaponList[0]].SetActive(false);
                loadOut.mainWeaponObject[loadOut.equipMainWeaponList[1]].SetActive(false);
                break;
        }
    }

    // 버튼 클릭시 패널 및 메뉴 이름 변경
    void ChangeMenu(int _num)
    {
        for(int i = 0; i < menuPanel.Length; i++)
        {
            if(i == _num)
            {
                menuPanel[i].SetActive(true);                                       // 클릭된 버튼에 맞는 패널 오픈
                menuTxt[i].GetComponent<TextMeshProUGUI>().color = Color.white;     // 메뉴 이름 텍스트는 하얀색
            }
            else
            {
                menuPanel[i].SetActive(false);                                      // 다른 버튼은 패널 오프
                menuTxt[i].GetComponent<TextMeshProUGUI>().color = Color.gray;      // 메뉴 이름 텍스트는 회색

            }
        }
    }

    // 버튼 클릭시 빨간 밑줄 이동
    IEnumerator ImageMove(int _num, float _posX, int _width)
    {
        Vector2 _originPos = selectMenuImage.anchoredPosition;          // 빨간 밑줄 원래 포지션
        Vector2 _originSize = selectMenuImage.sizeDelta;                // 빨간 밑줄 원래 사이즈
        Vector2 _targetPos = new Vector2(_posX, _originPos.y);          // 이동할 포지션
        Vector2 _targetWidth = new Vector2(_width, _originSize.y);      // 변경될 사이즈

        float _elapsedTime = 0f;                                        // 경과 시간
        float _duration = 0.2f;                                         // 이동 시간

        while (_elapsedTime < _duration)
        {
            _elapsedTime += Time.deltaTime;
            float _imageMoveSpeed = Mathf.Clamp01(_elapsedTime / _duration);        // 이미지 이동속도

            selectMenuImage.anchoredPosition = Vector2.Lerp(_originPos, _targetPos,_imageMoveSpeed);    // 빨간 밑줄 이동
            selectMenuImage.sizeDelta = Vector2.Lerp(_originSize, _targetWidth,_imageMoveSpeed);        // 빨간 밑줄 사이즈 변경

            yield return null;
        }

        selectMenuImage.anchoredPosition = _targetPos;          // 근사치에 도달하면 지정 값으로 이동
        selectMenuImage.sizeDelta = _targetWidth;               // 근사치에 도달하면 지정 값으로 변경
        ChangeMenu(_num);                                       // 패널 및 이름 색 변경
    }
}

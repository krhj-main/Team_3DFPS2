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
    public GameObject exitWeaponEquip;          // 장비 장착 씬 나갈때 fadeOut 될 패널
    public GameObject playerCharacter;          // 플레이어 캐릭터
    AnimIKPlayer animIkPlayer;                  // 무기 장착 IK
    LoadOut loadOut;                            // LoadOut 스크립트
    EquipmentsInit equipmentsInit;              // EquipmentsInit 스크립트
    public GameObject[] equipMainWeapon;        // 메인웨폰무기 ( 새로 산 에셋 무기 )
    public GameObject equipSpecialWeapon;       // 스페셜무기
    public Stack<GameObject> selectPanelStack = new Stack<GameObject>();        // 켜질 패널들 스택에 담아둠
    Customize customize;

    // 마우스 커서 조작
    public MouseCursorMove mouseCursor;
    

    private void Awake()
    {
        animIkPlayer = playerCharacter.GetComponent<AnimIKPlayer>();
        loadOut = GetComponent<LoadOut>();
        equipmentsInit = GameManager.Instance.inventory.GetComponent<EquipmentsInit>();
        customize = GetComponent<Customize>();
    }

    private void OnEnable()
    {
        MouseCursorMove.ShowCursor();           // UI 패널 켜지면 마우스 커서 보임
        GameManager.Instance.openUI = true;     // 움직임 제한
    }


    private void OnDisable()
    {
        MouseCursorMove.HideCursor();               // UI 패널 꺼지면 마우스 커서 안보임
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
                    ApplyWeaponEquip();
                    customize.ApplyCustom();
                    equipmentsInit.Init();
                    exitWeaponEquip.SetActive(true);
                    GameManager.Instance.openUI = false;
                }
                else
                {
                    ClosePanel();
                }
            }
        }
    }


    #region 무기 실제 장착
    void ApplyWeaponEquip()
    {
        ApplyMainEquip();
        ApplyThrowingEquip();
        ApplySpecialEquip();
    }

    void ApplyMainEquip()
    {
        MainWeapon _weapon;
        for (int i = 0; i < loadOut.equipMainWeaponList.Count; i++)
        {
            _weapon = Instantiate( equipMainWeapon[loadOut.equipMainWeaponList[i]].GetComponent<MainWeapon>());
            equipmentsInit.mainWeapons[i] = _weapon;
        }
    }

    void ApplyThrowingEquip()
    {
        equipmentsInit.frag = loadOut.countThrwing[0];
        equipmentsInit.smoke = loadOut.countThrwing[1];
        equipmentsInit.flash = loadOut.countThrwing[2];
    }

    void ApplySpecialEquip()
    {   
        SpecialWeapon _weapon;
        _weapon = Instantiate(equipSpecialWeapon.GetComponent<SpecialWeapon>());
        equipmentsInit.specialWeapons[0] = _weapon;
    }
    #endregion


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

    #region 메뉴 버튼
    // 메뉴 버튼에 OnClick 연결
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

    // 빨간 밑줄 이동
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

    // 버튼 클릭시 패널 및 메뉴 이름 변경
    void ChangeMenu(int _num)
    {
        for (int i = 0; i < menuPanel.Length; i++)
        {
            if (i == _num)
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
    #endregion

}

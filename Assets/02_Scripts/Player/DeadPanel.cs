using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeadPanel : MonoBehaviour
{
    public Image deadPanel;
    float deadPanelAlpha;
    public GameObject[] deadPanelInUI;
    public TextMeshProUGUI tipTxt;

    public void OnEnable()
    {
        deadPanelAlpha = 0;
        StartCoroutine(DeadPanelFadeOut());

        MouseCursorMove.ShowCursor();               // UI 패널 켜지면 마우스 커서 보임
        GameManager.Instance.openUI = true;     // 움직임 제한
    }

    private void OnDisable()
    {
        MouseCursorMove.HideCursor();                   // UI 패널 꺼지면 마우스 커서 안보임
        GameManager.Instance.openUI = false;        // 움직임 제한
    }

    IEnumerator DeadPanelFadeOut()
    {
        while (deadPanelAlpha <= 1f)
        {
            deadPanelAlpha += Time.deltaTime;
            deadPanel.color = new Color(0, 0, 0, deadPanelAlpha);
            yield return null;
        }

        Time.timeScale = 0;
        ChangeTipText();
        // 검정화면 끝나면 데드패널에 있는 모든 ui 켜줌
        foreach (GameObject ui in deadPanelInUI)
        {
            ui.SetActive(true);
        }
    }

    void ChangeTipText()
    {
        int _index;
        _index = Random.Range(0, 4);

        switch(_index)
        {
            case 0:
                tipTxt.text = "Tip : 건물 안 곳곳에 있는 보급을 적절히 활용하세요";
                break;
            case 1:
                tipTxt.text = "Tip : 드론을 이용해 맵 곳곳을 살펴보세요";
                break;
            case 2:
                tipTxt.text = "Tip : 투척무기의 효과는 각자 다릅니다 전략적으로 이용해보세요";
                break;
            case 3:
                tipTxt.text = "Tip : 가까운 적에게는 샷건이 효과적입니다";
                break;
        }
    }

}

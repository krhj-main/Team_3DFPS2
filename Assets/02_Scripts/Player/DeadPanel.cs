using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeadPanel : MonoBehaviour
{
    public Image deadPanel;
    float deadPanelAlpha;
    public GameObject[] deadPanelInUI;

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

        // 검정화면 끝나면 데드패널에 있는 모든 ui 켜줌
        foreach (GameObject ui in deadPanelInUI)
        {
            ui.SetActive(true);
        }
    }
}

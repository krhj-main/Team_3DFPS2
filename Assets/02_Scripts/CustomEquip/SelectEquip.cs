using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SelectEquip : MonoBehaviour
{
    public TextMeshProUGUI titleTxt;
    public TextMeshProUGUI[] menuTxt;
    public RectTransform selectMenuImage;
    public GameObject[] menuPanel;

    public void SelectMenu(int _num)
    {
        switch (_num)
        {
            case 0:
                titleTxt.text = "로드아웃";
                StartCoroutine(ImageMove(_num,65,110));
                break;

            case 1:
                titleTxt.text = "커스터마이즈";
                StartCoroutine(ImageMove(_num, 190, 160));
                break;
        }
    }

    void OpenMenu(int _num)
    {
        for(int i = 0; i < menuPanel.Length; i++)
        {
            if(i == _num)
            {
                menuPanel[i].SetActive(true);
                menuTxt[i].GetComponent<TextMeshProUGUI>().color = Color.white;
            }
            else
            {
                menuPanel[i].SetActive(false);
                menuTxt[i].GetComponent<TextMeshProUGUI>().color = Color.gray;

            }
        }
    }

    IEnumerator ImageMove(int _num, float _posX, int _width)
    {
        Vector2 _originPos = selectMenuImage.anchoredPosition;
        Vector2 _originSize = selectMenuImage.sizeDelta;
        Vector2 _targetPos = new Vector2(_posX, _originPos.y);
        Vector2 _targetWidth = new Vector2(_width, _originSize.y);

        float _elapsedTime = 0f;
        float _duration = 0.2f;

        while (_elapsedTime < _duration)
        {
            _elapsedTime += Time.deltaTime;
            float _imageMoveSpeed = Mathf.Clamp01(_elapsedTime / _duration);

            selectMenuImage.anchoredPosition = Vector2.Lerp(_originPos, _targetPos,_imageMoveSpeed);
            selectMenuImage.sizeDelta = Vector2.Lerp(_originSize, _targetWidth,_imageMoveSpeed);

            yield return null;
        }

        selectMenuImage.anchoredPosition = _targetPos;
        selectMenuImage.sizeDelta = _targetWidth;
        OpenMenu(_num);
    }
}

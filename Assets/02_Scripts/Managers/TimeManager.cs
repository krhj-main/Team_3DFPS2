using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimeManager : MonoBehaviour
{
    int clearTime;
    public static int timeScore = 500;

    private void Start()
    {
        GameManager.Instance.stopTime = false;
        StartCoroutine(OverTime());
        StartCoroutine(CalculateTimeScore());
    }

    IEnumerator OverTime()
    {
        while (!GameManager.Instance.stopTime)
        {
            GameManager.Instance.OverTime();
            yield return null;
        }
        clearTime = Mathf.RoundToInt(GameManager.Instance.missionTime);
        GameManager.Instance.clearTime = clearTime;
        // ui 매니저에 연결
    }

    IEnumerator CalculateTimeScore()
    {
        GameManager.Instance.clearGoals[0][1] = true;

        yield return new WaitForSeconds(300);

        GameManager.Instance.clearGoals[0][1] = false;

        while (true)
        {
            timeScore -= 10;
            if(timeScore <= 0)
            { 
                break;
            }
            yield return new WaitForSeconds(15f);
        } 
    }
}

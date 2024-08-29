using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public bool clear = false;          // 게임 클리어 되면 true로 변경해줘야함
    float missionTime;
    int clearTime;
    public static int timeScore = 500;                

    private void Start()
    {
        missionTime = 0;
        StartCoroutine(OverTime());
        StartCoroutine(CalculateTimeScore());
    }


    IEnumerator OverTime()
    {
        while (!clear)
        {
            missionTime += Time.deltaTime;
            yield return null;
        }
        clearTime = Mathf.RoundToInt(missionTime);
        GameManager.Instance.clearTime = clearTime;
    }

    IEnumerator CalculateTimeScore()
    {
        yield return new WaitForSeconds(300);

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

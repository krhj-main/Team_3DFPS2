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
        clear = false;
        missionTime = 0;
        StartCoroutine(OverTime());
        StartCoroutine(CalculateTimeScore());
    }

    private void Update()
    {
        if (GameManager.Instance.enemies.Count <= 0)
        {
            GameManager.Instance.clearGoals[0][0] = true;
            clear = true;
            GameManager.Instance.clearpanel.gameObject.SetActive(true);
        }
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

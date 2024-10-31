using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefadsManager : MonoBehaviour
{
    public GameObject timerFrefab;
    private List<GameObject> timerList = new List<GameObject>();

    public void TimerSetting()
    {
        GameObject settingTimer = Instantiate(timerFrefab);
        timerList.Add(settingTimer);
    }

    public void ResetALLTimer()
    {
        foreach (GameObject timer in timerList)
        {
            timer.GetComponent<TimerManager>().ResetTimer();
        }
    }
}

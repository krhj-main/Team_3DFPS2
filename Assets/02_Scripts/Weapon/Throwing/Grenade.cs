using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Grenade 
{
    int duration;
    float lastTime = 0, targetTime=1;
    float escTime = 20;
    GameObject target;
    public Grenade(int _duration,GameObject _target) {
        duration = _duration;
        target = _target;
        InputManger.Instance.keyAction += test;

    }


    public void test()
    {
        escTime -= 0.1f;
        if (Time.time - lastTime > targetTime)
        {


            InputManger.Instance.keyAction -= test;

        }

    }


}

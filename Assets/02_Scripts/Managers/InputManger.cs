using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting;

public class InputManger : Singleton<InputManger>
{
    public Action keyAction;

    bool isPress=false;

    // Update is called once per frame
    void Update()
    {
        if (keyAction!=null&&!PlayerController.Instance.pState.isDead)
        {
            if (isPress && !Input.anyKey)
            {
                keyAction.Invoke();
            }
            if (Input.anyKey)
            {
                keyAction.Invoke();
            }
            isPress = Input.anyKey;
        }
    }
}

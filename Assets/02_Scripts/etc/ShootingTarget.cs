using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingTarget : MonoBehaviour, IDamageAble
{
    public enum TargetState
    {
        Up,
        Down,
    }
    public TargetState tState;
    Quaternion upRot;
    Quaternion downRot;
    Quaternion currentRot;


    float upDelayCount;
    // Start is called before the first frame update
    void Start()
    {
        downRot = Quaternion.Euler(0, transform.eulerAngles.y, transform.eulerAngles.z);
        upRot = Quaternion.Euler(90, transform.eulerAngles.y, transform.eulerAngles.z);
    }

    // Update is called once per frame
    void Update()
    {
        switch (tState)
        {
            case TargetState.Up:
                TargetUp();
                break;
            case TargetState.Down:
                TargetDown();
                break;
        }
    }


    void TargetUp()
    {
        currentRot= Quaternion.Lerp(currentRot,upRot,0.05f);
        transform.rotation = currentRot;
    }
    void TargetDown()
    {
        
        currentRot = Quaternion.Lerp(currentRot, downRot, 0.05f);
        transform.rotation = currentRot;

        if (upDelayCount + 2f < Time.time)
        {
            tState = TargetState.Up;
        }
    }

    public void Damaged(int damage, Vector3 hitPoint)
    {
        tState = TargetState.Down;
        upDelayCount = Time.time;
    }

    public void Damaged(int damage, Vector3 hitPoint)
    {
        throw new System.NotImplementedException();
    }
}

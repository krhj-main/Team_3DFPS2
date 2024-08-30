using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 적 체력 UI카메라에 보이게 하는 스크립트ㄴ
public class Billboard : MonoBehaviour
{
    Transform target;

    private void Start()
    {
        target = Camera.main.transform;
    }

    void Update()
    {
        // 자기 자신의 방향을 카메라의 방향과 일치 시킨다
        transform.forward = target.forward;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Interactable 
{
    //상호작용을 위한 인터페이스
    //상속받으면 상호작용 가능

    //상호작용시 실행될 부분
    public void Interaction(GameObject target);
}
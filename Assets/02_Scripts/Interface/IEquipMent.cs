using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEquipMent 
{
    Transform transform { get; set; }
    GameObject gameObject { get; set; }
    EquipType type { get; set; }
    public void OnHand(Transform _tr,Vector3 _offset);               //손에 들고있을때 작동할 부분
    public void InputKey();  //버튼입력
    public void OutHand();              //들고있다가 손에서 놓을때
}
public enum EquipType
{
    Weapon,
    Throw,
    Special
}

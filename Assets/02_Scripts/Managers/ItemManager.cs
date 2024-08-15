using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager
{//�������� �����ϴ� �Ŵ��� 
    public const int weaponSlotSize = 2;
    public const int throwSlotSize = 1;
    public const int specialSlotSize = 1;
    public Vector2 weaponSlot { get{return new Vector2(0, weaponSlotSize); } }
    public Vector2 throwSlot { get { return new Vector2(weaponSlotSize, throwSlotSize); } }
    public Vector2 specialSlot { get { return new Vector2(throwSlotSize+ weaponSlotSize, specialSlotSize); } }
    public GameObject[] Guns = new GameObject[weaponSlotSize + throwSlotSize + specialSlotSize];
 

}

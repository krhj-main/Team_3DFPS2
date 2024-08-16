using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager
{//아이템을 관리하는 매니저 
    public const int weaponSlotSize = 2;
    public const int throwSlotSize = 1;
    public const int specialSlotSize = 1;

    public const int weaponSlotWeight = 2;
    public const int throwSlotWeight = 1;
    public const int specialSlotWeight = 1;

    public Vector2 weaponSlot { get{return new Vector2(0, weaponSlotSize); } }
    public Vector2 throwSlot { get { return new Vector2(weaponSlotSize, throwSlotSize); } }
    public Vector2 specialSlot { get { return new Vector2(throwSlotSize+ weaponSlotSize, specialSlotSize); } }
    public GameObject[] Guns = new GameObject[weaponSlotSize + throwSlotSize + specialSlotSize];
    public IEquipMent[] EqupMents = new IEquipMent[weaponSlotSize + throwSlotSize + specialSlotSize];

    public EquipmentsSlot equipmentsSlot=new EquipmentsSlot(weaponSlotSize, 4);

    public EquipmentsSlot weapon = new EquipmentsSlot(weaponSlotSize, weaponSlotWeight);
    public EquipmentsSlot trow = new EquipmentsSlot(throwSlotSize, throwSlotWeight);
    public EquipmentsSlot special = new EquipmentsSlot(specialSlotSize, specialSlotWeight);


   

}

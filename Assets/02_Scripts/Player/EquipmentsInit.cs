using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentsInit : MonoBehaviour
{
    public MainWeapon[] mainWeapons = new MainWeapon[ItemManager.weaponSlotSize];
    public ThrowingWeapon[] throwingWeapons = new ThrowingWeapon[ItemManager.throwSlotSize];
    public SpecialWeapon[] specialWeapons = new SpecialWeapon[ItemManager.specialSlotSize];
    public EquipmentsSwap swap;
    
    void Start()
    {

    }

    [ContextMenu("장비 초기화 테스트")]
    public void EquipInventory() 
    {
        swap.Inventory.Clear();
        InitInventory(mainWeapons);
        InitInventory(throwingWeapons);
        InitInventory(specialWeapons);
    }

    void InitInventory(IEquipMent[] _array) 
    {
        for (int i = 0; i < _array.Length; i++) 
        {
            swap.WeaponChange(_array[i], _array[i].type);
        }
    }
}

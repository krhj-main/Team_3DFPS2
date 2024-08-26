using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentsInit : MonoBehaviour
{
    public MainWeapon[] mainWeapons = new MainWeapon[ItemManager.weaponSlotSize];
    public SpecialWeapon[] specialWeapons = new SpecialWeapon[ItemManager.specialSlotSize];
    EquipmentsSwap swap;
    public int frag = 0;
    public int flash = 0;
    public int smoke = 0;
    // Start is called before the first frame update
    void Start()
    {
        swap = PlayerController.Instance.gameObject.GetComponent<EquipmentsSwap>();
    }

    [ContextMenu("장비 초기화 테스트")]
    public void Init()
    {
        swap.Inventory.Clear();
        swap.GrenadeFactory.SetGrenadeCount(frag, flash, smoke);
        InitInventory(specialWeapons);
        InitInventory(mainWeapons);
    }

    void InitInventory(IEquipMent[] _array)
    {
        for (int i = 0; i < _array.Length; i++) {
            swap.WeaponChange(_array[i], _array[i].type);
        }
    }
}

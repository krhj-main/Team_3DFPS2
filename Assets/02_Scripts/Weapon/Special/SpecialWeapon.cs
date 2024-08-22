using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialWeapon : MonoBehaviour, IEquipMent, Interactable
{
    Transform IEquipMent.transform { get => transform; set { } }
    GameObject IEquipMent.gameObject { get => gameObject; set { } }
    [field: SerializeField]
    public EquipType type { get; set; }
    public EquipmentsSwap Swap { get; set; }
    private void Awake()
    {
        type = EquipType.Special;
    }

    public virtual void InputKey()
    {
    }

    public virtual void OnHand(Transform _tr, Vector3 _offset)
    {
    }

    public virtual void OnHandEnter()
    {
    }

    public virtual void OnHandExit()
    {
    }

    public virtual void Interaction(GameObject target)
    {
        EquipmentsSwap swap = target.GetComponent<EquipmentsSwap>();
        if (swap != null)
        {
            swap.WeaponChange(this, type);
        }
    }
}

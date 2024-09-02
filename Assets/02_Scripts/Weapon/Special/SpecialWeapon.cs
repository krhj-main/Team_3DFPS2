using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpecialWeapon : MonoBehaviour, IEquipMent, Interactable
{
    Transform IEquipMent.transform { get => transform; set { } }
    GameObject IEquipMent.gameObject { get => gameObject; set { } }
    [field: SerializeField]
    public EquipType type { get; set; }
    public EquipmentsSwap Swap { get; set; }
    public Sprite myImage;
    private void Awake()
    {
        type = EquipType.Special;
    }

    public virtual void InputKey()
    {
    }

    public virtual void OnHand(Transform _tr, Vector3 _offset)
    {
        UIManager.Instance.ChangeSpecialWeaponUIUpdate(myImage);
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

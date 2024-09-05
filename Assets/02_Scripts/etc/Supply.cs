using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Supply : MonoBehaviour,Interactable
{
    [SerializeField][Header("남은 사용횟수(-1이면 무제한)")]int count = 0;
    [SerializeField] [Header("보급량%")] [Range(0,100)] int supplyPersent = 10;
    [SerializeField] [Header("체력 회복량%")] [Range(0, 100)] int HealPersent = 10;
    EquipmentsSwap swap;
    public void Interaction(GameObject target)
    {
        if (Mathf.Abs(count) > 0) {
            swap = target.GetComponent<EquipmentsSwap>();
            for (int i = 0; i < 2; i++) {
                MainWeapon _weapon = (MainWeapon)swap.Inventory.Get(i);
                if (_weapon) {
                    _weapon.AddAmmo(((float)supplyPersent) / 100);
                }
            }
            PlayerController.Instance.pHP += (int)(PlayerController.Instance.maxHP * (HealPersent / 100f));
            count--;
        }
        
    }
}
 
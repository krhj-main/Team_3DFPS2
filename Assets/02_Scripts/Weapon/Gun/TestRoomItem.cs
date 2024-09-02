using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TestRoomItem : MonoBehaviour,Interactable
{
    [SerializeField] GameObject weaponPrefab;
    public void Interaction(GameObject target)
    {
        MainWeapon weapon = Instantiate(weaponPrefab).GetComponent<MainWeapon>();
        weapon.Interaction(target);
    }
}

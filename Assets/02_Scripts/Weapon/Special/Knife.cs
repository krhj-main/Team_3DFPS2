using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knife : MonoBehaviour, IEquipMent
{
    public EquipType type { get; set; }
    Transform IEquipMent.transform { get => transform; set { } }
    GameObject IEquipMent.gameObject { get => gameObject; set { } }

    Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void InputKey()
    {
        
    }

    public void OnHand(Transform _tr, Vector3 _offset)
    {
        
    }

    public void OnHandEnter()
    {
        PlayerController.Instance.anim = anim;
    }

    public void OnHandExit()
    {
        
    }

}

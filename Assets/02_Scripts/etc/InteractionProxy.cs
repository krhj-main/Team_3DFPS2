using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionProxy : MonoBehaviour,Interactable
{
    [SerializeField] Interactable interactable;
    public void Interaction(GameObject target)
    {
        interactable.Interaction(target); 
    }
}

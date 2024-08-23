using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionProxy : MonoBehaviour,Interactable
{
    [SerializeReference] GameObject interactableObj;
    Interactable interactable;
    private void Awake()
    {
        interactable = interactableObj.GetComponent<Interactable>();
    }

    public void Interaction(GameObject target)
    {
        interactable.Interaction(target); 
    }
}

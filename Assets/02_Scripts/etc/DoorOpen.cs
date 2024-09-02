using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorOpen : MonoBehaviour, Interactable
{
    bool doorOpen = false;
    Animator anim;
    public void Interaction(GameObject target)
    {
        doorOpen = !doorOpen;
        anim.SetBool("DoorOpen", doorOpen);
    }

    // Start is called before the first frame update
    void Start()
    {
        anim.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorOpen : MonoBehaviour, Interactable
{
    
    bool doorOpen = false;
    
    Animator anim;


    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }
    

    public void Interaction(GameObject target)
    {        
        Debug.Log($"문 {doorOpen}");
        doorOpen = !doorOpen;
        anim.SetBool("DoorOpen", doorOpen);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DronAnimTrigger : MonoBehaviour
{

    [SerializeField] DronController dc;     // Start is called before the first frame update

    public void Use() {

        dc.Use();
    }
}

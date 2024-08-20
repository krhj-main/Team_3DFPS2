using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour,Interactable
{
    public GunsSwap.WeaponType type;
    public void Interaction(GameObject target)
    {
        GunsSwap swap =  target.GetComponent<GunsSwap>();
        //MainWeapon gun = gameObject.GetComponent<MainWeapon>();
        if (swap != null) {
            //swap.WeaponChange(gameObject);
            swap.WeaponChange2(gameObject, type);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

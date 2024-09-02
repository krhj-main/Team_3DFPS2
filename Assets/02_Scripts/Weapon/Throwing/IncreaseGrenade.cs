using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncreaseGrenade : MonoBehaviour,Interactable
{
    [SerializeField] int fragCount;
    [SerializeField] int flashCount;
    [SerializeField] int smokeCount;

    public void Interaction(GameObject target)
    {
        GrenadeFactory _factory = target.GetComponent<EquipmentsSwap>().GrenadeFactory;

                _factory.IncreaseGrenade(GrenadeType.FragGrenade, fragCount);

                _factory.IncreaseGrenade(GrenadeType.FlashGrenade, flashCount);
                _factory.IncreaseGrenade(GrenadeType.SmokeGrenade, smokeCount);
        // _factory.SetGrenadeCount(fragCount, flashCount, smokeCount);
    }
}

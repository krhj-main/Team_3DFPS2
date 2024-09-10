using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodEffect : MonoBehaviour
{

    [SerializeField] ParticleSystem blood;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!blood.IsAlive())
        {
            GameManager.Instance.bloodList.Add(blood);
            gameObject.SetActive(false);
        }
    }
}

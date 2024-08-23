using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamAttatch : MonoBehaviour
{
    [Header ("카메라가 붙을 위치 트랜스폼")]
    [SerializeField] Transform arm;
    [SerializeField] CharacterController cc;
    [Tooltip ("카메라의 Z축 간격 값")]
    [SerializeField] float offSetZ = 0;

    
    // Start is called before the first frame update
    void Start()
    {
        transform.SetParent(arm);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ChangeParent(Transform parent) {
        transform.SetParent(parent);
    }

}

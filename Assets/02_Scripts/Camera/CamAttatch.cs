using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamAttatch : MonoBehaviour
{
    [Header ("ī�޶� ���� ��ġ Ʈ������")]
    [SerializeField] Transform arm;
    [SerializeField] CharacterController cc;
    [Tooltip ("ī�޶��� Z�� ���� ��")]
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

}

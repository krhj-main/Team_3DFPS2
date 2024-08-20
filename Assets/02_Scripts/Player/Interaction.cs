using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interaction : MonoBehaviour
{
    //플레이어 상호작용기능 클래스
    Interactable selected;                                  //선택된 상호작용이 가능한 오브젝트
    [Header("상호작용 조건")]
    [SerializeField] KeyCode selectKey = KeyCode.F;         //상호작용시 입력받을 키
    [SerializeField] float distace = 3;                     //상호작용이 가능한 최대 거리
    [SerializeField] LayerMask hitLayer;                    //상호작용이 일어나는 레이어
    [Space(5)] [Header("UI")] 
    [SerializeField] GameObject lootImage;                  //상호작용이 가능하다고 알려줄 UI

    Camera main;
    void Start()
    {
        main = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        //레이캐스트를 카메라를 기준으로 특정 레이어로 실시
        Ray ray = new Ray(main.transform.position, main.transform.forward);

        RaycastHit hit;
        Debug.DrawRay(main.transform.position, main.transform.forward,Color.black, distace);
        if (Physics.Raycast(ray, out hit, distace, hitLayer))
        {
            lootImage.SetActive(true);
            if (Input.GetKeyDown(selectKey))
            {
                //첫번째로 hit 한 오브젝트의 Interectable컴포넌트롤 가져오는걸 시도
                selected = hit.collider.GetComponent<Interactable>();
                //컴포넌트가 있으면 상호작용기능 메서드 실행
                if (selected != null) 
                {
                    selected.Interaction(gameObject);
                }
            }
        }
        //상호작용이 가능한 오브젝트가 검출되지 않았다면 UI 꺼짐
        else
        {
            lootImage.SetActive(false);
        }
    }
}

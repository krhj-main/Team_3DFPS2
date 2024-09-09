using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class Interaction : MonoBehaviour
{
    //플레이어 상호작용기능 클래스
    Interactable selected;                                  //선택된 상호작용이 가능한 오브젝트
    [Header("상호작용 조건")]
    [SerializeField] KeyCode selectKey = KeyCode.F;         //상호작용시 입력받을 키
    [SerializeField] float distace = 3;                     //상호작용이 가능한 최대 거리
    [SerializeField] LayerMask hitLayer;                    //상호작용이 일어나는 레이어
    [Space(5)] [Header("UI")] 
    [SerializeField] Image lootImage;                  //상호작용이 가능하다고 알려줄 UI
    RaycastHit hit;
    GameObject selectedObj;
    Camera main;
    int count=0;
    void Start()
    {
        main = Camera.main;
    }
    private void Update()
    {
        if (Input.GetKeyDown(selectKey)&& selectedObj!=null)
        {
            //첫번째로 hit 한 오브젝트의 Interectable컴포넌트롤 가져오는걸 시도
            selected = selectedObj.GetComponent<Interactable>();
            //컴포넌트가 있으면 상호작용기능 메서드 실행
            if (selected != null)
            {
                selected.Interaction(gameObject);
            }
        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        //레이캐스트를 카메라를 기준으로 특정 레이어로 실시
        Ray ray = new Ray(main.transform.position, main.transform.forward);

        
        //Debug.DrawRay(main.transform.position, main.transform.forward,Color.black, distace);
        if (Physics.Raycast(ray, out hit, distace, hitLayer))
        {
            count = Mathf.Max(1, count + 1);
            if (count > 2) {
                selectedObj = hit.collider.gameObject;
                lootImage.enabled = true;
            }
            
            
        }
        //상호작용이 가능한 오브젝트가 검출되지 않았다면 UI 꺼짐
        else
        {
            count = Mathf.Min(-1, count - 1);
            if (count < -2) {
                selectedObj = null;
                lootImage.enabled = false;
            }
            
        }
    }
}

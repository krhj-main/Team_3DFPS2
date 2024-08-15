using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GunShootTest : MonoBehaviour
{
    private MainWeapon myMainWeapon;    // 주무기 
    private ThrowingWeapon myThrow;     // 투척 무기
    private SpecialWeapon mySpecial;
    private GunsSwap gunSwap;           // 총기 관리

    public Transform firePos;           // 총알 나가는 곳
    public bool canThrow = false;       // 던질 수 있는 상태
    public bool isADS = false;          // 정조준 ( Aiming Down Sight )


    private void Start()
    {
        PlayerController.Instance.inputAction -= ShootTest;
        PlayerController.Instance.inputAction += ShootTest;

        gunSwap = GameObject.FindGameObjectWithTag("Player").GetComponent<GunsSwap>();
    }


    private void Update()
    {
        // 투척무기 던지기
        if (Input.GetMouseButtonUp(0))
        {
            ThrowWeapon();
        }
    }

    void ShootTest()
    {
        myMainWeapon = GetComponentInChildren<MainWeapon>();
        myThrow = GetComponentInChildren<ThrowingWeapon>();
        mySpecial = GetComponentInChildren<SpecialWeapon>();

        // 마우스 왼쪽 버튼 눌렀을때 ( 발사, 투척무기 궤적 )
        if (Input.GetMouseButton(0))
        {
            if (myMainWeapon != null)
            {
                myMainWeapon.Shoot(firePos);
                GameManager.Instance.AggroEnemy(firePos.position, 30f);
            }
            else if (myThrow != null)
            {
                canThrow = true;
                myThrow.UpdateTrajectory(firePos);
            }
            else if (mySpecial != null)
            {
                mySpecial.Use();
            }
        }

        // 정조준 버튼
        if (myMainWeapon != null)
        {
            if (Input.GetMouseButtonDown(1))
            {
                myMainWeapon.Aming(!isADS);
                isADS = !isADS;

                // 스나이퍼 들고 있으면 줌 UI 이미지 보이게함
                float _zoomUIdealyTime;
                if (isADS)
                {
                    _zoomUIdealyTime = 0.5f;
                }
                else
                {
                    _zoomUIdealyTime = 0.1f;
                }

                if(GetComponentInChildren<Sniper>() != null)
                {
                    StartCoroutine(myMainWeapon.AmingUI(isADS, _zoomUIdealyTime));
                }
            }
        }

        // R키 ( 재장전 )
        if (Input.GetKeyDown(KeyCode.R) && myMainWeapon != null)
        {
            myMainWeapon.Reload();
        }
    }

    // Input.anyKey를 사용하여 inputAction을 호출하고 있어서 키가 눌려있는 동안에만 호출하고 키를 떼는 순간은 감지 못함
    private void ThrowWeapon()
    {
        myThrow = GetComponentInChildren<ThrowingWeapon>();
        if (gunSwap != null && myThrow != null)
        {
            gunSwap.DropWeapon();
            myThrow.trajectoryLine.enabled = false;
        }
        canThrow = false;
    }
}


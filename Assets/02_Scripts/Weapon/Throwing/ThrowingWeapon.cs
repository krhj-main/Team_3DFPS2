using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ThrowingWeapon : MonoBehaviour,IEquipMent,Interactable
{
    public float explosiondelay { get; set; }       // 폭발 시간
    public float explosionRadius { get; set; }      // 폭발 반경
    public float effectDuration { get; set; }       // 효과 지속시간 ( 섬광, 연막 )
    public int damage { get; set; }                 // 데미지 ( 수류탄 )
    Transform IEquipMent.transform { get => transform; set { } }
    GameObject IEquipMent.gameObject { get => gameObject; set { } }
    [field: SerializeField]
    public EquipType type { get ; set ; }

    public LayerMask attackableMask;                // 효과 및 데미지 입을 대상
    public float throwForce = 10f;                  // 던지는 힘
    public LineRenderer trajectoryLine;             // 궤적 라인
    int trajectoryLinePoint = 30;                   // 궤적 포인트 갯수
    public Transform firePos;
    protected Rigidbody rb;
    protected bool isThrow=false;
    Grenade Grenade;
    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
        trajectoryLine = GetComponent<LineRenderer>();
        trajectoryLine.enabled = false;
    }

    private void OnDisable()
    {
        //transform.SetParent(PlayerController.Instance.transform);
        isThrow = false;
    }
    // 라인 렌더러 설정
    void SetupTrajectoryLine()
    {
        trajectoryLine.startWidth = 0.1f;
        trajectoryLine.endWidth = 0.1f;
        trajectoryLine.startColor = Color.red;
        trajectoryLine.endColor = Color.red;
    }

    protected (Vector3 velocity, Vector3 position) CalculateTrajectoryVector(Transform _firePos)
    {
        Vector3 _velocity = _firePos.forward * throwForce;
        Vector3 _localStartPos = new Vector3(0.5f, 0, 1f);      // 카메라 기준으로 궤적 시작 위치 설정
        Vector3 _position = _firePos.TransformPoint(_localStartPos);

        return (_velocity,_position);
    }

    // 궤적 업데이트
    public void UpdateTrajectory(Transform _firePos)
    {
        SetupTrajectoryLine();
        trajectoryLine.positionCount = trajectoryLinePoint;     // 라인렌더러 점 카운트

        var(_velocity, _position) = CalculateTrajectoryVector(_firePos);

        float _timeStpe = 1f/30f; // 등가속도 운동 값

        // 속도와 중력에 의해 변화를 계산해서 점 찍기
        for (int i = 0; i < trajectoryLinePoint; i++)
        {
            trajectoryLine.SetPosition(i, _position);
            _velocity += Physics.gravity * _timeStpe;
            _position += _velocity * _timeStpe;
        }
        trajectoryLine.enabled = true;      // 궤적 활성화
    }

    // 던지기
    public virtual void Throw(Transform _firePos)
    {
        var (_velocity, _position) = CalculateTrajectoryVector(_firePos);
        this.transform.position = _position;

        rb.velocity = _velocity;
        trajectoryLine.enabled = false;
    }



    public virtual void OnHand(Transform _tr, Vector3 _offset)
    {
        if (!isThrow) 
        {
            gameObject.SetActive(true);
            transform.position = _tr.position + _offset;  //오브젝트 위치 조정
            transform.rotation = _tr.rotation;
        }
    }

    public virtual void InputKey()
    {
        if (!isThrow)
        {
            if (Input.GetMouseButton(0))
            {
                UpdateTrajectory(firePos);

            }
            if (Input.GetMouseButtonUp(0))
            {
                Throw(firePos);
                Utill.DestroyOnLoad(gameObject);
                isThrow = !isThrow;
            }
        }
        
    }

    public virtual void OnHandExit()
    {
        
    }

    public void Interaction(GameObject target)
    {
        EquipmentsSwap swap = target.GetComponent<EquipmentsSwap>();
        if (swap != null)
        {
            swap.WeaponChange(this, type);
        }
    }

    public virtual void OnHandEnter()
    {
    }
}

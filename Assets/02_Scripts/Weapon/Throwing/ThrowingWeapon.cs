using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class ThrowingWeapon : MonoBehaviour,IEquipMent,Interactable
{
    Transform IEquipMent.transform { get => transform; set { } }
    GameObject IEquipMent.gameObject { get => gameObject; set { } }
    [field: SerializeField][field: Header("무기정보")][Tooltip("무기타입")]
    public EquipType type { get ; set ; }
    [Tooltip("공격 레이어")]
    public LayerMask attackableMask;                // 효과 및 데미지 입을 대상
    [Tooltip("던지는 힘")]
    public float throwForce = 10f;                  // 던지는 힘
    [HideInInspector] public LineRenderer trajectoryLine;             // 궤적 라인
    int trajectoryLinePoint = 60;                   // 궤적 포인트 갯수
    [HideInInspector] public Transform firePos;                       // 던지는 방향
    protected Rigidbody rb;
    protected bool isThrow=false;
    Grenade Grenade;
    Animator anim;
    protected virtual void Awake()
    {
        anim = GetComponent<Animator>();
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
        Vector3 _localStartPos = new Vector3(0.2f, -0.1f, 0.1f);      // 카메라 기준으로 궤적 시작 위치 설정
        Vector3 _position = _firePos.TransformPoint(_localStartPos);

        return (_velocity,_position);
    }

    // 궤적 업데이트
    public void UpdateTrajectory(Transform _firePos)
    {
        SetupTrajectoryLine();
        trajectoryLine.positionCount = trajectoryLinePoint;     // 라인렌더러 점 카운트

        var(_velocity, _position) = CalculateTrajectoryVector(_firePos);

        _velocity.y *= 1.2f;

        float _timeStep = 1f/30f; // 등가속도 운동 값
        int _actualPoints = 0;

        // 속도와 중력에 의해 변화를 계산해서 점 찍기
        for (int i = 0; i < trajectoryLinePoint; i++)
        {
            trajectoryLine.SetPosition(i, _position);
            _actualPoints++;

            if (Physics.Raycast(_position, _velocity.normalized, out RaycastHit hit, _velocity.magnitude * _timeStep))
            {
                // 충돌 지점을 마지막 점으로 설정
                trajectoryLine.SetPosition(i + 1, hit.point);
                _actualPoints++;
                break; // 루프 종료
            }

            _velocity += Physics.gravity * _timeStep;
            _position += _velocity * _timeStep;
        }
        trajectoryLine.positionCount = _actualPoints;
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

    public virtual void OnHandEnter()
    {
        PlayerController.Instance.anim = anim;
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
}

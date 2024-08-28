using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using TMPro;
using UnityEngine;

public class PlayerController : Singleton<PlayerController>, IDamageAble
{
    
    [SerializeField] Transform arm;
    [SerializeField] public Transform waist;
    Vector3 armPos;
    
    [SerializeField] Transform cam;
    [SerializeField] Transform character;
    //[SerializeField] Transform player;

    [Space(5)] [Header("플레이어 이동 조작 관련")]
    [Tooltip ("마우스 이동속도")]
    [SerializeField] float mouseSensitivity = 2;
    [Header("캐릭터 이동속도")]
    [SerializeField] float moveSpeed = 5f;
    [Header("캐릭터 이동속도 배율")]
    [SerializeField] public float moveSpeedScale = 0f;
    [Space(5)]
    [Header("그라운드 체크")]
    [Tooltip("T = 기즈모 킴 F = 기즈모 끔")]
    [SerializeField] bool drawGizmo;
    [Tooltip("그라운드 체크할 박스의 사이즈")]
    [SerializeField] Vector3 boxSize;
    [Tooltip("플레이어로부터 그라운드 박스의 거리")]
    [SerializeField] float maxDistance;
    [Tooltip("땅으로 인식할 레이어")]
    [SerializeField] LayerMask groundLayer;

    [Space(5)]
    [Header("점프 관련")]
    [Tooltip("점프력")]
    [SerializeField] float jumpForce;
    [Tooltip("중력 속도")]
    [SerializeField] float gravityAcc;

    [Space(5)]
    [Header("앉기 관련")]
    [Tooltip("앉고 일어나는 속도")]
    [SerializeField] float crouchSpeed;
    [Tooltip("서있을 때의 높이")]
    [SerializeField] float normalHeight;
    [Tooltip("앉았을 때의 높이")]
    [SerializeField] float crouchHeight;
    [Tooltip("앉았을 때 캐릭터 컨트롤러 중심 위치 값")]
    [SerializeField] Vector3 crouchCenter;
    [Tooltip("서있을 때 캐릭터 컨트롤러 중심 위치 값")]
    [SerializeField] Vector3 normalCenter;

    [HideInInspector]
    public Animator anim;

    [Space(5)]
    [Header("플레이어 체력")]
    int HP;
    [SerializeField] public int maxHP = 10;

    // 플레이어 죽었을 때
    public bool death = false;
    public static event Action OnPlayerDeath;

    public int pHP
    {
        get
        {
            return HP;
        }
        set
        {
            if(HP != value)
            {
                HP = Mathf.Clamp(value,0,maxHP);
            }
        }

    }

    public event Action inputAction;


    // 플레이어 상태 리스트
    [HideInInspector]
    public PlayerStateList pState;
    // 마우스 이동값
    Vector2 mouseDelta;
    // 키보드 입력값
    Vector3 moveInput;
    // 캐릭터의 속도값
    Vector3 velocity;
    // 캐릭터 컨트롤러
    [HideInInspector] public CharacterController cc;

    Camera main;
    public Camera PlayerCamera {
        get => main;
        private set {; }
    }

    public AudioSource playerSound;
    public AudioClip walkSound;

    /*
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            if (instance != this)
            {
                Destroy(this.gameObject);
            }
        }
    }
    */

    // Start is called before the first frame update
    void Start()
    {
        pHP = maxHP;
        //armPos = arm.transform.position;        // 사용되고 있지 않는듯함
        pState = GetComponent<PlayerStateList>();
        cc = GetComponent<CharacterController>();
        anim = GetComponentInChildren<Animator>();
        playerSound = GetComponent<AudioSource>();
        main = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (!main.enabled) { return; }
        InputKey();
        //LookAround();
        PlayerDir();
        ActiveCrouch();
        OpenViewer();

        if (inputAction != null)
        {
            if (Input.anyKey)
            {
                inputAction.Invoke();
            }
        }
    }
    
    private void FixedUpdate()
    {
        ActiveMove();
    }

    // 속도를 사용해 실제로 움직이는 부분
    void ActiveMove()
    {
        Vector3 _groundVelocity = MovingUpdate(moveInput.x, moveInput.z);

        // 이동 상태일 때
        if (pState.isMoving)
        {
            playerSound.volume = 1;
            anim.SetFloat("Speed", velocity.magnitude);
        }

        // 걷기키가 눌렸을 때
        if (pState.isWalking)
        {
            // 이동속도 및 애니메이션 속도 조절
            _groundVelocity *= 0.4f;
            playerSound.volume = 0.5f;
        }
        // 뛰기키가 눌렸을 때 / 걷기키를 누를때는 같이 동작안함 -> 키 입력에 있어 걷기키가 최우선?
        if (pState.isRunning && !pState.isWalking)
        {
            _groundVelocity *= 1.5f;
        }

        // 앉기키를 눌렀을 때 
        if (pState.isCrouch)
        {
            _groundVelocity /= 1.8f;
        }

        _groundVelocity *= (1+moveSpeedScale);

        float _yVelocity = JumpingUpdate();
        velocity = new Vector3(_groundVelocity.x, _yVelocity, _groundVelocity.z);

        //transform.position += velocity * Time.fixedDeltaTime;
        cc.Move(velocity * Time.fixedDeltaTime);
    }

    // 땅과 닿아있는지 체크
    bool Grounded()
    {
        bool _isGrounded = Physics.BoxCast(transform.position, boxSize, -transform.up, transform.rotation, maxDistance, groundLayer);
        return _isGrounded;
    }
    // 기즈모 그리기 메서드
    private void OnDrawGizmos()
    {
        if (!drawGizmo) return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawCube(transform.position - transform.up * maxDistance, boxSize);
    }


    // 키 입력을 받았을 때 변수 값 전달 메서드
    void InputKey()
    {
        if (GameManager.Instance.openUI || pState.isDead)
        {
            return;
        }


        // WASD 이동키
        moveInput = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
        pState.isMoving = moveInput.magnitude != 0;


        // 점프키
        if (Input.GetKeyDown(KeyCode.Space)) pState.isJumping = true;

        // 걷기 키
        pState.isWalking = Input.GetKey(KeyCode.LeftControl);
        // 달리기
        pState.isRunning = Input.GetKey(KeyCode.LeftShift);
        // 앉기
        pState.isCrouch = Input.GetKey(KeyCode.C);
        // if (Input.GetKey(KeyCode.C))            pState.isCrouch = !pState.isCrouch;
        // 토글식일 경우

        // 기울이기
        pState.isTiltingL = Input.GetKey(KeyCode.Q);
        pState.isTiltingR = Input.GetKey(KeyCode.E);

        pState.isOnViewer = Input.GetKey(KeyCode.Tab);


        //if (Input.GetKeyDown(KeyCode.LeftControl)) pState.isWalking = true;
    }

    // 점프시 속도 메서드
    float JumpingUpdate()
    {
        if (!Grounded())
        {
            return velocity.y - gravityAcc * Time.deltaTime;
        }

        if (pState.isJumping)
        {
            pState.isJumping = false;
            return velocity.y + jumpForce;
        }
        else
        {
            return Mathf.Max(0.0f, velocity.y);
        }
    }

    // 이동 조작시 메서드
    Vector3 MovingUpdate(float _hInput, float _vInput)
    {
        Vector3 _moveVelocity = character.forward * _vInput + character.right * _hInput;
        Vector3 _moveDir = _moveVelocity.normalized;

        float _axis = Mathf.Min(_moveVelocity.magnitude, 1f) * moveSpeed;

        return _moveDir * _axis;
    }


    // 플레이어의 방향 구하기 메서드
    void PlayerDir()
    {
        Debug.DrawRay(arm.position,new Vector3(arm.forward.x,0,arm.forward.z),Color.red * 5f);
        Vector3 _lookForward = new Vector3(arm.forward.x, 0, arm.forward.z).normalized;
        Vector3 _lookRIght = new Vector3(arm.right.x, 0, arm.right.z).normalized;

        Vector3 _moveDir = ((_lookForward * moveInput.z) + (_lookRIght * moveInput.x)) * moveSpeed;
        

        character.forward = _lookForward;
    }




    // 앉기 메서드
    void ActiveCrouch()
    {
        
        if (pState.isCrouch == true)
        {
            cc.height = cc.height - crouchSpeed * Time.deltaTime;


            arm.localPosition = Vector3.Lerp(arm.localPosition, crouchCenter + Vector3.up * 0.5f, 0.03f);

            cc.center = Vector3.Lerp(cc.center, crouchCenter, 0.03f);

            if (cc.height <= crouchHeight)
            {
                cc.height = crouchHeight;
                cc.center = crouchCenter;
            }
        }
        
        if (pState.isCrouch == false)
        {
            cc.height = cc.height + crouchSpeed * Time.deltaTime;


            arm.localPosition = Vector3.Lerp(arm.localPosition, normalCenter +Vector3.up*0.5f, 0.03f);


            cc.center = Vector3.Lerp(cc.center, normalCenter, 0.03f);
            if (cc.height >= normalHeight)
            {
                cc.height = normalHeight;
                cc.center = normalCenter;
            }
        }
    }

    void OpenViewer()
    {
        
        UIManager.Instance.missionViewer.SetActive(pState.isOnViewer);
        
    }

    // 데미지 관련 임시 메서드
    public void Damaged(int _damage, Vector3 hitpoint)
    {
        pHP -= _damage;
        if(pHP <= 0)
        {
            pState.isDead = true;
            cc.enabled = false;
        }
    }
}

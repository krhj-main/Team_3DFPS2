using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dron : MonoBehaviour,Interactable
{
    //총을 상속받아 총이랑 비슷하게 동작

    //발사 대신에 드론을 던짐 장탄수는 1
    //드론을 직접 주으면 장탄수가 늘어남

    //드론이 던져진 상태에서 사용시 드론으로 카메라 전환하고 드론조작
    //특정 키를 누르면 원래 카메라로 돌아옴
    //-> 돌아올 카메라를 담고있어야 한다,카메라에 따라 조작이 달라져야 한다

    public Rigidbody rig;
    Camera dronCam;
    [SerializeField] GameObject dronUI;
    public RawImage cam;
    [SerializeField] float jumpeForce = 20f;
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float jumpCoolTime = 2f;
    
    [SerializeField]float h=0;
    [SerializeField] float v=0;
    [SerializeField] public Collider col;
    public DronController dronController;

    public KeyCode returnKey;

    public bool isActive = false;
    [Tooltip("그라운드 체크할 박스의 사이즈")]
    [SerializeField] Vector3 boxSize;
    [Tooltip("플레이어로부터 그라운드 박스의 거리")]
    [SerializeField] float maxDistance;
    [Tooltip("땅으로 인식할 레이어")]
    [SerializeField] LayerMask groundLayer;
    [SerializeField] float mouseSensitivity = 2;
    [SerializeField] Image cooltime;
    [SerializeField] Animator anim;
    Vector2 mouseDelta;
    public bool IsGround;
    public bool jumpAble = true;
    // Start is called before the first frame update
    void Start()
    {
        rig = GetComponent<Rigidbody>();
        dronCam = GetComponentInChildren<Camera>();
        InputManger.Instance.keyAction+= Inputkey;
    }
    private void FixedUpdate()
    {
        if (v == 0 && h == 0)
        {
            anim.SetBool("Walk_Anim", false);
        }
        else {
            anim.SetBool("Walk_Anim", true);
        }
        IsGround = Grounded();
        if (IsGround)
        {
            
            PlayerDir();
            Move();
        }
        anim.SetBool("Roll_Anim", !IsGround);
    }
    // Update is called once per frame
    void Update()
    {
        
            LookAround();
       
        
        
    }
    void Inputkey() {
        if (dronCam.enabled && isActive)
        {
            if (Input.GetKey(KeyCode.W)) { v = 1; }
            if (Input.GetKey(KeyCode.S)) { v = -1; }
            if (Input.GetKey(KeyCode.A)) { h = -1; }
            if (Input.GetKey(KeyCode.D)) { h = 1; }
            if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.S)) { v = 0; }
            if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D)) { h = 0; }



            if (Input.GetKeyDown(KeyCode.Space) && jumpAble)
            {
                jumpAble = false;
                anim.SetBool("Roll_Anim", true);
                rig.AddForce(dronCam.transform.forward*jumpeForce, ForceMode.Impulse);
                StartCoroutine(JumpCoolTime());
            }
            if (Input.GetKeyDown(returnKey))
            {
                DronDisable();
            }
            
        }

    }
    private void OnEnable()
    {
        anim.enabled = false;
    }

    bool Grounded()
    {
        bool _isGrounded = Physics.BoxCast(transform.position, boxSize, -transform.up, transform.rotation, maxDistance, groundLayer);
        return _isGrounded;
    }
    void PlayerDir()
    {
        Vector3 _lookForward = new Vector3(dronCam.transform.forward.x, 0, dronCam.transform.forward.z).normalized;
        transform.forward = _lookForward;
        Quaternion camrot = dronCam.transform.localRotation;
        camrot.y = 0;
        camrot.z = 0;
        dronCam.transform.localRotation = camrot;
    }
    public void Move()
    {
        Vector3 vel = transform.forward * v + transform.right * h;
        vel = vel.normalized * moveSpeed;
        vel.y = rig.velocity.y;
        rig.velocity = vel;
    }
    public void DronAwake() {
        
        cam.enabled = false;
        dronUI.SetActive(true);
        dronCam.enabled = true;
        dronController.charCamera.enabled = false;
        transform.rotation = Quaternion.Euler(0,0,0);
        anim.enabled = true;
        anim.SetBool("Open_Anim", true);
        dronController.phoneMat.color = Color.white;
    }
    public void DronDisable()
    {
        cam.enabled = true;
        dronUI.SetActive(false);
        dronCam.enabled = false;
        dronController.charCamera.enabled = true;
        anim.SetBool("Open_Anim", false);
        v = 0;
        h = 0;
    }

        public void Interaction(GameObject target)
    {
        cam.enabled = false;
        isActive = false;
        dronController.phoneMat.color = Color.black;
        dronController.DronReturn();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!isActive) {
            Quaternion rot = Quaternion.Euler(0, 0, 0);
            rot.y = transform.rotation.y;
            transform.rotation = rot;
            isActive = true;
        }
    }
    void LookAround()
    {
        if (!dronCam.enabled)
        {
            return;
        }


        mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        Vector3 _camAngle = dronCam.transform.rotation.eulerAngles;

        float _limit = _camAngle.x - mouseDelta.y;

        if (_limit < 180)
        {
            _limit = Mathf.Clamp(_limit, -1f, 80f);
        }
        else
        {
            _limit = Mathf.Clamp(_limit, 320f, 361f);
        }
        dronCam.transform.rotation = Quaternion.Euler(_limit, _camAngle.y + (mouseDelta.x * mouseSensitivity), _camAngle.z);
    }

    public IEnumerator JumpCoolTime() {
        float time =0;
        while (time < jumpCoolTime) {
            yield return new WaitForSeconds(0.1f);
            time += 0.1f;
            cooltime.fillAmount = (jumpCoolTime - time) / jumpCoolTime;
        }
        
        
        jumpAble = true;
    }
}

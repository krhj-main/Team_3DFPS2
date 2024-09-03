using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(LineRenderer))]
public class GrenadeFactory : ThrowingWeapon
{
    [SerializeField][Header("오브젝트 풀링")] [Tooltip("풀링되는 오브젝트 리스트")] public List<Grenade> grenades = new List<Grenade>();
    [SerializeField] [Tooltip("풀링되는 오브젝트 프리팹")] Grenade prefab;
    [SerializeField] [Tooltip("풀링되는 오브젝트 개수")] int objectSIze = 3;
    Collider col;
    GrenadeType grenadeType;
    Grenade current;
    [SerializeField] [Header("수류탄 개수")]
    [Tooltip("파열수류탄 개수")]int fragCount;
    [SerializeField] [Tooltip("섬광탄 개수")] int flashCount;
    [SerializeField] [Tooltip("연막탄 개수")] int smokeCount;
    [SerializeField] Transform hand;
    [SerializeField] GameObject arms;
    [SerializeField] Transform CameraPos;
    [SerializeField] Animator animator;
    int FragCount 
    { 
        set 
        {   fragCount = value;
            if (fragCount < 0) {
                fragCount = 0;
            }
        } 
        get => fragCount; 
    }
     int FlashCount 
    { set 
        { 
            flashCount = value;
            if (flashCount < 0)
            {
                flashCount = 0;
            }
        } 
        get => flashCount; 
    }
    [SerializeField] int SmokeCount 
    { set 
        { 
            smokeCount = value;
            if (smokeCount < 0)
            {
                smokeCount = 0;
            }
        } 
        get => smokeCount; 
    }
    private void SpawnGrenade() {
        for (int i = 0; i < objectSIze; i++) {
            Grenade grenade = GameObject.Instantiate(prefab, transform);
            grenades.Add(grenade);
            grenade.factory = this;
            grenade.gameObject.SetActive(false);
        }
    }
    // Start is called before the first frame update

    protected override void Awake()
    {
        base.Awake();
        col = GetComponent<Collider>();
        SpawnGrenade();
        UdateUI();
    }
    void Start()
    {
        
    }
    public override void OnHandEnter()
    {
        base.OnHandEnter();
        col.enabled = false;
        arms.SetActive(true);
        firePos.SetParent(CameraPos);
        firePos.localPosition = Vector3.zero;
        firePos.localRotation = Quaternion.Euler(0, 180, -0.15f);

    }

    public override void OnHandExit()
    {
        base.OnHandExit();
        col.enabled = true;
        arms.SetActive(false);
        firePos.SetParent(null);
    }
    public override void OnHand(Transform _tr, Vector3 _offset)
    {
        transform.position = _tr.position + _offset;  //오브젝트 위치 조정
        transform.rotation = _tr.rotation;
        if (true)
        {
            if (current == null)
            {
                current = grenades[0];
            }
            else {
                if (!isEmpty())
                {
                    if (!current.gameObject.activeSelf)
                    {
                        current.Changetype(grenadeType);
                        current.gameObject.SetActive(true);
                    }
                    current.transform.position = hand.position + _offset;  //오브젝트 위치 조정
                    current.transform.rotation = hand.rotation;
                }
                else {
                    current.gameObject.SetActive(false);
                }
                
            }
            
        }
        
    }
    
    public override void InputKey()
    {
        if (!isThrow&& !isEmpty())
        {
            if (Input.GetMouseButton(0))
            {
                UpdateTrajectory(firePos);

            }
            if (Input.GetMouseButtonUp(0))
            {
                trajectoryLine.enabled = false;
                isThrow = true;
                //Throw(firePos);
                animator.SetTrigger("doThrow");
            }
        }
    }

    public override void Throw(Transform _firePos)
    {
        isThrow = false;
        var (_velocity, _position) = CalculateTrajectoryVector(_firePos);
        current.transform.position = _position;

        current.rb.velocity = _velocity;
        current.Explode();
        current.transform.SetParent(null);
        current = null;
        grenades.RemoveAt(0);
        DecreaseGrenade(grenadeType,1);
        if (grenades.Count < 1) {
            isThrow = true;
        }
        
    }
    public void UdateUI() {
        switch (grenadeType)
        {
            case GrenadeType.FragGrenade:
                UIManager.Instance.ThrowUIUpdate(FragGrenade.sprite, FragCount );
                break;
            case GrenadeType.FlashGrenade:
                UIManager.Instance.ThrowUIUpdate(FlashGrenade.sprite, FlashCount);
                break;
            case GrenadeType.SmokeGrenade:
                UIManager.Instance.ThrowUIUpdate(SmokeGrenade.sprite, SmokeCount);
                break;
        }
        
    }

    public void Changetype()
    {
        
        switch (grenadeType)
        {
            case GrenadeType.FragGrenade:
                grenadeType = GrenadeType.FlashGrenade;
                break;
            case GrenadeType.FlashGrenade:
                grenadeType = GrenadeType.SmokeGrenade;
                break;
            case GrenadeType.SmokeGrenade:
                grenadeType = GrenadeType.FragGrenade;
                break;
        }
        UdateUI();
        if (grenades.Count > 0)
        {
            grenades[0].gameObject.SetActive(false);
        }
        animator.SetTrigger("doGet");
    }

    public bool isEmpty() {
        switch (grenadeType)
        {
            case GrenadeType.FragGrenade:
                return FragCount == 0;
            case GrenadeType.FlashGrenade:
                return FlashCount == 0;
            case GrenadeType.SmokeGrenade:
                return SmokeCount == 0;
            default:
                return true;
        }
    }
    public void IncreaseGrenade(GrenadeType _type,int _num) {
        switch (_type)
        {
            case GrenadeType.FragGrenade:
                FragCount += _num;
                break;
            case GrenadeType.FlashGrenade:
                FlashCount += _num;
                break;
            case GrenadeType.SmokeGrenade:
                SmokeCount += _num;
                break;
        }
        UdateUI();
    }
    public void DecreaseGrenade(GrenadeType _type, int _num)
    {
        IncreaseGrenade(_type, -_num);
    }

        public void ReturnGrenade( Grenade _grenade) 
    {
        grenades.Add(_grenade);
        isThrow = false;
    }

    public void test() {
        Throw(firePos);
    }

    public void SetGrenadeCount(int _frag,int _flash,int _smoke) {
        FragCount = _frag;
        FlashCount = _flash;
        SmokeCount = _smoke;
        UdateUI();

    }

}

using System.Collections.Generic;
using UnityEngine;

// 수류탄 타입
public enum GrenadeType
{
    FragGrenade,
    FlashGrenade,
    SmokeGrenade
}

public class Grenade :MonoBehaviour
{
    FragGrenade frag;
    FlashGrenade flash;
    SmokeGrenade smoke;
    public float _radius;
    public float _delay;
    public float _value;
    [SerializeField] Transform grenadeBase;
    [SerializeField] MeshFilter meshFilter;
    [SerializeField] MeshRenderer meshRenderer;
    public Rigidbody rb;
    [SerializeField] GrenadeType type;
    public GrenadeFactory factory;
    protected void Awake()
    {
        rb= GetComponent<Rigidbody>();
        frag = new FragGrenade();
        flash = new FlashGrenade();
        smoke = new SmokeGrenade();
    }
    private void OnEnable()
    {
        switch(type)
        {
            case GrenadeType.FragGrenade:
                meshFilter.mesh = FragGrenade.mesh;
                meshRenderer.material = FragGrenade.material;
                grenadeBase.localPosition = FragGrenade.midle;
                grenadeBase.localScale = FragGrenade.scale;
                break;
            case GrenadeType.FlashGrenade:
                meshFilter.mesh = FlashGrenade.mesh;
                meshRenderer.material = FlashGrenade.material;
                grenadeBase.localPosition = FlashGrenade.midle;
                grenadeBase.localScale = FlashGrenade.scale;
                break;
            case GrenadeType.SmokeGrenade:
                meshFilter.mesh = SmokeGrenade.mesh;
                meshRenderer.material = SmokeGrenade.material;
                grenadeBase.localPosition = SmokeGrenade.midle;
                grenadeBase.localScale = SmokeGrenade.scale;
                break;
        }
    }

    private void OnDisable()
    {
        if (transform.parent == null)
        {
            Invoke("Pulling", Time.deltaTime);//그냥 호출하면 오류 발생 다음프레임정도에 작동
        }
    }
    private void Pulling() {
            factory.ReturnGrenade(this);
            transform.SetParent(factory.transform);
    }


    // 실제로 사용될 폭발 효과 메서드
    public void Explode()
    {
        switch(type)
        {
            case GrenadeType.FragGrenade:
                StartCoroutine(frag.FlagGrenadeExplode(transform));
                break;
            case GrenadeType.FlashGrenade:
                StartCoroutine(flash.FlashGrenadeExplode(transform));
                break;
            case GrenadeType.SmokeGrenade:
                StartCoroutine(smoke.SmokeGrenadeExplode(transform));
                break;
        }
    }

    public void Changetype(GrenadeType _type ) {
        type = _type;
    }
}

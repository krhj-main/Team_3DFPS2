using System.Collections.Generic;
using UnityEngine;

// 수류탄 타입
public enum GrenadeType
{
    FragGrenade,
    FlashGrenade,
    SmokeGrenade
}

public class Grenade : ThrowingWeapon
{
    FragGrenade frag;
    FlashGrenade flash;
    SmokeGrenade smoke;
    public float _radius;
    public float _delay;
    public float _value;
    MeshFilter meshFilter;
    MeshRenderer meshRenderer;
    [SerializeField] GrenadeType type;
    protected override void Awake()
    {
        base.Awake();
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
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
                break;
            case GrenadeType.FlashGrenade:
                meshFilter.mesh = FlashGrenade.mesh;
                meshRenderer.material = FlashGrenade.material;
                break;
            case GrenadeType.SmokeGrenade:
                meshFilter.mesh = SmokeGrenade.mesh;
                meshRenderer.material = SmokeGrenade.material;
                break;
        }
    }
    // 실제로 사용될 폭발 효과 메서드
    protected override void Explode()
    {
        switch(type)
        {
            case GrenadeType.FragGrenade:
                StartCoroutine(frag.FlagGrenadeExplode(transform, _radius, _delay, _value));
                break;
            case GrenadeType.FlashGrenade:
                StartCoroutine(flash.FlashGrenadeExplode(transform, _radius, _delay, _value));
                break;
            case GrenadeType.SmokeGrenade:
                StartCoroutine(smoke.SmokeGrenadeExplode(transform, _radius, _delay, _value));
                break;
        }
    }

    public void Changetype() {
        switch (type)
        {
            case GrenadeType.FragGrenade:
                type = GrenadeType.FlashGrenade;
                break;
            case GrenadeType.FlashGrenade:
                type = GrenadeType.SmokeGrenade;
                break;
            case GrenadeType.SmokeGrenade:
                type = GrenadeType.FragGrenade;
                break;
        }
    }
}

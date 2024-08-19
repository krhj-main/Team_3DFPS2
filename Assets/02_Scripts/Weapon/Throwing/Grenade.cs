using UnityEngine;

// 수류탄 타입
public enum GrenadeType
{
    FragGrenade,
    FlashGrenade,
    SmokeGrenade
}

public class Grenade : MonoBehaviour
{
    FragGrenade frag = new FragGrenade();
    FlashGrenade flash = new FlashGrenade();
    SmokeGrenade smoke = new SmokeGrenade();

    // 실제로 사용될 폭발 효과 메서드
    public void Explode(Transform _explode, float _radius, float _delay, float _value, GrenadeType _type)
    {
        switch(_type)
        {
            case GrenadeType.FragGrenade:
                StartCoroutine(frag.FlagGrenadeExplode(_explode, _radius, _delay, _value));
                break;
            case GrenadeType.FlashGrenade:
                StartCoroutine(flash.FlashGrenadeExplode(_explode, _radius, _delay, _value));
                break;
            case GrenadeType.SmokeGrenade:
                StartCoroutine(smoke.SmokeGrenadeExplode(_explode, _radius, _delay, _value));
                break;
        }
    }
}

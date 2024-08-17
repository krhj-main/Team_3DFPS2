using UnityEngine;

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

    public void Explode(Transform _explode, float _radius, float _delay, float _value, GrenadeType _type)
    {
        switch(_type)
        {
            case GrenadeType.FragGrenade:
                frag.FlagGrenadeExplode(_explode, _radius, _delay, _value);
                break;
            case GrenadeType.FlashGrenade:
                flash.FlashGrenadeExplode(_explode, _radius, _delay, _value);
                break;
            case GrenadeType.SmokeGrenade:
                smoke.SmokeGrenadeExplode(_explode, _radius, _delay, _value);
                break;
        }
    }
}

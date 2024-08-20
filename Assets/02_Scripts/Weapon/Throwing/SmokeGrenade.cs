using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokeGrenade
{
    public static Mesh mesh;
    public static Material material;

    public SmokeGrenade ()
    {
        if (mesh == null)
        {
            mesh = Resources.Load<Mesh>("Grenades, Bombs & explosives Pack/Models & Textures/Smoke_Grenade/Smoke_Grenade");
        }
        if (material == null)
        {
            material = Resources.Load<Material>("Grenades, Bombs & explosives Pack/Models & Textures/Smoke_Grenade/Materials/Smoke_Grenade_Base_Color");
        }


    }
    public IEnumerator SmokeGrenadeExplode(Transform _explode, float _radius, float _delay, float _effectDuration)
    {
        yield return new WaitForSeconds(_delay);
        _explode.gameObject.SetActive(false);
        // 연막 이펙트 생성
    }
}

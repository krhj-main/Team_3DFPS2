using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokeGrenade
{
    public static Mesh mesh;
    public static Material material;
    public static Vector3 midle = new Vector3(0, -0.0026f, 0);
    public static Vector3 scale = new Vector3(0.5f, 0.5f, 0.5f);
    float radius = 5;
    float delay = 2;
    float effectDuration = 5f;
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
    public IEnumerator SmokeGrenadeExplode(Transform _explode)
    {
        yield return new WaitForSeconds(delay);
        _explode.gameObject.SetActive(false);
        Debug.Log("연막탄 폭발");
        // 연막 이펙트 생성

        yield return new WaitForSeconds(effectDuration);
        // 연막 이펙트 제거
    }
}

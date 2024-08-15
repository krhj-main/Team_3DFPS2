using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokeGrenade : ThrowingWeapon
{
    protected override void Awake()
    {
        base.Awake();
        explosiondelay = 3f;        // 폭발 시간
        explosionRadius = 5f;       // 폭발 반경
        effectDuration = 15f;       // 효과 지속시간
    }


    protected override IEnumerator Explode()
    {
        yield return new WaitForSeconds(explosiondelay);

        // 아래에 연막탄 이펙트 구현, 연막탄으로 Enemy가 받는 영향은 Enemy에서 OnTriggerStay로 구현
    }
}

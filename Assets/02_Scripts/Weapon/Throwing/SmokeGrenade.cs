using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokeGrenade
{
    public IEnumerator SmokeGrenadeExplode(Transform _explode, float _radius, float _delay, float _effectDuration)
    {
        yield return new WaitForSeconds(_delay);
        _explode.gameObject.SetActive(false);
        // 연막 이펙트 생성
    }
}

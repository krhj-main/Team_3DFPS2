using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FragGrenade : ThrowingWeapon
{
    protected override void Awake()
    {
        base.Awake();
        explosiondelay = 5f;         // 폭발 시간
        explosionRadius = 10f;       // 폭발 반경
        damage = 100;                // 데미지
    }

    protected override IEnumerator Explode()
    {
        yield return new WaitForSeconds(explosiondelay);

        /*
        Collider[] _colliders = Physics.OverlapSphere(transform.position, explosionRadius, attackableMask);
        foreach(Collider _collider in _colliders)
        {
            RaycastHit hit;
            Vector3 _hitCollDir = (_collider.transform.position -transform.position).normalized;
            float _distance = Vector3.Distance(transform.position, _collider.transform.position);

            if(Physics.Raycast(transform.position,_hitCollDir,out hit, _distance))
            {
                if(_collider == hit.collider)
                {
                    float _damagePersent = 1 - (_distance / explosionRadius);
                    int _calDamage = Mathf.RoundToInt(damage * _damagePersent);
                    _collider.GetComponent<IDamageAble>().Damaged(_calDamage);
                }
            }
        }
        */
        GameManager.Instance.FlagGrenadeExplode(transform, explosionRadius, damage);
        // 이펙트, 데스트로이 혹은 셋액티브펄스
    }
}

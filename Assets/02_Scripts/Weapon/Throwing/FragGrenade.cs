using System.Collections;
using UnityEngine;

public class FragGrenade
{
    #region "수류탄 효과"
    // 수류탄 효과 ( 데미지 )
    public IEnumerator FlagGrenadeExplode(Transform _explode, float _radius, float _delay ,float _damage)
    {
        yield return new WaitForSeconds(_delay);
        _explode.gameObject.SetActive(false);
        // 플레이어와 폭발한 곳의 거리 계산
        float _distanceToPlayer = Vector3.Distance(_explode.position, PlayerController.Instance.transform.position);
        if (_distanceToPlayer < _radius)
        {
            // 거리별 값 판별 ( 멀어질수록 작은 값 )
            float _damagePersentToPlayer = 1 - (_distanceToPlayer / _radius);
            int _calDamage = Mathf.RoundToInt(_damage * _damagePersentToPlayer);
            PlayerController.Instance.Damaged(_calDamage);
        }

        // 에너미
        foreach (Enemy enemy in GameManager.Instance.enemies)
        {
            RaycastHit hit;
            // 에너미와 폭발물의 방향 계산
            Vector3 _hitDir = (enemy.transform.position - _explode.position).normalized;
            // 에너미와 폭발한 곳의 거리 계산
            float _distance = Vector3.Distance(_explode.position, enemy.transform.position);

            // 폭발물에서 에너미 방향으로 레이 발사
            if (Physics.Raycast(_explode.position, _hitDir, out hit, _distance))
            {
                // 맞은 콜라이더가 에너미가 맞다면 데미지
                if (hit.collider.CompareTag("Enemy"))
                {
                    // 거리별 값 판별 ( 멀어질수록 작은 값 )
                    float _damagePersent = 1 - (_distance / _radius);
                    int _calDamage = Mathf.RoundToInt(_damage * _damagePersent);
                    enemy.Damaged(_calDamage);
                }
            }
        }
        
    }
    #endregion
}

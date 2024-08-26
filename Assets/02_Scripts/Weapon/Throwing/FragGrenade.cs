using System.Collections;
using UnityEngine;

public class FragGrenade
{
    public static Mesh mesh;
    public static Material material;

    public FragGrenade()
    {
        if (mesh == null)
        {
            mesh = Resources.Load<Mesh>("Grenades, Bombs & explosives Pack/Models & Textures/MK2_Frag/MK2FRAG");
        }
        if (material == null) {
            material = Resources.Load<Material>("Grenades, Bombs & explosives Pack/Models & Textures/MK2_Frag/Materials/Grenade_DefaultMaterial_BaseColor");
        }

        
    }
    #region "수류탄 효과"
    // 수류탄 효과 ( 데미지 )
    public IEnumerator FlagGrenadeExplode(Transform _explode, float _radius, float _delay ,float _damage)
    {
        
        yield return new WaitForSeconds(_delay);
        _explode.gameObject.SetActive(false);

        // 폭발 소리로 인한 에너미 어그로
        GameManager.Instance.AggroEnemy(_explode.position, 30f);

        // 플레이어와 폭발한 곳의 거리 계산
        float _distanceToPlayer = Vector3.Distance(_explode.position, PlayerController.Instance.transform.position);
        if (_distanceToPlayer < _radius)
        {
            // 거리별 값 판별 ( 멀어질수록 작은 값 )
            float _damagePersentToPlayer = 1 - (_distanceToPlayer / _radius);
            int _calDamage = Mathf.RoundToInt(_damage * _damagePersentToPlayer);
            PlayerController.Instance.Damaged(_calDamage, Vector3.zero) ;
        }


        for (int i = 0; i < GameManager.Instance.enemies.Count; i++) {
            RaycastHit hit;
            // 에너미와 폭발물의 방향 계산
            Vector3 _hitDir = (GameManager.Instance.enemies[i].transform.position - _explode.position).normalized;
            // 에너미와 폭발한 곳의 거리 계산
            float _distance = Vector3.Distance(_explode.position, GameManager.Instance.enemies[i].transform.position);

            // 폭발물에서 에너미 방향으로 레이 발사
            if (Physics.Raycast(_explode.position, _hitDir, out hit, _distance))
            {
                // 맞은 콜라이더가 에너미가 맞다면 데미지
                if (hit.collider.CompareTag("Enemy"))
                {

                    // 거리별 값 판별 ( 멀어질수록 작은 값 )
                    float _damagePersent = 1 - (_distance / _radius);
                    if (_damagePersent > 0)
                    { //너무 멀면 오히려 피가 차오르는 현상 발생 1퍼센트이상의 대미지를 줄때만 대미지 함수 호출
                        int _calDamage = Mathf.RoundToInt(_damage * _damagePersent);
                        GameManager.Instance.enemies[i].Damaged(_calDamage, hit.point);
                    }
                }
            }
        }
        Debug.Log("수류탄 폭발");
        /*
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
                    if (_damagePersent < 0) { //너무 멀면 오히려 피가 차오르는 현상 발생
                        _damagePersent = 0;
                    }
                    int _calDamage = Mathf.RoundToInt(_damage * _damagePersent);
                    enemy.Damaged(_calDamage,hit.point);
                }
            }
        }*/
        
    }
    #endregion
}

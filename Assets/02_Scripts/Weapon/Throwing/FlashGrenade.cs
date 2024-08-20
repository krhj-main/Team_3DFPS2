using System.Collections;
using UnityEngine;

public class FlashGrenade
{
    float calDuration;
    public Mesh mesh;
    public Material material;
    public FlashGrenade() {
        mesh = Resources.Load<Mesh>("Grenades, Bombs & explosives Pack/Models & Textures/Flashbang/Flashbang.obj/default");

        material = Resources.Load<Material>("Grenades, Bombs & explosives Pack/Models & Textures/Flashbang/Materials/Flashbang_Base_Color");
    }

    #region "섬광탄"
    // 섬광탄 효과 ( 눈뽕, 에너미 멈춤 등 )
    public IEnumerator FlashGrenadeExplode(Transform _explode, float _radius, float _delay, float _effectDuration)
    {
        yield return new WaitForSeconds(_delay);
        _explode.gameObject.SetActive(false);
        // 플레이어와 폭발한 곳의 거리 계산
        float _distanceToPlayer = Vector3.Distance(_explode.position, PlayerController.Instance.transform.position);

        // 거리별 값 판별 ( 멀어질수록 작은 값 )
        float _rangePersentPlayer = 1 - (_distanceToPlayer / _radius);
        calDuration = Mathf.RoundToInt(_effectDuration * _rangePersentPlayer);

        if (_distanceToPlayer <= _radius)
        {
            if (IsLookingAtFlash(_explode, PlayerController.Instance.transform))
            {
                // 거리별 값 판별 ( 멀어질수록 작은 값 )
                //float _rangePersentToPlayer = 1 - (_distanceToPlayer / _radius);
                //calduration = Mathf.RoundToInt(_effectDuration * _rangePersentToPlayer);
                // 눈뽕
                UIManager.Instance.FlashImage.gameObject.SetActive(true);
            }
        }

        // 에너미
        foreach (Enemy enemy in GameManager.Instance.enemies)
        {
            // 에너미와 폭발한 곳의 거리 계산
            float _distance = Vector3.Distance(_explode.position, enemy.transform.position);

            // 거리별 값 판별 ( 멀어질수록 작은 값 )
            float _rangePersent = 1 - (_distance / _radius);
            float _baseTime = 1.5f;
            enemy.blindTime = Mathf.RoundToInt(_effectDuration * _rangePersent) + _baseTime;

            // 거리가 범위 이내라면
            if (_distance < _radius)
            {
                // Enemy가 섬광탄을 보고있다면
                if (IsLookingAtFlash(_explode, enemy.transform))
                {
                    enemy.enemyState = EnemyState.Blind;
                }
            }
        }

        // 거리별 시간 이후 시야 복구
        yield return new WaitForSeconds(calDuration);

        UIManager.Instance.FlashImage.gameObject.SetActive(false);

    }

    // 캐릭터가 섬광탄을 보고있는지 판단하는 메서드
    bool IsLookingAtFlash(Transform _flash, Transform _character)
    {
        // 플레이어 위치에서 섬광탄 위치로의 방향 벡터를 계산
        Vector3 _dirToFlash = _flash.position - _character.position;

        // 카메라가 바라보는 방향과, 플레이어에서 섬광탄으로의 방향 사이의 각도를 계산
        float angle = Vector3.Angle(_character.forward, _dirToFlash);
        // 시야각 확인 // 60 = 좌우로 60
        if (angle < 60f)
        {
            // 레이캐스트로 장애물 체크
            RaycastHit hit;
            if (Physics.Raycast(_character.position, _dirToFlash, out hit))
            {
                // 레이캐스트가 섬광탄에 먼저 닿았는지 확인
                if (hit.collider.gameObject == _flash.gameObject)
                {
                    return true;
                }
            }
        }
        return false;
    }
    #endregion
}

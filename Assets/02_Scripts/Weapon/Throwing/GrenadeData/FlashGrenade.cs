using System.Collections;
using System.Linq.Expressions;
using UnityEngine;

public class FlashGrenade
{
    float calDuration;
    public static Mesh mesh;
    public static Material material;
    public static Sprite sprite;
    public static Vector3 midle = new Vector3(0, -0.0052f, 0);
    public static Vector3 scale = new Vector3(0.75f, 0.75f, 0.75f);
    public static GameObject effect;
    float radius=10;
    float delay=3f;
    float effectDuration=5f;
    public FlashGrenade() {
        if (mesh == null) {
            mesh = Resources.Load<Mesh>("Grenades, Bombs & explosives Pack/Models & Textures/Flashbang/Flashbang");
        }

        if (material == null) {
            material = Resources.Load<Material>("Grenades, Bombs & explosives Pack/Models & Textures/Flashbang/Materials/Flashbang_Base_Color");
        }
        if (effect == null)
        {
            effect = Resources.Load<GameObject>("FlashGranadeEffect");
        }
        if (sprite == null) {
            Sprite[] spriteAll = Resources.LoadAll<Sprite>("Light theme spritesheet 1");
            sprite = spriteAll[27];
        }
        
    }

    #region "섬광탄"
    // 섬광탄 효과 ( 눈뽕, 에너미 멈춤 등 )
    public IEnumerator FlashGrenadeExplode(Transform _explode)
    {
        yield return new WaitForSeconds(delay);
        GameObject go = GameObject.Instantiate(effect);
        go.transform.position = _explode.position;
        _explode.gameObject.SetActive(false);
        // 플레이어와 폭발한 곳의 거리 계산
        float _distanceToPlayer = Vector3.Distance(_explode.position, PlayerController.Instance.transform.position);

        // 거리별 값 판별 ( 멀어질수록 작은 값 )
        float _rangePersentPlayer = 1 - (_distanceToPlayer / radius);
        calDuration = Mathf.RoundToInt(effectDuration * _rangePersentPlayer);
        
        if (_distanceToPlayer <= radius)
        {
            if (IsLookingAtFlash(_explode, PlayerController.Instance.waist))
            {
                // 거리별 값 판별 ( 멀어질수록 작은 값 )
                //float _rangePersentToPlayer = 1 - (_distanceToPlayer / _radius);
                //calduration = Mathf.RoundToInt(_effectDuration * _rangePersentToPlayer);
                // 눈뽕
                UIManager.Instance.FlashImage.Duration = calDuration;
            }
        }

        for (int i = 0; i < GameManager.Instance.enemies.Count; i++)
        {
            float _distance = Vector3.Distance(_explode.position, GameManager.Instance.enemies[i].transform.position);
            // 거리가 범위 이내라면
            if (_distance < radius)
            {
                // Enemy가 섬광탄을 보고있다면
                if (IsLookingAtFlash(_explode, GameManager.Instance.enemies[i].transform))
                {// 거리별 값 판별 ( 멀어질수록 작은 값 )
                    float _rangePersent = 1 - (_distance / radius);
                    float _baseTime = 1.5f;
                    GameManager.Instance.enemies[i].blindTime = Mathf.RoundToInt(effectDuration * _rangePersent) + _baseTime;
                    GameManager.Instance.enemies[i].enemyState = EnemyState.Blind;
                }
            }
        }
        /*
        // 거리별 시간 이후 시야 복구
        yield return new WaitForSeconds(calDuration);
        UIManager.Instance.FlashImage.gameObject.SetActive(false);
        */
    }

    // 캐릭터가 섬광탄을 보고있는지 판단하는 메서드
    bool IsLookingAtFlash(Transform _flash, Transform _character)
    {
        // 플레이어 위치에서 섬광탄 위치로의 방향 벡터를 계산
        Vector3 _dirToFlash = _flash.position - _character.position;

        // 카메라가 바라보는 방향과, 플레이어에서 섬광탄으로의 방향 사이의 각도를 계산
        float angle = Vector3.Angle(_character.forward, _dirToFlash);

        // 시야각 확인 // 60 = 좌우로 60
        if (angle <= 90)
        {
            // 레이캐스트로 장애물 체크
            RaycastHit hit;
            if (Physics.Raycast(_flash.position, _dirToFlash*-1, out hit))
            {
                Debug.DrawRay(_flash.position, _dirToFlash * -1, Color.black, 10f);
                // 레이캐스트가 섬광탄에 먼저 닿았는지 확인
                if (hit.collider.CompareTag(_character.tag))
                {
                    return true;
                }
            }
        }
        return false;
    }
    #endregion
}

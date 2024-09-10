using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimEvent : MonoBehaviour
{
    [SerializeField] MainWeapon weapon;

    // 플레이어의 직접적인 움직임과 관련해서 소리가 나서 Enemy의 어그로가 끌리는 이벤트 ( 애니메이션에 적용 )
    public void PlayerAggroEnemy(float _radius)
    {
        GameManager.Instance.AggroEnemy(transform.position, _radius);
    }

    public void PlayerWalkSound(float _radius)
    {
        if (PlayerController.Instance.anim.GetFloat("Speed") >= 1f)
        {
            PlayerController.Instance.playerSound.clip = PlayerController.Instance.walkSound;
            PlayerController.Instance.playerSound.Play();
            GameManager.Instance.AggroEnemyFoot(transform.position, _radius);
        }
    }

    public void PlayerFireBullet()
    {
        weapon.PlayerFireBullet();
    }

    #region "샷건 재장전 로직"
    public void PlayerShotgunReload()
    {
        weapon.ReloadFinish();
    }

    public void PlayerShotgunReloadCheck()
    {
        weapon.ReloadCheck();
    }
    #endregion

    #region "무기 재장전 로직"
    public void ReloadEnter()
    {
        weapon.ReloadEnter();
    }
    public void ReloadExit()
    {
        weapon.ReloadExit();
    }
    #endregion


    public void PlayerReloadSound()
    {
        weapon.PlayerReloadSound();
    }

    public void PlayerReloadFinishSound()
    {
        weapon.PlayerReloadFinishSound();
    }
}

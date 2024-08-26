using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimEvent : MonoBehaviour
{
    // 플레이어의 직접적인 움직임과 관련해서 소리가 나서 Enemy의 어그로가 끌리는 이벤트 ( 애니메이션에 적용 )
    public void PlayerAggroEnemy(float _radius)
    {
        GameManager.Instance.AggroEnemy(transform.position, _radius);
    }

    public void PlayerWalkSound()
    {
        PlayerController.Instance.playerSound.clip = PlayerController.Instance.walkSound;
        PlayerController.Instance.playerSound.Play();
    }
}

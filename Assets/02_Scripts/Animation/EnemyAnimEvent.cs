using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimEvent : MonoBehaviour
{
    [SerializeField] MainWeapon weapon;
    [SerializeField] Enemy enemy;

    public void EnemyWalkSound()
    {
        weapon.enemySound.clip = enemy.walkSound;
        weapon.enemySound.Play();
    }

    public void EnemyFireBullet()
    {
        weapon.EnemyFireBullet();
    }
}

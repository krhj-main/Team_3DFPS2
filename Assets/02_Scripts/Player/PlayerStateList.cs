using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateList : MonoBehaviour
{
    public bool isMoving;                              // 플레이어 이동중인지 확인
    public bool isJumping;                              // 점프상태인지 확인
    public bool isRunning;                           // 달리기 상태인지
    public bool isWalking;                           // 걷기 상태인지
    public bool isCrouch;                            // 앉기 상태인지
    public bool isTiltingL;                           // 기울이기 상태인지
    public bool isTiltingR;                           // 기울이기 상태인지

    public bool isOnViewer;                         // 뷰어를 켰는지
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimIKPlayer : MonoBehaviour
{

    // 총 위치 변수
    public Transform[] leftHands;
    public Transform[] rightHands;
    public int currentIkIndex = 0;
    Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }


    // 애니메이터의 IK 갱신
    private void OnAnimatorIK(int layerIndex)
    {
        // IK를 사용하여 왼손의 위치와 회전을 총의 오른쪽 손잡이에 맞춘다
        anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1f);
        anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1f);

        anim.SetIKPosition(AvatarIKGoal.LeftHand, leftHands[currentIkIndex].position);
        anim.SetIKRotation(AvatarIKGoal.LeftHand, leftHands[currentIkIndex].rotation);

        
        // IK를 사용하여 오른손의 위치와 회전을 총의 오른쪽 손잡이에 맞춘다
        anim.SetIKPositionWeight(AvatarIKGoal.RightHand, 1.0f);
        anim.SetIKRotationWeight(AvatarIKGoal.RightHand, 1.0f);

        anim.SetIKPosition(AvatarIKGoal.RightHand, rightHands[currentIkIndex].position);
        anim.SetIKRotation(AvatarIKGoal.RightHand, rightHands[currentIkIndex].rotation);
        
    }
}

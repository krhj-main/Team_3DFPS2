using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimIK : MonoBehaviour
{
    // 총 위치 변수
    public Transform gunPivot;
    public Transform leftHand;
    public Transform rightHand;

    Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        
    }

    // Update에서 플레이어가 총을 갖고있을때 (예외처리) -> 갖고있는 총의 자식에 있는 LeftHand, RightHand를 -> 여기 leftHand, rightHand에 대입

    // 총이 변경될 때 마다 -> 갖고있는 총의 자식에 있는 LeftHand, RightHand를 -> 여기 leftHand, rightHand에 대입

    // 애니메이터의 IK 갱신
    private void OnAnimatorIK(int layerIndex)
    {
        
        if (leftHand == null || rightHand == null)
        {
            return;
        }
        
        // 총의 기준점 gunPivot을 3D 모델의 오른쪽 팔꿈치 위치로 이동
        //gunPivot.position = anim.GetIKHintPosition(AvatarIKHint.RightElbow);  

        // IK를 사용하여 왼손의 위치와 회전을 총의 오른쪽 손잡이에 맞춘다
        anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1f);
        anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1f);

        anim.SetIKPosition(AvatarIKGoal.LeftHand, leftHand.position);
        anim.SetIKRotation(AvatarIKGoal.LeftHand, leftHand.rotation);

        
        // IK를 사용하여 오른손의 위치와 회전을 총의 오른쪽 손잡이에 맞춘다
        anim.SetIKPositionWeight(AvatarIKGoal.RightHand, 1.0f);
        anim.SetIKRotationWeight(AvatarIKGoal.RightHand, 1.0f);

        anim.SetIKPosition(AvatarIKGoal.RightHand, rightHand.position);
        anim.SetIKRotation(AvatarIKGoal.RightHand, rightHand.rotation);
        
    }
}

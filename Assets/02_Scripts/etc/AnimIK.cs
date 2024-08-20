using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimIK : MonoBehaviour
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

    
    // 애니메이터의 IK 갱신
    private void OnAnimatorIK(int layerIndex)
    {
        // 총의 기준점 gunPivot을 3D 모델의 오른쪽 팔꿈치 위치로 이동
        //gunPivot.position = anim.GetIKHintPosition(AvatarIKHint.RightElbow);

        anim.SetLookAtPosition(PlayerController.Instance.transform.position);
        anim.SetLookAtWeight(0.8f);   

        // IK를 사용하여 왼손의 위치와 회전을 총의 오른쪽 손잡이에 맞춘다
        anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1f);
        anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1f);

        anim.SetIKPosition(AvatarIKGoal.LeftHand, leftHand.position);
        anim.SetIKRotation(AvatarIKGoal.LeftHand, leftHand.rotation);

        /*
        // IK를 사용하여 오른손의 위치와 회전을 총의 오른쪽 손잡이에 맞춘다
        anim.SetIKPositionWeight(AvatarIKGoal.RightHand, 1.0f);
        anim.SetIKRotationWeight(AvatarIKGoal.RightHand, 1.0f);

        anim.SetIKPosition(AvatarIKGoal.RightHand, rightHand.position);
        anim.SetIKRotation(AvatarIKGoal.RightHand, rightHand.rotation);
        */
    }
}

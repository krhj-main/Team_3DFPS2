using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimIK : MonoBehaviour
{

    // 총 위치 변수
    public Transform gunPivot;
    public Transform leftHand;
    public Transform rightHand;
    public Vector3 Offset;
    public bool trigger; 
    Animator anim;
    Transform chest;
    private void Awake()
    {
        Offset = new Vector3(-34.3f, 31.4f, -60.40f);
        anim = GetComponent<Animator>();
        if (anim)

            chest = anim.GetBoneTransform(HumanBodyBones.UpperChest); // 해당 본의 transform 가져오기
    }
    private void LateUpdate()
    {

        //Offset= chest.localRotation.eulerAngles;
        //Debug.Log(chest.localRotation.eulerAngles);
        if (trigger) {
            chest.LookAt(PlayerController.Instance.waist.position);
            chest.localRotation = Quaternion.Euler(Offset+ chest.localRotation.eulerAngles);
            Debug.Log(chest.localRotation.eulerAngles);
        }

    }

    void test() { 
    
    
    }

    // 애니메이터의 IK 갱신
    private void OnAnimatorIK(int layerIndex)
    {
        // 총의 기준점 gunPivot을 3D 모델의 오른쪽 팔꿈치 위치로 이동
        //gunPivot.position = anim.GetIKHintPosition(AvatarIKHint.RightElbow);

        anim.SetLookAtPosition(PlayerController.Instance.transform.position);
        anim.SetLookAtWeight(0.3f);

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

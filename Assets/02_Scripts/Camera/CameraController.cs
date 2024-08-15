using UnityEngine;

public class CameraController : MonoBehaviour
{
    // 카메라 기울기의 최대 회전 각도
    public float tiltAngle = 15f;
    // 카메라 기울기 시 이동할 최대 거리
    public float tiltPositionOffset = 0.7f;
    // 카메라 움직임의 부드러움을 조절하는 속도
    public float smoothSpeed = 5f;

    // 카메라의 초기 위치를 저장
    private Vector3 originalPosition;
    // 카메라의 초기 회전값을 저장
    private Quaternion originalRotation;

    // 목표로 하는 기울어진 위치
    private Vector3 targetTiltPosition;
    // 목표로 하는 기울어진 회전값
    private Quaternion targetTiltRotation;

    // 현재의 반동으로 인한 회전값
    private Vector3 recoilRotation;
    // 최종적으로 적용될 회전값
    private Vector3 finalRotation;

    // 현재 장착된 무기에 대한 참조
    private MainWeapon mainWeapon;

    Vector3 oriPos;

    float lastRecoilTime;
    float recoilRecoveryDelay = 0.2f;

    void Start()
    {
        oriPos = transform.position;
        // 초기 위치와 회전값을 저장
        originalPosition = transform.localPosition;
        originalRotation = transform.localRotation;
    }

    void Update()
    {
        // 매 프레임마다 실행되는 주요 함수들
        UpdateWeaponReference();
        UpdateTilt();
        UpdateRecoil();
        ApplyFinalRotation();
    }

    void UpdateWeaponReference()
    {
        // 현재 장착된 무기를 찾아 참조를 업데이트
        mainWeapon = GetComponentInParent<CharacterController>().GetComponentInChildren<MainWeapon>();
    }

    void UpdateTilt()
    {
        // 오른쪽으로 기울이는 경우
        if (PlayerController.Instance.pState.isTiltingR)
        {
            targetTiltPosition = new Vector3(tiltPositionOffset, 0, 0);
            targetTiltRotation = Quaternion.Euler(0, 0, -tiltAngle);
        }
        // 왼쪽으로 기울이는 경우
        else if (PlayerController.Instance.pState.isTiltingL)
        {
            targetTiltPosition = new Vector3(-tiltPositionOffset, 0, 0);
            targetTiltRotation = Quaternion.Euler(0, 0, tiltAngle);
        }
        // 기울이지 않는 경우
        else
        {
            targetTiltPosition = Vector3.zero;
            targetTiltRotation = Quaternion.identity;
        }

        // 현재 위치에서 목표 위치로 부드럽게 이동
        transform.localPosition = Vector3.Lerp(transform.localPosition, originalPosition + targetTiltPosition, Time.deltaTime * smoothSpeed);
    }

    void UpdateRecoil()
    {
        // 무기가 있으면 해당 무기의 반동 회복 속도를 사용, 없으면 기본값 5 사용
        float recoilRecoverySpeed = mainWeapon != null ? mainWeapon.recoilRecoverySpeed : 5f;

        // 반동을 서서히 0으로 줄임
        if(Time.time - lastRecoilTime > recoilRecoveryDelay)
        {
            recoilRotation = Vector3.Lerp(recoilRotation, Vector3.zero, Time.deltaTime * recoilRecoverySpeed);
        }
    }

    void ApplyFinalRotation()
    {
        // 현재 회전값에서 목표 회전값으로 부드럽게 전환 ( 기울기 )
        Quaternion tiltRotation = Quaternion.Slerp(transform.localRotation, targetTiltRotation, Time.deltaTime * smoothSpeed);

        // 반동 회전값을 Quaternion으로 변환
        Quaternion recoilRotationQuat = Quaternion.Euler(recoilRotation);

        // 원래 회전값, 기울기 회전값, 반동 회전값을 모두 합산
        finalRotation = (originalRotation * tiltRotation * recoilRotationQuat).eulerAngles;

        // 최종 회전값을 카메라에 적용
        transform.localRotation = Quaternion.Euler(finalRotation);

    }

    public void ApplyRecoil(float recoilX, float recoilY)
    {
        // 새로운 반동값을 현재 반동에 추가
        recoilRotation += new Vector3(-recoilY, Random.Range(-recoilX, recoilX), 0);
        recoilRotation.x = Mathf.Clamp(recoilRotation.x, -1.5f, 0f);

        lastRecoilTime = Time.time;
    }

    public Vector3 GetTiltRotation()
    {
        // 현재의 기울기 회전값을 반환
        return targetTiltRotation.eulerAngles;
    }
}
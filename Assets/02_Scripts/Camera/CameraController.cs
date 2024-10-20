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
    private Vector3 targetRotation;

    // 현재 장착된 무기에 대한 참조
    private IEquipMent mainWeapon;

    Vector3 oriPos;

    float lastRecoilTime;
    float recoilRecoveryDelay = 0.2f;
    private Vector2 mouseDelta;
    public static float mouseSensitivity = 2f;
    Quaternion rot;
    Quaternion tilt;
    Vector3 recoilAmount;

    [SerializeField] Transform arm;
    [SerializeField] Transform waist;
    void Start()
    {
        oriPos = arm.position;
        // 초기 위치와 회전값을 저장
        originalPosition = waist.localPosition;
        originalRotation = arm.localRotation;
        mainWeapon = GetComponentInParent<EquipmentsSwap>().equip;
        tilt = Quaternion.Euler(Vector3.zero);
    }

    void Update()
    {
        if (PlayerController.Instance.PlayerCamera.enabled) {
            // 매 프레임마다 실행되는 주요 함수들
            UpdateTilt();
            UpdateRecoil();
            ApplyFinalRotation();
            LookAround();
        }
    }
    void LookAround()
    {
        if (GameManager.Instance.openUI || PlayerController.Instance.pState.isOnViewer || PlayerController.Instance.pState.isOnESCMenu)
        {
            return;
        }


        mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        Vector3 _camAngle = arm.localRotation.eulerAngles;

        float _limit = _camAngle.x - mouseDelta.y;

        if (_limit < 180)
        {
            _limit = Mathf.Clamp(_limit, -1f, 60f);
        }
        else
        {
            _limit = Mathf.Clamp(_limit, 320f, 361f);
        }
        rot = Quaternion.Euler(_limit, _camAngle.y + (mouseDelta.x * mouseSensitivity), _camAngle.z);
    }

    void UpdateTilt()
    {
        // 오른쪽으로 기울이는 경우
        if (PlayerController.Instance.pState.isTiltingR)
        {
            targetTiltPosition = new Vector3(+tiltPositionOffset, 0, 0);
            targetTiltRotation = Quaternion.Euler(new Vector3(0, 0, -tiltAngle));
        }
        // 왼쪽으로 기울이는 경우
        else if (PlayerController.Instance.pState.isTiltingL)
        {
            targetTiltPosition = new Vector3(-tiltPositionOffset, 0, 0);
            targetTiltRotation = Quaternion.Euler( new Vector3(0, 0, +tiltAngle));
        }
        // 기울이지 않는 경우
        else
        {
            targetTiltPosition = Vector3.zero;
            targetTiltRotation = Quaternion.Euler(Vector3.zero);
        }

        // 현재 위치에서 목표 위치로 부드럽게 이동
        waist.localPosition = Vector3.Lerp(waist.localPosition,targetTiltPosition+ originalPosition, Time.deltaTime * smoothSpeed);
    }

    void UpdateRecoil()
    {
        // 무기가 있으면 해당 무기의 반동 회복 속도를 사용, 없으면 기본값 5 사용
        float recoilRecoverySpeed = (mainWeapon != null&& mainWeapon.type==EquipType.Weapon) ? ((MainWeapon)mainWeapon).recoilRecoverySpeed : 5f;

        // 반동방향에 따라 다르게 작동 변화량을 저장
        if (Vector3.Distance(recoilRotation, targetRotation) < 0.5f) {
        //중앙으로 점점 돌아옴
            recoilRotation = Vector3.Lerp(recoilRotation, Vector3.zero, Time.deltaTime * recoilRecoverySpeed);
            recoilAmount += (Vector3.zero - recoilRotation) * Time.deltaTime * recoilRecoverySpeed;
            targetRotation = recoilRotation;
        }
        else {
        //화면 윗부분으로 이동
            recoilRotation = Vector3.Lerp(recoilRotation, targetRotation, Time.deltaTime * recoilRecoverySpeed*1.5f);
            recoilAmount += (targetRotation - recoilRotation) * Time.deltaTime * recoilRecoverySpeed * 1.5f;
        }
    }

    void ApplyFinalRotation()
    {
        // 현재 회전값에서 목표 회전값으로 부드럽게 전환 ( 기울기 )
        tilt = Quaternion.Slerp(tilt, targetTiltRotation, Time.deltaTime * smoothSpeed);
        // 반동 회전값을 Quaternion으로 변환
        // 원래 회전값, 기울기 회전값, 반동 회전 변화량값을 모두 합산
        arm.localRotation = Quaternion.Euler(new Vector3(rot.eulerAngles.x, rot.eulerAngles.y, tilt.eulerAngles.z) + recoilAmount);
        recoilAmount = Vector3.zero;
    }

    public void ApplyRecoil(float recoilX, float recoilY)
    {
        Vector3 recoil= new Vector3(-recoilY, Random.Range(-recoilX, recoilX), 0);
        recoil.x = Mathf.Clamp(recoil.x, -100f, 100f);
        // 새로운 반동값을 현재 반동에 추가
        targetRotation = recoilRotation + recoil;
        targetRotation.x = Mathf.Clamp(targetRotation.x, -40, 360);
        
        lastRecoilTime = Time.time;
    }

    public Vector3 GetTiltRotation()
    {
        // 현재의 기울기 회전값을 반환
        return targetTiltRotation.eulerAngles;
    }
}

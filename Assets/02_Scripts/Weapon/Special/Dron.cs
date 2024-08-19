using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dron : MonoBehaviour,Interectable
{
    //총을 상속받아 총이랑 비슷하게 동작

    //발사 대신에 드론을 던짐 장탄수는 1
    //드론을 직접 주으면 장탄수가 늘어남

    //드론이 던져진 상태에서 사용시 드론으로 카메라 전환하고 드론조작
    //특정 키를 누르면 원래 카메라로 돌아옴
    //-> 돌아올 카메라를 담고있어야 한다,카메라에 따라 조작이 달라져야 한다

    Rigidbody rig;
    Camera dronCam;
    float jumpeForce = 5f;
    float moveSpeed = 5f;
    float rotateSpeed = 10;
    float h=0;
    float v=0;
    public DronController dronController;

    public KeyCode returnKey;

    bool isActive = false;
    // Start is called before the first frame update
    void Start()
    {
        rig = GetComponent<Rigidbody>();
        dronCam = GetComponentInChildren<Camera>();
    }
    private void FixedUpdate()
    {
        Vector3 vel = transform.forward * v * moveSpeed;
        vel.y = rig.velocity.y;
        rig.velocity = vel;

        transform.Rotate(Vector3.up * rotateSpeed * h);
        
    }

    // Update is called once per frame
    void Update()
    {
        if (dronCam.enabled&&isActive) {
            h = Input.GetAxis("Horizontal");
            v = Input.GetAxis("Vertical");
            if (Input.GetKeyDown(KeyCode.Space))
            {
                rig.AddForce(new Vector3(0, jumpeForce, 0), ForceMode.Impulse);
            }
            if (Input.GetKeyDown(returnKey))
            {
                dronCam.enabled = false;
                dronController.DronReturn();
            }
        }
    }
    public void DronAwake() {
        dronCam.enabled = true;
        dronController.charCamera.enabled = false;
        transform.rotation = Quaternion.Euler(0,0,0);
        //rig.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    public void Interection(GameObject target)
    {
        dronController.isOut = false;
        gameObject.SetActive(false);
        transform.SetParent(dronController.transform);
        isActive = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!isActive) {
            Quaternion rot = Quaternion.Euler(0, 0, 0);
            rot.y = transform.rotation.y;
            transform.rotation = rot;
            isActive = true;
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CompassSystem : MonoBehaviour
{
    [Tooltip ("나침반 눈금 이미지")]
    public RawImage CompassImage;
    [Tooltip("전방이 바뀌는 값, 이 프로젝트에서는 플레이어의 방향을 구하는 카메라 암")]
    public Transform player;
    [Tooltip("바라본 방향의 값을 표시할 텍스트")]
    public Text CompassDirectionText;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CompassImage.uvRect = new Rect(player.localEulerAngles.y / 360, 0, 1, 1);

        Vector3 forward = player.transform.forward;

        forward.y = 0;

        float headingAngle = Quaternion.LookRotation(forward).eulerAngles.y;
        headingAngle = 5 * (Mathf.RoundToInt(headingAngle / 5.0f));

        int displayAngle = Mathf.RoundToInt(headingAngle);

        switch (displayAngle)
        {
            case 0:
                CompassDirectionText.text = "N";
                break;
            case 360:
                CompassDirectionText.text = "N";
                break;
            case 45:
                CompassDirectionText.text = "NE";
                break;
            case 90:
                CompassDirectionText.text = "E";
                break;
            case 130:
                CompassDirectionText.text = "SE";
                break;
            case 180:
                CompassDirectionText.text = "S";
                break;
            case 225:
                CompassDirectionText.text = "SW";
                break;
            case 270:
                CompassDirectionText.text = "W";
                break;
            default:
                CompassDirectionText.text = headingAngle.ToString();
                break;

        }
        //Debug.Log(displayAngle);
    }

    
}

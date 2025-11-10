using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportStraight : MonoBehaviour
{

    public Transform teleportCircleUI; //텔레포트를 표시할 UI
    LineRenderer lr; //선을 그릴 라인 렌더러
    Vector3 originScale = Vector3.one * 0.02f; //최초 텔레포트 UI 크기

    // Start is called before the first frame update
    void Start()
    {
        teleportCircleUI.gameObject.SetActive(false); //시작할때 비활성화
        lr = GetComponent<LineRenderer>(); //라인 렌더러 컴포 얻어오기
    }

    // Update is called once per frame
    void Update()
    {
        if(ARAVRInput.GetDown(ARAVRInput.Button.One, ARAVRInput.Controller.LTouch)) //왼쪽 컨트롤러 One 버튼을 누르면
        {
            lr.enabled = true; //라인 렌더러 컴포넌트 활성화
        }

        else if(ARAVRInput.GetUp(ARAVRInput.Button.One, ARAVRInput.Controller.LTouch)) //왼쪽 컨트롤러 버튼에서 손을 떼면
        {
            lr.enabled = false; //라인 렌더러 비활성화
            if(teleportCircleUI.gameObject.activeSelf)
            {
                GetComponent<CharacterController>().enabled = false;
                transform.position = teleportCircleUI.position + Vector3.up; //텔레포트 UI 위치로 순간 이동
                GetComponent<CharacterController>().enabled = true;
            }

            teleportCircleUI.gameObject.SetActive(false); //텔레포트 UI 비활성화
        }

        else if(ARAVRInput.Get(ARAVRInput.Button.One, ARAVRInput.Controller.LTouch)) //왼쪽 컨트롤러 One 버튼을 누르고 있을 때
        {
            Ray ray = new Ray(ARAVRInput.LHandPosition, ARAVRInput.LHandDirection); //왼쪽 컨트롤러 기준으로 Ray 만듦
            RaycastHit hitInfo;
            int layer = 1 << LayerMask.NameToLayer("Terrain"); //배경과 충돌
            if(Physics.Raycast(ray, out hitInfo, 200, layer)) //Terrain만 Ray 충돌 검출
            {
                lr.SetPosition(0, ray.origin); //Ray 부딪힌 지점에 라인 그리기
                lr.SetPosition(1, hitInfo.point);
                teleportCircleUI.gameObject.SetActive(true); //Ray가 부딪힌 지점에 텔레포트 UI 표시
                teleportCircleUI.position = hitInfo.point;
                teleportCircleUI.forward = hitInfo.normal; //텔레포트 UI가 위로 누워있도록 방향 설정
                teleportCircleUI.localScale = originScale * Mathf.Max(1, hitInfo.distance); //텔레포트 UI 크기가 거리에 따라 보정되도록
            }
            else
            {
                lr.SetPosition(0, ray.origin); //Ray 충돌이 발생하지 않으면 선이 Ray 방향으로 그려지도록 처리
                lr.SetPosition(1, ray.origin + ARAVRInput.LHandDirection * 200); 
                teleportCircleUI.gameObject.SetActive(false); //텔레포트 UI는 화면에서 비활성화
            }
        }
    }
}

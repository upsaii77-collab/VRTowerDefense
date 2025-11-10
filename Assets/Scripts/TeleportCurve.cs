using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportCurve : MonoBehaviour
{

    public Transform teleportCircleUI; //텔레포트 표시할 UI
    LineRenderer lr; //선 그릴 라인 렌더러
    Vector3 originScale = Vector3.one * 0.02f; //최초 텔레포트 UI 크기
    public int lineSmooth = 40; //커브의 부드러운 정도
    public float curveLength = 50; //커브 길이
    public float gravity = -60; //커브 중력
    public float simulateTime = 0.02f; //곡선 시뮬레이션 간격 및 시간
    List<Vector3> lines = new List<Vector3>(); //곡선 이루는 점들을 기억할 리스트

    // Start is called before the first frame update
    void Start()
    {
        teleportCircleUI.gameObject.SetActive(false);
        lr = GetComponent<LineRenderer>();
        lr.startWidth = 0.2f;
        lr.endWidth = 0.4f;
    }

    // Update is called once per frame
    void Update()
    {
        if(ARAVRInput.GetDown(ARAVRInput.Button.HandTrigger, ARAVRInput.Controller.RTouch)) //마우스 버전이고 VR버전에서는 One 버튼, LTouch
        {
            lr.enabled = true; //라인 렌더러 활성화
        }

        else if(ARAVRInput.GetUp(ARAVRInput.Button.HandTrigger, ARAVRInput.Controller.RTouch))
        {
            lr.enabled = false;
            if(teleportCircleUI.gameObject.activeSelf)
            {
                GetComponent<CharacterController>().enabled = false;
                transform.position = teleportCircleUI.position + Vector3.up;
                GetComponent<CharacterController>().enabled = true;
            }
            teleportCircleUI.gameObject.SetActive(false);
        }

        else if(ARAVRInput.Get(ARAVRInput.Button.HandTrigger, ARAVRInput.Controller.RTouch))
        {
            MakeLines(); //주어진 길이 크기의 커브 만들고 싶다
        }
    }

    void MakeLines()
    {
        lines.RemoveRange(0, lines.Count); //리스트에 담긴 위치 정보들을 비워줌
        Vector3 dir = ARAVRInput.RHandDirection * curveLength;
        Vector3 pos = ARAVRInput.RHandPosition;
        lines.Add(pos);

        for(int i = 0; i < lineSmooth; i++) //라인 스무스 개수만큼 반복
        {
            Vector3 lastPos = pos; //현재 위치 기억
            dir.y += gravity * simulateTime; //중력 적용한 속도 계산
            pos += dir * simulateTime; //등속 운동으로 다음 위치 계산

            if(CheckHitRay(lastPos, ref pos))
            {
                lines.Add(pos);
                break;
            }
            else
            {
                teleportCircleUI.gameObject.SetActive(false);
            }
            lines.Add(pos);//구한 위치 등록
        }
        lr.positionCount = lines.Count; //라인 렌더러가 표현할 점의 개수를 등록된 개수의 크기로 할당
        lr.SetPositions(lines.ToArray()); //러안 렌더러에 구해진 점의 정보 지정
    }

    private bool CheckHitRay(Vector3 lastPos, ref Vector3 pos)
    {
        Vector3 rayDir = pos - lastPos;
        Ray ray = new Ray(lastPos, rayDir);
        RaycastHit hitInfo;

        if(Physics.Raycast(ray, out hitInfo, rayDir.magnitude))
        {
            pos = hitInfo.point;
            int layer = LayerMask.NameToLayer("Terrain");
            if(hitInfo.transform.gameObject.layer == layer)
            {
                teleportCircleUI.gameObject.SetActive(true);
                teleportCircleUI.position = pos;
                teleportCircleUI.forward = hitInfo.normal;
                float distance = (pos - ARAVRInput.RHandPosition).magnitude;
                teleportCircleUI.localScale = originScale * Mathf.Max(1, distance);
            }
            return true;
        }
        return false;
    }
}

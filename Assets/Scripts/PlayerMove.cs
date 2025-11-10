using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float speed = 5; //이동속도 
    CharacterController cc; //CharacterController 컴포넌트
    public float gravity = -10; //중력 가속도 크기
    float yVelocity = 0; //수직 속도
    public float jumpPower = 5; //점프 크기

    // Start is called before the first frame update
    void Start()
    {
        cc = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        //사용자의 입력에 따라 전후좌우로 이동
        float h = ARAVRInput.GetAxis("Horizontal"); //사용자의 입력을 받음
        float v = ARAVRInput.GetAxis("Vertical");
        Vector3 dir = new Vector3(h, 0, v); //3차원 구조의 방향 생성

        dir = Camera.main.transform.TransformDirection(dir);  //사용자가 바라보는 방향으로 입력 값 변화

        yVelocity += gravity * Time.deltaTime; //중력을 적용한 수직 방향(미래 속도=현재 속도 + 가속도*)

        if(cc.isGrounded) //바닥에 있을 경우, 수직항력을 처리하기 위해 속도 0으로 설정(떨어질 때 자연스럽게 중력이 적용되도록)
        {
            yVelocity = 0;
        }
        if(ARAVRInput.GetDown(ARAVRInput.Button.Two, ARAVRInput.Controller.RTouch))
        {
            yVelocity = jumpPower;
        }

        dir.y = yVelocity;

        cc.Move(dir * speed * Time.deltaTime); //이동함
    }
}

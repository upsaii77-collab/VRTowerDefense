using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    //마우스로는 휠 클릭해야 총알 발사 이펙트 보임
    public Transform bulletImpact;//총알 파편 효과
    ParticleSystem bulletEffect; //총알 파편 파티클 시스템
    AudioSource bulletAudio; //총알 발사 사운드

    // Start is called before the first frame update
    void Start()
    {
        bulletEffect = bulletImpact.GetComponent<ParticleSystem>(); //총알 효과 파티클 시스템 컴포넌트 가져오기
        bulletAudio = bulletImpact.GetComponent<AudioSource>(); //총알 효과 오디오 소스 컴포넌트
    }

    // Update is called once per frame
    void Update()
    {
        if(ARAVRInput.GetDown(ARAVRInput.Button.IndexTrigger))
        {

            bulletAudio.Stop(); //총알 오디오 재생
            bulletAudio.Play();

            Ray ray = new Ray(ARAVRInput.RHandPosition, ARAVRInput.RHandDirection); //Ray를 카메라 위치로부터 나가도록
            RaycastHit hitInfo; //Ray 충돌 정보 저장하기 위한 변수 설정
            int playerLayer = 1 << LayerMask.NameToLayer("Player"); //플레이어 레이어 얻어오기, 1을 옆으로 옮겨주는 코드
            int towerLayer = 1 << LayerMask.NameToLayer("Tower"); //타워 레이어 얻어오기
            int layerMask = playerLayer | towerLayer;

            if(Physics.Raycast(ray, out hitInfo, 200, ~layerMask))
            {
                bulletEffect.Stop();  //총알 이펙트 진행되고 있으면 멈추고 재생
                bulletEffect.Play();
                bulletImpact.position = hitInfo.point; //부딪힌 지점 바로 위에서 이펙트가 보이도록 설정
                bulletImpact.forward = hitInfo.normal; //부딪힌 지점의 방향으로 총알 이펙트 방향 설정
            }

        }
    }
}

using UnityEngine;
using System.Collections;



public class FollowCam : MonoBehaviour {
    public Transform targetTr; // 추적할 타깃 게임 오브젝트의 Transform 변수
    public float dist = 10.0f; // 카메라와의 일정 거리
    public float height = 3.0f; // 카메라의 높이 설정
    public float dampTrace = 20.0f; // 부드러운 추적을 위한 변수

    //카메라 자신의 Transform 변수
    private Transform tr;


	// Use this for initialization
	void Start () {
        tr = this.gameObject.GetComponent<Transform>();
	}

    //Update 함수 호추 이후 한번씩 호출되는 함수인 LateUpdate 사용
    //추적할 타깃의 이동이 종료된 이후에 카메라가 추저가기 위해 LateUpdate 사용
    void LateUpdate()
    {
        //카메라의 위치를 추적대상의 dist 변수만큼 뒤쪽으로 배치하고 height 변수만큼 위로 올림
        // Vector3.Lerp는 Linear Interpolation. 카메라의 부드러운 이동을 위해 사용. 
        // Lerp(vector3 시작위치, vector3 종료위치, float 보간시간)
        tr.position = Vector3.Lerp(tr.position, targetTr.position - (targetTr.forward * dist) + (Vector3.up * height), Time.deltaTime * dampTrace);
        // 카메라가 타깃 게임obj를 바라보게 설정
        tr.LookAt(targetTr.position);
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}

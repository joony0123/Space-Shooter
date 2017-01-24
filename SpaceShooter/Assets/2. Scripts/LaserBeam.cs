using UnityEngine;
using System.Collections;

public class LaserBeam : MonoBehaviour {

    private Transform tr;
    private LineRenderer line;
    //광선에 충돌한 게임옵젝트의 정보를 받아올 변수
    private RaycastHit hit;

	// Use this for initialization
	void Start () {

        //component 할당
        tr = GetComponent<Transform>();
        line = GetComponent<LineRenderer>();

        //로컬좌표를 기준으로 변경
        line.useWorldSpace = false;
        //초기에 비활성함. 
        line.enabled = false;
        //라인의 시작폭과 종료포 설정
        line.SetWidth(0.3f, 0.01f);
	
	}
	
	// Update is called once per frame
	void Update () {
        //광선을 미리 생성
        Ray ray = new Ray(tr.position + (Vector3.up * 0.02f), tr.forward);

        //광선이 눈에 보이게 설정
        Debug.DrawRay(ray.origin, ray.direction * 100, Color.blue);

        if (Input.GetMouseButtonDown(0))
        {
            //Line renderer 의 첫번째 점의 위치 설정
            line.SetPosition(0, tr.InverseTransformPoint(ray.origin));

            //어떤 물체에 광선이 맞았을 때의 위치를 line renderer의 끝점으로 설정
            if(Physics.Raycast(ray, out hit, 100.0f))
            {
                line.SetPosition(1, tr.InverseTransformPoint(hit.point));

            }else
            {
                line.SetPosition(1, tr.InverseTransformPoint(ray.GetPoint(100.0f)));
            }

            //광선을 표시하는 코루틴 함수 호출
            StartCoroutine(this.ShowLaserBeam());
        }
	}

    IEnumerator ShowLaserBeam()
    {
        line.enabled = true;
        yield return new WaitForSeconds(Random.Range(0.01f, 0.2f));
        line.enabled = false;
    }
}

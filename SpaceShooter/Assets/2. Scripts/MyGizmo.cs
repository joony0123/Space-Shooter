using UnityEngine;
using System.Collections;

public class MyGizmo : MonoBehaviour {
    public Color _color = Color.yellow;
    public float _radius = 0.1f;

    void OnDrawGizmos()
    {
        //기즈모 색상 설정
        Gizmos.color = _color;
        //구체 모양의 기즈모 생성. 인자는 (생성위치, 반지름)
        Gizmos.DrawSphere(transform.position, _radius);
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

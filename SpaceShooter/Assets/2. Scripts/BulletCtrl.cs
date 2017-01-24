using UnityEngine;
using System.Collections;

public class BulletCtrl : MonoBehaviour {
    //총알의 파괴력
    public int damage = 20;

    //총알의 발사 속도
    public float speed = 1000.0f;
	// Use this for initialization
    //start는 맨 먼저 시행되기에, 총알이 생성됌과 동시에 z축으로 날아감
	void Start () {
        this.gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * speed);
        //GetComponent<RigidBody>().AddRelativeForce(Vector3.forward * speed) = 로컬좌표로 날아감
        //AddForce(Vector3.forward *speed)하면 Global 좌표 쓰기에 안 좋음. 그래서 transform.forward 쓰면 local로 이해함
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

using UnityEngine;
using System.Collections;

public class WallCtrl : MonoBehaviour {

    public GameObject sparkEffect;
    //충돌이 시작했을 떄
    void OnCollisionEnter(Collision coll)
    {
        //충돌한 오브젝트와 태그 비교(이름보단 태그가 확실)
        if(coll.gameObject.tag == "BULLET")
        {
            //Quaternion.identity = 회전 없이 설정
            GameObject spark = (GameObject) Instantiate(sparkEffect, coll.transform.position, Quaternion.identity);
            Destroy(spark, spark.GetComponent<ParticleSystem>().duration + 0.2f);
            
            //충돌한 게임 옵젝 삭제
            Destroy(coll.gameObject);
        }
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

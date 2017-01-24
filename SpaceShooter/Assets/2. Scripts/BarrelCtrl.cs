using UnityEngine;
using System.Collections;

public class BarrelCtrl : MonoBehaviour {
    public GameObject expEffect;
    private Transform tr;
    private int hitCount = 0;
    public Texture[] textures;

	// Use this for initialization
	void Start () {
        tr = this.gameObject.GetComponent<Transform>();

        // int면 0~해당숫자-1 까지 float면 0~해당숫자 까지
        int ind = Random.Range(0, textures.Length);
        GetComponentInChildren<MeshRenderer>().material.mainTexture = textures[ind];
	}
	
    void OnCollisionEnter(Collision coll)
    {
        if(coll.gameObject.tag == "BULLET")
        {
            if(++hitCount >= 3)
            {
                ExpBarrel();
            }
        }
    }

    void ExpBarrel()
    {
        //폭발 효과 파티클 생성
        Instantiate(expEffect, tr.position, Quaternion.identity);

        //지점한 원점을 중심으로 10.0f 반경 내에 들어와 있는 Collider 객체 추출
        // Physics.OverlapSphere(원점, 반경) = 원점으로 부터 반경 안에 있는 콜라이더의 리스트 반환
        Collider[] colls = Physics.OverlapSphere(tr.position, 10.0f);

        //foreach 안하면 한 드럼통만 날라감. 전부다 하려면 전부다 하나씩 force 적용
        foreach(Collider coll in colls)
       {
            Rigidbody rbody = coll.gameObject.GetComponent<Rigidbody>();
            if(rbody != null && coll.gameObject.tag == "BARREL")
            {
                rbody.mass = 1.0f;
                // 원점으로 부터 받는 폭발력/폭발을 물리법칙에 의해 해당 물체에 적용
                rbody.AddExplosionForce(1000.0f, tr.position, 10.0f, 300.0f);
                Destroy(coll.gameObject, 5.0f);
            }
        }

        
    }

    void OnDamage(object[] _params)
    {
        //발사위치
        Vector3 firePos = (Vector3)_params[0];
        //location of hit
        Vector3 hitPos = (Vector3) _params[1];
        //입사벡터(ray 각도) = 맞은좌표 - 발사원점
        Vector3 incomeVector = hitPos - firePos;

        //입사벡터 normalized 변경
        incomeVector = incomeVector.normalized;

        //ray의 hit 좌표에 입사벡터의 각도로 힘을 생성
        GetComponent<Rigidbody>().AddForceAtPosition(incomeVector * 1000f, hitPos);

        //총알 맞은 횟수를 증가시키고 3회 이상이면 폭발 처리
        if(++hitCount >= 3)
        {
            ExpBarrel();
        }
    }

	// Update is called once per frame
	void Update () {
	
	}
}

using UnityEngine;
using System.Collections;

//반드시 필요한 컴포넌트를 명시해 해당 컴포넌트가 삭제되는 것을 방지하는 Attribute
[RequireComponent(typeof(AudioSource))]
public class FireCtrl : MonoBehaviour {
    public GameObject bullet;
    public Transform firePos;
    public AudioClip fireSfx;
    //AudioSource 컴포넌트를 저장할 변수
    private AudioSource source = null;
    //MuzzleFlash의 MuzzleRenderer 컴포넌트 연결 변수
    public GameObject muzzleFlash;

	// Use this for initialization
	void Start () {
        source = this.gameObject.GetComponent<AudioSource>();
        muzzleFlash.GetComponent<MeshRenderer>().enabled = false;
       
	}
	
	// Update is called once per frame
	void Update () {
        //Ray를 시각적으로 표시하기 위해 사용
        Debug.DrawRay(firePos.position, firePos.forward * 10.0f, Color.green);

        //마우스 왼쪽 버튼을 클릭 했을 때 Fire 함수 호출
        if (Input.GetMouseButtonDown(0))
        {
            Fire();

            //Ray에 맞은 옵젝트의 정보를 받아올 변수
            RaycastHit hit;

            //Raycast 함수로 Ray를 발사해 맞은 게임옵젝트가 있을 때 true 반환
            //raycast는 실제 충돌이 발생하지 않아 OnCollisionEnter 함수 발생하지 않음
            //너 총 맞았으니까 피 흘려라 얘기하는 것
            if(Physics.Raycast(firePos.position, firePos.forward, out hit, 10.0f))
            {
                //ray에 맞은 게임옵젝트의 Tag 값을 비교해 몬스터 여부 체크
                if(hit.collider.tag == "MONSTER")
                {
                    //sendmessage 이용해 전달한 인자를 배열에 담음. 
                    object[] _params = new object[2];
                    _params[0] = hit.point; // ray에 맞은 정확한 위치값(Vector3)
                    _params[1] = 20; //damage on monster

                    //call function that damages monster
                    hit.collider.gameObject.SendMessage("OnDamage", _params, SendMessageOptions.DontRequireReceiver);

                }

                //ray에 맞은 옵젝트가 barrel인지 확인
                if(hit.collider.tag == "BARREL")
                {
                    //드럼통에 맞은 ray의 입사각을 계산하기 위해 발사 원점과 맞은 지점 전달
                    object[] _params = new object[2];
                    _params[0] = firePos.position;
                    _params[1] = hit.point;
                    hit.collider.gameObject.SendMessage("OnDamage", _params, SendMessageOptions.DontRequireReceiver);
                }
            }
        }
	}

    void Fire()
    {
        //동적으로 총알 생성
        CreateBullet();
        //사운드 발생 함수
        //source에 사운드 없어도 발생 가능
        //source.PlayOneShot(fireSfx, 0.9f);
        GameMgr.instance.PlaySfx(firePos.position, fireSfx);

        //잠시 기다리는 루틴을 위해 코루틴 함수로 호출
        StartCoroutine(this.ShowMuzzleFlash());
    }

    void CreateBullet()
    {
        //Bullet prefab을 동적으로 생성
        //static Object Instantiate(Object original, Vector3 position, Quaternion rotation);
        Instantiate(bullet, firePos.position, firePos.rotation);
    }

    //MuzzleFlash 활성/비활성화를 짧은 시간동안 반복
    IEnumerator ShowMuzzleFlash()
    {
        //MuzleFlash 스케일을 불규칙하게 변경
        float scale = Random.Range(1.0f, 2.0f);
        muzzleFlash.transform.localScale = Vector3.one * scale;
        

        //MuzzleFlash를 z축을 기준으로 불규칙하게 회전
        Quaternion rot = Quaternion.Euler(0, 0, Random.Range(0, 360));
        muzzleFlash.transform.localRotation = rot;

        //활성화해서 보이게 함
        muzzleFlash.GetComponent<MeshRenderer>().enabled = true;

        //불규칙적인 시간동안 delay한 다음 meshrenderer를 비활성화
        yield return new WaitForSeconds(Random.Range(0.05f, 0.3f));

        //비활성화해서 보이지 않게 함
        muzzleFlash.GetComponent<MeshRenderer>().enabled = false;
    }
}

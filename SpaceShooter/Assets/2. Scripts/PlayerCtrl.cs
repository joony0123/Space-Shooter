using UnityEngine;
using UnityEngine.UI; // UI 항목 접근
using System.Collections;

//클래스에 System.Serializable이라는 Attribute를 명시해야
//Inspector 뷰에 노출이 됌
[System.Serializable]
//_animation이 플레이 할 수 있게 필요한 것 미리 준비해 놓는 용도
public class Anim
{
    public AnimationClip idle;
    public AnimationClip runForward;
    public AnimationClip runBackward;
    public AnimationClip runRight;
    public AnimationClip runLeft;
}
public class PlayerCtrl : MonoBehaviour {
    //게임매니저에 접근하기 위한 변수
    private GameMgr gameMgr;

    private float h = 0.0f;
    private float v = 0.0f;
    private int initHp;
    public Image imgHpbar;

    //접근해야 하는 컴포넌트는 반드시 변수에 할당한 후에 사용
    private Transform tr;
    //이동속도 변수, public으로써 Unity에서 변경 가능
    public float moveSpeed = 10.0f;
    public float rotSpeed = 100.0f;

    //inspector 뷰에 표시할 애니메이션 클래스 변수
    public Anim anim;
    //아래에 있는 3d 모델의 Animation 컴포넌트 점근하기 위한 변수
    public Animation _animation;

    public int hp = 100;

    //델리게이트 및 이벤트 선언
    // 델리게이트명 이벤트명
    public delegate void PlayerDieHandler();
    public static event PlayerDieHandler OnPlayerDie;

    // Use this for initialization
    void Start () {
        //스크립트 처음에 Transform 컴포넌트 할당
        tr = GetComponent<Transform>();

        //todaud chrltrkqt tjfwjd
        initHp = hp;

        //GameMgr 스크립트 할당
        gameMgr = GameObject.Find("GameManager").GetComponent<GameMgr>();
        

        //자신의 하위에 있는 Animation 컴포넌트를 찾아와 변수에 할당
        //결국 Play하는 것은 이 _animation을 play 하는 것임.
        _animation = GetComponentInChildren<Animation>();

        //Animation 컴포넌트의 애니메이션 클립을 지정하고 실행
        //_animation.Play(anim.idle.name) 과 같음
        _animation.clip = anim.idle;
        _animation.Play();
	}
	
	// Update is called once per frame
	void Update () {
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");


        //전후좌우 이동 방향 벡터 계산
        Vector3 moveDir = (Vector3.forward * v) + (Vector3.right * h);

        //Translate(이동방향 * 속도 * 변위값* Time.deltaTime, 기준좌표)
        //Space.Self는 로컬, 즉 캐릭터가 어느 방향 보고 있는지에 따라 기준이 됌. 
        //Space.World는 글로벌 좌표로 이동. 캐릭터 바라보고 있는 곳 관계 없음
        //기준좌표 생략시 로컬로 자동
        tr.Translate(moveDir.normalized * moveSpeed * Time.deltaTime, Space.Self);

        //Vector3.up(Y축)을 기준으로 마우스 돌릴때 rotSpeed만큼의 속도로 회전
        tr.Rotate(Vector3.up * Time.deltaTime * rotSpeed * Input.GetAxis("Mouse X"));

        //키보드 입력값을 기준으로 동작할 애니메이션 수행
        if (v >= 0.1f)
        {
            //전진 애니메이션
            _animation.CrossFade(anim.runForward.name, 0.3f);
        }
        else if ( v <= -0.1f)
        {
            _animation.CrossFade(anim.runBackward.name, 0.3f);
        }
        else if (h >= 0.1f)
        {
            _animation.CrossFade(anim.runRight.name, 0.3f);
        }
        else if (h <= -0.1f)
        {
            _animation.CrossFade(anim.runLeft.name, 0.3f);
        }
        else
        {
            //정지시 idle 애니메이션
            _animation.CrossFade(anim.idle.name, 0.3f);
        }
    }

    //충돌한 collider의 istrigger 체크됬을 때 발생
    void OnTriggerEnter(Collider coll)
    {

        if (coll.gameObject.tag == "PUNCH")
        {
            hp -= 10;
            Debug.Log("Player HP = " + hp.ToString());

            //Image UI 항목의 fillAmount 속성을 조절해 생명 게이지 조절
            imgHpbar.fillAmount = (float)hp / (float)initHp;
            if(hp <= 0)
            {
                PlayerDie();
            }
        }
    }

    void PlayerDie()
    {
        Debug.Log("Player Die!!");

        ////Monster라는 tag를 가진 모은 게임 옵젝트 찾아옴
        //GameObject[] monsters = GameObject.FindGameObjectsWithTag("MONSTER");

        //foreach(GameObject monster in monsters)
        //{
        //    //특정 게임옵젝트의 함수를 호출하는명령 (명령 내리는 듯한, 그 옵젝에 sendmessage 해서 어떤 함수 하라 명령함)
        //    //모든 몬스터의 OnPlayerDie함수 순차적으로 호출
        //    //SendMessageOptions.DontRequireReceiver = 해당 함수가 없더라도 함수가 없다는 메세지를 리턴받지 않겠다(빠른 실행 가능)
        //    monster.SendMessage("OnPlayerDie", SendMessageOptions.DontRequireReceiver);
        //}

        //이벤트 발생시킴
        OnPlayerDie();

        //게임 매니저의 isGameOver 변숫값을 변경해 몬스터 출현 정지시킴. 
        gameMgr.isGameOver = true;
    }
}

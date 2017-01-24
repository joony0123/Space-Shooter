using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class MonsterCtrl : MonoBehaviour {
    //몬스터의 상태정보가 있는 enumerable 변수 선언
    public enum MonsterState { idle, trace, attack, die};
    //몬스터의 현재상태 정보를 저장할 Enum 변수
    public MonsterState monsterState = MonsterState.idle;

    

    //속도향상을 위해 각종 컴포넌트를 변수에 할당
    private Transform monsterTr;
    private Transform playerTr;
    private NavMeshAgent nvAgent;
    private Animator animator;
    private GameUI gameUI;

    public GameObject bloodeffect;
    public GameObject bloodDecal;

    //추적 사정거리
    public float traceDist = 10.0f;
    //공격 사정거리
    public float attackDist = 2.0f;

    //몬스터의 사망 여부
    private bool isDie = false;

    private int hp = 100;

    // Use this for initialization
    void Awake()
    {
        gameUI = GameObject.Find("GameUI").GetComponent<GameUI>();
        monsterTr = this.gameObject.GetComponent<Transform>();
        //GameObject.find는 가급적 update에서 사용하지 말기. 왜냐면 처리속도 느림
        playerTr = GameObject.FindWithTag("Player").GetComponent<Transform>();
        nvAgent = this.gameObject.GetComponent<NavMeshAgent>();
        //Animator 컴포넌트 할당
        animator = this.gameObject.GetComponent<Animator>();
        nvAgent.destination = playerTr.position;
    


    }

    //이벤트는 반드시 스크립트 활성화 시점에 연결하고 비활성화될 때 해제해야 한다
    //이벤트 발생시 수행할 함수 연결
    //OnEnable/OnDisable은 스크립트 또는 GameObject가 활성화되거나 비활성화될 때 수행되는 함수, 연결 및 해제 여기서
    // (이벤트 선언된 클래스명).(이벤트명) += (이벤트 발생시 호출할 함수)
    // (이벤트 선언된 클래스명).(이벤트명) -= (이벤트 발생시 호출할 함수)

    void OnEnable()
    {

        PlayerCtrl.OnPlayerDie += this.OnPlayerDie;

        //일정한 간격으로 몬스터의 행동상테를 체크하는 코루틴 함수 실행
        StartCoroutine(this.CheckMonsterState());

        //몬스터의 상태에 따라 동작하는 루틴을 실행하는 코루틴 함수 실행
        StartCoroutine(this.MonsterAction());

    }

    //이벤트 발생시 연결된 함수 해제
    void OnDisable()
    {
        PlayerCtrl.OnPlayerDie -= this.OnPlayerDie;
    }

//일정한 간격으로 몬스터의 행동 상태를 체크하고 monsterState 값 변경
IEnumerator CheckMonsterState()
{
    while (!isDie)
    {
        //0.2초 기다렷다 다음으로 넘어감
        // 기다리는 동안 while 바깥으로 나가서 다른 구문 처리ㄴ
        yield return new WaitForSeconds(0.2f);
        //vector distance 구할 떄는 Vector3.Distance(A, B)이용**
        float dist = Vector3.Distance(playerTr.position, monsterTr.position);

        if (dist <= attackDist) //공격범위 이내로 들어왔는지 확인
        {
            monsterState = MonsterState.attack;
        }
        else if (dist <= traceDist) // 추적 범위내로 들어왔는지
        {
            monsterState = MonsterState.trace;
        }
        else
        {
            monsterState = MonsterState.idle;
        }
    }
}

//몬스터 상태값에 따라 적절한 동작 수행
IEnumerator MonsterAction()
{
    while (!isDie)
    {
        switch (monsterState)
        {
            //idle 상태
            case MonsterState.idle:
                //추적 중지, 물리적으로 움직이는 것 스톱
                nvAgent.Stop();
                //그냥 두면 계속 워킹하면서 가만히 있으니 idle 모션 추가
                animator.SetBool("IsTrace", false);
                break;
            //추적상태
            case MonsterState.trace:
                //추적대상 위치 넘겨줌
                nvAgent.destination = playerTr.position;
                //추적 재시작
                nvAgent.Resume();
                animator.SetBool("IsTrace", true);
                animator.SetBool("IsAttack", false);
                break;
                //공격 상태
            case MonsterState.attack:
                //추적 중지
                nvAgent.Stop();
                animator.SetBool("IsAttack", true);
                break;
        }
        yield return null;
    }
}

    void OnCollisionEnter(Collision coll)
    {
        if (coll.gameObject.tag == "BULLET")
        {
            CreateBloodEffect(coll.transform.position);
            hp -= coll.gameObject.GetComponent<BulletCtrl>().damage;
            Destroy(coll.gameObject);
            //IsHit trigger를 발생시키면 any state에서 gothit으로 전이됌
            animator.SetTrigger("IsHit");
            if (hp <= 0)
            {
                MonsterDie();
            }
            
            


            
        }
    }

    void MonsterDie()
    {
        //사망한 몬스터 태그를 untagged로 변경
        gameObject.tag = "Untagged";

        //All Coroutines stop
        StopAllCoroutines();

        isDie = true;
        monsterState = MonsterState.die;
        nvAgent.Stop();
        animator.SetTrigger("IsDie");

        //Deactivate collider in Monster
        gameObject.GetComponent<CapsuleCollider>().enabled = false;
        // Parent포함 하위의 모든 spherecollider Array 형식으로 반환
        foreach(Collider coll in gameObject.GetComponentsInChildren<SphereCollider>())
        {
            coll.enabled = false;
        }

        //GameUI 스코어 표시 함수 호출
        //gameUI는 GameUI object의 GameUI Script임. 그러므로 바로.DispScore() 사용 가능
        gameUI.DispScore(50);

        //Monster 옵젝트 풀로 환원시키는 코루틴 함수 호출
        StartCoroutine(this.PushObjectPool());
        
    }

    IEnumerator PushObjectPool()
    {
        yield return new WaitForSeconds(3.0f);

        //각종 변수 초기화
        isDie = false;
        hp = 100;
        gameObject.tag = "MONSTER";
        monsterState = MonsterState.idle;

        //몬스터 추가된 Collider 다시 활성화
        gameObject.GetComponentInChildren<CapsuleCollider>().enabled = true;

        foreach(Collider coll in gameObject.GetComponentsInChildren<SphereCollider>())
        {
            coll.enabled = true;
        }

        //몬스터 비활성화
        gameObject.SetActive(false);
    }

    //몬스터가 레이에 맞았을 때 호출되는 함수
    void OnDamage(object[] _params)
    {
        Debug.Log(string.Format("Hit ray {0} : {1}", _params[0], _params[1]));

        //혈흔효과 함수 호출
        CreateBloodEffect((Vector3)_params[0]);

        //맞은 총알의 Damage를 추출해 몬스터 hp 차감.
        hp -= (int)_params[1];
        if(hp <= 0)
        {
            MonsterDie();
        }
        //IsHit Trigger를 발생시키면 Any State에서 gothit로 전이됌
        animator.SetTrigger("IsHit");

    }

    void CreateBloodEffect(Vector3 pos)
    {
        //혈흔 효과 생성
        GameObject blood1 = (GameObject)Instantiate(bloodeffect, pos, Quaternion.identity);
        Destroy(blood1, 2.0f);

        //데칼생성 위치 - 바닥에서 조금 올린 위치 산출, 겹치는 깜빡거릴 수 있음
        Vector3 decalPos = monsterTr.position + (Vector3.up * 0.05f);
        //데탈의 회전값을 무작위로 설정(회전값은 이렇게)
        Quaternion decalRot = Quaternion.Euler(90, 0, Random.Range(0, 360));

        //데칼 프리팹 생성
        GameObject blood2 = (GameObject)Instantiate(bloodDecal, decalPos, decalRot);
        //데칼의 크기도 불규칙적으로 나타나게끔 스케일 조정
        float scale = Random.Range(0.5f, 3.0f);
        blood2.transform.localScale = Vector3.one * scale;

        //5초후에 혈흔 효과 삭제
        Destroy(blood2, 5.0f);


    }

    void OnPlayerDie()
    {
        //몬스터의 상태를 체크하는 코루틴 모두 정지
        StopAllCoroutines();
        //추적 정지하고 애니메이션 수행
        nvAgent.Stop();
        animator.SetTrigger("IsPlayerDie");
    }

// Update is called once per frame
void Update () {
  
        }
}

using UnityEngine;
using System.Collections;
//List 자료형을 사용하기 위해 추가해야 하는 네임스페이스
using System.Collections.Generic;

public class GameMgr : MonoBehaviour {
    //몬스터 출연 위치 배열
    public Transform[] points;
    //몬스터 프리팹 할당 변수
    public GameObject monsterPrefab;
    //몬스터 미리 생성해 저장할 리스트 자료형
    public List<GameObject> monsterPool = new List<GameObject>();

    //몬스터 발생시킬 주기
    public float createTime = 2.0f;
    //몬스터 최대 발생 개수
    public int maxMonster = 10;

    //게임 종료 여부 변수
    public bool isGameOver = false;

    //사운드 볼륨 설정 변수
    public float sfxVolumn = 1.0f;
    //사운드 뮤트 기능
    public bool isSfxMute = false;

    //싱글턴 패턴을 위한 인스턴스 변수 선언
    //싱글턴으로 하면 다른 곳에서 쓸 때 매변 변수 할당 안해도 됌
    //게임매니저에 전역적으로 접근하기 위해 static으로 선언해 메모리에 상주시킴
    public static GameMgr instance = null;

    void Awake()
    {
        //GameMgr클래스를 인스턴스에 대입
        instance = this;
    }

	// Use this for initialization
	void Start () {
        //hierarchy 뷰의 spawnpoint를 찾아 하위에 잇는 모든 transform 컴포넌트 찾아옴
        //@@주의@@parent도 추가되어 0번에 있음
        points = GameObject.Find("SpawnPoint").GetComponentsInChildren<Transform>();

        //몬스터 생성해 object pool에 저장
        for (int i = 0; i < maxMonster; i++)
        {
            //몬스터 프리팹 생성
            GameObject monster = (GameObject) Instantiate(monsterPrefab);
            //생성한 몬스터의 이름 설정
            monster.name = "Monster_" + i.ToString();
            //생성한 몬스터 비활성화
            monster.SetActive(false);
            //생성한 몬스터 object pool에 추가
            monsterPool.Add(monster);
        }

        if(points.Length > 0)
        {
            //몬스터 생성 코루틴 함수 호출
            StartCoroutine(this.CreateMonster());
        }
	
	}

    //몬스터 생성 코루틴 함수
    IEnumerator CreateMonster()
    {
        //게임 종료시까지 무한루프
        while (!isGameOver)
        {
            //몬스터의 생성 주기 시간만큼 메인 루프에 양보
            yield return new WaitForSeconds(createTime);

            //플레이어가 사망핼 을 때 코루틴을 종료해 다음 루틴 징행하지 않음
            if (isGameOver) yield break;

            //object pool의 첨부터 끝까지 순회
            foreach(GameObject monster in monsterPool)
            {
                //비활성화 여부로 사용 가능한 몬스터를 판단
                if (!monster.activeSelf)
                {
                    //몬스터 출현시킬 위치 포인트 중에 랜덤 정하고, 활성화
                    //몬스터 출현시킬 위치의 인덱스값을 추출
                    int ind = Random.Range(1, points.Length);
                    //몬스터 출현위치 설정
                    monster.transform.position = points[ind].position;
                    //몬스터 활성화
                    monster.SetActive(true);
                    //object 풀에서 몬스터 프리팹 하나 활성화 후 for loop 빠져나감, 왜냐면 이미 코루틴으로 돌기 때문에 하나만 하고 다음에 또함
                    break;
                }
            }

            ////몬스터의 최대 생성 개수보다 작을 때만 몬스터 생성
            //if (monsterCount < maxMonster)
            //{
                

            //    //불규칙적인 위치 산출
            //    //난수 발생 인덱스 1로 시작, 왜냐하면 parent인 spawnpoint가 0번에 들어가 있기 때문
            //    int idx = Random.Range(1, points.Length);

            //    //monster 동적 생성
            //    Instantiate(monsterPrefab, points[idx].position, points[idx].rotation);

            //}else
            //{
            //    yield return null;
            //}
        }
    }

    //사운드 공용 함수
    public void PlaySfx(Vector3 pos, AudioClip sfx)
    {
        //음소거 옵션이 설정되면 바로 빠져나감
        if (isSfxMute) return;

        //게임오브젝트를 동적으로 생성
        GameObject soundObj = new GameObject("Sfx");
        //사운드 발생위치 지정
        soundObj.transform.position = pos;

        //생성한 게임오브젝트에 AudioSource 컴포넌트 추가
        AudioSource audioSource = soundObj.AddComponent<AudioSource>();
        //AudioSource 속성 설정
        audioSource.clip = sfx;
        audioSource.minDistance = 10.0f;
        audioSource.maxDistance = 30.0f;
        //sfxVolumn 변수로 게임의 전체적인 볼륨 설정 가능
        audioSource.volume = sfxVolumn;

        //사운드 실행
        audioSource.Play();

        //사운드의 플레이가 종료되면 동적으로 생성한 게임오브젝트 삭제, 종료시간은 sfx.length로
        Destroy(soundObj, sfx.length);

    }
	
	// Update is called once per frame
	void Update () {
	
	}
}

using UnityEngine;
//UI component 에 접근하기 위해 추가한 네임스페이스
using UnityEngine.UI;
using System.Collections;

public class GameUI : MonoBehaviour {
    //Text UI 항목 연결을 위한 변수
    public Text txtScore;
    //누적점수 기록
    private int totScore = 0;

	// Use this for initialization
	void Start () {
        totScore = PlayerPrefs.GetInt("TOT_SCORE", 0);
        DispScore(0);
	}

    public void DispScore(int score)
    {
        totScore += score;
        //txtScore라는 object의 text
        txtScore.text = "score <color=#ff0000>" + totScore.ToString() + "</color>";

        //스코어 저장
        PlayerPrefs.SetInt("TOT_SCORE", totScore);
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}

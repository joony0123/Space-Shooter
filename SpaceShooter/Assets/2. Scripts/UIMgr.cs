using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class UIMgr : MonoBehaviour {

    public void OnClickStartBtn()
    {
        Debug.Log("Click Button");
        SceneManager.LoadScene("scLevel01");
        SceneManager.LoadScene("scPlay", LoadSceneMode.Additive);
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

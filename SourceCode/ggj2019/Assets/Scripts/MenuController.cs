using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour {

    public string NextSceneName;

    // Use this for initialization
    void Start () {

    }
	
	// Update is called once per frame
	void Update () {
        //Wait for any key input to start the game play.
        if (Input.anyKey)
        {
            //this.audioSource_backgroundMusic.Stop();

            SceneManager.LoadScene(this.NextSceneName);
        }
    }
}

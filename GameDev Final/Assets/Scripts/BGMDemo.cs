using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMDemo : MonoBehaviour {

    public AudioClip bgmOne;
    public AudioClip bgmTwo;
    public AudioSource source;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            source.Stop();
            source.volume = 1f;
            source.clip = bgmOne;
            source.Play();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            source.Stop();
            source.volume = 0.75f;
            source.clip = bgmTwo;
            source.Play();
        }
    }
}

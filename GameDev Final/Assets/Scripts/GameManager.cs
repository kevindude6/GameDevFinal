using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public GameObject player;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void SignalReady(MapGenerator.Room r)
    {
        float centerX = (r.innerRect.xMin+ r.innerRect.xMax) / 2;
        float centerY = (r.innerRect.yMin + r.innerRect.yMax) / 2;
        player.transform.position = new Vector3(centerX, centerY);
        player.GetComponent<SpriteRenderer>().enabled = true;
    }
}
